using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace DuckAndCover.Views
{
    public partial class SettingOption: VerticalStackLayout
    {
        public SettingOption()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(
                nameof(Title), 
                typeof(string), 
                typeof(SettingOption),
                string.Empty);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty OptionContentProperty =
            BindableProperty.Create(
                nameof(OptionContent), 
                typeof(View), 
                typeof(SettingOption),
                default(View));

        public View OptionContent
        {
            get => (View)GetValue(OptionContentProperty);
            set => SetValue(OptionContentProperty, value);
        }
    }
}