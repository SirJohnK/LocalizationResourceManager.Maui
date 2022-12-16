using System.Globalization;
using System.Resources;

namespace LocalizationResourceManager.Maui;

public static class AppHostBuilderExtensions
{
    public static MauiAppBuilder UseLocalizationResourceManager(this MauiAppBuilder builder, ResourceManager resource, CultureInfo? initialCulture = null)
    {
        //Setup Localization Resource Manager
        LocalizationResourceManager.Current.Init(resource, initialCulture ?? CultureInfo.CurrentCulture);

        //Add Service
        builder.Services.AddSingleton(LocalizationResourceManager.Current);

        //Return Builder
        return builder;
    }
}