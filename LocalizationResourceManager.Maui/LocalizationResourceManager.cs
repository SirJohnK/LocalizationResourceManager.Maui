using System.Resources;
using System.Globalization;
using System.Runtime.CompilerServices;
using LocalizationResourceManager.Maui.ComponentModel;
using LocalizationResourceManager.Maui.Helpers;

[assembly: InternalsVisibleTo("LocalizationResourceManager.Maui.Tests")]

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Manager to track current resource manager and current culture.
/// </summary>
public class LocalizationResourceManager : ObservableObject, ILocalizationResourceManager, ILocalizationSettings
{
    private static readonly Lazy<LocalizationResourceManager> currentHolder = new(() => new LocalizationResourceManager());

    private List<ResourceManager> resources = [];

    private IMicroContainer container = new MicroContainer();

    private bool suppressTextNotFoundException = false;

    private bool usePlaceholder = false;

    private string placeholderText = "{0}";

    private IPreferences? preferences;
    private IPreferences? Preferences => preferences ??= container.Get<IPreferences>();

    internal static LocalizationResourceManager Current => currentHolder.Value;

    internal Dictionary<string, SpecificLocalizationResourceManager> KeyedResources = [];

    internal bool IsNameWithDotsSupported { get; private set; } = false;

    internal string DotSubstitution { get; private set; } = "_";

    internal bool HasKeyedResources { get; private set; } = false;

    /// <summary>
    /// Private Localization Resource Manager Constructor, to ensure singleton
    /// </summary>
    private LocalizationResourceManager()
    {
        //Store Default/Current System Culture
        DefaultCulture = CultureInfo.CurrentCulture;
    }

    #region ILocalizationSettings

    /// <summary>
    /// Register ResourceManager and verify access.
    /// </summary>
    /// <param name="resource">The ResourceManager to add</param>
    /// <returns>Flag indication if ResourceManager was added successfully</returns>
    public bool AddResource(ResourceManager resource)
    {
        //Verify parameters
        ArgumentNullException.ThrowIfNull(resource);

        try
        {
            //Verify if ResourceManager is already added!
            if (resources.Contains(resource)) return false;

            //Attempt to add ResourceManager
            resources.Add(resource);

            //Return successful status
            return true;
        }
        catch (Exception)
        {
            //Access attempt was not successful!
            return false;
        }
    }

