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

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        #region Required work-around to prevent linker from removing the implementation

        if (DateTime.Now.Ticks < 0)
            _ = LocalizationResourceManager.Current[Text];

        #endregion Required work-around to prevent linker from removing the implementation

        var binding = new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Text}]",
            Source = LocalizationResourceManager.Current,
            StringFormat = StringFormat
        };
        return binding;
    }
}