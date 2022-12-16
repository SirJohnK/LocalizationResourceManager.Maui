using LocalizationResourceManager.Maui.Sample.Resources;
using System.Globalization;

namespace LocalizationResourceManager.Maui.Sample.Converters
{
    public class ClickCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = value as int? ?? 0;
            if (count == 0) return AppResources.ClickMe;
            if (count == 1) return string.Format(AppResources.ClickedOneTime, count);
            return string.Format(AppResources.ClickedManyTimes, count);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}