using LocalizationResourceManager.Maui.ComponentModel;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Localized string, updated by tracking current culture from current localizarion resource manager.
/// </summary>
public class LocalizedString : ObservableObject
{
    private readonly Func<string> generator;

    public LocalizedString(Func<string> generator, ILocalizationResourceManager resourceManager)
    {
        this.generator = generator;

        // This instance will be unsubscribed and GCed if no one references it
        // since LocalizationResourceManager uses WeekEventManger
        resourceManager.PropertyChanged += (sender, e) => OnPropertyChanged((string?)null);
    }

    public LocalizedString(Func<ILocalizationResourceManager, string> generator) : this(this.generator = () => generator(resourceManager))
    {
    }

    [Microsoft.Maui.Controls.Internals.Preserve(Conditional = true)]
    public string Localized => generator();
}