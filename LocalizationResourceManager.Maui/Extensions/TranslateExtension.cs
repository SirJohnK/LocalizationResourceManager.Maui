using System.Resources;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Markup extension (XAML) for handling and updating localized string by tracking current culture from current localization resource manager.
/// </summary>
[ContentProperty(nameof(Text))]
public class TranslateExtension : IMarkupExtension<BindingBase>
{
    private string text = string.Empty;

    public string Text
    {
        get => text;
        set => text = LocalizationResourceManager.Current.IsNameWithDotsSupported
            ? value.Replace(".", LocalizationResourceManager.Current.DotSubstitution)
            : value;
    }

    public string? StringFormat { get; set; }

    public IValueConverter? Converter { get; set; }

    public object? ConverterParameter { get; set; }

    public string? ResourceManager { get; set; }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

    [Microsoft.Maui.Controls.Internals.Preserve(Conditional = true)]
    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        //Init
        ILocalizationResourceManager resourceManagerInstance = LocalizationResourceManager.Current;

        // Handle specific resource manager
        if (LocalizationResourceManager.Current.HasKeyedResources)
        {
            //Any specific resource manager specfiied?
            ResourceManager ??= (serviceProvider.GetService<IRootObjectProvider>()?.RootObject as ISpecificResourceManager)?.ResourceManager;

            //Attempt to resolve specific resource manager
            if (!string.IsNullOrWhiteSpace(ResourceManager))
            {
                resourceManagerInstance = LocalizationResourceManager.Current.GetResourceManager(ResourceManager) ?? LocalizationResourceManager.Current;
            }
        }

        //Setup binding
        var binding = new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Text}]",
            Source = resourceManagerInstance,
            StringFormat = StringFormat,
            Converter = Converter,
            ConverterParameter = ConverterParameter
        };

        //Return translation binding
        return binding;
    }
}