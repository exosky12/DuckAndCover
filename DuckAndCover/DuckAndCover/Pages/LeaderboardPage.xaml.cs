using Microsoft.Maui.Controls;
using Models.Game;
using System.Collections.ObjectModel;
using Grid = Microsoft.Maui.Controls.Grid;

namespace DuckAndCover.Pages;

public partial class LeaderboardPage : ContentPage
{
    public Game GameManager => (App.Current as App)?.GameManager ?? throw new InvalidOperationException("GameManager not initialized");

    public LeaderboardPage()
    {
        InitializeComponent();
        LoadLeaderboard();
    }

    private void LoadLeaderboard()
    {
        try
        {
            // Effacer la liste existante
            LeaderboardList.Children.Clear();

            // Créer une liste de joueurs avec leurs scores
            var playersWithScores = GameManager.AllPlayers
                .Select(p => new
                {
                    Player = p,
                    TotalScore = p.Scores.Sum(),
                    GamesPlayed = p.Scores.Count
                })
                .OrderByDescending(p => p.TotalScore)
                .ToList();

            // Ajouter chaque joueur à la liste
            for (int i = 0; i < playersWithScores.Count; i++)
            {
                var playerInfo = playersWithScores[i];
                var rank = i + 1;

                var playerFrame = new Frame
                {
                    BackgroundColor = Colors.White,
                    BorderColor = Color.FromArgb("#E0E0E0"),
                    CornerRadius = 10,
                    Padding = new Thickness(15),
                    HasShadow = true
                };

                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(50) },  // Rang
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },  // Nom
                        new ColumnDefinition { Width = new GridLength(100) },  // Score
                        new ColumnDefinition { Width = new GridLength(100) }   // Parties
                    }
                };

                // Rang
                var rankLabel = new Label
                {
                    Text = $"#{rank}",
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = rank switch
                    {
                        1 => Color.FromArgb("#FFD700"), // Or
                        2 => Color.FromArgb("#C0C0C0"), // Argent
                        3 => Color.FromArgb("#CD7F32"), // Bronze
                        _ => Color.FromArgb("#222222")  // Noir
                    },
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                Grid.SetColumn(rankLabel, 0);

                // Nom du joueur
                var nameLabel = new Label
                {
                    Text = playerInfo.Player.Name,
                    FontSize = 18,
                    TextColor = Color.FromArgb("#222222"),
                    VerticalOptions = LayoutOptions.Center
                };
                Grid.SetColumn(nameLabel, 1);

                // Score total
                var scoreLabel = new Label
                {
                    Text = $"{playerInfo.TotalScore} pts",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#4CAF50"),
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                };
                Grid.SetColumn(scoreLabel, 2);

                // Nombre de parties
                var gamesLabel = new Label
                {
                    Text = $"{playerInfo.GamesPlayed} parties",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#666666"),
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                };
                Grid.SetColumn(gamesLabel, 3);

                grid.Children.Add(rankLabel);
                grid.Children.Add(nameLabel);
                grid.Children.Add(scoreLabel);
                grid.Children.Add(gamesLabel);

                playerFrame.Content = grid;
                LeaderboardList.Children.Add(playerFrame);
            }

            // Si aucun joueur n'a de score
            if (!playersWithScores.Any())
            {
                var noScoresLabel = new Label
                {
                    Text = "Aucun score enregistré",
                    FontSize = 18,
                    TextColor = Color.FromArgb("#666666"),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                LeaderboardList.Children.Add(noScoresLabel);
            }
        }
        catch (Exception ex)
        {
            var errorLabel = new Label
            {
                Text = $"Erreur lors du chargement du classement : {ex.Message}",
                FontSize = 16,
                TextColor = Color.FromArgb("#FF0000"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            LeaderboardList.Children.Add(errorLabel);
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
} 