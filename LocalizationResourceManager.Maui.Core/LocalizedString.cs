using System;
using System.ComponentModel;

namespace LocalizationResourceManager.Maui.Core;

/// <summary>
/// Localized string, updated by tracking current culture from current localization resource manager.
/// </summary>
public class LocalizedString : INotifyPropertyChanged
{
    private readonly Func<string> generator;
    private readonly ILocalizationResourceManager resourceManager;

    /// <summary>
    /// Create a localized string using the provided generator and resource manager.
    /// </summary>
    /// <param name="resourceManager">Current <see cref="ILocalizationResourceManager"/> implementation.</param>
    /// <param name="generator">Localized string generator function.</param>
    public LocalizedString(ILocalizationResourceManager resourceManager, Func<string> generator)
    {
        //Store localized string generator
        this.generator = generator;

        //store localization resource manager
        this.resourceManager = resourceManager;

        //Prevent linker from removing the implementation
        if (DateTime.Now.Ticks < 0) _ = Localized;
    }

    /// <summary>
    /// Localized string value.
    /// </summary>
    public string Localized => generator();

    #region INotifyPropertyChanged

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => resourceManager.PropertyChanged += value;
        remove => resourceManager.PropertyChanged -= value;
    }

    #endregion INotifyPropertyChanged
}