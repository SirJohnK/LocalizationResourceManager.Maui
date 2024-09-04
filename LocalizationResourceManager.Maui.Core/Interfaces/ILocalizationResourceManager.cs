using System.ComponentModel;
using System.Globalization;

namespace LocalizationResourceManager.Maui
{
    /// <summary>
    /// Interface for Localization Resource Manager
    /// </summary>
    public interface ILocalizationResourceManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Get Default / System culture.
        /// </summary>
        CultureInfo DefaultCulture { get; }

        /// <summary>
        /// Get/Set Current culture for resource manager.
        /// </summary>
        CultureInfo CurrentCulture { get; set; }

        /// <summary>
        /// Get resource text value for <see cref="CurrentCulture"/>.
        /// </summary>
        /// <param name="text">Resource name.</param>
        /// <remarks>Will search all registered resources, in the order they were added, until first match is found!</remarks>
        /// <returns>Found resource text value.</returns>
        string GetValue(string text);

        string GetValue(string text, string resourceManager);

        /// <summary>
        /// Get formatted resource text value for <see cref="CurrentCulture"/> with specified parameters.
        /// </summary>
        /// <param name="text">Resource name.</param>
        /// <param name="arguments">Parameters used when formatting resource text value.</param>
        /// <remarks>
        /// Uses <see cref="string.Format(string, object?[])"/> syntax.
        /// Will search all registered resources, in the order they were added, until first match is found!
        /// </remarks>
        /// <returns>Formatted resource text value.</returns>
        string GetValue(string text, params object[] arguments);

        string GetValue(string text, string resourceManager, params object[] arguments);

        /// <summary>
        /// Indexer property to Get resource text value for <see cref="CurrentCulture"/>.
        /// </summary>
        /// <param name="text">Resource name.</param>
        /// <remarks>Will search all registered resources, in the order they were added, until first match is found!</remarks>
        /// <returns>Found resource text value.</returns>
        string this[string text] { get; }

        string this[string text, string resourceManager] { get; }

        /// <summary>
        /// Indexer property to Get formatted resource text value for <see cref="CurrentCulture"/> with specified parameters.
        /// </summary>
        /// <param name="text">Resource name.</param>
        /// <param name="arguments">Parameters used when formatting resource text value.</param>
        /// <remarks>
        /// Uses <see cref="string.Format(string, object?[])"/> syntax.
        /// Will search all registered resources, in the order they were added, until first match is found!
        /// </remarks>
        /// <returns>Formatted resource text value.</returns>
        string this[string text, params object[] arguments] { get; }

        string this[string text, string resourceManager, params object[] arguments] { get; }

        /// <summary>
        /// Release All Resources for All registered resources.
        /// </summary>
        void ReleaseAllResources();
    }
}