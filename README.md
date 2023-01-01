# LocalizationResourceManager.Maui <a href="https://www.buymeacoffee.com/sirjohnk" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" align="right" style="height: 37px !important;width: 170px !important;" ></a>

Enhanced .NET MAUI version of the Xamarin Community Toolkit LocalizationResourceManager.

## NuGet

|Name|Info|
| ------------------- | :------------------: |
|LocalizationResourceManager.Maui|[![NuGet](https://buildstats.info/nuget/LocalizationResourceManager.Maui?includePreReleases=true)](https://www.nuget.org/packages/LocalizationResourceManager.Maui/)|

## Background

I have been a fan of the Localization helpers and extensions in the [Xamarin Community Toolkit](https://github.com/xamarin/XamarinCommunityToolkit) and have been using this in my Xamarin projects. Since moving to [.NET MAUI](https://github.com/dotnet/maui), I hoped for this to be part of the [MAUI Community Toolkit](https://github.com/CommunityToolkit/Maui). For good reasons the team has decided not to include this in MCT and a [proposal](https://github.com/CommunityToolkit/dotnet/issues/312) is issued in the [.NET Community Toolkit](https://github.com/CommunityToolkit/dotnet). But the XCT solution have dependencies to the Xamarin `IMarkupExtension` interface and the XCT `WeakEventManager` helper class, which makes it tricky to port to a non MAUI library. So until we have a official solution, or anyway is added to MCT, I have created this library for .NET MAUI.

Big shoutout to the original authors, [Charlin Agramonte](https://github.com/Char0394), [Brandon Minnick](https://github.com/brminnick), [Maksym Koshovyi](https://github.com/maxkoshevoi) and the entire [Xamarin Community Toolkit Team](https://github.com/xamarin/XamarinCommunityToolkit/graphs/contributors)!

## What's included?
Compared to the original solution we have some enhanced and added features:
- Easy setup with builder pattern extension
- Supports multiple Resource managers
- Supports storing and restoring of the latest set culture
- New `ILocalizationResourceManager` interface registered for constructor injection with DI
- Stores current Default / System culture
- Uses the WeakEventHandler (.NET MAUI)

For localized texts used in XAML and/or code behind, we still have:
- TranslateExtension (XAML Markup Extension)
- LocalizedString (Track Culture Change in code behind)

## Setup
Use the `UseLocalizationResourceManager`builder pattern extension method for library configuration.
```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
    })
    .UseLocalizationResourceManager(settings =>
    {
        settings.AddResource(AppResources.ResourceManager);
        settings.RestoreLatestCulture(true);
    });
```
Settings contains 3 methods for configuration:
- **AddResource** (Add one or more Resource Managers)
- **InitialCulture** (Set initial/startup culture, Default: Current System Culture)
- **RestoreLatestCulture** (Restore latest set culture flag, Default: false, Note: Will override InitalCulture!)

## Use in XAML
When used for localized texts in XAML pages, use the `TranslateExtension`:
- Add namespace reference to library.
- Use Translate extension with name of resource. (All resource libraries will be searched until name is found!)
```csharp
<ContentPage
    x:Class="LocalizationResourceManager.Maui.Sample.MainPage"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:localization="clr-namespace:LocalizationResourceManager.Maui;assembly=LocalizationResourceManager.Maui">
```
```csharp
<Label
    FontSize="18"
    HorizontalOptions="Center"
    SemanticProperties.Description="{localization:Translate WelcomeToMAUI}"
    SemanticProperties.HeadingLevel="Level2"
    Text="{localization:Translate WelcomeToMAUI}" />
```

## Use in Code
When used to handle localized texts in code behind or ViewModel, use the `LocalizedString` class:
- Add LocalizedString to code behind or ViewModel to track culture changes
- If needed, make binding to LocalizedString in XAML
```csharp
public LocalizedString HelloWorld { get; } = new(() => $"{AppResources.Hello}, {AppResources.World}!");
```
...or to support multiple Resource managers...
```csharp
public LocalizedString HelloWorld { get; }

public MainPage(ILocalizationResourceManager resourceManager)
{
    HelloWorld = new(() => $"{resourceManager["Hello"]}, {resourceManager["World"]}!");
```
```csharp
<Label
    FontSize="32"
    HorizontalOptions="Center"
    SemanticProperties.HeadingLevel="Level1"
    Text="{Binding HelloWorld.Localized}" />
```

## Set and Get Culture
To handle and access the Current or Default Culture, we inject the `ILocalizationResourceManager` interface into our code behind or ViewModel to access the LocalizationResourceManager instance:
- Add the `ILocalizationResourceManager` interface to your constructor and store locally for later access.
- Use **CurrentCulture** property to Get or Set CurrentCulture. (All text accessed by `TranslateExtension` or `LocalizedString` will be updated immediately!)
- Use **DefaultCulture** property to Get Default/System culture.
- Use **GetValue** method or Indexer operator **[]** to manually retrieve localized text based on Current culture.
```csharp
public partial class MainPage : ContentPage
{
    private readonly ILocalizationResourceManager resourceManager;

    public MainPage(ILocalizationResourceManager resourceManager)
    {
        InitializeComponent();
        this.resourceManager = resourceManager;
```
```csharp
public string? CurrentCulture => resourceManager?.CurrentCulture.NativeName;
```
...or...
```csharp
public LocalizedString CurrentCulture { get; }

public MainPage(ILocalizationResourceManager resourceManager)
{
    CurrentCulture = new(() => resourceManager.CurrentCulture.NativeName);
```
One line to change Current Culture and Refresh ALL localized texts!
```csharp
resourceManager.CurrentCulture = new CultureInfo("en");
```

## Sample
Look at the [Sample project](https://github.com/SirJohnK/LocalizationResourceManager.Maui/tree/main/LocalizationResourceManager.Maui.Sample) for a example of how to use this library in an .NET MAUI application.

[![Sample Application](https://github.com/SirJohnK/LocalizationResourceManager.Maui/blob/main/Docs/LocalizationResourceManager.gif)](https://github.com/SirJohnK/LocalizationResourceManager.Maui/tree/main/LocalizationResourceManager.Maui.Sample)
