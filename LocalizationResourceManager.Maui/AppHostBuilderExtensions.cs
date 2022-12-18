using Microsoft.Maui.Storage;
using System.Globalization;
using System.Resources;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// MAUI App Builder Extension for library setup.
/// </summary>
public static class AppHostBuilderExtensions
{
    /// <summary>
    /// Setup and Initialize the LocalizationResourceManager.
    /// </summary>
    /// <param name="builder">Current MAUI App Builder</param>
    /// <param name="resource">ResourceManager tracked by the LocalizationResourceManager</param>
    /// <param name="initialCulture">Inital CultureInfo used by the LocalizationResourceManager. (Optional, Default: CurrentThread.CurrentUICulture)</param>
    /// <param name="restoreLatest">Restore latest CultureInfo saved by library. (Optional, Default: false)</param>
    /// <returns>The MAUI App Builder</returns>
    /// <remarks>
    /// Latest CultureInfo is stored in Preferrences and updated everytime CurrentCulture is updated.
    /// </remarks>
    public static MauiAppBuilder UseLocalizationResourceManager(this MauiAppBuilder builder, ResourceManager resource, CultureInfo? initialCulture = null, bool restoreLatest = false)
    {
        //Store Default Culture
        LocalizationResourceManager.DefaultCulture = CultureInfo.CurrentCulture;

        //Setup Localization Resource Manager
        LocalizationResourceManager.Current.Init(resource, restoreLatest ? LocalizationResourceManager.LatestCulture : initialCulture);

        //Add Service
        builder.Services.AddSingleton(LocalizationResourceManager.Current);

        //Return Builder
        return builder;
    }
}