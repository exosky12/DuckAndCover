using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using DuckAndCover.Views;

namespace DuckAndCover;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("ionicons.ttf", "Ionicons");
                fonts.AddFont("Kalam-Light.otf", "KalamLight");
                fonts.AddFont("Kalam-Regular.otf", "KalamRegular");
                fonts.AddFont("Kalam-Bold.otf", "KalamBold");
                fonts.AddFont("ClashDisplay-Variable.ttf", "Clash");
            });

        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddTransient<Views.Button>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}