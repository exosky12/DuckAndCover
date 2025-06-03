using System.Diagnostics;
using DTOs;
using Models.Game;
using Models.Interfaces;
using Models.Rules;
using Models.Exceptions;

namespace DuckAndCover.Pages;

public partial class MenuPlayer : ContentPage
{
    private readonly GameSettingsDto _gameSettings;
    
    public Game GameManager => (App.Current as App).GameManager;

    public MenuPlayer(GameSettingsDto gameSettings)
    {
        InitializeComponent();
        _gameSettings = gameSettings;

        switch (_gameSettings.Rules)
        {
            case "Classic":
                GameManager.Rules = new ClassicRules();
                break;
            case "Blitz":
                GameManager.Rules = new BlitzRules();
                break;
            case "Insane":
                GameManager.Rules = new InsaneRules();
                break;
            default:
                GameManager.Rules = new ClassicRules();
                break;
        }

        GeneratePlayerInputs();
    }

    private void GeneratePlayerInputs()
    {
        for (int i = 0; i < _gameSettings.PlayerCount; i++)
        {
            var entry = new Entry
            {
                Placeholder = $"Nom du joueur {i + 1}"
            };
            entry.Style = (Style)Application.Current.Resources["InputEntryStyle"];
            PlayerInputsLayout.Children.Add(entry);
        }
    }
    
    public async void PlayClicked(object sender, EventArgs e)
    {
        try
        {
            var players = new List<Player>();
            foreach (var child in PlayerInputsLayout.Children)
            {
                if (child is Entry entry && !string.IsNullOrWhiteSpace(entry.Text))
                {
                    players.Add(new Player(entry.Text));
                }
            }

            if (players.Count == 0)
            {
                await DisplayAlert("Erreur", "Veuillez entrer au moins un nom de joueur", "OK");
                return;
            }

            var deck = new Deck();
            if (deck.Cards.Count == 0)
            {
                await DisplayAlert("Erreur", "Le deck est vide", "OK");
                return;
            }

            GameManager.InitializeGame(
                id: Guid.NewGuid().ToString("N").Substring(0, 5),
                players: players,
                deck: deck,
                currentDeckCard: deck.Cards.First()
            );

            await Navigation.PushAsync(new GamePage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur est survenue : {ex.Message}", "OK");
        }
    }
}