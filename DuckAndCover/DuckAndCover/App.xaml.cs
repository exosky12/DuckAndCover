using Models.Game;
using Models.Rules;
using Models.Interfaces;
using DataPersistence;
using System.Diagnostics;

namespace DuckAndCover
{
    public partial class App : Application
    {
        /// <summary>
        /// Persistance de l’historique des parties terminées.
        /// </summary>
        public IDataPersistence DataPersistence { get; }

        /// <summary>
        /// GameManager partagé par toute l’application (MAUI).
        /// </summary>
        public Game GameManager { get; set; }

        public App()
        {
            InitializeComponent();

            DataPersistence = new JsonPersistency();

            var (players, games) = DataPersistence.LoadData();

            Debug.WriteLine(players);

            GameManager = new Game(new ClassicRules())
            {
                AllPlayers = players,
                Games = games
            };

            Debug.WriteLine($"[App] Chargé : {players.Count} joueurs et {games.Count} parties.");

            // 4) On s’abonne à l’événement “GameIsOver”
            GameManager.GameIsOver += OnGameIsOver;

        }

        private void OnGameIsOver(object? sender, Models.Events.GameIsOverEventArgs e)
        {
            try
            {
                // Mise à jour des collections en mémoire
                GameManager.SavePlayers();
                GameManager.SaveGame();

                // Sauvegarde finale de l’historique (players + games) dans le JSON
                DataPersistence.SaveData(GameManager.AllPlayers, GameManager.Games);

                Debug.WriteLine("[App] OnGameIsOver : historique sauvegardé.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[App] Erreur OnGameIsOver : {ex.Message}");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}