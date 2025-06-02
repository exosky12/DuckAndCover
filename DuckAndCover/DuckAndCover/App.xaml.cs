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
        
        // Garder une référence à l'instance de persistance
        private IDataPersistence _dataPersistence;
        
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
            
            // Initialiser la persistance une seule fois
            _dataPersistence = new JsonPersistency();
            
            string fullPath = Path.Combine(FilePath, FileName);
            Debug.WriteLine($"Checking for save file at: {fullPath}");
            
            if (File.Exists(fullPath))
            {
                Debug.WriteLine("Save file found, loading data...");
                (ObservableCollection<Player> players, ObservableCollection<Game> games) = _dataPersistence.LoadData();
                GameManager.AllPlayers = players;
                GameManager.Games = games;
                Debug.WriteLine($"Loaded {players.Count} players and {games.Count} games");
            }
            else
            {
                Debug.WriteLine("No save file found, starting fresh");
                // Initialiser avec des collections vides si pas de fichier
                GameManager.AllPlayers = new ObservableCollection<Player>();
                GameManager.Games = new ObservableCollection<Game>();
            }
            
            GameManager.GameIsOver += OnGameIsOver;
            Debug.WriteLine("GameIsOver event subscribed");
        }

        private void OnGameIsOver(object? sender, Models.Events.GameIsOverEventArgs e)
        {
            Debug.WriteLine("Game is over, saving data...");
            try
            {
                GameManager.IsFinished = true;
                GameManager.SavePlayers();
                GameManager.SaveGame();
                _dataPersistence.SaveData(GameManager.AllPlayers, GameManager.Games);
                Debug.WriteLine("Data saved successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}