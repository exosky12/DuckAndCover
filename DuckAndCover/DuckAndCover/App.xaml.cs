using Models.Game;
using DataPersistence;
using Models.Interfaces;
using Models.Rules;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DuckAndCover
{
    public partial class App : Application
    {
        public string FileName { get; set; } = "duckAndCover_data.json";
        
        public string FilePath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DuckAndCover");
        
        public Game GameManager { get; private set; }
        
        public App()
        {
            Debug.WriteLine("Initializing App...");
            InitializeComponent();
            
            GameManager = new Game(new ClassicRules());
            Debug.WriteLine("GameManager initialized with ClassicRules");

            if (!Directory.Exists(FilePath))
            {
                Debug.WriteLine($"Creating directory at: {FilePath}");
                Directory.CreateDirectory(FilePath);
            }
            
            string fullPath = Path.Combine(FilePath, FileName);
            Debug.WriteLine($"Checking for save file at: {fullPath}");
            
            if (File.Exists(fullPath))
            {
                Debug.WriteLine("Save file found, loading data...");
                IDataPersistence dataPersistence = new JsonPersistency();
                (ObservableCollection<Player> players, ObservableCollection<Game> games) = dataPersistence.LoadData();
                GameManager.AllPlayers = players;
                GameManager.Games = games;
                Debug.WriteLine($"Loaded {players.Count} players and {games.Count} games");
                
                GameManager.GameIsOver += (s, e) =>
                {
                    Debug.WriteLine("Game is over, saving data...");
                    GameManager.IsFinished = true;
                    GameManager.SavePlayers();
                    GameManager.SaveGame();
                    dataPersistence.SaveData(GameManager.AllPlayers, GameManager.Games);
                    Debug.WriteLine("Data saved successfully");
                };
            }
            else
            {
                Debug.WriteLine("No save file found, starting fresh");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}