using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace LocalizationResourceManager.Maui;

public class SpecificLocalizationResourceManager(ResourceManager resource) : ILocalizationResourceManager
{
    public ResourceManager Resource => resource;

    public string this[string text] => GetValue(text);

    public string this[string text, string resourceManager] => GetValue(text, resourceManager);

    public string this[string text, params object[] arguments] => GetValue(text, arguments);

    public string this[string text, string resourceManager, params object[] arguments] => GetValue(text, resourceManager, arguments);

    public CultureInfo DefaultCulture => LocalizationResourceManager.Current.DefaultCulture;

    public CultureInfo CurrentCulture { get => LocalizationResourceManager.Current.CurrentCulture; set => LocalizationResourceManager.Current.CurrentCulture = value; }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => LocalizationResourceManager.Current.PropertyChanged += value;
        remove => LocalizationResourceManager.Current.PropertyChanged -= value;
    }

    public string GetValue(string text) => LocalizationResourceManager.Current.GetValue(text, Resource);

    public string GetValue(string text, string resourceManager) => LocalizationResourceManager.Current.GetValue(text, resourceManager);

    public string GetValue(string text, params object[] arguments) => LocalizationResourceManager.Current.GetValue(text, Resource, arguments);

    public string GetValue(string text, string resourceManager, params object[] arguments) => LocalizationResourceManager.Current.GetValue(text, resourceManager, arguments);

    public void ReleaseAllResources() => Resource.ReleaseAllResources();
}