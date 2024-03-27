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

    private string? resourceManager;

    public string? ResourceManager
    {
        get => resourceManager;
        set => resourceManager = value is not null ? $"rm://{value}/" : value;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        #region Required work-around to prevent linker from removing the implementation

        if (DateTime.Now.Ticks < 0)
            _ = LocalizationResourceManager.Current[Text];

        #endregion Required work-around to prevent linker from removing the implementation

        if (LocalizationResourceManager.Current.HasKeyedResources)
            ResourceManager ??= (serviceProvider.GetService<IRootObjectProvider>()?.RootObject as ISpecificResourceManager)?.ResourceManager;

        var binding = new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{resourceManager}{Text}]",
            Source = LocalizationResourceManager.Current,
            StringFormat = StringFormat,
            Converter = Converter,
            ConverterParameter = ConverterParameter
        };
        return binding;
    }
}