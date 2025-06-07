using Microsoft.Maui.Controls.Shapes;
using Models.Game;
using Models.Events;
using Models.Enums;
using Models.Exceptions;

namespace DuckAndCover.Pages;

public partial class GamePage : ContentPage
{
    public Game GameManager => (Application.Current as App)?.GameManager ?? 
                               throw new ErrorException(ErrorCodes.GameManagerNotInitialized);

    private GameCard? _selectedCard;
    private GameCard? _cardToCover;
    private bool _isWaitingForCoverTarget;
    private bool _isWaitingForDuckTarget;
    private List<Position> _validDuckTargets = new List<Position>();

    public GamePage()
    {
        InitializeComponent();
        UpdateDarkModeButtonText();
        LoadGrid();
        LoadCurrentCard();
        SubscribeToGameEvents();
        StartGame();
    }

    private AppTheme CurrentAppTheme => Application.Current?.UserAppTheme ?? AppTheme.Light;

    private void StartGame()
    {
        GameManager.StartGame();
    }

    private void SubscribeToGameEvents()
    {
        GameManager.PlayerChanged += OnPlayerChanged;
        GameManager.GameIsOver += OnGameIsOver;
        GameManager.ErrorOccurred += OnErrorOccurred;
        GameManager.PlayerChooseCoin += OnPlayerChooseCoin;
        GameManager.PlayerChooseDuck += OnPlayerChooseDuck;
        GameManager.PlayerChooseCover += OnPlayerChooseCover;
        GameManager.PlayerChooseQuit += OnPlayerChooseQuit;
        GameManager.DisplayMenuNeeded += OnDisplayMenuNeeded;
        GameManager.CardEffectProcessed += OnCardEffectProcessed;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        GameManager.PlayerChanged -= OnPlayerChanged;
        GameManager.GameIsOver -= OnGameIsOver;
        GameManager.ErrorOccurred -= OnErrorOccurred;
        GameManager.PlayerChooseCoin -= OnPlayerChooseCoin;
        GameManager.PlayerChooseQuit -= OnPlayerChooseQuit;
        GameManager.DisplayMenuNeeded -= OnDisplayMenuNeeded;
        GameManager.CardEffectProcessed -= OnCardEffectProcessed;
    }

