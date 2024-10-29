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
    /// <param name="settings">Delegate to set inital settings</param>
    /// <returns>The MAUI App Builder</returns>
    /// <remarks>
    /// At least one resource must be added with Settings.AddResource!
    /// </remarks>
    public static MauiAppBuilder UseLocalizationResourceManager(this MauiAppBuilder builder, Action<ILocalizationSettings> settings)
    {
        //Init
        var currentResourceManager = LocalizationResourceManager.Current;

        //Setup Internal Settings
        currentResourceManager.RegisterService(Preferences.Default);
        currentResourceManager.RegisterService(() => PlatformCulture.Current);

        //Setup Settings
        settings.Invoke(currentResourceManager);

        //Add Services
        builder.Services.AddSingleton<ILocalizationSettings>(currentResourceManager);
        builder.Services.AddSingleton<ILocalizationResourceManager>(currentResourceManager);
        builder.Services.AddSingleton((serviceProvider) => PlatformCulture.Current);
        builder.Services.AddKeyedSingleton<ILocalizationResourceManager>(KeyedService.AnyKey, (serviceProvider, key) =>
        {
            // Attempt to resolve the keyed service
            if (key is string keyString && currentResourceManager.KeyedResources.TryGetValue(keyString, out var keyedResource))
                return keyedResource;
            else
                return default!;
        });

        //Return Builder
        return builder;
    }
}