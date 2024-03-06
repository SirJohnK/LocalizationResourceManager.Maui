using System.Globalization;

namespace LocalizationResourceManager.Maui.Sample.Converters;

public class ToUpperConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        (value as string ?? value?.ToString())?.ToUpper(culture);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}