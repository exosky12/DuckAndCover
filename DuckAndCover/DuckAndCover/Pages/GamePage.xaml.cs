using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Models.Enums;
using Models.Events;

// **Ne pas importer `using Models.Game;`**, pour éviter l’ambiguïté avec Grid.
// Toute référence aux types du domaine Game (Game, GameCard, Position, Grid…) 
// sera qualifiée par le namespace complet `Models.Game`.

namespace DuckAndCover.Pages
{
    public partial class GamePage : ContentPage
    {
        // Accès au GameManager global (initialisé dans App.xaml.cs)
        public Models.Game.Game GameManager
            => (App.Current as App)?.GameManager
               ?? throw new InvalidOperationException("GameManager not initialized");

        private Models.Game.GameCard? _selectedCard;
        private Models.Game.GameCard? _cardToCover;
        private bool _isWaitingForCoverTarget = false;
        private bool _isWaitingForDuckTarget = false;

        public GamePage()
        {
            InitializeComponent();

            // Charger l’état initial de la grille et de la carte courante
            LoadGrid();
            LoadCurrentCard();

            // S’abonner aux événements du GameManager
            SubscribeToGameEvents();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Démarrer la boucle de jeu dans un thread de fond
            Task.Run(() =>
            {
                try
                {
                    GameManager.GameLoop();
                }
                catch
                {
                    // Ignorer toute exception non prévue ici
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Se désabonner pour éviter les fuites mémoire
            GameManager.PlayerChanged -= OnPlayerChanged;
            GameManager.GameIsOver -= OnGameIsOver;
            GameManager.ErrorOccurred -= OnErrorOccurred;
        }

        private void SubscribeToGameEvents()
        {
            GameManager.PlayerChanged += OnPlayerChanged;
            GameManager.GameIsOver += OnGameIsOver;
            GameManager.ErrorOccurred += OnErrorOccurred;
        }

        private void OnPlayerChanged(object? sender, PlayerChangedEventArgs e)
        {
            // Appelé depuis GameLoop (thread de fond). Pour mettre à jour l’UI :
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadGrid();
                LoadCurrentCard();
                InstructionsLabel.Text = $"Tour de {e.CurrentPlayer.Name}";
                DebugLabel.Text = "";
            });
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
                await DisplayAlert("Erreur", e.Error.Message, "OK");
            });
        }

        private void LoadGrid()
        {
            try
            {
                // 1) Récupérer le joueur courant et sa grille métier
                var currentPlayer = GameManager.CurrentPlayer;
                var playerGrid = currentPlayer.Grid; // Models.Game.Grid

                // Mettre à jour la carte courante affichée
                LoadCurrentCard();

                if (playerGrid == null || playerGrid.GameCardsGrid == null
                    || !playerGrid.GameCardsGrid.Any())
                {
                    DebugLabel.Text = $"Aucune carte dans la grille de {currentPlayer.Name}";
                    return;
                }

                // 2) Vider la grille MAUI existante
                GameGrid.Children.Clear();
                GameGrid.RowDefinitions.Clear();
                GameGrid.ColumnDefinitions.Clear();

                // 3) Calculer les bornes de la grille métier
                var positions = playerGrid.GameCardsGrid.Select(c => c.Position).ToList();
                var bounds = Models.Game.Grid.GetBounds(positions);

                int gridWidth = bounds.maxX - bounds.minY + 1;
                int gridHeight = bounds.maxY - bounds.minY + 1;

                // 4) Ajouter les RowDefinitions en MAUI
                for (int i = 0; i < gridHeight; i++)
                    GameGrid.RowDefinitions.Add(
                        new RowDefinition { Height = new GridLength(140) }
                    );

                // 5) Ajouter les ColumnDefinitions en MAUI
                for (int j = 0; j < gridWidth; j++)
                    GameGrid.ColumnDefinitions.Add(
                        new ColumnDefinition { Width = new GridLength(110) }
                    );

                // 6) Parcourir toutes les cartes métier et les afficher dans la grille MAUI
                foreach (var card in playerGrid.GameCardsGrid)
                {
                    if (card == null) continue;

                    var cardView = CreateCardView(card);

                    int relativeRow = card.Position.Row - bounds.minY;
                    int relativeCol = card.Position.Column - bounds.minX;

                    // Positionner dans la grille MAUI 
                    Microsoft.Maui.Controls.Grid.SetRow(cardView, relativeRow);
                    Microsoft.Maui.Controls.Grid.SetColumn(cardView, relativeCol);

                    GameGrid.Children.Add(cardView);
                }

                DebugLabel.Text = $"Grille de {currentPlayer.Name} : " +
                                  $"{gridWidth}×{gridHeight}, " +
                                  $"{playerGrid.GameCardsGrid.Count} cartes";
                GridInfoLabel.Text = $"Grille de {currentPlayer.Name}";
            }
            catch (Exception ex)
            {
                DebugLabel.Text = $"Erreur lors du chargement : {ex.Message}";
            }
        }

