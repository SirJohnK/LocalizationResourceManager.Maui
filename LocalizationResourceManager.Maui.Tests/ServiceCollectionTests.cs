using FluentAssertions;
using FluentAssertions.Microsoft.Extensions.DependencyInjection;

namespace LocalizationResourceManager.Maui.Tests;

[TestClass]
public class T01_ServiceCollectionTests
{
    [TestMethod]
    public void T01_ServiceCollection_Should_Contain_LocalizationResourceManager()
    {
        AssembyContext.ServiceProvider!.GetService<ILocalizationResourceManager>().Should().NotBeNull();
    }

    [TestMethod]
    public void T02_ServiceCollection_Should_Contain_Keyed_SpecificResourceManager()
    {
        AssembyContext.ServiceProvider!.GetKeyedService<ILocalizationResourceManager>("SpecificPage").Should().NotBeNull();
    }

    [TestMethod]
    public void T03_ServiceCollection_Should_Contain_PlatformCulture()
    {
        AssembyContext.ServiceProvider!.GetService<IPlatformCulture>().Should().NotBeNull();
    }
}