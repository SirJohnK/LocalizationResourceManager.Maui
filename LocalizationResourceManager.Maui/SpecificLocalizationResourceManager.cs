using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Manager to track specific resource manager and current culture.
/// </summary>
internal class SpecificLocalizationResourceManager(ResourceManager resource) : ILocalizationResourceManager
{
    /// <summary>
    /// Specific resource manager to use.
    /// </summary>
    public ResourceManager Resource => resource;

    #region ILocalizationResourceManager

    /// <summary>
    /// Get Default / System culture.
    /// </summary>
    public CultureInfo DefaultCulture => LocalizationResourceManager.Current.DefaultCulture;

    /// <summary>
    /// Get/Set Current culture for resource manager.
    /// </summary>
    public CultureInfo CurrentCulture { get => LocalizationResourceManager.Current.CurrentCulture; set => LocalizationResourceManager.Current.CurrentCulture = value; }

    /// <summary>
    /// Get resource text value for <see cref="CurrentCulture"/>.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <remarks>Will only search this specific resource manager!</remarks>
    /// <returns>Found resource text value.</returns>
    public string GetValue(string text) => LocalizationResourceManager.Current.GetValue(text, Resource);

    /// <summary>
    /// Get resource text value for <see cref="CurrentCulture"/> from specific resource manager.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <param name="resourceManager">Name of registered Resource manager.</param>
    /// <returns>Found resource text value.</returns>
    public string GetValue(string text, string resourceManager) => LocalizationResourceManager.Current.GetValue(text, resourceManager);

    /// <summary>
    /// Get formatted resource text value for <see cref="CurrentCulture"/> with specified parameters.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <param name="arguments">Parameters used when formatting resource text value.</param>
    /// <remarks>
    /// Uses <see cref="string.Format(string, object?[])"/> syntax.
    /// Will only search this specific resource manager!
    /// </remarks>
    /// <returns>Formatted resource text value.</returns>
    public string GetValue(string text, params object[] arguments) => LocalizationResourceManager.Current.GetValue(text, Resource, arguments);

    /// <summary>
    /// Get formatted resource text value for <see cref="CurrentCulture"/> from specific resource manager with specified parameters.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <param name="resourceManager">Name of registered Resource manager.</param>
    /// <param name="arguments">Parameters used when formatting resource text value.</param>
    /// <remarks>
    /// Uses <see cref="string.Format(string, object?[])"/> syntax.
    /// </remarks>
    /// <returns>Formatted resource text value.</returns>
    public string GetValue(string text, string resourceManager, params object[] arguments) => LocalizationResourceManager.Current.GetValue(text, resourceManager, arguments);

    /// <summary>
    /// Indexer property to Get resource text value for <see cref="CurrentCulture"/>.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <remarks>Will only search this specific resource manager!</remarks>
    /// <returns>Found resource text value.</returns>
    public string this[string text] => GetValue(text);

    /// <summary>
    /// Indexer property to Get resource text value for <see cref="CurrentCulture"/> from specific resource manager.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <param name="resourceManager">Name of registered Resource manager.</param>
    /// <returns>Found resource text value.</returns>
    public string this[string text, string resourceManager] => GetValue(text, resourceManager);

    /// <summary>
    /// Indexer property to Get formatted resource text value for <see cref="CurrentCulture"/> with specified parameters.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <param name="arguments">Parameters used when formatting resource text value.</param>
    /// <remarks>
    /// Uses <see cref="string.Format(string, object?[])"/> syntax.
    /// Will only search this specific resource manager!
    /// </remarks>
    /// <returns>Formatted resource text value.</returns>
    public string this[string text, params object[] arguments] => GetValue(text, arguments);

    /// <summary>
    /// Indexer property to Get formatted resource text value for <see cref="CurrentCulture"/> from specific resource manager with specified parameters.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <param name="resourceManager">Name of registered Resource manager.</param>
    /// <param name="arguments">Parameters used when formatting resource text value.</param>
    /// <remarks>
    /// Uses <see cref="string.Format(string, object?[])"/> syntax.
    /// </remarks>
    /// <returns>Formatted resource text value.</returns>
    public string this[string text, string resourceManager, params object[] arguments] => GetValue(text, resourceManager, arguments);

    /// <summary>
    /// Release All Resources for All registered resources.
    /// </summary>
    public void ReleaseAllResources() => Resource.ReleaseAllResources();

    #endregion ILocalizationResourceManager

    #region INotifyPropertyChanged

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => LocalizationResourceManager.Current.PropertyChanged += value;
        remove => LocalizationResourceManager.Current.PropertyChanged -= value;
    }

    #endregion INotifyPropertyChanged
}