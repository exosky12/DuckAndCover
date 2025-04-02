using DuckAndCover.Pages;
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
        
        public Button()
        {
            InitializeComponent();
            BindingContext = this;
        }
        
        
    }
}