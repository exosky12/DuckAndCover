using Models.Game;
using DataPersistence;
using Models.Interfaces;

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
            
            GameManager = new Game(new Stub());

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            string fullPath = Path.Combine(FilePath, FileName);
            if(File.Exists(fullPath))
                GameManager.LoadData();

            MainPage = new AppShell();

            MainPage.Disappearing += (s, a) => GameManager.Save();

            


        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}