using Models.Game;
using DataPersistence;
using Models.Interfaces;
using Models.Rules;
using System.Collections.ObjectModel;

namespace DuckAndCover
{
    public partial class App : Application
    {
        public string FileName { get; set; } = "DuckAndCover_data.json";
        
        public string FilePath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DuckAndCover");
        
        public Game GameManager { get; private set; }
        public App()
        {
            InitializeComponent();
            
            GameManager = new Game(new ClassicRules());

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            string fullPath = Path.Combine(FilePath, FileName);
            if (File.Exists(fullPath))
            {
                IDataPersistence dataPersistence = new JsonPersistency();
                (ObservableCollection<Player> players, ObservableCollection<Game> games) = dataPersistence.LoadData();
                GameManager.AllPlayers = players;
                GameManager.Games = games;
            }

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}