using FluentAssertions;
using FluentAssertions.Microsoft.Extensions.DependencyInjection;

namespace LocalizationResourceManager.Maui.Tests;

[TestClass]
public class T02_ResourceManagerTests
{
    [TestMethod()]
    public void T01_DefaultCulture_Should_Be_en_US()
    {
        AssembyContext.ResourceManager!.DefaultCulture.Name.Should().Be("en-US");
    }

    [TestMethod]
    public void T02_GetValue_Should_Return_Hello_World()
    {
        AssembyContext.ResourceManager!.GetValue("Hello").Should().Be("Hello");
        AssembyContext.ResourceManager!.GetValue("World").Should().Be("World");
    }

    [TestMethod]
    public void T03_GetValue_Should_Return_Swedish_Hello_World()
    {
        AssembyContext.ResourceManager!.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
        AssembyContext.ResourceManager!.GetValue("Hello").Should().Be("Hej");
        AssembyContext.ResourceManager!.GetValue("World").Should().Be("Världen");
    }

    [TestMethod]
    public void T04_TranslateExtension_Should_Return_Hello_World()
    {
        //Set US English culture
        AssembyContext.ResourceManager!.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        //Setup Hello TranslateExtension Label
        var helloLabel = new Label();
        var translateHello = new TranslateExtension { Text = "Hello" };
        helloLabel.SetBinding(Label.TextProperty, translateHello.ProvideValue(AssembyContext.ServiceProvider!));

        //Setup World TranslateExtension Label
        var worldLabel = new Label();
        var translateWorld = new TranslateExtension { Text = "World" };
        worldLabel.SetBinding(Label.TextProperty, translateWorld.ProvideValue(AssembyContext.ServiceProvider!));

        //Test Hello Label
        helloLabel.Text.Should().Be("Hello");

        //Test World Label
        worldLabel.Text.Should().Be("World");

        //Cleanup
        helloLabel.RemoveBinding(Label.TextProperty);
        worldLabel.RemoveBinding(Label.TextProperty);
    }

    [TestMethod]
    public void T05_TranslateExtension_Should_Return_Swedish_Hello_World()
    {
        //Set Swedish culture
        AssembyContext.ResourceManager!.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");

        //Setup Hello TranslateExtension Label
        var helloLabel = new Label();
        var translateHello = new TranslateExtension { Text = "Hello" };
        helloLabel.SetBinding(Label.TextProperty, translateHello.ProvideValue(AssembyContext.ServiceProvider!));

        //Setup World TranslateExtension Label
        var worldLabel = new Label();
        var translateWorld = new TranslateExtension { Text = "World" };
        worldLabel.SetBinding(Label.TextProperty, translateWorld.ProvideValue(AssembyContext.ServiceProvider!));

        //Test Hello Label
        helloLabel.Text.Should().Be("Hej");

        //Test World Label
        worldLabel.Text.Should().Be("Världen");

        //Cleanup
        helloLabel.RemoveBinding(Label.TextProperty);
        worldLabel.RemoveBinding(Label.TextProperty);
    }

    [TestMethod]
    public void T06_GetValue_Should_Return_Generic_Resource_Title()
    {
        //Set US English culture
        AssembyContext.ResourceManager!.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        //Verify Generic Title
        AssembyContext.ResourceManager!.GetValue("Title").Should().Be("Home");
    }

    [TestMethod]
    public void T07_GetValue_Should_Return_Specific_Resource_Title()
    {
        //Set US English culture
        AssembyContext.ResourceManager!.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        //Verify Generic Title
        AssembyContext.ResourceManager!.GetValue("Title", "SpecificPage").Should().Be("Specific Resources");
    }

    public int Count { get; set; }

    [TestMethod]
    public void T08_TranslateBindingExtension_Should_Return_Count_Binding_Text()
    {
        //Set US English culture
        AssembyContext.ResourceManager!.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        //Setup Count TranslateExtension Label
        var countLabel = new Label() { BindingContext = this };
        var translateCount = new TranslateBindingExtension() { Path = "Count", TranslateZero = "ClickMe", TranslateOne = "ClickedOneTime", TranslateFormat = "ClickedManyTimes" };

        //Test Count = 0 Label
        Count = 0;
        countLabel.SetBinding(Label.TextProperty, (translateCount as IMarkupExtension<BindingBase>).ProvideValue(AssembyContext.ServiceProvider!));
        countLabel.Text.Should().Be("Click me");

        //Test Count = 1 Label
        Count = 1;
        countLabel.SetBinding(Label.TextProperty, (translateCount as IMarkupExtension<BindingBase>).ProvideValue(AssembyContext.ServiceProvider!));
        countLabel.Text.Should().Be($"Clicked {Count} time");

        //Test Count > 1 Label
        Count = 2;
        countLabel.SetBinding(Label.TextProperty, (translateCount as IMarkupExtension<BindingBase>).ProvideValue(AssembyContext.ServiceProvider!));
        countLabel.Text.Should().Be($"Clicked {Count} times");

        //Cleanup
        countLabel.RemoveBinding(Label.TextProperty);
    }

    [TestMethod]
    public void T09_TranslateBindingExtension_Should_Return_Swedish_Count_Binding_Text()
    {
        //Set Swedish culture
        AssembyContext.ResourceManager!.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");

        //Setup Count TranslateExtension Label
        var countLabel = new Label() { BindingContext = this };
        var translateCount = new TranslateBindingExtension() { Path = "Count", TranslateZero = "ClickMe", TranslateOne = "ClickedOneTime", TranslateFormat = "ClickedManyTimes" };

        //Test Count = 0 Label
        Count = 0;
        countLabel.SetBinding(Label.TextProperty, (translateCount as IMarkupExtension<BindingBase>).ProvideValue(AssembyContext.ServiceProvider!));
        countLabel.Text.Should().Be("Klicka på mig");

        //Test Count = 1 Label
        Count = 1;
        countLabel.SetBinding(Label.TextProperty, (translateCount as IMarkupExtension<BindingBase>).ProvideValue(AssembyContext.ServiceProvider!));
        countLabel.Text.Should().Be($"Klickat {Count} gång");

        //Test Count > 1 Label
        Count = 2;
        countLabel.SetBinding(Label.TextProperty, (translateCount as IMarkupExtension<BindingBase>).ProvideValue(AssembyContext.ServiceProvider!));
        countLabel.Text.Should().Be($"Klickat {Count} gånger");

        //Cleanup
        countLabel.RemoveBinding(Label.TextProperty);
    }
}