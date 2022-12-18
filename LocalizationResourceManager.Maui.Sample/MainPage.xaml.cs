using LocalizationResourceManager.Maui.Sample.Resources;
using System.Globalization;

namespace LocalizationResourceManager.Maui.Sample
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;
        private readonly LocalizationResourceManager resourceManager;

        public LocalizedString HelloWorld { get; } = new(() => $"{AppResources.Hello}, {AppResources.World}!");

        public MainPage(LocalizationResourceManager resourceManager)
        {
            InitializeComponent();
            BindingContext = this;
            this.resourceManager = resourceManager;
        }

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
            var culture = resourceManager.CurrentCulture;
            if (culture.TwoLetterISOLanguageName.Equals("en"))
                culture = new CultureInfo("sv");
            else
                culture = new CultureInfo("en");

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            AppResources.Culture = culture;
            resourceManager.CurrentCulture = culture;
            OnPropertyChanged(nameof(CounterBtnText));
        }
    }
}