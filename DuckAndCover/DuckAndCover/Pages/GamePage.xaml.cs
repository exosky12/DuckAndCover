using Microsoft.Maui.Controls;
using Models.Game;
using Models.Enums;
using Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckAndCover.Pages;

public partial class GamePage : ContentPage
{
    public Game GameManager => (App.Current as App)?.GameManager ?? throw new InvalidOperationException("GameManager not initialized");

    private GameCard? _selectedCard;
    private GameCard? _cardToCover;
    private bool _isWaitingForCoverTarget = false;
    private bool _isWaitingForDuckTarget = false;

    public GamePage()
    {
        InitializeComponent();
        if (GameManager.CurrentPlayer == null && GameManager.Players.Any())
        {
            GameManager.CurrentPlayer = GameManager.Players.First();
        }
        LoadGrid();
        LoadCurrentCard();
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
            InstructionsLabel.Text = $"Tour de {e.CurrentPlayer.Name}";
            DebugLabel.Text = "";
            LoadGrid();
            LoadCurrentCard();
            ResetSelectionStatePartial();
        });
    }

    private void ResetSelectionStatePartial()
    {
        _isWaitingForCoverTarget = false;
        _isWaitingForDuckTarget = false;
        _selectedCard = null;
        _cardToCover = null;
    }

    private async void OnGameIsOver(object sender, GameIsOverEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            GameManager.SavePlayers();
            GameManager.SaveGame();
            
            await DisplayAlert("Fin de partie", "La partie est terminée !", "OK");
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        });
    }

    private async void OnErrorOccurred(object sender, ErrorOccurredEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await DisplayAlert("Erreur", e.Error.Message, "OK");
            ResetSelectionState();
        });
    }

    private void OnPlayerChooseCoin(object sender, PlayerChooseCoinEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InstructionsLabel.Text = $"{e.Player.Name} a passé son tour (Coin).";
            ResetSelectionState();
        });
    }

    private void OnPlayerChooseDuck(object sender, PlayerChooseDuckEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InstructionsLabel.Text = $"{e.Player.Name}: Déplacement Duck. Sélectionnez la carte à déplacer.";
        });
    }

    private void OnPlayerChooseCover(object sender, PlayerChooseCoverEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InstructionsLabel.Text = $"{e.Player.Name}: Recouvrement. Sélectionnez la carte à déplacer.";
        });
    }

    private async void OnPlayerChooseShowPlayersGrid(object sender, PlayerChooseShowPlayersGridEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var gridInfo = string.Join("\n", e.Players.Select(p => 
                $"{p.Name}: {p.Grid.GameCardsGrid.Count} cartes, Stack: {p.StackCounter}"));
            await DisplayAlert("Grilles des joueurs", gridInfo, "OK");
        });
    }

    private async void OnPlayerChooseShowScores(object sender, PlayerChooseShowScoresEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var scoresInfo = string.Join("\n", e.Players.Select(p => 
                $"{p.Name}: {p.Scores.LastOrDefault()} points (Total: {p.Scores.Sum()})"));
            await DisplayAlert("Scores", scoresInfo, "OK");
        });
    }

    private async void OnPlayerChooseQuit(object sender, PlayerChooseQuitEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            bool confirm = await DisplayAlert("Quitter", 
                $"{e.Player.Name} veut quitter la partie. Confirmer ?", 
                "Oui", "Non");
            
            if (confirm)
            {
                e.CurrentGame.Quit = true;
                if (GameManager.CheckGameOverCondition())
                {
                }
            }
        });
    }

    private void OnDisplayMenuNeeded(object sender, DisplayMenuNeededEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            InstructionsLabel.Text = $"Tour de {e.CurrentPlayer.Name}. Choisissez une action.";
            LoadCurrentCard();
        });
    }

    private void ResetSelectionState()
    {
        _isWaitingForCoverTarget = false;
        _isWaitingForDuckTarget = false;
        _selectedCard = null;
        _cardToCover = null;
        LoadGrid();
        LoadCurrentCard();
        if (GameManager.CurrentPlayer != null)
        {
             InstructionsLabel.Text = $"Tour de {GameManager.CurrentPlayer.Name}. Choisissez une action.";
        }
    }

    private void LoadGrid()
    {
        try
        {
            if (GameManager?.Players == null || !GameManager.Players.Any() || GameManager.CurrentPlayer == null)
            {
                DebugLabel.Text = "Joueur non défini ou aucun joueur.";
                GameGrid.Children.Clear();
                GridInfoLabel.Text = "Aucun joueur actif";
                return;
            }

            var currentPlayer = GameManager.CurrentPlayer;
            var playerGrid = currentPlayer.Grid;

            if (playerGrid?.GameCardsGrid == null || !playerGrid.GameCardsGrid.Any())
            {
                DebugLabel.Text = $"Aucune carte trouvée dans la grille de {currentPlayer.Name}";
                GameGrid.Children.Clear();
                GameGrid.RowDefinitions.Clear();
                GameGrid.ColumnDefinitions.Clear();
                GridInfoLabel.Text = $"Grille de {currentPlayer.Name} (vide)";
                return;
            }

            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();

            var positions = playerGrid.GameCardsGrid.Select(c => c.Position).ToList();
            if (!positions.Any())
            {
                 GridInfoLabel.Text = $"Grille de {currentPlayer.Name} (vide après positions)";
                 return;
            }
            var bounds = Models.Game.Grid.GetBounds(positions);

            int gridWidth = bounds.maxX - bounds.minX + 1;
            int gridHeight = bounds.maxY - bounds.minY + 1;

            for (int i = 0; i < gridHeight; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(140) });
            }
            for (int i = 0; i < gridWidth; i++)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });
            }

            foreach (var card in playerGrid.GameCardsGrid)
            {
                if (card == null) continue;
                var cardView = CreateCardView(card);
                int relativeRow = card.Position.Row - bounds.minY;
                int relativeCol = card.Position.Column - bounds.minX;
                Microsoft.Maui.Controls.Grid.SetRow(cardView, relativeRow);
                Microsoft.Maui.Controls.Grid.SetColumn(cardView, relativeCol);
                GameGrid.Children.Add(cardView);
            }

            DebugLabel.Text = $"Grille de {currentPlayer.Name}: {gridWidth}x{gridHeight}, {playerGrid.GameCardsGrid.Count} cartes";
            GridInfoLabel.Text = $"Grille de {currentPlayer.Name}";
        }
        catch (Exception ex)
        {
            DebugLabel.Text = $"Erreur chargement grille: {ex.Message} {ex.StackTrace}";
             if (GameManager.CurrentPlayer != null)
             {
                GridInfoLabel.Text = $"Erreur grille {GameManager.CurrentPlayer.Name}";
             } else {
                GridInfoLabel.Text = "Erreur grille";
             }
        }
    }

    // ############### MODIFICATION ICI POUR REVENIR A L'ANCIEN STYLE ###############
    private View CreateCardView(GameCard card)
    {
        if (card == null) throw new ArgumentNullException(nameof(card));

        var backgroundColor = GetSplashColor(card.Splash);
        var textColor = GetContrastingTextColor(backgroundColor);

        var frame = new Frame
        {
            BackgroundColor = backgroundColor,
            BorderColor = Colors.White, // Style original
            CornerRadius = 15,          // Style original
            WidthRequest = 100,
            HeightRequest = 130,
            HasShadow = true,
            Padding = 0                 // Style original
        };

        if (card == _selectedCard)
        {
            frame.BorderColor = Colors.Yellow;
            frame.Scale = 1.1; // Augmenté un peu par rapport à 1.05, mais peut être ajusté
        }

        var mainGrid = new Microsoft.Maui.Controls.Grid(); // Utilisation de Grid

        var numberLabel = new Label
        {
            Text = card.Number.ToString(),
            FontSize = 36, // Style original
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            TextColor = textColor
        };

        var splashLabel = new Label
        {
            Text = card.Splash.ToString(), // Style original (juste le numéro)
            FontSize = 20, // Style original
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.End, // Style original
            VerticalOptions = LayoutOptions.End,   // Style original
            TextColor = textColor,
            Margin = new Thickness(0, 0, 8, 8) // Style original
        };

        var positionLabel = new Label
        {
            Text = $"{card.Position.Row},{card.Position.Column}", // Style original
            FontSize = 8,  // Style original
            HorizontalOptions = LayoutOptions.Start, // Style original
            VerticalOptions = LayoutOptions.Start,   // Style original
            TextColor = textColor,
            Opacity = 0.7, // Style original
            Margin = new Thickness(4, 4, 0, 0) // Style original
        };

        mainGrid.Children.Add(numberLabel);
        mainGrid.Children.Add(splashLabel);
        mainGrid.Children.Add(positionLabel);

        frame.Content = mainGrid; // Affectation à mainGrid

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnCardTapped(card);
        frame.GestureRecognizers.Add(tapGesture);

        return frame;
    }
    // ############### FIN DE LA MODIFICATION DU STYLE ###############

    private Color GetSplashColor(int splash)
    {
        return splash switch
        {
            0 => Color.FromArgb("#FFFFFF"), 
            1 => Color.FromArgb("#FFD700"), 
            2 => Color.FromArgb("#B8860B"), 
            3 => Color.FromArgb("#FF0000"), 
            4 => Color.FromArgb("#8B0000"), 
            _ => Color.FromArgb("#FF00FF")
        };
    }

    private Color GetContrastingTextColor(Color backgroundColor)
    {
        var luminance = 0.299 * backgroundColor.Red + 0.587 * backgroundColor.Green + 0.114 * backgroundColor.Blue;
        return luminance > 0.5 ? Colors.Black : Colors.White;
    }

    private async void OnCoinClicked(object sender, EventArgs e)
    {
        try
        {
            GameManager.HandlePlayerChoice(GameManager.CurrentPlayer, "3");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur Coin", ex.Message, "OK");
            ResetSelectionState();
        }
    }

    private async void OnCoverClicked(object sender, EventArgs e)
    {
        try
        {
            _isWaitingForCoverTarget = true;
            _isWaitingForDuckTarget = false;
            _selectedCard = null;
            _cardToCover = null;
            LoadGrid();
            InstructionsLabel.Text = "COVER: Sélectionnez la carte à DÉPLACER.";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur Cover Setup", ex.Message, "OK");
        }
    }

    private async void OnDuckClicked(object sender, EventArgs e)
    {
        try
        {
            _isWaitingForCoverTarget = false;
            _isWaitingForDuckTarget = true;
            _selectedCard = null;
            LoadGrid();
            InstructionsLabel.Text = "DUCK: Sélectionnez la carte à DÉPLACER.";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur Duck Setup", ex.Message, "OK");
        }
    }

    private async void OnCardTapped(GameCard card)
    {
        try
        {
            if (GameManager.CurrentDeckCard == null)
            {
                await DisplayAlert("Info", "Aucune carte de deck active.", "OK");
                return;
            }
            int effectiveDeckCardNumber = GameManager.GetEffectiveDeckCardNumber(GameManager.CurrentPlayer, GameManager.CurrentDeckCard);

            if (_isWaitingForCoverTarget)
            {
                if (_selectedCard == null)
                {
                    if (card.Number != effectiveDeckCardNumber)
                    {
                        await DisplayAlert("Action invalide", $"La carte N°{card.Number} ne correspond pas au N°{effectiveDeckCardNumber} (effectif) du deck.", "OK");
                        return;
                    }
                    _selectedCard = card;
                    LoadGrid();
                    InstructionsLabel.Text = "COVER: Sélectionnez la carte à RECOUVRIR.";
                }
                else
                {
                    if (card == _selectedCard) {
                        await DisplayAlert("Action invalide", "Vous ne pouvez pas recouvrir une carte avec elle-même.", "OK");
                        return;
                    }
                    _cardToCover = card;
                    GameManager.HandlePlayerChooseCover(GameManager.CurrentPlayer, _selectedCard.Position, _cardToCover.Position);
                    if (GameManager.CurrentPlayer.HasPlayed) ResetSelectionState();
                }
            }
            else if (_isWaitingForDuckTarget)
            {
                if (_selectedCard == null)
                {
                    if (card.Number != effectiveDeckCardNumber)
                    {
                        await DisplayAlert("Action invalide", $"La carte N°{card.Number} ne correspond pas au N°{effectiveDeckCardNumber} (effectif) du deck.", "OK");
                        return;
                    }
                    _selectedCard = card;
                    LoadGrid();
                    InstructionsLabel.Text = "DUCK: Sélectionnez la CASE de destination (peut être une carte existante si la règle le permet, ou une case vide).";
                }
                else
                {
                    GameManager.HandlePlayerChooseDuck(GameManager.CurrentPlayer, _selectedCard.Position, card.Position);
                    if (GameManager.CurrentPlayer.HasPlayed) ResetSelectionState();
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur Tap", ex.Message, "OK");
            ResetSelectionState();
        }
    }

    private async void OnShowGridsClicked(object sender, EventArgs e)
    {
        try
        {
            GameManager.HandlePlayerChoice(GameManager.CurrentPlayer, "4");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private async void OnShowScoresClicked(object sender, EventArgs e)
    {
        try
        {
            GameManager.HandlePlayerChoice(GameManager.CurrentPlayer, "5");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private async void OnQuitClicked(object sender, EventArgs e)
    {
        try
        {
            GameManager.HandlePlayerChoice(GameManager.CurrentPlayer, "6");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }
    
    private void OnCardEffectProcessed(object sender, CardEffectProcessedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            InstructionsLabel.Text = e.Message;
            LoadCurrentCard();
        
            if (e.ProcessedCard.Bonus == Bonus.Again || e.ProcessedCard.Bonus == Bonus.Max)
            {
                await DisplayAlert("Effet de carte!", e.Message, "OK");
            }
        });
    }
    
    private void LoadCurrentCard()
    {
        try
        {
            if (GameManager?.CurrentDeckCard != null && GameManager.CurrentPlayer != null)
            {
                CurrentCardFrame.IsVisible = true;
                int effectiveNumber = GameManager.GetEffectiveDeckCardNumber(GameManager.CurrentPlayer, GameManager.CurrentDeckCard);
                CurrentCardNumber.Text = effectiveNumber.ToString();
            
                switch (GameManager.CurrentDeckCard.Bonus)
                {
                    case Bonus.Again:
                        CurrentCardBorder.BackgroundColor = Colors.OrangeRed;
                        break;
                    case Bonus.Max:
                        CurrentCardBorder.BackgroundColor = Colors.Purple;
                        break;
                    default:
                        CurrentCardBorder.BackgroundColor = Colors.SeaGreen;
                        break;
                }
            
                CurrentCardNumber.TextColor = Colors.White;
            }
            else
            {
                CurrentCardFrame.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            CurrentCardFrame.IsVisible = false;
            DebugLabel.Text += $" | Erreur Carte Actuelle: {ex.Message}";
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
            GameManager.PlayerChooseDuck -= OnPlayerChooseDuck;
            GameManager.PlayerChooseCover -= OnPlayerChooseCover;
            GameManager.PlayerChooseShowPlayersGrid -= OnPlayerChooseShowPlayersGrid;
            GameManager.PlayerChooseShowScores -= OnPlayerChooseShowScores;
            GameManager.PlayerChooseQuit -= OnPlayerChooseQuit;
            GameManager.DisplayMenuNeeded -= OnDisplayMenuNeeded;
        
            GameManager.CardEffectProcessed -= OnCardEffectProcessed;
        }
    }
}