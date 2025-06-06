

using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Models.Game;
using DataPersistence;

// Si votre GameManager est dans un autre namespace et que vous en avez besoin ici
// (bien que pour OnNoClicked seul, il ne soit pas directement utilisé)
// using DuckAndCover.Models.Game; // Exemple

namespace DuckAndCover.Pages
{
    public partial class ResumeGame : ContentPage
    {
        public ResumeGame()
        {
            InitializeComponent();
        }
        
        private async void OnNoClicked(object sender, EventArgs e) => await Navigation.PushAsync(new GameMenu());
        
        private async Task NavigateToMenuPlayer()
        {
            try
            {
                await Shell.Current.GoToAsync($"//{nameof(MenuPlayer)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de navigation vers MenuPlayer: {ex.Message}");
                await DisplayAlert("Erreur de navigation", "Impossible d'accéder à la page du menu des joueurs.", "OK");
            }
        }
        


        private async void OnYesClicked(object sender, EventArgs e)
        {
            var lastGame = new JsonPersistency().LoadLastUnfinishedGame(); // ou LoadMostRecentGame()

            if (lastGame == null)
            {
                await DisplayAlert("Aucune partie", "Aucune partie précédente à reprendre.", "OK");
                return;
            }

            GameManager.Instance.LoadFrom(lastGame); // à implémenter si besoin

            await Shell.Current.GoToAsync($"//{nameof(GameBoard)}");
        }
        
    }
}