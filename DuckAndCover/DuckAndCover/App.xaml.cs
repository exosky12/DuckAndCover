using Models.Game;
using DataPersistence;
using Models.Interfaces;

namespace DuckAndCover
{
    public partial class App : Application
    {
        // public IDataPersistence? DataManager { get; set; } = new Stub();
        // public string FileName { get; set; } = "DuckAndCover.json";
        //
        // public string FilePath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DuckAndCover");
        //
        // public Game GameManager { get; private set; }
        public App()
        {
            InitializeComponent();
            
            // GameManager = new Game(new PersistenceJSON());

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}