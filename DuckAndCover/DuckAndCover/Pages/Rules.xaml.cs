using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckAndCover.Pages;

public partial class Rules : ContentPage
{
    public Rules()
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
}