    private void OnPlayerChanged(object? sender, PlayerChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (e.CurrentPlayer.IsBot && e.CurrentPlayer is Bot b)
            {
                Task.Delay(1000).ContinueWith(_ => Bot.PlayTurnAutomatically(GameManager));
            }

            InstructionsLabel.Text = $"Tour de {e.CurrentPlayer.Name}";
            DebugLabel.Text = "";
            ResetSelectionStatePartial();
            LoadGrid();
            LoadCurrentCard();
        });
    }

    private void ResetSelectionStatePartial()
    {
        _isWaitingForCoverTarget = false;
        _isWaitingForDuckTarget = false;
        _selectedCard = null;
        _cardToCover = null;
        _validDuckTargets.Clear();
    }

    private void ResetSelectionState()
    {
        ResetSelectionStatePartial();
        LoadGrid();
        LoadCurrentCard();
        if (GameManager.CurrentPlayer != null)
        {
            InstructionsLabel.Text = $"Tour de {GameManager.CurrentPlayer.Name}. Choisissez une action.";
        }
        else
        {
            InstructionsLabel.Text = "En attente d'un joueur...";
        }
    }

    private async void OnGameIsOver(object? sender, GameIsOverEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await DisplayAlert("Fin de partie", "La partie est terminée !", "OK");
            await Navigation.PopAsync();
        });
    }

    private async void OnErrorOccurred(object? sender, ErrorOccurredEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            string errorMessage = e.ErrorException?.Message ?? "Une erreur inconnue est survenue.";
            await DisplayAlert("Erreur", errorMessage, "OK");
            ResetSelectionState();
        });
    }


    private void OnPlayerChooseCoin(object? sender, PlayerChooseCoinEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InstructionsLabel.Text = $"{e.Player.Name} a passé son tour";
            ResetSelectionState();
        });
    }

    private void OnPlayerChooseDuck(object? sender, PlayerChooseDuckEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InstructionsLabel.Text = $"{e.Player.Name} effectue un déplacement Duck";
        });
    }

    private void OnPlayerChooseCover(object? sender, PlayerChooseCoverEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InstructionsLabel.Text = $"{e.Player.Name} effectue un recouvrement";
        });
    }

    private async void OnPlayerChooseQuit(object? sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (GameManager.CurrentPlayer == null || GameManager == null) return;
            bool confirm = await DisplayAlert("Quitter",
                $"{GameManager.CurrentPlayer.Name} veut quitter la partie. Confirmer ?",
                "Oui", "Non");

            if (confirm)
            {
                GameManager.Quit = true;
                if (GameManager.CheckGameOverCondition())
                {
                    var pers = (App.Current as App)?.DataPersistence;
                    if (pers != null)
                    {
                        pers.SaveData(
                            GameManager.AllPlayers,
                            GameManager.Games
                        );
                    }
                }
            }
        });
    }

    private void OnDisplayMenuNeeded(object? sender, DisplayMenuNeededEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (e.CurrentPlayer == null)
            {
                InstructionsLabel.Text = "En attente d'un joueur...";
                return;
            }

            InstructionsLabel.Text = $"Tour de {e.CurrentPlayer.Name}. Choisissez une action.";
            LoadCurrentCard();
        });
    }

    private void OnCardEffectProcessed(object? sender, CardEffectProcessedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (e.ProcessedCard == null) return;
            InstructionsLabel.Text = e.Message;
            LoadCurrentCard();

            if (e.ProcessedCard.Bonus == Bonus.Again || e.ProcessedCard.Bonus == Bonus.Max)
            {
                await DisplayAlert("Effet de carte!", e.Message, "OK");
            }
        });
    }

    private void LoadGrid()
    {
        try
        {
            if (GameManager?.CurrentPlayer?.Grid == null)
            {
                DebugLabel.Text = "Joueur actuel ou grille non défini pour LoadGrid.";
                GameGrid.Children.Clear();
                GameGrid.RowDefinitions.Clear();
                GameGrid.ColumnDefinitions.Clear();
                GridInfoLabel.Text = "Grille non disponible";
                return;
            }

            var currentPlayer = GameManager.CurrentPlayer;
            var playerGrid = currentPlayer.Grid;

            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();

            var allRelevantPositions = new List<Position>();
            if (playerGrid.GameCardsGrid != null && playerGrid.GameCardsGrid.Any(c => c != null))
            {
                allRelevantPositions.AddRange(playerGrid.GameCardsGrid.Where(c => c != null).Select(c => c.Position));
            }

            if (_isWaitingForDuckTarget && _selectedCard != null && _validDuckTargets != null &&
                _validDuckTargets.Any())
            {
                allRelevantPositions.AddRange(_validDuckTargets);
            }

            if (!allRelevantPositions.Any())
            {
                DebugLabel.Text = $"Aucune carte à afficher ou cible pour {currentPlayer.Name}.";
                GridInfoLabel.Text = $"Grille de {currentPlayer.Name} (vide)";
                return;
            }

            var distinctPositions = allRelevantPositions.Distinct().ToList();
            if (!distinctPositions.Any())
            {
                GridInfoLabel.Text = $"Grille de {currentPlayer.Name} (vide après distinct)";
                return;
            }

            var bounds = Models.Game.Grid.GetBounds(distinctPositions);

            int gridWidth = bounds.maxX - bounds.minX + 1;
            int gridHeight = bounds.maxY - bounds.minY + 1;

            if (gridWidth <= 0 || gridHeight <= 0)
            {
                GridInfoLabel.Text = $"Grille de {currentPlayer.Name} (dimensions invalides)";
                return;
            }

            for (int i = 0; i < gridHeight; i++)
                GameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(140) });
            for (int i = 0; i < gridWidth; i++)
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });

            for (int r_offset = 0; r_offset < gridHeight; r_offset++)
            {
                for (int c_offset = 0; c_offset < gridWidth; c_offset++)
                {
                    int actualRow = bounds.minY + r_offset;
                    int actualCol = bounds.minX + c_offset;
                    Position currentCellPosition = new Position(actualRow, actualCol);
                    GameCard? cardInCell = playerGrid.GetCard(currentCellPosition);

                    if (cardInCell != null)
                    {
                        var cardView = CreateCardView(cardInCell);
                        Microsoft.Maui.Controls.Grid.SetRow(cardView, r_offset);
                        Microsoft.Maui.Controls.Grid.SetColumn(cardView, c_offset);
                        GameGrid.Children.Add(cardView);
                    }
                    else
                    {
                        if (_isWaitingForDuckTarget && _selectedCard != null && _validDuckTargets != null &&
                            _validDuckTargets.Contains(currentCellPosition))
                        {
                            var duckTargetView = CreateValidDuckTargetCell(currentCellPosition);
                            Microsoft.Maui.Controls.Grid.SetRow(duckTargetView, r_offset);
                            Microsoft.Maui.Controls.Grid.SetColumn(duckTargetView, c_offset);
                            GameGrid.Children.Add(duckTargetView);
                        }
                    }
                }
            }

            GridInfoLabel.Text = $"Grille de {currentPlayer.Name}";
            DebugLabel.Text =
                $"Cartes: {playerGrid.GameCardsGrid?.Count(c => c != null) ?? 0}, Cibles Duck: {_validDuckTargets?.Count ?? 0}";
        }
        catch (ErrorException ex)
        {
            DebugLabel.Text = $"Erreur chargement grille: {ex.Message}";
            GridInfoLabel.Text = "Erreur chargement grille";
        }
    }

    private View CreateValidDuckTargetCell(Position position)
    {
        var border = new Border
        {
            Stroke = Colors.DodgerBlue,
            Background = Colors.LightSkyBlue.WithAlpha(0.7f),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle()
            {
                CornerRadius = new CornerRadius(15)
            },
            WidthRequest = 100,
            HeightRequest = 130,
            Padding = new Thickness(5),
            Content = new Label
            {
                Text = "Duck Ici",
                TextColor = Colors.DarkBlue,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold
            }
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnDuckTargetCellTapped(position);
        border.GestureRecognizers.Add(tapGesture);

        return border;
    }

    private async void OnDuckTargetCellTapped(Position targetPosition)
    {
        try
        {
            if (_selectedCard == null || !_isWaitingForDuckTarget || GameManager.CurrentPlayer == null)
                return;
            GameManager.HandlePlayerChooseDuck(GameManager.CurrentPlayer, _selectedCard.Position, targetPosition);
        }
        catch (ErrorException ex)
        {
            var handler = new ErrorHandler(ex);
            await DisplayAlert("Erreur Duck Cible", handler.Handle(), "OK");
            ResetSelectionState();
        }
    }

    private View CreateCardView(GameCard card)
    {
        if (card == null) throw new ErrorException(ErrorCodes.ArgumentNull);
        var backgroundColor = GetSplashColor(card.Splash);
        var textColor = GetContrastingTextColor(backgroundColor);
        var border = new Border
        {
            Background = new SolidColorBrush(backgroundColor),
            Stroke = Colors.White,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(15)
            },
            WidthRequest = 100,
            HeightRequest = 130,
            Padding = 0,
            Shadow = new Shadow
            {
                Brush = Brush.Black,
                Offset = new Point(5, 5),
                Radius = 10,
                Opacity = 0.4f
            }
        };


        if (card == _selectedCard)
        {
            border.Stroke = Colors.Yellow;
            border.Scale = 1.1;
        }
        else
        {
            border.Stroke = Colors.White;
            border.Scale = 1.0;
        }

        var mainGrid = new Microsoft.Maui.Controls.Grid();
        var numberLabel = new Label
        {
            Text = card.Number.ToString(), FontSize = 36, FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, TextColor = textColor
        };
        var splashLabel = new Label
        {
            Text = card.Splash.ToString(), FontSize = 20, FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.End, TextColor = textColor,
            Margin = new Thickness(0, 0, 8, 8)
        };
        var positionLabel = new Label
        {
            Text = $"{card.Position.Row},{card.Position.Column}", FontSize = 8, HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start, TextColor = textColor, Opacity = 0.7,
            Margin = new Thickness(4, 4, 0, 0)
        };
        mainGrid.Children.Add(numberLabel);
        mainGrid.Children.Add(splashLabel);
        mainGrid.Children.Add(positionLabel);
        border.Content = mainGrid;
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnCardTapped(card);
        border.GestureRecognizers.Add(tapGesture);
        return border;
    }


    private Color GetSplashColor(int splash) => splash switch
    {
        0 => Color.FromArgb("#FFFFFF"), 1 => Color.FromArgb("#FFD700"), 2 => Color.FromArgb("#B8860B"),
        3 => Color.FromArgb("#FF0000"), 4 => Color.FromArgb("#8B0000"), _ => Color.FromArgb("#FF00FF")
    };

    private Color GetContrastingTextColor(Color backgroundColor) =>
        (0.299 * backgroundColor.Red + 0.587 * backgroundColor.Green + 0.114 * backgroundColor.Blue) > 0.5
            ? Colors.Black
            : Colors.White;

    private async void OnCoinClicked(object? sender, EventArgs e)
    {
        try
        {
            if (GameManager.CurrentPlayer == null) return;
            GameManager.HandlePlayerChoice(GameManager.CurrentPlayer, "3");
        }
        catch (ErrorException ex)
        {
            var handler = new ErrorHandler(ex);
            await DisplayAlert("Erreur Coin", handler.Handle(), "OK");
            ResetSelectionState();
        }
    }

    private async void OnCoverClicked(object? sender, EventArgs e)
    {
        try
        {
            _isWaitingForCoverTarget = true;
            _isWaitingForDuckTarget = false;
            _selectedCard = null;
            _cardToCover = null;
            _validDuckTargets.Clear();
            LoadGrid();
            InstructionsLabel.Text = "COVER: Sélectionnez la carte à DÉPLACER.";
        }
        catch (ErrorException ex)
        {
            var handler = new ErrorHandler(ex);
            await DisplayAlert("Erreur Cover Setup", handler.Handle(), "OK");
        }
    }

    private async void OnDuckClicked(object? sender, EventArgs e)
    {
        try
        {
            _isWaitingForCoverTarget = false;
            _isWaitingForDuckTarget = true;
            _selectedCard = null;
            _validDuckTargets.Clear();
            LoadGrid();
            InstructionsLabel.Text = "DUCK: Sélectionnez la carte à DÉPLACER.";
        }
        catch (ErrorException ex)
        {
            var handler = new ErrorHandler(ex);
            await DisplayAlert("Erreur Duck Setup", handler.Handle(), "OK");
        }
    }

    private async void OnCardTapped(GameCard card)
    {
        try
        {
            if (GameManager.CurrentDeckCard == null || GameManager.CurrentPlayer == null)
            {
                await DisplayAlert("Info", "Aucune carte de deck active ou joueur non défini.", "OK");
                return;
            }

            int effectiveDeckCardNumber =
                Game.GetEffectiveDeckCardNumber(GameManager.CurrentPlayer, GameManager.CurrentDeckCard);

            if (_isWaitingForCoverTarget)
            {
                if (_selectedCard == null)
                {
                    if (card.Number != effectiveDeckCardNumber)
                    {
                        await DisplayAlert("Action invalide",
                            $"La carte N°{card.Number} ne correspond pas au N°{effectiveDeckCardNumber} (effectif) du deck pour Cover.",
                            "OK");
                        return;
                    }

                    _selectedCard = card;
                    LoadGrid();
                    InstructionsLabel.Text = "COVER: Sélectionnez la carte à RECOUVRIR.";
                }
                else
                {
                    if (card == _selectedCard)
                    {
                        await DisplayAlert("Action invalide", "Vous ne pouvez pas recouvrir une carte avec elle-même.",
                            "OK");
                        return;
                    }

                    _cardToCover = card;
                    GameManager.HandlePlayerChooseCover(GameManager.CurrentPlayer, _selectedCard.Position,
                        _cardToCover.Position);
                }
            }
            else if (_isWaitingForDuckTarget)
            {
                if (_selectedCard == null)
                {
                    if (card.Number != effectiveDeckCardNumber)
                    {
                        await DisplayAlert("Action invalide",
                            $"La carte N°{card.Number} (à déplacer) ne correspond pas au N°{effectiveDeckCardNumber} (effectif) du deck pour Duck.",
                            "OK");
                        return;
                    }

                    _selectedCard = card;
                    _validDuckTargets = Game.GetValidDuckTargetPositions(GameManager.CurrentPlayer,
                        _selectedCard.Position, GameManager.CurrentDeckCard);
                    LoadGrid();
                    InstructionsLabel.Text = "DUCK: Sélectionnez la CASE de destination.";
                }
                else
                {
                    await DisplayAlert("Action",
                        "Sélectionnez une case vide en surbrillance pour 'Ducker', ou cliquez à nouveau sur 'Duck' pour changer la carte à déplacer.",
                        "OK");
                }
            }
        }
        catch (ErrorException ex)
        {
            var handler = new ErrorHandler(ex);
            await DisplayAlert("Erreur Tap Carte", handler.Handle(), "OK");
            ResetSelectionState();
        }
    }

    private void LoadCurrentCard()
    {
        try
        {
            if (GameManager?.CurrentDeckCard != null && GameManager.CurrentPlayer != null)
            {
                CurrentCardFrame.IsVisible = true;
                int effectiveNumber =
                    Game.GetEffectiveDeckCardNumber(GameManager.CurrentPlayer, GameManager.CurrentDeckCard);
                string bonusText = "";
                Color borderColor = Colors.SeaGreen;
                switch (GameManager.CurrentDeckCard.Bonus)
                {
                    case Bonus.Again:
                        borderColor = Colors.OrangeRed;
                        bonusText = " (Again)";
                        break;
                    case Bonus.Max:
                        borderColor = Colors.Purple;
                        bonusText = " (Max)";
                        break;
                    case Bonus.None: break;
                    default:
                        borderColor = Colors.DarkGoldenrod;
                        bonusText = $" ({GameManager.CurrentDeckCard.Bonus})";
                        break;
                }

                CurrentCardBorder.BackgroundColor = borderColor;
                CurrentCardNumber.Text = effectiveNumber.ToString() + bonusText;
                CurrentCardNumber.TextColor = Colors.White;
            }
            else
            {
                CurrentCardFrame.IsVisible = false;
            }
        }
        catch (ErrorException ex)
        {
            CurrentCardFrame.IsVisible = false;
            if (DebugLabel != null)
                DebugLabel.Text += $" | ErrCarte: {ex.Message.Substring(0, Math.Min(ex.Message.Length, 20))}";
        }
    }
    
    private void UpdateDarkModeButtonText()
    {
        if (DarkModeButton != null)
        {
            if (CurrentAppTheme == AppTheme.Dark)
            {
                DarkModeButton.Text = "☀️";
            }
            else
            {
                DarkModeButton.Text = "🌙";
            }
        }
    }

    private void OnDarkModeClicked(object sender, EventArgs e)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = CurrentAppTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
            UpdateDarkModeButtonText();
        }
    }
}
