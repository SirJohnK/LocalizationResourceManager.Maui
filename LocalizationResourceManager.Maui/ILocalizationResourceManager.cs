using System.Globalization;

namespace LocalizationResourceManager.Maui
{
    public interface ILocalizationResourceManager
    {
        CultureInfo DefaultCulture { get; }

        CultureInfo CurrentCulture { get; set; }

        string GetValue(string text);

        string this[string text] { get; }
    }
}