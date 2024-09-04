using System;

namespace LocalizationResourceManager.Maui.Core;

public static class LocalizationResourceManagerExtensions
{
    public static LocalizedString CreateLocalizedString(this ILocalizationResourceManager resourceManager, Func<string> generator)
    {
        return new LocalizedString(resourceManager, generator);
    }
}