    /// <summary>
    /// Register ResourceManager with a specific key.
    /// </summary>
    /// <param name="resource">The ResourceManager to add</param>
    /// <param name="resourceKey">Key to identify the ResourceManager</param>
    /// <returns>Flag indication if ResourceManager was added successfully</returns>
    public bool AddResource(ResourceManager resource, string resourceKey)
    {
        try
        {
            if (AddResource(resource) && KeyedResources.TryAdd(resourceKey, new(resource)))
                return HasKeyedResources = true;
            else
                return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Try to register file based ResourceManager and create default .resources file, if missing.
    /// </summary>
    /// <param name="baseName">The root name of the resource.</param>
    /// <param name="resourceDir">Path to resource directory.</param>
    /// <param name="resourceManager">Created ResourceManager</param>
    /// <param name="usingResourceSet">Optional, Type of the ResourceSet the ResourceManager uses to construct ResourceSets.</param>
    /// <returns>Flag indication if ResourceManager was added successfully</returns>
    /// <exception cref="ArgumentNullException">If baseName or resourceDir is null, empty or whitespace.</exception>
    private bool TryAddFileResource(string baseName, string resourceDir, out ResourceManager? resourceManager, Type? usingResourceSet = null)
    {
        //Verify parameters
        ArgumentNullException.ThrowIfNullOrWhiteSpace(baseName);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(resourceDir);

        try
        {
            //Init
            var resourceFileName = Path.Combine(resourceDir, $"{baseName}.resources");

            //Check if default .resources file exits, otherwise create it!
            if (!File.Exists(resourceFileName))
            {
                //Attempt to create default .resources file!
                using (var writer = new ResourceWriter(resourceFileName))
                {
                    //Add default empty string resource
                    writer.AddResource(string.Empty, string.Empty);
                }
            }

            //Create and register file based ResourceManager
            resourceManager = ResourceManager.CreateFileBasedResourceManager(baseName, resourceDir, usingResourceSet);
            return AddResource(resourceManager);
        }
        catch (Exception)
        {
            //Registration of file resource failed!
            resourceManager = null;
            return false;
        }
    }

    /// <summary>
    /// Register file based ResourceManager and create default .resources file, if missing.
    /// </summary>
    /// <param name="baseName">The root name of the resource.</param>
    /// <param name="resourceDir">Path to resource directory.</param>
    /// <param name="usingResourceSet">Optional, Type of the ResourceSet the ResourceManager uses to construct ResourceSets.</param>
    /// <returns>Flag indication if ResourceManager was added successfully</returns>
    /// <exception cref="ArgumentNullException">If baseName or resourceDir is null, empty or whitespace.</exception>
    public bool AddFileResource(string baseName, string resourceDir, Type? usingResourceSet = null)
    {
        //Try to add file based ResourceManager
        return TryAddFileResource(baseName, resourceDir, out _, usingResourceSet);
    }

    /// <summary>
    /// Register file based ResourceManager with a specific key and create default .resources file, if missing.
    /// </summary>
    /// <param name="baseName">The root name of the resource.</param>
    /// <param name="resourceDir">Path to resource directory.</param>
    /// <param name="resourceKey">Key to identify the ResourceManager</param>
    /// <param name="usingResourceSet">Optional, Type of the ResourceSet the ResourceManager uses to construct ResourceSets.</param>
    /// <returns>Flag indication if ResourceManager was added successfully</returns>
    /// <exception cref="ArgumentNullException">If baseName or resourceDir is null, empty or whitespace.</exception>
    public bool AddFileResource(string baseName, string resourceDir, string resourceKey, Type? usingResourceSet = null)
    {
        try
        {
            if (TryAddFileResource(baseName, resourceDir, out var resource, usingResourceSet) && KeyedResources.TryAdd(resourceKey, new(resource!)))
                return HasKeyedResources = true;
            else
                return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Set initial/startup culture.
    /// </summary>
    /// <param name="culture">Culture to set</param>
    /// <remarks>
    /// If, <see cref="RestoreLatestCulture(bool)"/> is set to <see langword="true"/>, this will be ignored!
    /// </remarks>
    public void InitialCulture(CultureInfo culture) => CurrentCulture = (restoreLatestCulture ? CurrentCulture : culture);

    private bool restoreLatestCulture = false;

    /// <summary>
    /// Set if latest set culture should be restored. Default: <see langword="false"/>
    /// </summary>
    /// <param name="restore">Flag indicating if latest set culture should be restored. (Optional, Default: <see langword="true"/>)</param>
    /// <remarks>
    /// If set to <see langword="true"/>, this will override <see cref="InitialCulture(CultureInfo)"/>!
    /// </remarks>
    public void RestoreLatestCulture(bool restore = true)
    {
        //Set state and Update Current Culture
        restoreLatestCulture = restore;
        if (restoreLatestCulture)
            CurrentCulture = LatestCulture ?? DefaultCulture;
    }

    /// <summary>
    /// Activate support for Resource Names with Dots.
    /// </summary>
    /// <remarks>Dots in names will be temporarily replaced by the substitution text when handled by the <see cref="TranslateExtension"/>.</remarks>
    /// <param name="substitution">Replacement text for dots in resource names.</param>
    public void SupportNameWithDots(string substitution = "_")
    {
        //Activate Support!
        IsNameWithDotsSupported = true;
        DotSubstitution = substitution;
    }

    /// <summary>
    /// Suppress/Deactive throwing the text not found exception.
    /// </summary>
    /// <param name="usePlaceholder">Flag indicating if a placeholder text should be displayed if text is not found. (Optional, Default: <see langword="false"/>)</param>
    /// <param name="placeholderText">Placeholder text displayed if text is not found, when UsePlaceHolder is activated.</param>
    /// <remarks>"{0}" in the placeholder text, will be replaced with the resource name.</remarks>
    public void SuppressTextNotFoundException(bool usePlaceholder = false, string placeholderText = "{0}")
    {
        //Verify parameters
        if (string.IsNullOrWhiteSpace(placeholderText)) throw new ArgumentNullException(nameof(placeholderText), "Placeholder text can not be empty!");

        //Activate suppress text not found exception
        suppressTextNotFoundException = true;

        //Set placeholder settings
        this.usePlaceholder = usePlaceholder;
        this.placeholderText = placeholderText;
    }

    /// <summary>
    /// Monitor the platform culture and update the ResourceManager current culture.
    /// Includes live update of the culture, if the platform culture changes.
    /// </summary>
    /// <param name="activate">Flag indicating if the platform culture should be monitored. (Optional, Default: <see langword="true"/>)</param>
    /// <remarks>With <see cref="RestoreLatestCulture(bool)"/> activated, Latest Culture will be restored on startup, regardless of current platform culture.</remarks>
    public void MonitorPlatformCulture(bool activate = true)
    {
        //Get Platform Culture Monitoring Service
        var platformCulture = container.Get<IPlatformCulture>();
        if (platformCulture is not null)
        {
            //Unsubscribe from Platform Culture Changed Event
            platformCulture.PlatformCultureChanged -= OnPlatformCultureChanged;

            //Activate / Deactivate Platform Culture Monitoring
            if (activate)
            {
                //Subscribe to Platform Culture Changed Event
                platformCulture.PlatformCultureChanged += OnPlatformCultureChanged;
            }
        }
        else
        {
            //Platform Culture Monitoring Service not available!
            throw new InvalidOperationException("Platform Culture Monitoring Service not available!");
        }
    }

    /// <summary>
    /// Change Current Culture to Platform Culture.
    /// </summary>
    /// <param name="sender">PlatformCultureChanged event sender.</param>
    /// <param name="e">PlatformCultureChanged event arguments.</param>
    private void OnPlatformCultureChanged(object? sender, PlatformCultureChangedEventArgs e) => CurrentCulture = e.Culture;

    /// <summary>
    /// Register a service to the LocalizationResourceManager.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="service">Instance of the <typeparamref name="TService"/> type.</param>
    /// <remarks>Only for internal use!</remarks>
    /// <returns>Flag indication if service was added successfully.</returns>
    internal bool RegisterService<TService>(TService service) => container.Add(service);

    /// <summary>
    /// Register a service factory to the LocalizationResourceManager.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="factory">Factory of the <typeparamref name="TService"/> type.</param>
    /// <remarks>Only for internal use!</remarks>
    /// <returns>Flag indication if service was added successfully.</returns>
    internal bool RegisterService<TService>(Func<TService> factory) => container.Add(factory);

    #endregion ILocalizationSettings

    #region Internal Value Handling

    /// <summary>
    /// Get resource text value for <see cref="CurrentCulture"/> from specific resource manager.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <param name="resource">The ResourceManager to use.</param>
    /// <returns>Found resource text value.</returns>
    /// <exception cref="NullReferenceException">Will be thrown if no resource was found.</exception>
    internal string GetValue(string text, ResourceManager? resource)
    {
        //If supported, handle names with dots!
        text = IsNameWithDotsSupported ? text.Replace(DotSubstitution, ".") : text;

        //Attemp to get localized string with Current Culture
        var value = resource?.GetString(text, CurrentCulture) ?? ((resources?.Count ?? 0) > 0 ? GetValue(text) : null);

        //Return Result
        return value ?? (suppressTextNotFoundException ? (usePlaceholder ? string.Format(placeholderText, text) : string.Empty) : throw new NullReferenceException($"{nameof(text)}: '{$"{resource?.BaseName}/{text}"}' not found!"));
    }

    /// <summary>
    /// Get formatted resource text value for <see cref="CurrentCulture"/> from specific resource manager and with specified parameters.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <param name="resource">The ResourceManager to use.</param>
    /// <param name="arguments">Parameters used when formatting resource text value.</param>
    /// <remarks>
    /// Uses <see cref="string.Format(string, object?[])"/> syntax.
    /// </remarks>
    /// <returns>Formatted resource text value.</returns>
    internal string GetValue(string text, ResourceManager? resource, params object[] arguments) => string.Format(GetValue(text, resource), arguments);

    /// <summary>
    /// Get specific resource manager by key.
    /// </summary>
    /// <param name="resourceManager">Key to identify the ResourceManager.</param>
    /// <returns>Specific <see cref="ILocalizationResourceManager"/> resource manager identified.</returns>
    internal ILocalizationResourceManager? GetResourceManager(string resourceManager) => KeyedResources.GetValueOrDefault(resourceManager);

    #endregion Internal Value Handling

    #region ILocalizationResourceManager

    /// <summary>
    /// Get Default / System culture.
    /// </summary>
    public CultureInfo DefaultCulture { get; }

    private CultureInfo currentCulture = CultureInfo.CurrentCulture;

    /// <summary>
    /// Get/Set Current culture for resource manager.
    /// </summary>
    public CultureInfo CurrentCulture
    {
        get => currentCulture;
        set
        {
            CultureInfo.CurrentCulture = value;
            CultureInfo.CurrentUICulture = value;
            CultureInfo.DefaultThreadCurrentCulture = value;
            CultureInfo.DefaultThreadCurrentUICulture = value;
            if (SetProperty(ref currentCulture, value, null))
                LatestCulture = value;
        }
    }

    private string LatestCultureName => Preferences?.Get(nameof(LatestCulture), DefaultCulture.Name) ?? DefaultCulture.Name;

    /// <summary>
    /// Get/Set Current / Latest culture.
    /// </summary>
    /// <remarks>
    /// Latest CultureInfo is stored in Preferrences and updated everytime CurrentCulture is updated.
    /// </remarks>
    private CultureInfo LatestCulture
    {
        get => CultureInfo.GetCultureInfo(LatestCultureName);
        set => Preferences?.Set(nameof(LatestCulture), value.Name);
    }

    /// <summary>
    /// Get resource text value for <see cref="CurrentCulture"/>.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <remarks>Will search all registered resources, in the order they were added, until first match is found!</remarks>
    /// <returns>Found resource text value.</returns>
    /// <exception cref="InvalidOperationException">Will be thrown if no resources are added.</exception>
    /// <exception cref="NullReferenceException">Will be thrown if no resource was found.</exception>
    public string GetValue(string text)
    {
        //Verify Resources
        if ((resources?.Count ?? 0) == 0)
            throw new InvalidOperationException($"At least one resource must be added with Settings.{nameof(AddResource)}!");

        //If supported, handle names with dots!
        text = IsNameWithDotsSupported ? text.Replace(DotSubstitution, ".") : text;

        //Attemp to get localized string with Current Culture
        var value = resources?.Select(resource => resource.GetString(text, CurrentCulture)).FirstOrDefault(output => output is not null);

        //Return Result
        return value ?? (suppressTextNotFoundException ? (usePlaceholder ? string.Format(placeholderText, text) : string.Empty) : throw new NullReferenceException($"{nameof(text)}: '{text}' not found!"));
    }

    /// <summary>
    /// Get resource text value for <see cref="CurrentCulture"/> from specific resource manager.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <param name="resourceManager">Name of registered Resource manager.</param>
    /// <returns>Found resource text value.</returns>
    /// <exception cref="NullReferenceException">Will be thrown if no resource was found.</exception>
    public string GetValue(string text, string resourceManager)
    {
        //Init
        var keyedResource = KeyedResources.GetValueOrDefault(resourceManager);

        //Return Result
        return GetValue(text, keyedResource?.Resource);
    }

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
    public string GetValue(string text, params object[] arguments) => string.Format(GetValue(text), arguments);

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
    public string GetValue(string text, string resourceManager, params object[] arguments) => string.Format(GetValue(text, resourceManager), arguments);

    /// <summary>
    /// Indexer property to Get resource text value for <see cref="CurrentCulture"/>.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <remarks>Will search all registered resources, in the order they were added, until first match is found!</remarks>
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
    /// Will search all registered resources, in the order they were added, until first match is found!
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
    public void ReleaseAllResources()
    {
        resources?.ForEach(resource => resource?.ReleaseAllResources());
    }

    #endregion ILocalizationResourceManager
}