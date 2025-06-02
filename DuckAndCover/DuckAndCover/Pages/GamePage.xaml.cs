using Microsoft.Maui.Controls;
using Models.Game;
using Models.Events;
using Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DuckAndCover.Pages;

public partial class GamePage : ContentPage
{
    public Game GameManager => (App.Current as App)?.GameManager ?? throw new InvalidOperationException("GameManager not initialized");

    private GameCard? _selectedCard;
    private GameCard? _cardToCover;
    private bool _isWaitingForCoverTarget = false;
    private bool _isWaitingForDuckTarget = false;
    private List<Position> _validDuckTargets = new List<Position>();

    public GamePage()
    {
        InitializeComponent();
        if (GameManager.CurrentPlayer == null && GameManager.Players.Any())
        {
            GameManager.CurrentPlayer = GameManager.Players.First();
        }
        SubscribeToGameEvents();
        StartGame(); 
    }

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
        GameManager.PlayerChooseShowPlayersGrid += OnPlayerChooseShowPlayersGrid;
        GameManager.PlayerChooseShowScores += OnPlayerChooseShowScores;
        GameManager.PlayerChooseQuit += OnPlayerChooseQuit;
        GameManager.DisplayMenuNeeded += OnDisplayMenuNeeded;
        GameManager.CardEffectProcessed += OnCardEffectProcessed;
    }

    private void OnPlayerChanged(object sender, PlayerChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (e.CurrentPlayer == null) 
            {
                // Gérer le cas où le joueur est null, peut-être afficher un message ou attendre
                InstructionsLabel.Text = "En attente d'un joueur...";
                DebugLabel.Text = "CurrentPlayer est null dans OnPlayerChanged.";
                CurrentCardFrame.IsVisible = false;
                GameGrid.Children.Clear(); // Vider la grille si pas de joueur
                return;
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

        /// <summary>
        /// Appelé exactement une fois, quand la partie est terminée.
        /// </summary>
        private async void OnGameIsOver(object? sender, GameIsOverEventArgs e)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                GameManager.SavePlayers();
                GameManager.SaveGame();

                var pers = (App.Current as App)?.DataPersistence;
                if (pers != null)
                {
                    pers.SaveData(
                        GameManager.AllPlayers,
                        GameManager.Games
                    );
                }

                await DisplayAlert("Fin de partie", "La partie est terminée !", "OK");
                await Navigation.PopAsync();
            });
        }

    private async void OnErrorOccurred(object sender, ErrorOccurredEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            string errorMessage = e.Error?.Message ?? "Une erreur inconnue est survenue.";
            await DisplayAlert("Erreur", errorMessage, "OK");
            ResetSelectionState(); 
        });
    }


    private void OnPlayerChooseCoin(object sender, PlayerChooseCoinEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InstructionsLabel.Text = $"{e.Player.Name} a passé son tour";
            ResetSelectionState();
        });
    }

    private void OnPlayerChooseDuck(object sender, PlayerChooseDuckEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InstructionsLabel.Text = $"{e.Player.Name} effectue un déplacement Duck";
        });
    }

    private void OnPlayerChooseCover(object sender, PlayerChooseCoverEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InstructionsLabel.Text = $"{e.Player.Name} effectue un recouvrement";
        });
    }

    private async void OnPlayerChooseShowPlayersGrid(object sender, PlayerChooseShowPlayersGridEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var gridInfo = string.Join("\n", e.Players.Select(p => 
                $"{p.Name}: {p.Grid.GameCardsGrid.Count} cartes"));
            await DisplayAlert("Grilles des joueurs", gridInfo, "OK");
        });
    }

    private async void OnPlayerChooseShowScores(object sender, PlayerChooseShowScoresEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (e.Players == null || !e.Players.Any())
            {
                await DisplayAlert("Scores", "Aucun joueur à afficher les scores.", "OK");
                return;
            }
            var scoresInfo = string.Join("\n", e.Players.Where(p => p != null && p.Scores != null)
                .Select(p => $"{p.Name}: {p.Scores.LastOrDefault()} points (Total: {p.Scores.Sum()})"));
            await DisplayAlert("Scores", string.IsNullOrWhiteSpace(scoresInfo) ? "Aucune information de score disponible." : scoresInfo, "OK");
        });
    }

    private async void OnPlayerChooseQuit(object sender, PlayerChooseQuitEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (e.Player == null || e.CurrentGame == null) return;
            bool confirm = await DisplayAlert("Quitter", 
                $"{e.Player.Name} veut quitter la partie. Confirmer ?", 
                "Oui", "Non");
            
            if (confirm)
            {
                e.CurrentGame.Quit = true;
                if (GameManager.CheckGameOverCondition()) // CheckGameOverCondition devrait appeler OnGameIsOver si nécessaire
                {
                    // L'événement OnGameIsOver sera déclenché par CheckGameOverCondition si le jeu est terminé.
                }
            }
        });
    }

    private void OnDisplayMenuNeeded(object sender, DisplayMenuNeededEventArgs e)
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
    
    private void OnCardEffectProcessed(object sender, CardEffectProcessedEventArgs e)
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

            if (_isWaitingForDuckTarget && _selectedCard != null && _validDuckTargets != null && _validDuckTargets.Any())
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
            if (!distinctPositions.Any()) // Double vérification après distinct
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
                        if (_isWaitingForDuckTarget && _selectedCard != null && _validDuckTargets != null && _validDuckTargets.Contains(currentCellPosition))
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
            DebugLabel.Text = $"Cartes: {playerGrid.GameCardsGrid?.Count(c => c != null) ?? 0}, Cibles Duck: {_validDuckTargets?.Count ?? 0}";
        }
        catch (Exception ex)
        {
            DebugLabel.Text = $"Erreur chargement grille: {ex.Message}";
            GridInfoLabel.Text = "Erreur chargement grille";
        }
    }
    
    private View CreateValidDuckTargetCell(Position position)
    {
        var frame = new Frame
        {
            BorderColor = Colors.DodgerBlue,
            BackgroundColor = Colors.LightSkyBlue.WithAlpha(0.7f),
            CornerRadius = 10,
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
        frame.GestureRecognizers.Add(tapGesture);
        return frame;
    }
    
    private async void OnDuckTargetCellTapped(Position targetPosition)
    {
        try
        {
            if (_selectedCard == null || !_isWaitingForDuckTarget || GameManager.CurrentPlayer == null)
                return; 
            GameManager.HandlePlayerChooseDuck(GameManager.CurrentPlayer, _selectedCard.Position, targetPosition);
        }
        catch (Exception ex) 
        {
            await DisplayAlert("Erreur Duck Cible", $"Erreur: {ex.Message}", "OK");
            ResetSelectionState(); 
        }
    }

    private View CreateCardView(GameCard card)
    {
        if (card == null) throw new ArgumentNullException(nameof(card));
        var backgroundColor = GetSplashColor(card.Splash);
        var textColor = GetContrastingTextColor(backgroundColor);
        var frame = new Frame
        {
            BackgroundColor = backgroundColor,
            BorderColor = Colors.White, CornerRadius = 15,          
            WidthRequest = 100, HeightRequest = 130,
            HasShadow = true, Padding = 0                 
        };

        if (card == _selectedCard) 
        { frame.BorderColor = Colors.Yellow; frame.Scale = 1.1; }
        else
        { frame.BorderColor = Colors.White; frame.Scale = 1.0; }

        var mainGrid = new Microsoft.Maui.Controls.Grid(); 
        var numberLabel = new Label { Text = card.Number.ToString(), FontSize = 36, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, TextColor = textColor };
        var splashLabel = new Label { Text = card.Splash.ToString(), FontSize = 20, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.End, TextColor = textColor, Margin = new Thickness(0, 0, 8, 8) };
        var positionLabel = new Label { Text = $"{card.Position.Row},{card.Position.Column}", FontSize = 8,  HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, TextColor = textColor, Opacity = 0.7, Margin = new Thickness(4, 4, 0, 0) };
        mainGrid.Children.Add(numberLabel); mainGrid.Children.Add(splashLabel); mainGrid.Children.Add(positionLabel);
        frame.Content = mainGrid; 
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnCardTapped(card);
        frame.GestureRecognizers.Add(tapGesture);
        return frame;
    }

    private Color GetSplashColor(int splash) => splash switch { 0 => Color.FromArgb("#FFFFFF"), 1 => Color.FromArgb("#FFD700"), 2 => Color.FromArgb("#B8860B"), 3 => Color.FromArgb("#FF0000"), 4 => Color.FromArgb("#8B0000"), _ => Color.FromArgb("#FF00FF") };
    private Color GetContrastingTextColor(Color backgroundColor) => (0.299 * backgroundColor.Red + 0.587 * backgroundColor.Green + 0.114 * backgroundColor.Blue) > 0.5 ? Colors.Black : Colors.White;

    private async void OnCoinClicked(object sender, EventArgs e)
    {
        try
        {
            if (GameManager.CurrentPlayer == null) return;
            GameManager.HandlePlayerChoice(GameManager.CurrentPlayer, "3");
        }
        catch (Exception ex) { await DisplayAlert("Erreur Coin", ex.Message, "OK"); ResetSelectionState(); }
    }

    private async void OnCoverClicked(object sender, EventArgs e)
    {
        try
        {
            _isWaitingForCoverTarget = true; _isWaitingForDuckTarget = false; 
            _selectedCard = null; _cardToCover = null;
            _validDuckTargets.Clear(); 
            LoadGrid(); 
            InstructionsLabel.Text = "COVER: Sélectionnez la carte à DÉPLACER.";
        }
        catch (Exception ex) { await DisplayAlert("Erreur Cover Setup", ex.Message, "OK"); }
    }

    private async void OnDuckClicked(object sender, EventArgs e)
    {
        try
        {
            _isWaitingForCoverTarget = false; _isWaitingForDuckTarget = true;
            _selectedCard = null; 
            _validDuckTargets.Clear(); 
            LoadGrid(); 
            InstructionsLabel.Text = "DUCK: Sélectionnez la carte à DÉPLACER.";
        }
        catch (Exception ex) { await DisplayAlert("Erreur Duck Setup", ex.Message, "OK"); }
    }

    private async void OnCardTapped(GameCard card)
    {
        try
        {
            if (GameManager.CurrentDeckCard == null || GameManager.CurrentPlayer == null)
            {
                await DisplayAlert("Info", "Aucune carte de deck active ou joueur non défini.", "OK"); return;
            }
            int effectiveDeckCardNumber = GameManager.GetEffectiveDeckCardNumber(GameManager.CurrentPlayer, GameManager.CurrentDeckCard);

            if (_isWaitingForCoverTarget)
            {
                if (_selectedCard == null) 
                {
                    if (card.Number != effectiveDeckCardNumber)
                    { await DisplayAlert("Action invalide", $"La carte N°{card.Number} ne correspond pas au N°{effectiveDeckCardNumber} (effectif) du deck pour Cover.", "OK"); return; }
                    _selectedCard = card; LoadGrid(); 
                    InstructionsLabel.Text = "COVER: Sélectionnez la carte à RECOUVRIR.";
                }
                else 
                {
                    if (card == _selectedCard) 
                    { await DisplayAlert("Action invalide", "Vous ne pouvez pas recouvrir une carte avec elle-même.", "OK"); return; }
                    _cardToCover = card; 
                    GameManager.HandlePlayerChooseCover(GameManager.CurrentPlayer, _selectedCard.Position, _cardToCover.Position);
                }
            }
            else if (_isWaitingForDuckTarget)
            {
                if (_selectedCard == null) 
                {
                    if (card.Number != effectiveDeckCardNumber)
                    { await DisplayAlert("Action invalide", $"La carte N°{card.Number} (à déplacer) ne correspond pas au N°{effectiveDeckCardNumber} (effectif) du deck pour Duck.", "OK"); return; }
                    _selectedCard = card;
                    _validDuckTargets = GameManager.GetValidDuckTargetPositions(GameManager.CurrentPlayer, _selectedCard.Position, GameManager.CurrentDeckCard);
                    LoadGrid(); 
                    InstructionsLabel.Text = "DUCK: Sélectionnez la CASE de destination.";
                }
                else 
                {
                     await DisplayAlert("Action", "Sélectionnez une case vide en surbrillance pour 'Ducker', ou cliquez à nouveau sur 'Duck' pour changer la carte à déplacer.", "OK");
                }
            }
        }
        catch (Exception ex) { await DisplayAlert("Erreur Tap Carte", ex.Message, "OK"); ResetSelectionState(); }
    }

    private async void OnShowGridsClicked(object sender, EventArgs e)
    {
        try { if (GameManager.CurrentPlayer == null) return; GameManager.HandlePlayerChoice(GameManager.CurrentPlayer, "4"); }
        catch (Exception ex) { await DisplayAlert("Erreur Grilles", ex.Message, "OK"); }
    }

    private async void OnShowScoresClicked(object sender, EventArgs e)
    {
        try { if (GameManager.CurrentPlayer == null) return; GameManager.HandlePlayerChoice(GameManager.CurrentPlayer, "5"); }
        catch (Exception ex) { await DisplayAlert("Erreur Scores", ex.Message, "OK"); }
    }

    private async void OnQuitClicked(object sender, EventArgs e)
    {
        try { if (GameManager.CurrentPlayer == null) return; GameManager.HandlePlayerChoice(GameManager.CurrentPlayer, "6"); }
        catch (Exception ex) { await DisplayAlert("Erreur Quitter", ex.Message, "OK"); }
    }
    
    private void LoadCurrentCard()
    {
        try
        {
            if (GameManager?.CurrentDeckCard != null && GameManager.CurrentPlayer != null)
            {
                CurrentCardFrame.IsVisible = true;
                int effectiveNumber = GameManager.GetEffectiveDeckCardNumber(GameManager.CurrentPlayer, GameManager.CurrentDeckCard);
                string bonusText = ""; Color borderColor = Colors.SeaGreen; 
                switch (GameManager.CurrentDeckCard.Bonus)
                {
                    case Bonus.Again: borderColor = Colors.OrangeRed; bonusText = " (Again)"; break;
                    case Bonus.Max: borderColor = Colors.Purple; bonusText = " (Max)"; break;
                    case Bonus.None: break;
                    default: borderColor = Colors.DarkGoldenrod; bonusText = $" ({GameManager.CurrentDeckCard.Bonus})"; break;
                }
                CurrentCardBorder.BackgroundColor = borderColor;
                CurrentCardNumber.Text = effectiveNumber.ToString() + bonusText; 
                CurrentCardNumber.TextColor = Colors.White;
            }
            else { CurrentCardFrame.IsVisible = false; }
        }
        catch (Exception ex)
        {
            CurrentCardFrame.IsVisible = false;
            if(DebugLabel != null) DebugLabel.Text += $" | ErrCarte: {ex.Message.Substring(0, Math.Min(ex.Message.Length, 20))}";
        }
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (GameManager != null)
        {
            GameManager.PlayerChanged -= OnPlayerChanged;
            GameManager.GameIsOver -= OnGameIsOver;
            GameManager.ErrorOccurred -= OnErrorOccurred;
            GameManager.PlayerChooseCoin -= OnPlayerChooseCoin;
            GameManager.PlayerChooseShowPlayersGrid -= OnPlayerChooseShowPlayersGrid;
            GameManager.PlayerChooseShowScores -= OnPlayerChooseShowScores;
            GameManager.PlayerChooseQuit -= OnPlayerChooseQuit;
            GameManager.DisplayMenuNeeded -= OnDisplayMenuNeeded;
            GameManager.CardEffectProcessed -= OnCardEffectProcessed;
        }
    }
}