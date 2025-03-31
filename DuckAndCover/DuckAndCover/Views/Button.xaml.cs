namespace DuckAndCover.Views
{
    public partial class Button : ContentView
    {
        public static readonly BindableProperty ButtonNameProperty =
            BindableProperty.Create(
                nameof(ButtonName),
                typeof(string),
                typeof(Button),
                default(string));

        public string ButtonName
        {
            get => (string)GetValue(ButtonNameProperty);
            set => SetValue(ButtonNameProperty, value);
        }

        public Button()
        {
            InitializeComponent();
            // Le BindingContext doit pointer vers lui-même pour que le binding fonctionne
            BindingContext = this;
        }
    }
}