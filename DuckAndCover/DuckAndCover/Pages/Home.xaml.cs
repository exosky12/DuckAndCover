namespace DuckAndCover.Pages
{
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();
        }
        
        private async void PlayClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GameMenu());
        }
        
        // private async void LeaderboardClicked(object sender, EventArgs e)
        // {
        //     await Navigation.PushAsync(new GameMenu());
        // }
        
        private async void RulesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Rules());
        }
        
        
        private async void CreditsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Credits());
        }
    }
}