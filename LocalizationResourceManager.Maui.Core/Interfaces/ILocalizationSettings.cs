using System;
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
        /// Register ResourceManager.
        /// </summary>
        /// <param name="resource">The ResourceManager to add</param>
        /// <remarks>Multiple resources can be added and will be search in the order they were added.</remarks>
        /// <returns>Flag indication if ResourceManager was added successfully</returns>
        bool AddResource(ResourceManager resource);

        /// <summary>
        /// Register ResourceManager with a specific key.
        /// </summary>
        /// <param name="resource">The ResourceManager to add</param>
        /// <param name="resourceKey">Key to identify the ResourceManager</param>
        /// <returns>Flag indication if ResourceManager was added successfully</returns>
        bool AddResource(ResourceManager resource, string resourceKey);

        /// <summary>
        /// Register file based ResourceManager and create default .resources file, if missing.
        /// </summary>
        /// <param name="baseName">The root name of the resource.</param>
        /// <param name="resourceDir">Path to resource directory.</param>
        /// <param name="usingResourceSet">Optional, Type of the ResourceSet the ResourceManager uses to construct ResourceSets.</param>
        /// <returns>Flag indication if ResourceManager was added successfully</returns>
        /// <exception cref="ArgumentNullException">If baseName or resourceDir is null, empty or whitespace.</exception>
        bool AddFileResource(string baseName, string resourceDir, Type? usingResourceSet = null);

        /// <summary>
        /// Register file based ResourceManager with a specific key and create default .resources file, if missing.
        /// </summary>
        /// <param name="baseName">The root name of the resource.</param>
        /// <param name="resourceDir">Path to resource directory.</param>
        /// <param name="resourceKey">Key to identify the ResourceManager</param>
        /// <param name="usingResourceSet">Optional, Type of the ResourceSet the ResourceManager uses to construct ResourceSets.</param>
        /// <returns>Flag indication if ResourceManager was added successfully</returns>
        /// <exception cref="ArgumentNullException">If baseName or resourceDir is null, empty or whitespace.</exception>
        bool AddFileResource(string baseName, string resourceDir, string resourceKey, Type? usingResourceSet = null);

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
        /// <param name="restore">Flag indicating if latest set culture should be restored. (Optional, Default: <see langword="true"/>)</param>
        /// <remarks>
        /// If set to <see langword="true"/>, this will override <see cref="InitialCulture(CultureInfo)"/>!
        /// </remarks>
        void RestoreLatestCulture(bool restore = true);

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

        /// <summary>
        /// Monitor the platform culture and update the ResourceManager current culture.
        /// Includes live update of the culture, if the platform culture changes.
        /// </summary>
        /// <param name="activate">Flag indicating if the platform culture should be monitored. (Optional, Default: <see langword="true"/>)</param>
        /// <remarks>With <see cref="RestoreLatestCulture(bool)"/> activated, Latest Culture will be restored on startup, regardless of current platform culture.</remarks>
        void MonitorPlatformCulture(DevicePlatform platform = DevicePlatform.All);
    }
}