# LocalizationResourceManager.Maui <a href="https://www.buymeacoffee.com/sirjohnk" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" align="right" style="height: 37px !important;width: 170px !important;" ></a>

.NET MAUI version of the Xamarin Community Toolkit LocalizationResourceManager.

## NuGet

|Name|Info|
| ------------------- | :------------------: |
|LocalizationResourceManager.Maui|[![NuGet](https://buildstats.info/nuget/LocalizationResourceManager.Maui?includePreReleases=true)](https://www.nuget.org/packages/LocalizationResourceManager.Maui/)|

## Background

I have been a fan of the Localization helpers and extensions in the [Xamarin Community Toolkit](https://github.com/xamarin/XamarinCommunityToolkit) and have been using this in my Xamarin projects. Since moving to [.NET MAUI](https://github.com/dotnet/maui), I hoped for this to be part of the [MAUI Community Toolkit](https://github.com/CommunityToolkit/Maui). For good reasons the team has decided not to include this in MCT and a [proposal](https://github.com/CommunityToolkit/dotnet/issues/312) is issued in the [.NET Community Toolkit](https://github.com/CommunityToolkit/dotnet). But the XCT solution have dependencies to the Xamarin `IMarkupExtension` interface and the XCT `WeakEventManager` helper class, which makes it tricky to port to a non MAUI library. So until we have a official solution, or anyway is added to MCT, I have created this library for .NET MAUI.

Big shout out to the original authors, [Charlin Agramonte](https://github.com/Char0394), [Brandon Minnick](https://github.com/brminnick), [Maksym Koshovyi](https://github.com/maxkoshevoi) and the entire [Xamarin Community Toolkit Team](https://github.com/xamarin/XamarinCommunityToolkit/graphs/contributors)!
