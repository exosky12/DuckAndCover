namespace DuckAndCover.Pages;

public partial class GameMenu : ContentPage
{
    public GameMenu()
    {
        InitializeComponent();
    }

    private async void ContinueClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MenuPlayer());
    }
}