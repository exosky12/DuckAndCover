using System.Diagnostics;
using DTOs;
using Models.Game;
using Models.Interfaces;
using Models.Rules;
using Models.Exceptions;
using Models.Enums;

namespace DuckAndCover.Pages;

public partial class MenuPlayer : ContentPage
{
    private readonly GameSettingsDto _gameSettings;
    
    public Game GameManager => (Application.Current as App)?.GameManager ?? 
                               throw new ErrorException(ErrorCodes.GameManagerNotInitialized);
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
        UpdateDarkModeButtonText();
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
                var handler = new ErrorHandler(new ErrorException(ErrorCodes.NoPlayerNameProvided));
                await DisplayAlert("Erreur", handler.Handle(), "OK");
                return;
            }

            var deck = new Deck();
            if (deck.Cards.Count == 0)
            {
                var handler = new ErrorHandler(new ErrorException(ErrorCodes.DeckEmpty));
                await DisplayAlert("Erreur", handler.Handle(), "OK");
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
        catch (ErrorException ex)
        {
            var handler = new ErrorHandler(ex);
            await DisplayAlert("Erreur", handler.Handle(), "OK");
        }
    }
    
    private void UpdateDarkModeButtonText()
    {
        if (DarkModeButton != null)
        {
            if (Application.Current.UserAppTheme == AppTheme.Dark)
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
        if (Application.Current.UserAppTheme == AppTheme.Dark)
        {
            Application.Current.UserAppTheme = AppTheme.Light;
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
        }
        UpdateDarkModeButtonText();
    }
}