        private void LoadCurrentCard()
        {
            try
            {
                var deckCard = GameManager.CurrentDeckCard;
                if (deckCard != null)
                {
                    CurrentCardFrame.IsVisible = true;
                    CurrentCardNumber.Text = deckCard.Number.ToString();
                    CurrentCardBorder.BackgroundColor = Color.FromArgb("#4CAF50");
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

        private View CreateCardView(Models.Game.GameCard card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            var backgroundColor = GetSplashColor(card.Splash);
            var textColor = GetContrastingTextColor(backgroundColor);

            var frame = new Frame
            {
                BackgroundColor = backgroundColor,
                BorderColor = (card == _selectedCard ? Colors.Yellow : Colors.White),
                CornerRadius = 15,
                WidthRequest = 100,
                HeightRequest = 130,
                HasShadow = true,
                Padding = 0,
                Scale = (card == _selectedCard ? 1.1 : 1.0)
            };

            var mainGrid = new Grid();

            // Numéro
            var numberLabel = new Label
            {
                Text = card.Number.ToString(),
                FontSize = 36,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = textColor
            };

            // Splash
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

            // Position pour debug
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

            // Gestionnaire de tap
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnCardTapped(card);
            frame.GestureRecognizers.Add(tapGesture);

            return frame;
        }

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

        private Color GetContrastingTextColor(Color background)
        {
            var luminance = (0.299 * background.Red +
                             0.587 * background.Green +
                             0.114 * background.Blue);
            return luminance > 0.5 ? Colors.Black : Colors.White;
        }

        private async void OnCoinClicked(object sender, EventArgs e)
        {
            try
            {
                _isWaitingForCoverTarget = false;
                _isWaitingForDuckTarget = false;
                _selectedCard = null;
                _cardToCover = null;

                // "3" correspond à "Coin" dans HandlePlayerChoice
                GameManager.HandlePlayerChoice(GameManager.CurrentPlayer, "3");
                InstructionsLabel.Text = "Tour passé";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
            }
        }

        private void OnCoverClicked(object sender, EventArgs e)
        {
            // Activer le mode "Cover"
            _isWaitingForCoverTarget = true;
            _isWaitingForDuckTarget = false;
            _selectedCard = null;
            _cardToCover = null;

            LoadGrid();
            InstructionsLabel.Text = "Sélectionnez la carte à déplacer";
        }

        private void OnDuckClicked(object sender, EventArgs e)
        {
            // Activer le mode "Duck"
            _isWaitingForCoverTarget = false;
            _isWaitingForDuckTarget = true;
            _selectedCard = null;
            _cardToCover = null;

            LoadGrid();
            InstructionsLabel.Text = "Sélectionnez la carte à déplacer";
        }

        private async void OnCardTapped(Models.Game.GameCard card)
        {
            try
            {
                if (!_isWaitingForCoverTarget && !_isWaitingForDuckTarget)
                    return;

                // Logique Cover
                if (_isWaitingForCoverTarget)
                {
                    if (_selectedCard == null)
                    {
                        _selectedCard = card;
                        InstructionsLabel.Text = "Sélectionnez la carte ADJACENTE à recouvrir";
                    }
                    else
                    {
                        _cardToCover = card;
                        bool areAdjacent = AreAdjacent(
                            _selectedCard.Position,
                            _cardToCover.Position
                        );
                        if (!areAdjacent)
                        {
                            await DisplayAlert("Erreur",
                                "La carte à recouvrir n'est pas adjacente à la carte sélectionnée.",
                                "OK");
                            _selectedCard = null;
                            _cardToCover = null;
                            InstructionsLabel.Text = "Sélectionnez à nouveau la carte à déplacer";
                            return;
                        }

                        if (_selectedCard.Number != GameManager.CurrentDeckCard.Number)
                        {
                            await DisplayAlert("Erreur",
                                "La carte à déplacer ne correspond pas au numéro courant du deck.",
                                "OK");
                            _selectedCard = null;
                            _cardToCover = null;
                            InstructionsLabel.Text = "Sélectionnez à nouveau la carte à déplacer";
                            return;
                        }

                        try
                        {
                            GameManager.HandlePlayerChooseCover(
                                GameManager.CurrentPlayer,
                                _selectedCard.Position,
                                _cardToCover.Position
                            );
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Erreur", ex.Message, "OK");
                        }
                        finally
                        {
                            _isWaitingForCoverTarget = false;
                            _selectedCard = null;
                            _cardToCover = null;
                            InstructionsLabel.Text = "";
                            LoadGrid();
                        }
                    }
                }
                // Logique Duck
                else if (_isWaitingForDuckTarget)
                {
                    if (_selectedCard == null)
                    {
                        _selectedCard = card;
                        InstructionsLabel.Text = "Sélectionnez une CASE ADJACENTE pour déplacer";
                    }
                    else
                    {
                        var targetPos = card.Position;
                        bool areAdjacent = AreAdjacent(
                            _selectedCard.Position,
                            targetPos
                        );
                        if (!areAdjacent)
                        {
                            await DisplayAlert("Erreur",
                                "La case n'est pas adjacente à la carte sélectionnée.",
                                "OK");
                            _selectedCard = null;
                            InstructionsLabel.Text = "Sélectionnez à nouveau la carte à déplacer";
                            return;
                        }

                        if (_selectedCard.Number != GameManager.CurrentDeckCard.Number)
                        {
                            await DisplayAlert("Erreur",
                                "La carte à déplacer ne correspond pas au numéro courant du deck.",
                                "OK");
                            _selectedCard = null;
                            InstructionsLabel.Text = "Sélectionnez à nouveau la carte à déplacer";
                            return;
                        }

                        try
                        {
                            GameManager.HandlePlayerChooseDuck(
                                GameManager.CurrentPlayer,
                                _selectedCard.Position,
                                targetPos
                            );
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Erreur", ex.Message, "OK");
                        }
                        finally
                        {
                            _isWaitingForDuckTarget = false;
                            _selectedCard = null;
                            InstructionsLabel.Text = "";
                            LoadGrid();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur inattendue", ex.Message, "OK");
                _isWaitingForCoverTarget = false;
                _isWaitingForDuckTarget = false;
                _selectedCard = null;
                _cardToCover = null;
                InstructionsLabel.Text = "";
                LoadGrid();
            }
        }

        /// <summary>
        /// Vérifie si deux positions sont adjacentes (haut/bas/gauche/droite).
        /// </summary>
        private bool AreAdjacent(Models.Game.Position a, Models.Game.Position b)
        {
            int dx = Math.Abs(a.Row - b.Row);
            int dy = Math.Abs(a.Column - b.Column);
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }
    }
}
