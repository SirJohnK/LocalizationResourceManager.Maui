﻿using LocalizationResourceManager.Maui.Sample.Resources;
using System.Globalization;

namespace LocalizationResourceManager.Maui.Sample
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;
        private readonly ILocalizationResourceManager resourceManager;

        public LocalizedString HelloWorld { get; }
        public LocalizedString CurrentCulture { get; }

        public MainPage(ILocalizationResourceManager resourceManager)
        {
            InitializeComponent();
            this.resourceManager = resourceManager;

            HelloWorld = new(() => $"{resourceManager["Hello"]}, {resourceManager["World"]}!");
            CurrentCulture = new(() => resourceManager.CurrentCulture.NativeName);

            BindingContext = this;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.SetTranslate(Button.TextProperty, "ClickedOneTime", count);
            else
                CounterBtn.SetTranslate(Button.TextProperty, "ClickedManyTimes", count);

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void OnToggleLanguage(object sender, EventArgs e)
        {
            var languages = new List<string>() { "en", "fr", "de", "es", "sv" };
            var culture = resourceManager.CurrentCulture;
            var index = languages.IndexOf(culture.TwoLetterISOLanguageName);
            resourceManager.CurrentCulture = new CultureInfo(languages[++index < languages.Count ? index : 0]);
        }
    }
}