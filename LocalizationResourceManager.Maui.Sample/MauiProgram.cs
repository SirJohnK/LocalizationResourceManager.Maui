using LocalizationResourceManager.Maui;
using LocalizationResourceManager.Maui.Sample.Resources;
using Microsoft.Extensions.Logging;

namespace LocalizationResourceManager.Maui.Sample
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
                })
                .UseLocalizationResourceManager(AppResources.ResourceManager, restoreLatest: true);

            //Add Views
            builder.Services.AddSingleton<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}