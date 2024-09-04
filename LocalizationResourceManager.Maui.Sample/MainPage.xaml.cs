using LocalizationResourceManager.Maui.Sample.Common;
using System.Globalization;

namespace LocalizationResourceManager.Maui.Sample
{
    public partial class MainPage : ContentPage
    {
        private readonly ILocalizationResourceManager resourceManager;

        public MainPage(ILocalizationResourceManager resourceManager, MainViewModel viewModel)
        {
            //Init
            InitializeComponent();
            this.resourceManager = resourceManager;
            BindingContext = viewModel;
        }

        private void OnToggleLanguage(object sender, EventArgs e)
        {
            var languages = new List<string>() { "en", "fr", "de", "es", "sv" };
            var culture = resourceManager.CurrentCulture;
            var index = languages.IndexOf(culture.TwoLetterISOLanguageName);
            resourceManager.CurrentCulture = new CultureInfo(languages[++index < languages.Count ? index : 0]);
        }

        private void SpecificPage_Button_Clicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(SpecificPage));
        }
    }
}