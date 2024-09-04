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

    public LocalizedString(ILocalizationResourceManager resourceManager, Func<string> generator)
    {
        //Store localized string generator
        this.generator = generator;

        //store localization resource manager
        this.resourceManager = resourceManager;

        //Prevent linker from removing the implementation
        if (DateTime.Now.Ticks < 0) _ = Localized;
    }

    public string Localized => generator();

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => resourceManager.PropertyChanged += value;
        remove => resourceManager.PropertyChanged -= value;
    }
}