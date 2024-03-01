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
                .UseLocalizationResourceManager(settings =>
                {
                    settings.AddResource(AppResources.ResourceManager);
                    settings.AddResource(SpecificResources.ResourceManager, nameof(SpecificPage));
                    settings.SuppressTextNotFoundException(true, "'{0}' not found!");
                    settings.RestoreLatestCulture(true);
                });

            //Add Views
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<SpecificPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}