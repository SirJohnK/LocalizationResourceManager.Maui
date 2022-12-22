using LocalizationResourceManager.Maui.Sample.Resources;
using System.Globalization;

namespace LocalizationResourceManager.Maui.Sample
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;
        private readonly ILocalizationResourceManager resourceManager;

        public LocalizedString HelloWorld { get; } = new(() => $"{AppResources.Hello}, {AppResources.World}!");

        public MainPage(ILocalizationResourceManager resourceManager)
        {
            InitializeComponent();
            BindingContext = this;
            this.resourceManager = resourceManager;
            OnPropertyChanged(nameof(CurrentCulture));
        }

        public string? CurrentCulture => resourceManager?.CurrentCulture.NativeName;

        public string CounterBtnText
        {
            get
            {
                if (count == 0) return AppResources.ClickMe;
                if (count == 1) return string.Format(AppResources.ClickedOneTime, count);
                return string.Format(AppResources.ClickedManyTimes, count);
            }
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            OnPropertyChanged(nameof(CounterBtnText));
        }

        private void OnToggleLanguage(object sender, EventArgs e)
        {
            var languages = new List<string>() { "en", "fr", "de", "es", "sv" };
            var culture = resourceManager.CurrentCulture;
            var index = languages.IndexOf(culture.TwoLetterISOLanguageName);
            resourceManager.CurrentCulture = new CultureInfo(languages[++index < languages.Count ? index : 0]);
            OnPropertyChanged(nameof(CounterBtnText));
            OnPropertyChanged(nameof(CurrentCulture));
        }
    }
}