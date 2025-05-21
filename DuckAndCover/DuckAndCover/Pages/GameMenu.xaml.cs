using DTOs;
namespace DuckAndCover.Pages;

public partial class GameMenu : ContentPage
{
    public GameMenu()
    {
        InitializeComponent();
        UpdateBotCountVisibility(BotSwitch.IsToggled);
    }

    private void BotSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        UpdateBotCountVisibility(e.Value);
    }

    private void UpdateBotCountVisibility(bool isVisible)
    {
        BotCountEntry.IsVisible = isVisible;
        BotCountLabel.IsVisible = isVisible;
    }

    private async void ContinueClicked(object sender, EventArgs e)
    {
        int.TryParse(PlayerCountEntry.Text, out int playerCount);
        int.TryParse(BotCountEntry.Text, out int botCount);

        var dto = new GameSettingsDTO
        {
            PlayerCount = playerCount,
            UseBots = BotSwitch.IsToggled,
            BotCount = BotSwitch.IsToggled ? botCount : 0,
        };

        await Navigation.PushAsync(new MenuPlayer(dto));
    }
}