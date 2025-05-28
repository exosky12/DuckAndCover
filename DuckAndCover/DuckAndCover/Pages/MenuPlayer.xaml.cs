using DTOs;
using Models.Interfaces;

namespace DuckAndCover.Pages;

public partial class MenuPlayer : ContentPage
{
    private readonly GameSettingsDTO _gameSettings;
    
    // public IDataPersistence? DataManager => (App.Current)?.DataManager as App;

    public MenuPlayer(GameSettingsDTO gameSettings)
    {
        InitializeComponent();
        _gameSettings = gameSettings;

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
        throw new NotImplementedException("La logique de démarrage du jeu n'est pas implémentée.");
    }
}