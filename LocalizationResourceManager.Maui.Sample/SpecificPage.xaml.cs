using System.Globalization;

namespace LocalizationResourceManager.Maui.Sample;

public partial class SpecificPage : ContentPage
{
    private readonly ILocalizationResourceManager resourceManager;

    public SpecificPage(ILocalizationResourceManager resourceManager)
    {
        InitializeComponent();

        this.resourceManager = resourceManager;
    }

    private void OnToggleLanguage(object sender, EventArgs e)
    {
        var languages = new List<string>() { "en", "fr", "de", "es", "sv" };
        var culture = resourceManager.CurrentCulture;
        var index = languages.IndexOf(culture.TwoLetterISOLanguageName);
        resourceManager.CurrentCulture = new CultureInfo(languages[++index < languages.Count ? index : 0]);
    }
}