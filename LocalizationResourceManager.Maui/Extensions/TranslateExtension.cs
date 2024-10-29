namespace LocalizationResourceManager.Maui;

/// <summary>
/// Markup extension (XAML) for handling and updating localized string by tracking current culture from current localization resource manager.
/// </summary>
/// <remarks>Supports page specific resource manager.</remarks>
[ContentProperty(nameof(Text))]
public class TranslateExtension : IMarkupExtension<BindingBase>
{
    private string text = string.Empty;

    /// <summary>
    /// Gets or sets the resource key for the translated text.
    /// </summary>
    public string Text
    {
        get => text;
        set => text = LocalizationResourceManager.Current.IsNameWithDotsSupported
            ? value.Replace(".", LocalizationResourceManager.Current.DotSubstitution)
            : value;
    }

    /// <summary>
    /// Gets or sets the string format for the translated text.
    /// </summary>
    public string? StringFormat { get; set; }

    /// <summary>
    /// Gets or sets the value converter used for the translated text.
    /// </summary>
    public IValueConverter? Converter { get; set; }

    /// <summary>
    /// Gets or sets the value converter parameter used for the translated text.
    /// </summary>
    public object? ConverterParameter { get; set; }

    /// <summary>
    /// Gets or sets the name of a specific resource manager used for the translated text.
    /// </summary>
    /// <remarks>If nout found, the default resource manager will be used!</remarks>
    public string? ResourceManager { get; set; }

    /// <inheritdoc/>
    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

    /// <summary>
    /// Provides the value for the translation binding.
    /// </summary>
    /// <param name="serviceProvider">Basic service provider.</param>
    /// <remarks>Preserve attribute added to ensure implementation is preserved by linker.</remarks>
    /// <returns>Resource manager binding.</returns>
    [Microsoft.Maui.Controls.Internals.Preserve(Conditional = true)]
    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        //Init
        ILocalizationResourceManager resourceManagerInstance = LocalizationResourceManager.Current;

        // Handle specific resource manager
        if (LocalizationResourceManager.Current.HasKeyedResources)
        {
            //Any specific resource manager specified?
            ResourceManager ??= (serviceProvider.GetService<IProvideValueTarget>()?.GetRootObject() as ISpecificResourceManager)?.ResourceManager;

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