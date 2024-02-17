using System.Collections.ObjectModel;
using System.Globalization;

namespace LocalizationResourceManager.Maui;

[ContentProperty(nameof(Path))]
public class TranslateBindingExtension : IMarkupExtension<BindingBase>, IMultiValueConverter
{
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

    /// <inheritdoc/>
    public bool TranslateValue { get; set; } = false;

    /// <inheritdoc/>
    public string? TranslateFormat { get; set; }

    /// <inheritdoc/>
    public string? TranslateOne { get; set; }

    /// <inheritdoc/>
    public string? TranslateZero { get; set; }

    /// <inheritdoc/>
    public string? TranslateTrue { get; set; }

    /// <inheritdoc/>
    public string? TranslateFalse { get; set; }

    public string? ResourceManager { get; set; }

    /// <inheritdoc/>
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return (this as IMarkupExtension<BindingBase>).ProvideValue(serviceProvider);
    }

    BindingBase IMarkupExtension<BindingBase>.ProvideValue(IServiceProvider serviceProvider)
    {
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

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        //Get and Verify Current Value
        var value = values?.FirstOrDefault();
        if (value is null) return string.Empty;

        //Init
        string? text = null;
        var resourceManager = LocalizationResourceManager.Current;

        //Get Translation Text
        if (!string.IsNullOrWhiteSpace(TranslateZero) && IsZero(value)) text = TranslateZero;
        else if (!string.IsNullOrWhiteSpace(TranslateOne) && IsOne(value)) text = TranslateOne;
        else if (!string.IsNullOrWhiteSpace(TranslateTrue) && IsTrue(value)) text = TranslateTrue;
        else if (!string.IsNullOrWhiteSpace(TranslateFalse) && IsFalse(value)) text = TranslateFalse;
        else if (!string.IsNullOrWhiteSpace(TranslateFormat)) text = TranslateFormat;
        else if (TranslateValue) text = $"{value}";

        //Resolve Translation Text
        if (text is not null) return ResourceManager is null ? resourceManager[text, value] : resourceManager[text, ResourceManager, value];

        //Return Value since translation was not found or resolved!
        return value;
    }

    private static bool IsZero(object value) => (value is int number && number == 0);

    private static bool IsOne(object value) => (value is int number && number == 1);

    private static bool IsTrue(object value) => (value is bool flag && flag);

    private static bool IsFalse(object value) => (value is bool flag && !flag);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}