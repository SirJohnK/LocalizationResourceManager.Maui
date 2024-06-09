using System.Globalization;

namespace LocalizationResourceManager.Maui.Sample
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;
        private readonly ILocalizationResourceManager resourceManager;

        public int Count
        {
            get => count;
            set
            {
                count = value;
                OnPropertyChanged(nameof(Count));
            }
        }

        public LocalizedString HelloWorld { get; }
        public LocalizedString CurrentCulture { get; }

        public MainPage(ILocalizationResourceManager resourceManager)
        {
            InitializeComponent();
            this.resourceManager = resourceManager;

            var test = LocalizedString(() => "Hello, Hello!");

            HelloWorld = resourceManager.LocalizedString(new(() => $"{resourceManager["Hello"]}, {resourceManager["World"]}!"));
            CurrentCulture = new(() => resourceManager.CurrentCulture.NativeName);

            LocalizedString newtest = delegate () { return resourceManager["Hello"]; };

            BindingContext = this;
        }

        private void OnCounterClicked(object sender, EventArgs e) => Count++;

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