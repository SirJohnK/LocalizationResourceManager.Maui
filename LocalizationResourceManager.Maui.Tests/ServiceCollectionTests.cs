using FluentAssertions;
using LocalizationResourceManager.Maui.Tests.Resources;
using FluentAssertions.Microsoft.Extensions.DependencyInjection;

namespace LocalizationResourceManager.Maui.Tests
{
    [TestClass]
    public class ServiceCollectionTests
    {
        private readonly ILocalizationSettings settings;
        private readonly IServiceCollection services = new ServiceCollection();
        private readonly IServiceProvider serviceProvider;

        public ServiceCollectionTests()
        {
            //Init
            LocalizationResourceManager.Current.Services = services;

            //Configure Settings
            settings = LocalizationResourceManager.Current;
            settings.AddResource(AppResources.ResourceManager);
            settings.AddResource(SpecificResources.ResourceManager, "SpecificPage");

            //Register Service
            services.AddSingleton(Preferences.Default);
            services.AddSingleton<ILocalizationResourceManager>(LocalizationResourceManager.Current);

            //Build Service Provider
            serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void ServiceCollection_Should_Contain_Three()
        {
            services.Should().HaveCount(3);
        }
    }
}