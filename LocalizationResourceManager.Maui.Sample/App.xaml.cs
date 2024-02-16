namespace LocalizationResourceManager.Maui.Sample
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF1cWWhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEZiWH1WcXNXTmNfWExxXQ==");

            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}