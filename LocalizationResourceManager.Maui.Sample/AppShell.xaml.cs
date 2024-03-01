namespace LocalizationResourceManager.Maui.Sample
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(SpecificPage), typeof(SpecificPage));
        }
    }
}