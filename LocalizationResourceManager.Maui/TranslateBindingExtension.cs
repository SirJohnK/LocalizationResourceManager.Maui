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
    public IValueConverter Converter { get; set; } = null;

    /// <inheritdoc/>
    public object ConverterParameter { get; set; } = null;

    /// <inheritdoc/>
    public object Source { get; set; } = null;

    /// <inheritdoc/>
    public bool TranslateValue { get; set; } = false;

    /// <inheritdoc/>
    public string TranslateFormat { get; set; }

    /// <inheritdoc/>
    public string TranslateOne { get; set; }

    /// <inheritdoc/>
    public string TranslateZero { get; set; }

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
                new Binding(Path, Mode, Converter, ConverterParameter, null, Source),
                new Binding(nameof(LocalizationResourceManager.CurrentCulture), BindingMode.OneWay, null, null, null, LocalizationResourceManager.Current)
            }
        };
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 0 || values[0] == null)
        {
            return "";
        }

        if (!string.IsNullOrEmpty(TranslateZero) && IsZero(values[0]))
        {
            return LocalizationResourceManager.Current[TranslateZero, values];
        }

        if (!string.IsNullOrEmpty(TranslateOne) && IsOne(values[0]))
        {
            return LocalizationResourceManager.Current[TranslateOne, values];
        }

        if (!string.IsNullOrEmpty(TranslateFormat))
        {
            return LocalizationResourceManager.Current[TranslateFormat, values];
        }

        if (TranslateValue)
        {
            return LocalizationResourceManager.Current[values[0].ToString()];
        }

        return values[0];
    }

    static bool IsZero(object value)
    {
        if (value == null)
        {
            return false;
        }
        if (value.GetType() == typeof(int) && (int)value == 0)
        {
            return true;
        }
        return false;
    }

    static bool IsOne(object value)
    {
        if (value == null)
        {
            return false;
        }
        if (value.GetType() == typeof(int) && (int)value == 1)
        {
            return true;
        }
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
