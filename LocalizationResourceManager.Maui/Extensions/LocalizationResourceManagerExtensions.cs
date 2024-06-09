namespace LocalizationResourceManager.Maui;

public static class LocalizationResourceManagerExtensions
{
    public static LocalizedString LocalizedString(this ILocalizationResourceManager resourceManager, Func<string> generator)
    {
        return new LocalizedString(generator, resourceManager);
    }
}