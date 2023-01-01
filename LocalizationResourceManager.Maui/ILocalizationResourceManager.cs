using System.Globalization;

namespace LocalizationResourceManager.Maui
{
    public interface ILocalizationResourceManager
    {
        CultureInfo DefaultCulture { get; }

        CultureInfo CurrentCulture { get; set; }

        string GetValue(string text);

        string GetValue(string text, params object[] arguments);

        string this[string text] { get; }

        string this[string text, params object[] arguments] { get; }
    }
}