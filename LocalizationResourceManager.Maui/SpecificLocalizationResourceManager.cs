using System.Resources;
using System.Globalization;
using System.ComponentModel;
using LocalizationResourceManager.Maui.ComponentModel;

namespace LocalizationResourceManager.Maui;

public class SpecificLocalizationResourceManager(ILocalizationResourceManager resourceManager, ResourceManager resource) : ObservableObject, ILocalizationResourceManager
{
    public CultureInfo DefaultCulture => resourceManager.DefaultCulture;

    public CultureInfo CurrentCulture { get => resourceManager.CurrentCulture; set => resourceManager.CurrentCulture = value; }

    public bool IsNameWithDotsSupported => resourceManager.IsNameWithDotsSupported;

    public string DotSubstitution => resourceManager.DotSubstitution;

    public string this[string text] => resourceManager[text, resource];

    public string this[string text, ResourceManager resource] => resourceManager[text, resource];

    public string this[string text, params object[] arguments] => resourceManager[text, resource, arguments];

    public string this[string text, ResourceManager resource, params object[] arguments] => resourceManager[text, resource, arguments];

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
    }

    public string GetValue(string text) => resourceManager.GetValue(text, resource);

    public string GetValue(string text, ResourceManager resource) => resourceManager.GetValue(text, resource);

    public string GetValue(string text, params object[] arguments) => resourceManager.GetValue(text, resource, arguments);

    public string GetValue(string text, ResourceManager resource, params object[] arguments) => resourceManager.GetValue(text, resource, arguments);

    public void ReleaseAllResources() => resourceManager.ReleaseAllResources();
}