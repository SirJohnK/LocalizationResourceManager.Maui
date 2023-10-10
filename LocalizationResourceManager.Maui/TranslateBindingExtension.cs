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
        var value = values?.FirstOrDefault();
        if (value is null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(TranslateZero) && IsZero(value))
        {
            return LocalizationResourceManager.Current[TranslateZero, value];
        }

        if (!string.IsNullOrWhiteSpace(TranslateOne) && IsOne(value))
        {
            return LocalizationResourceManager.Current[TranslateOne, value];
        }

        if (!string.IsNullOrWhiteSpace(TranslateFormat))
        {
            return LocalizationResourceManager.Current[TranslateFormat, value];
        }

        if (TranslateValue)
        {
            return LocalizationResourceManager.Current[$"{value}"];
        }

        return value;
    }

    private static bool IsZero(object value) => (value is int number && number == 0);

    private static bool IsOne(object value) => (value is int number && number == 1);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}