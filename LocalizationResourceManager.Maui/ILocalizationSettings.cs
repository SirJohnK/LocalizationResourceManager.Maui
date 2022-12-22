using System.Globalization;
using System.Resources;

namespace LocalizationResourceManager.Maui
{
    public interface ILocalizationSettings
    {
        void AddResource(ResourceManager resource);

        void InitialCulture(CultureInfo culture);

        void RestoreLatestCulture(bool restore);
    }
}