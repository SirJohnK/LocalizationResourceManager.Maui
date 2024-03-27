using System.Globalization;

namespace LocalizationResourceManager.Maui;

public class SpecificLocalizationResourceManager(string resourceManager) : ILocalizationResourceManager
{
    public string this[string text] => LocalizationResourceManager.Current[text, resourceManager];

    public string this[string text, string resourceManager] => LocalizationResourceManager.Current[text, resourceManager];

    public string this[string text, params object[] arguments] => LocalizationResourceManager.Current[text, resourceManager, arguments];

    public string this[string text, string resourceManager, params object[] arguments] => LocalizationResourceManager.Current[text, resourceManager, arguments];

    public CultureInfo DefaultCulture => LocalizationResourceManager.Current.DefaultCulture;

    public CultureInfo CurrentCulture { get => LocalizationResourceManager.Current.CurrentCulture; set => LocalizationResourceManager.Current.CurrentCulture = value; }

    public string GetValue(string text) => LocalizationResourceManager.Current.GetValue(text, resourceManager);

    public string GetValue(string text, string resourceManager) => LocalizationResourceManager.Current.GetValue(text, resourceManager);

    public string GetValue(string text, params object[] arguments) => LocalizationResourceManager.Current.GetValue(text, resourceManager, arguments);

    public string GetValue(string text, string resourceManager, params object[] arguments) => LocalizationResourceManager.Current.GetValue(text, resourceManager, arguments);

    public void ReleaseAllResources() => LocalizationResourceManager.Current.ReleaseAllResources();
}