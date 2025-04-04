using DuckAndCover.Pages;
using Plugin.Maui.Audio;

namespace DuckAndCover.Views
{
    public partial class Button
    {
        public static readonly BindableProperty NameProperty =
            BindableProperty.Create(
                nameof(Name),
                typeof(string),
                typeof(Button),
                default(string));

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        private readonly IAudioManager audioManager;

        // Constructeur par défaut pour XAML
        public Button() : this(Plugin.Maui.Audio.AudioManager.Current)
        {
        }

        // Constructeur avec injection de dépendance
        public Button(IAudioManager audioManager)
        {
            InitializeComponent();
            this.audioManager = audioManager;
            BindingContext = this;
        }

        private async void OnClick(object sender, EventArgs e)
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("DuckSound.mp3"));
            player.Play();
        }
    }
}