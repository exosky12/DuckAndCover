using System.Diagnostics;
using Models.Game;
using DataPersistence;
using Models.Exceptions;
using Models.Enums;
using Models.Rules;


namespace DuckAndCover.Pages
{
    public partial class ResumeGame : ContentPage
    {
        public Game GameManager = (Application.Current as App)?.GameManager ?? 
                                   throw new ErrorException(ErrorCodes.GameManagerNotInitialized);
        public ResumeGame()
        {
            InitializeComponent();
        }
        
        private async void OnNoClicked(object sender, EventArgs e) => await Navigation.PushAsync(new GameMenu());
        

        private async void OnYesClicked(object sender, EventArgs e)
        {

            var rules = new ClassicRules(); 

            var lastGame = new JsonPersistency().LoadLastUnfinishedGame(rules);
            Debug.WriteLine(lastGame);
            Debug.WriteLine("Dernière partie non terminée : " + lastGame?.Id);

            if (lastGame == null)
            {
                await DisplayAlert("Aucune partie", "Aucune partie précédente à reprendre.", "OK");
                return;
            }
            
            Debug.WriteLine("game finie ?");
            Debug.WriteLine(lastGame.IsFinished);

            GameManager = lastGame;
            
            await Navigation.PushAsync(new GamePage());
        }
        
    }
}