using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;

namespace LocalizationResourceManager.Maui
{
    /// <summary>
    /// Interface for Localization Resource Manager Settings
    /// </summary>
    public interface ILocalizationSettings
    {
        /// <summary>
        /// Register ResourceManager and verify access.
        /// </summary>
        /// <param name="resource">The ResourceManager to add</param>
        /// <returns>Flag indication if ResourceManager was added successfully</returns>
        bool AddResource(ResourceManager resource);

        /// <summary>
        /// Register file based ResourceManager and create default .resources file, if missing.
        /// </summary>
        /// <param name="baseName">The root name of the resource.</param>
        /// <param name="resourceDir">Path to resource directory.</param>
        /// <param name="usingResourceSet">Optional, Type of the ResourceSet the ResourceManager uses to construct ResourceSets.</param>
        /// <returns>Flag indication if ResourceManager was added successfully</returns>
        /// <exception cref="ArgumentNullException">If baseName or resourceDir is null, empty or whitespace.</exception>
        bool AddFileResource(string baseName, string resourceDir, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type? usingResourceSet = null);

        /// <summary>
        /// Set initial/startup culture.
        /// </summary>
        /// <param name="culture">Culture to set</param>
        /// <remarks>
        /// If, <see cref="RestoreLatestCulture(bool)"/> is set to <see langword="true"/>, this will be ignored!
        /// </remarks>
        void InitialCulture(CultureInfo culture);

        /// <summary>
        /// Set if latest set culture should be restored. Default: <see langword="false"/>
        /// </summary>
        /// <param name="restore">Flag indicating if latest set culture should be restored</param>
        /// <remarks>
        /// If set to <see langword="true"/>, this will override <see cref="InitialCulture(CultureInfo)"/>!
        /// </remarks>
        void RestoreLatestCulture(bool restore);

        /// <summary>
        /// Activate support for Resource Names with Dots.
        /// </summary>
        /// <remarks>Dots in names will be temporarily replaced by the substitution text when handled by the <see cref="TranslateExtension"/>.</remarks>
        /// <param name="substitution">Replacement text for dots in resource names.</param>
        void SupportNameWithDots(string substitution = "_");

        /// <summary>
        /// Suppress/Deactive throwing the text not found exception.
        /// </summary>
        /// <param name="usePlaceholder">Flag indicating if a placeholder text should be displayed if text is not found. (Optional, Default: <see langword="false"/>)</param>
        /// <param name="placeholderText">Placeholder text displayed if text is not found, when UsePlaceHolder is activated.</param>
        /// <remarks>"{0}" in the placeholder text, will be replaced with the resource name.</remarks>
        void SuppressTextNotFoundException(bool usePlaceholder = false, string placeholderText = "{0}");
    }
}