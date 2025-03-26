namespace DuckAndCoverApp.Views
{
	public partial class BackButton : ContentView
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


		enum ButtonVariantType
		{
			Primary,
			Secondary
		}
		public static readonly BindableProperty ButtonVariantProperty =
					BindableProperty.Create(
						nameof(ButtonVariant),
						typeof(ButtonVariantType),
						typeof(Button),
						default(ButtonVariantType));


		public string ButtonVariant
		{
			get => (string)GetValue(ButtonVariantProperty);
			set => SetValue(ButtonVariantProperty, value);
		}

		public BackButton()
		{
			InitializeComponent();
			// Le BindingContext doit pointer vers lui-même pour que le binding fonctionne
			BindingContext = this;
		}
	}
}