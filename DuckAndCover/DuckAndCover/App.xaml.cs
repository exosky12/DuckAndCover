using System;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Models.Game;
using DataPersistence;
using Models.Interfaces;
using Models.Rules;

namespace DuckAndCover
{
    public partial class App : Application
    {
        // Nom du fichier JSON de persistance
        public string FileName { get; set; } = "DuckAndCover_data.json";

        // Répertoire dans %AppData%\DuckAndCover
        public string FilePath
            => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DuckAndCover"
            );

        // GameManager central
        public Game GameManager { get; private set; }

        public App()
        {
            InitializeComponent();

            // 1) Instanciation du GameManager avec les règles classiques
            //    ATTENTION : votre constructeur Game(IRules) doit initier :
            //      - Deck = new Deck(); CurrentDeckCard = Deck.Cards.FirstOrDefault();
            //      - AllPlayers = new ObservableCollection<Player>();
            //      - Games      = new ObservableCollection<Game>();
            GameManager = new Game(new ClassicRules());

            // 2) Créer le dossier de données s’il n’existe pas
            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);

            // 3) Construire le chemin complet vers le JSON
            string fullPath = Path.Combine(FilePath, FileName);

            // 4) Si le fichier existe, charger les collections via JsonPersistency
            if (File.Exists(fullPath))
            {
                IDataPersistence dataPersistence = new JsonPersistency();
                (ObservableCollection<Player> players, ObservableCollection<Game> games)
                    = dataPersistence.LoadData();

                // 5) Remplacer les collections du GameManager
                GameManager.AllPlayers = players ?? new ObservableCollection<Player>();
                GameManager.Games = games ?? new ObservableCollection<Game>();

                // 6) Si on a au moins un joueur, on le définit comme CurrentPlayer par défaut
                if (GameManager.AllPlayers.Any())
                {
                    GameManager.CurrentPlayer = GameManager.AllPlayers.First();
                }

                // 7) (Optionnel) : Si vous avez stocké dans chaque Game un indicateur IsInProgress
                //    et un CurrentPlayerIndex, vous pouvez restaurer la partie en cours ici :
                /*
                var inProgress = GameManager.Games.FirstOrDefault(g => !g.IsFinished);
                if (inProgress != null)
                {
                    GameManager.CurrentGame  = inProgress;
                    GameManager.CurrentPlayer = inProgress.Players[inProgress.CurrentPlayerIndex];
                }
                */
            }

            // 8) Démarrer sur la page de menu (ou d’accueil), pas directement sur GamePage
            MainPage = new NavigationPage(new Pages.Home());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
