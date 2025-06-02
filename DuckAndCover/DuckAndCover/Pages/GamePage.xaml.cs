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
        LoadGrid();
        SubscribeToGameEvents();
        StartGame();
    }

    private void StartGame()
    {
        // Démarrer le jeu (pas de boucle infinie, juste l'initialisation)
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
            LoadGrid();
            LoadCurrentCard();
            InstructionsLabel.Text = $"Tour de {e.CurrentPlayer.Name}";
            DebugLabel.Text = "";
            
            // Réinitialiser les états de sélection
            ResetSelectionState();
        });
    }

    private async void OnGameIsOver(object sender, GameIsOverEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            // Sauvegarder les joueurs et la partie
            GameManager.SavePlayers();
            GameManager.SaveGame();
            
            await DisplayAlert("Fin de partie", "La partie est terminée !", "OK");
            await Navigation.PopAsync();
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
                await Navigation.PopAsync();
            }
        });
    }

    private void OnDisplayMenuNeeded(object sender, DisplayMenuNeededEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Actualiser l'affichage après une demande de menu
            LoadGrid();
            LoadCurrentCard();
            InstructionsLabel.Text = $"Tour de {e.CurrentPlayer.Name}";
        });
    }


    private void ResetSelectionState()
    {
        _isWaitingForCoverTarget = false;
        _isWaitingForDuckTarget = false;
        _selectedCard = null;
        _cardToCover = null;
        LoadGrid(); // Recharger pour enlever les surlignages
    }

    private void LoadGrid()
    {
        try
        {
            // Vérifier qu'il y a au moins un joueur
            if (GameManager?.Players == null || !GameManager.Players.Any())
            {
                DebugLabel.Text = "Aucun joueur trouvé";
                return;
            }

            // Prendre la grille du joueur courant
            var currentPlayer = GameManager.CurrentPlayer;
            var playerGrid = currentPlayer.Grid;

            // Afficher la carte actuelle du deck
            LoadCurrentCard();

            if (playerGrid?.GameCardsGrid == null || !playerGrid.GameCardsGrid.Any())
            {
                DebugLabel.Text = $"Aucune carte trouvée dans la grille de {currentPlayer.Name}";
                return;
            }

            // Effacer la grille existante
            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();

            // Obtenir les positions de toutes les cartes du joueur
            var positions = playerGrid.GameCardsGrid.Select(c => c.Position).ToList();
            var bounds = Models.Game.Grid.GetBounds(positions);

            // Calculer les dimensions de la grille
            int gridWidth = bounds.maxX - bounds.minX + 1;
            int gridHeight = bounds.maxY - bounds.minY + 1;

            // Définir les lignes et colonnes avec des tailles plus grandes
            for (int i = 0; i < gridHeight; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(140) });
            }

            for (int i = 0; i < gridWidth; i++)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });
            }

            // Ajouter les cartes à la grille
            foreach (var card in playerGrid.GameCardsGrid)
            {
                if (card == null) continue;
                
                var cardView = CreateCardView(card);
                
                // Calculer les positions relatives à partir des bounds
                int relativeRow = card.Position.Row - bounds.minY;
                int relativeCol = card.Position.Column - bounds.minX;

                Microsoft.Maui.Controls.Grid.SetRow(cardView, relativeRow);
                Microsoft.Maui.Controls.Grid.SetColumn(cardView, relativeCol);
                
                GameGrid.Children.Add(cardView);
            }

            // Mettre à jour les informations de debug
            DebugLabel.Text = $"Grille de {currentPlayer.Name}: {gridWidth}x{gridHeight}, {playerGrid.GameCardsGrid.Count} cartes";
            GridInfoLabel.Text = $"Grille de {currentPlayer.Name}";
        }
        catch (Exception ex)
        {
            DebugLabel.Text = $"Erreur lors du chargement: {ex.Message}";
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
            BorderColor = Colors.White,
            CornerRadius = 15,
            WidthRequest = 100,
            HeightRequest = 130,
            HasShadow = true,
            Padding = 0
        };

        // Si c'est la carte sélectionnée, ajouter un style distinctif
        if (card == _selectedCard)
        {
            frame.BorderColor = Colors.Yellow;
            frame.Scale = 1.1;
        }

        var mainGrid = new Microsoft.Maui.Controls.Grid();

        // Numéro principal au centre en gros
        var numberLabel = new Label
        {
            Text = card.Number.ToString(),
            FontSize = 36,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            TextColor = textColor
        };

        // Splash en bas à droite
        var splashLabel = new Label
        {
            Text = card.Splash.ToString(),
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            TextColor = textColor,
            Margin = new Thickness(0, 0, 8, 8)
        };

        // Position pour debug (plus petit et discret)
        var positionLabel = new Label
        {
            Text = $"{card.Position.Row},{card.Position.Column}",
            FontSize = 8,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            TextColor = textColor,
            Opacity = 0.7,
            Margin = new Thickness(4, 4, 0, 0)
        };

        mainGrid.Children.Add(numberLabel);
        mainGrid.Children.Add(splashLabel);
        mainGrid.Children.Add(positionLabel);

        frame.Content = mainGrid;

        // Ajouter un gestionnaire de tap
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnCardTapped(card);
        frame.GestureRecognizers.Add(tapGesture);

        return frame;
    }

    private Color GetSplashColor(int splash)
    {
        return splash switch
        {
            0 => Color.FromArgb("#FFFFFF"), // Blanc
            1 => Color.FromArgb("#FFD700"), // Jaune
            2 => Color.FromArgb("#B8860B"), // Jaune foncé
            3 => Color.FromArgb("#FF0000"), // Rouge
            4 => Color.FromArgb("#8B0000"), // Rouge foncé
            _ => Color.FromArgb("#FF00FF")  // Magenta
        };
    }

    private Color GetContrastingTextColor(Color backgroundColor)
    {
        // Convertir la couleur en valeurs RGB
        var r = backgroundColor.Red;
        var g = backgroundColor.Green;
        var b = backgroundColor.Blue;

        // Calculer la luminosité relative
        var luminance = (0.299 * r + 0.587 * g + 0.114 * b);

        // Retourner noir ou blanc selon la luminosité
        return luminance > 0.5 ? Colors.Black : Colors.White;
    }

    private async void OnCoinClicked(object sender, EventArgs e)
    {
        try
        {
            ResetSelectionState();
            GameManager.HandlePlayerChoice(GameManager.CurrentPlayer, "3");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
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
            InstructionsLabel.Text = "Sélectionnez la carte à déplacer";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private async void OnDuckClicked(object sender, EventArgs e)
    {
        try
        {
            _isWaitingForCoverTarget = false;
            _isWaitingForDuckTarget = true;
            _selectedCard = null;
            _cardToCover = null;
            LoadGrid();
            InstructionsLabel.Text = "Sélectionnez la carte à déplacer";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private async void OnCardTapped(GameCard card)
{
    try
    {
        // Si aucune action n'est en cours, ignorer le tap
        if (!_isWaitingForCoverTarget && !_isWaitingForDuckTarget)
        {
            return;
        }

        if (_isWaitingForCoverTarget)
        {
            if (_selectedCard == null)
            {
                // Première sélection : la carte à déplacer - DOIT correspondre au deck
                if (GameManager.CurrentDeckCard == null || card.Number != GameManager.CurrentDeckCard.Number)
                {
                    await DisplayAlert("Erreur", "Cette carte ne correspond pas à la carte courante du deck", "OK");
                    return;
                }
                
                _selectedCard = card;
                InstructionsLabel.Text = "Sélectionnez la carte à recouvrir";
                LoadGrid(); // Recharger pour montrer la sélection
            }
            else
            {
                // Deuxième sélection : la carte à recouvrir - PAS de vérification du deck
                _cardToCover = card;
                try
                {
                    GameManager.HandlePlayerChooseCover(
                        GameManager.CurrentPlayer,
                        _selectedCard.Position,
                        _cardToCover.Position
                    );
                    ResetSelectionState();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", ex.Message, "OK");
                    ResetSelectionState();
                }
            }
        }
        else if (_isWaitingForDuckTarget)
        {
            if (_selectedCard == null)
            {
                // Première sélection : la carte à déplacer - DOIT correspondre au deck
                if (GameManager.CurrentDeckCard == null || card.Number != GameManager.CurrentDeckCard.Number)
                {
                    await DisplayAlert("Erreur", "Cette carte ne correspond pas à la carte courante du deck", "OK");
                    return;
                }
                
                _selectedCard = card;
                InstructionsLabel.Text = "Sélectionnez la position où déplacer la carte";
                LoadGrid(); // Recharger pour montrer la sélection
            }
            else
            {
                // Deuxième sélection : la position de destination - PAS de vérification du deck
                try
                {
                    GameManager.HandlePlayerChooseDuck(
                        GameManager.CurrentPlayer,
                        _selectedCard.Position,
                        card.Position
                    );
                    ResetSelectionState();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", ex.Message, "OK");
                    ResetSelectionState();
                }
            }
        }
    }
    catch (Exception ex)
    {
        await DisplayAlert("Erreur", ex.Message, "OK");
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
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Afficher le message d'effet dans le label d'instructions
            InstructionsLabel.Text = e.Message;
        
            // Actualiser l'affichage de la carte courante
            LoadCurrentCard();
        
            // Optionnel : afficher une alerte pour les effets importants
            if (e.ProcessedCard.Bonus == Bonus.Again || e.ProcessedCard.Bonus == Bonus.Max)
            {
                DisplayAlert("Effet de carte", e.Message, "OK");
            }
        });
    }
    
    private void LoadCurrentCard()
    {
        try
        {
            if (GameManager?.CurrentDeckCard != null)
            {
                CurrentCardFrame.IsVisible = true;
            
                // Afficher le numéro (qui peut avoir été modifié par les effets)
                CurrentCardNumber.Text = GameManager.CurrentDeckCard.Number.ToString();
            
                // Changer la couleur selon le bonus
                switch (GameManager.CurrentDeckCard.Bonus)
                {
                    case Bonus.Again:
                        CurrentCardBorder.BackgroundColor = Color.FromArgb("#FF9800"); // Orange
                        break;
                    case Bonus.Max:
                        CurrentCardBorder.BackgroundColor = Color.FromArgb("#9C27B0"); // Violet
                        break;
                    default:
                        CurrentCardBorder.BackgroundColor = Color.FromArgb("#4CAF50"); // Vert
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
            DebugLabel.Text += $" | CurrentCard error: {ex.Message}";
        }
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
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