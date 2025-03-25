namespace DuckAndCoverApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public async void CreditsBtn_Clicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new CreditPage());
        }
    }
}


