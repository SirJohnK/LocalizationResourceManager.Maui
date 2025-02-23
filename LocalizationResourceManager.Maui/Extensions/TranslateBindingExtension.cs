using System.Globalization;
using System.Collections.ObjectModel;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Markup extension (XAML) for handling and updating localized string with custom binding by tracking current culture from current localization resource manager.
/// </summary>
/// <remarks>Supports page specific resource manager.</remarks>
[ContentProperty(nameof(Path))]
[RequireService([typeof(IReferenceProvider), typeof(IProvideValueTarget)])]
public class TranslateBindingExtension : IMarkupExtension<BindingBase>, IMultiValueConverter
{
    // Internal Properties
    private ILocalizationResourceManager resourceManagerInstance = LocalizationResourceManager.Current;

    /// <inheritdoc/>
    public string Path { get; set; } = ".";

    /// <inheritdoc/>
    public BindingMode Mode { get; set; } = BindingMode.OneWay;

    /// <inheritdoc/>
    public string StringFormat { get; set; } = "{0}";

    /// <inheritdoc/>
    public IValueConverter? Converter { get; set; } = null;

    /// <inheritdoc/>
    public object? ConverterParameter { get; set; } = null;

    /// <inheritdoc/>
    public object? Source { get; set; } = null;

    /// <summary>
    /// Flag indicating if binded value should be translated.
    /// </summary>
    public bool TranslateValue { get; set; } = false;

    /// <summary>
    /// Resource key for the translated text used with the binded value.
    /// </summary>
    public string? TranslateFormat { get; set; }

    /// <summary>
    /// Resource key for the translated text used if the binded value is equal to numeric value 1.
    /// </summary>
    public string? TranslateOne { get; set; }

    /// <summary>
    /// Resource key for the translated text used if the binded value is equal to numeric value 0.
    /// </summary>
    public string? TranslateZero { get; set; }

    /// <summary>
    /// Resource key for the translated text used if the binded value is equal to boolean value true.
    /// </summary>
    public string? TranslateTrue { get; set; }

    /// <summary>
    /// Resource key for the translated text used if the binded value is equal to boolean value false.
    /// </summary>
    public string? TranslateFalse { get; set; }

    public string? ResourceManager { get; set; }

    /// <inheritdoc/>
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return (this as IMarkupExtension<BindingBase>).ProvideValue(serviceProvider);
    }

    /// <summary>
    /// Provides the value for the translation binding.
    /// </summary>
    /// <param name="serviceProvider">Basic service provider.</param>
    /// <returns>Resource manager and translate converter multi binding.</returns>
    BindingBase IMarkupExtension<BindingBase>.ProvideValue(IServiceProvider serviceProvider)
    {
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

        //Return translation binding
        return new MultiBinding()
        {
            StringFormat = StringFormat,
            Converter = this,
            Mode = Mode,
            Bindings = new Collection<BindingBase>
            {
                new Binding(Path, Mode, Converter, ConverterParameter, source: Source),
                new Binding(nameof(LocalizationResourceManager.CurrentCulture), BindingMode.OneWay, source:LocalizationResourceManager.Current)
            }
        };
    }

    /// <summary>
    /// Convert the binded value to translated text.
    /// </summary>
    /// <returns>Translated text with the binded value.</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        //Get and Verify Current Value
        var value = values?.FirstOrDefault();
        if (value is null) return string.Empty;

        //Init
        string? text = null;

        //Get Translation Text
        if (!string.IsNullOrWhiteSpace(TranslateZero) && IsZero(value)) text = TranslateZero;
        else if (!string.IsNullOrWhiteSpace(TranslateOne) && IsOne(value)) text = TranslateOne;
        else if (!string.IsNullOrWhiteSpace(TranslateTrue) && IsTrue(value)) text = TranslateTrue;
        else if (!string.IsNullOrWhiteSpace(TranslateFalse) && IsFalse(value)) text = TranslateFalse;
        else if (!string.IsNullOrWhiteSpace(TranslateFormat)) text = TranslateFormat;
        else if (TranslateValue) text = $"{value}";

        //Resolve Translation Text
        if (text is not null) return resourceManagerInstance.GetValue(text, value);

        //Return Value since translation was not found or resolved!
        return value;
    }

    /// <summary>
    /// Evaluate if a value is equal to numeric value 0.
    /// </summary>
    private static bool IsZero(object value) => (value is int number && number == 0);

    /// <summary>
    /// Evaluate if a value is equal to numeric value 1.
    /// </summary>
    private static bool IsOne(object value) => (value is int number && number == 1);

    /// <summary>
    /// Evaluate if a value is equal to boolean value true.
    /// </summary>
    private static bool IsTrue(object value) => (value is bool flag && flag);

    /// <summary>
    /// Evaluate if a value is equal to boolean value false.
    /// </summary>
    private static bool IsFalse(object value) => (value is bool flag && !flag);

    /// <inheritdoc/>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}