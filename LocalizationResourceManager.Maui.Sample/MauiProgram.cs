using Microsoft.Extensions.Logging;
using LocalizationResourceManager.Maui.Sample.Resources;
using LocalizationResourceManager.Maui.Sample.Common;

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
                    settings.RestoreLatestCulture();
                    settings.MonitorPlatformCulture();
                });

            //Add Views
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<SpecificPage>();

            //Add ViewModels
            builder.Services.AddTransient<MainViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}