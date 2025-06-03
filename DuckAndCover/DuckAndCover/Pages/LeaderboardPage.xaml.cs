using Microsoft.Maui.Controls;
using Models.Game;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Shapes;
using Grid = Microsoft.Maui.Controls.Grid;

namespace DuckAndCover.Pages;

public partial class LeaderboardPage : ContentPage
{
    public Game GameManager => (App.Current as App)?.GameManager ??
                               throw new InvalidOperationException("GameManager not initialized");

    public LeaderboardPage()
    {
        InitializeComponent();
        LoadLeaderboard();
    }

    private async void LoadLeaderboard()
    {
        LeaderboardList.Children.Clear();

        var playersWithScores = GameManager.AllPlayers
            .Select(p => new
            {
                Player = p,
                TotalScore = p.Scores.Sum(),
                GamesPlayed = p.Scores.Count
            })
            .OrderByDescending(p => p.TotalScore)
            .ToList();

        if (!playersWithScores.Any())
        {
            LeaderboardList.Children.Add(new Label
            {
                Text = "Aucun score enregistré 😔",
                FontSize = 20,
                FontAttributes = FontAttributes.Italic,
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Center
            });
            return;
        }

        for (int i = 0; i < playersWithScores.Count; i++)
        {
            var p = playersWithScores[i];
            var rank = i + 1;

            string emoji = rank switch
            {
                1 => "🥇",
                2 => "🥈",
                3 => "🥉",
                _ => "🏅"
            };

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(50) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(100) }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnSpacing = 10,
                VerticalOptions = LayoutOptions.Center
            };

            var emojiLabel = new Label
            {
                Text = emoji,
                FontSize = 28,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(emojiLabel, 0);
            Grid.SetColumn(emojiLabel, 0);
            grid.Children.Add(emojiLabel);

            var nameLabel = new Label
            {
                Text = p.Player.Name,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#222222"),
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetRow(nameLabel, 0);
            Grid.SetColumn(nameLabel, 1);
            grid.Children.Add(nameLabel);

            var scoreStack = new VerticalStackLayout
            {
                Spacing = 2,
                HorizontalOptions = LayoutOptions.End,
                Children =
                {
                    new Label
                    {
                        Text = $"{p.TotalScore} pts",
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#4CAF50"),
                        HorizontalOptions = LayoutOptions.End
                    },
                    new Label
                    {
                        Text = $"{p.GamesPlayed} parties",
                        FontSize = 14,
                        TextColor = Color.FromArgb("#888888"),
                        HorizontalOptions = LayoutOptions.End
                    }
                }
            };
            Grid.SetRow(scoreStack, 0);
            Grid.SetColumn(scoreStack, 2);
            grid.Children.Add(scoreStack);

            var border = new Border
            {
                Stroke = Color.FromArgb("#FFD93B"),
                StrokeThickness = 3,
                Background = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Color.FromArgb("#FFFBEA"), 0.0f),
                        new GradientStop(Color.FromArgb("#FFF2B2"), 1.0f)
                    }
                },
                StrokeShape = new RoundRectangle { CornerRadius = 20 },
                Padding = 20,
                Margin = new Thickness(0, 10),
                Shadow = new Shadow
                {
                    Brush = Brush.Black,
                    Opacity = 0.25f,
                    Offset = new Point(4, 4),
                    Radius = 8
                },
                Content = grid
            };

            border.Opacity = 0;
            border.Scale = 0.9;

            LeaderboardList.Children.Add(border);

            await Task.Delay(80);
            await Task.WhenAll(
                border.FadeTo(1, 300, Easing.CubicIn),
                border.ScaleTo(1, 300, Easing.SpringOut)
            );
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
