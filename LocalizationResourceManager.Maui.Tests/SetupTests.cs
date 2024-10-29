using LocalizationResourceManager.Maui.Tests.Mock;
using LocalizationResourceManager.Maui.Tests.Resources;
using System.Globalization;

namespace LocalizationResourceManager.Maui.Tests;

[TestClass]
public sealed class AssembyContext
{
    public static ILocalizationResourceManager? ResourceManager { get; private set; }

    public static ILocalizationSettings? Settings { get; private set; }

    public static IServiceProvider? ServiceProvider { get; private set; }

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        //Init
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        IServiceCollection services = new ServiceCollection();
        IPreferences preferences = new TestPreferences();

        //Setup LocalizationResourceManager
        var currentResourceManager = LocalizationResourceManager.Current;
        ResourceManager = currentResourceManager;

        //Add Internal Services
        currentResourceManager.RegisterService(preferences);
        currentResourceManager.RegisterService(() => PlatformCulture.Current);

        //Configure Settings
        Settings = currentResourceManager;
        Settings.AddResource(AppResources.ResourceManager);
        Settings.AddResource(SpecificResources.ResourceManager, "SpecificPage");

        //Register Services
        services.AddSingleton<ILocalizationSettings>(currentResourceManager);
        services.AddSingleton<ILocalizationResourceManager>(currentResourceManager);
        services.AddSingleton((serviceProvider) => PlatformCulture.Current);
        services.AddKeyedSingleton<ILocalizationResourceManager>(KeyedService.AnyKey, (serviceProvider, key) =>
        {
            // Attempt to resolve the keyed service
            if (key is string keyString && currentResourceManager.KeyedResources.TryGetValue(keyString, out var keyedResource))
                return keyedResource;
            else
                return default!;
        });

        //Build Service Provider
        ServiceProvider = services.BuildServiceProvider();
    }
}