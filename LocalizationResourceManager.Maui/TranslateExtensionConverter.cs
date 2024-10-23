using System.Globalization;

namespace LocalizationResourceManager.Maui;

internal sealed class TranslateExtensionConverter : IMultiValueConverter
{
    public object? Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not null && values[0] is string text && values[1] is CultureInfo c)
        {
            if (LocalizationResourceManager.Current.IsNameWithDotsSupported)
            {
                text = text.Replace(".", LocalizationResourceManager.Current.DotSubstitution);
            }

            object obj = LocalizationResourceManager.Current.GetValue(text);
            if (obj is null)
            {
                return null;
            }

            if (values.Length > 2 && obj is string format)
            {
                obj = string.Format(format, values.Skip(2).ToArray());
            }

            return $"{obj}";
        }

        return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
