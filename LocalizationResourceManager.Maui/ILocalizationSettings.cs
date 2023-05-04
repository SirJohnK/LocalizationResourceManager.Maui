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

        /// <summary>
        /// Activate support for Resource Names with Dots.
        /// </summary>
        /// <remarks>Dots in names will be temporarily replaced by the substitution text when handled by the <see cref="TranslateExtension"/>.</remarks>
        /// <param name="substitution">Replacement text for dots in resource names.</param>
        void SupportNameWithDots(string substitution = "_");
    }
}