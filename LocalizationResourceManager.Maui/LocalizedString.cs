using CoreLocalizedString = LocalizationResourceManager.Maui.Core.LocalizedString;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Localized string, updated by tracking current culture from current localization resource manager.
/// </summary>
public class LocalizedString : CoreLocalizedString
{
    public LocalizedString(Func<string> generator) : base(LocalizationResourceManager.Current, generator)
    {
    }

    [Microsoft.Maui.Controls.Internals.Preserve(Conditional = true)]
    public static implicit operator LocalizedString(Func<string> func) => new(func);
}