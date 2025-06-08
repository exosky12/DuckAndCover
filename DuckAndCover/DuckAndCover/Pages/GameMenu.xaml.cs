using System.Diagnostics;
using DTOs;
namespace DuckAndCover.Pages;

public partial class GameMenu : ContentPage
{
    public GameMenu()
    {
        InitializeComponent();
        UpdateBotCountVisibility(BotSwitch.IsToggled);
        UpdateDarkModeButtonText();
    }
    
    private AppTheme CurrentAppTheme => Application.Current?.UserAppTheme ?? AppTheme.Light;

    private void UpdateDarkModeButtonText()
    {
        if (DarkModeButton != null)
        {
            if (CurrentAppTheme == AppTheme.Dark)
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
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = CurrentAppTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
            UpdateDarkModeButtonText();
        }
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

        var dto = new GameSettingsDto
        {
            PlayerCount = playerCount,
            UseBots = BotSwitch.IsToggled,
            BotCount = BotSwitch.IsToggled ? botCount : 0,
            Rules = ClassicRulesSwitch.IsToggled ? "Classic" : 
                     BlitzRulesSwitch.IsToggled ? "Blitz" :
                     InsaneRulesSwitch.IsToggled ? "Insane" : "Classic"
        };
        
        if (playerCount < 1)
        {
            await DisplayAlert("Erreur", "Le nombre de joueurs doit être supérieur à 0.", "OK");
            return;
        }

        await Navigation.PushAsync(new MenuPlayer(dto));
    }
    
    
    private void ClassicRules_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            BlitzRulesSwitch.IsToggled = false;
            InsaneRulesSwitch.IsToggled = false;
        }
        else if (!BlitzRulesSwitch.IsToggled && !InsaneRulesSwitch.IsToggled)
        {
            ClassicRulesSwitch.IsToggled = true;
        }
    }

    private void BlitzRules_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            ClassicRulesSwitch.IsToggled = false;
            InsaneRulesSwitch.IsToggled = false;
        }
        else if (!ClassicRulesSwitch.IsToggled && !InsaneRulesSwitch.IsToggled)
        {
            BlitzRulesSwitch.IsToggled = true;
        }
    }

    private void InsaneRules_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            ClassicRulesSwitch.IsToggled = false;
            BlitzRulesSwitch.IsToggled = false;
        }
        else if (!ClassicRulesSwitch.IsToggled && !BlitzRulesSwitch.IsToggled)
        {
            InsaneRulesSwitch.IsToggled = true;
        }
    }
}