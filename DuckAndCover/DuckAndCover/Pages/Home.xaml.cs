﻿namespace DuckAndCover.Pages
{
    public partial class Home : ContentPage
    {
            public Home()
    {
        InitializeComponent();
        UpdateDarkModeButtonText();
    }
    
    private AppTheme CurrentAppTheme => Application.Current?.UserAppTheme ?? AppTheme.Light;

    private void UpdateDarkModeButtonText()
    {
        if (DarkModeButton != null)
        {
            if (CurrentAppTheme == AppTheme.Dark)
            {
                DarkModeButton.Text = "☀️";
            }
            else
            {
                DarkModeButton.Text = "🌙";
            }
        }
    }

    private void OnDarkModeClicked(object sender, EventArgs e)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = CurrentAppTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
            UpdateDarkModeButtonText();
        }
    }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var elements = new List<VisualElement>
            {
                JouerBtn,
                ClassementBtn,
                ReglesBtn,
                CreditsBtn
            };

            foreach (var el in elements)
            {
                el.Opacity = 0;
                el.Scale = 0.9;
            }

            for (int i = 0; i < elements.Count; i++)
            {
                var el = elements[i];
                await Task.Delay(100);
                await Task.WhenAll(
                    el.FadeTo(1, 300, Easing.CubicIn),
                    el.ScaleTo(1, 300, Easing.SpringOut)
                );
            }

            if (TitreImage is not null)
            {
                TitreImage.Opacity = 0;
                TitreImage.Scale = 0.9;
                await TitreImage.FadeTo(1, 400, Easing.CubicIn);
                await TitreImage.ScaleTo(1, 400, Easing.SpringOut);
            }
        }

        private async void PlayClicked(object sender, EventArgs e) => await Navigation.PushAsync(new GameMenu());
        private async void LeaderboardClicked(object sender, EventArgs e) => await Navigation.PushAsync(new LeaderboardPage());
        private async void RulesClicked(object sender, EventArgs e) => await Navigation.PushAsync(new Rules());
        private async void CreditsClicked(object sender, EventArgs e) => await Navigation.PushAsync(new Credits());
    }
}