using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;

namespace LocalizationResourceManager.Maui
{
    public interface ILocalizationSettings
    {
        bool AddResource(ResourceManager resource);

        bool AddFileResource(string baseName, string resourceDir, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type? usingResourceSet = null);

        void InitialCulture(CultureInfo culture);

        void RestoreLatestCulture(bool restore);
    }
}