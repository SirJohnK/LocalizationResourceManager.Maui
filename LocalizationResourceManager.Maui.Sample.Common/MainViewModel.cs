using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using LocalizationResourceManager.Maui.Core;

namespace LocalizationResourceManager.Maui.Sample.Common;

public partial class MainViewModel : ObservableObject
{
    public LocalizedString HelloWorld { get; }
    public LocalizedString CurrentCulture { get; }

    public MainViewModel(ILocalizationResourceManager resourceManager)
    {
        //Init
        HelloWorld = resourceManager.CreateLocalizedString(() => $"{resourceManager["Hello"]}, {resourceManager["World"]}!");
        CurrentCulture = new(resourceManager, () => resourceManager.CurrentCulture.NativeName);
    }

    [ObservableProperty]
    private int count;

    [RelayCommand]
    private void CounterClicked() => Count++;
}