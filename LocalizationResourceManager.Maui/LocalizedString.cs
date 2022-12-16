using LocalizationResourceManager.Maui.ComponentModel;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Localized string, updated by tracking current culture from current localizarion resource manager.
/// </summary>
public class LocalizedString : ObservableObject
{
    private readonly Func<string> generator;

    public LocalizedString(Func<string> generator)
        : this(LocalizationResourceManager.Current, generator)
    {
    }

    public LocalizedString(LocalizationResourceManager localizationManager, Func<string> generator)
    {
        this.generator = generator;

        // This instance will be unsubscribed and GCed if no one references it
        // since LocalizationResourceManager uses WeekEventManger
        localizationManager.PropertyChanged += (sender, e) => OnPropertyChanged((string?)null);
    }

    [Microsoft.Maui.Controls.Internals.Preserve(Conditional = true)]
    public string Localized => generator();

    [Microsoft.Maui.Controls.Internals.Preserve(Conditional = true)]
    public static implicit operator LocalizedString(Func<string> func) => new(func);
}