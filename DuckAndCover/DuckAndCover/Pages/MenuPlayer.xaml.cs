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
    
    public Game GameManager => (Application.Current as App)?.GameManager ?? 
                               throw new InvalidOperationException("GameManager not initialized");
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
        PlayerInputsLayout.Children.Clear();
        
        int humanCount = _gameSettings.PlayerCount 
                         - (_gameSettings.UseBots ? _gameSettings.BotCount : 0);
        if (humanCount < 0) humanCount = 0;
        
        for (int i = 0; i < humanCount; i++)
        {
            var entry = new Entry
            {
                Placeholder = $"Nom du joueur {i + 1}"
            };
            entry.Style = Application.Current?.Resources?["InputEntryStyle"] as Style ?? 
                new Style(typeof(Entry));
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
            
            if (_gameSettings.UseBots)
            {
                for (int i = 1; i <= _gameSettings.BotCount; i++)
                {
                    players.Add(new Bot(i.ToString()));
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