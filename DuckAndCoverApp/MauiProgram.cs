using Microsoft.Extensions.Logging;

namespace DuckAndCoverApp
{
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
<<<<<<< HEAD
                    fonts.AddFont("ionicons.ttf", "Ionicons");
=======
                    fonts.AddFont("Kalam-Variable.ttf", "Kalam");
>>>>>>> 83712ce8c651079e83640a773ba3cf56207fc8b2
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
