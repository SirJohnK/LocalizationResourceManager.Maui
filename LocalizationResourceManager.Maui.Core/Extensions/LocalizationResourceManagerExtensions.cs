using System;

namespace LocalizationResourceManager.Maui.Core;

/// <summary>
/// Extension methods for <see cref="ILocalizationResourceManager"/>.
/// </summary>
public static class LocalizationResourceManagerExtensions
{
    /// <summary>
    /// Create a localized string using the provided generator.
    /// </summary>
    /// <param name="resourceManager">Current <see cref="ILocalizationResourceManager"/> implementation.</param>
    /// <param name="generator">Localized string generator function.</param>
    /// <returns></returns>
    public static LocalizedString CreateLocalizedString(this ILocalizationResourceManager resourceManager, Func<string> generator)
    {
        return new LocalizedString(resourceManager, generator);
    }
}