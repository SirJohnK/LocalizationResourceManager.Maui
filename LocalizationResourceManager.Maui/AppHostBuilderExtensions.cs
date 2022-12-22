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
        //Setup Settings
        settings.Invoke(LocalizationResourceManager.Current);

        //Add Service
        builder.Services.AddSingleton(LocalizationResourceManager.Current);

        //Return Builder
        return builder;
    }
}