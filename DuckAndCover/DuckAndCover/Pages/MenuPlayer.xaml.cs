using DTOs;

namespace DuckAndCover.Pages;

public partial class MenuPlayer : ContentPage
{
    private readonly GameSettingsDTO _gameSettings;

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
}