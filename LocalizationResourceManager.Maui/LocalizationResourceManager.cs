using LocalizationResourceManager.Maui.ComponentModel;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Manager to track current resource manager and current culture.
/// </summary>
public class LocalizationResourceManager : ObservableObject, ILocalizationResourceManager, ILocalizationSettings
{
    private static readonly Lazy<LocalizationResourceManager> currentHolder = new(() => new LocalizationResourceManager());

    internal static LocalizationResourceManager Current => currentHolder.Value;

    private List<ResourceManager> resources = [];
    private Dictionary<string, ResourceManager> keyedResources = [];

    internal bool IsNameWithDotsSupported { get; private set; } = false;

    internal string DotSubstitution { get; private set; } = "_";

    internal bool HasKeyedResources { get; private set; } = false;

    internal IServiceCollection? Services { get; set; }

    private bool suppressTextNotFoundException = false;

    private bool usePlaceholder = false;

    private string placeholderText = "{0}";

    private LocalizationResourceManager()
    {
        //Init
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
        try
        {
            //Verify access by attempting to get empty string
            resource.GetString(string.Empty);

            //Access attempt was successful!
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

    public bool AddResource(ResourceManager resource, string resourceKey)
    {
        try
        {
            if (AddResource(resource))
            {
                keyedResources.Add(resourceKey, resource);
                Services?.AddKeyedSingleton<ILocalizationResourceManager>(resourceKey, new SpecificLocalizationResourceManager(resourceKey));
                HasKeyedResources = true;
                return true;
            }
            else
                return false;
        }
        catch (Exception)
        {
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
    public bool AddFileResource(string baseName, string resourceDir, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type? usingResourceSet = null)
    {
        //Verify parameters
        if (string.IsNullOrWhiteSpace(baseName)) throw new ArgumentNullException(nameof(baseName));
        if (string.IsNullOrWhiteSpace(resourceDir)) throw new ArgumentNullException(nameof(resourceDir));

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
            return AddResource(ResourceManager.CreateFileBasedResourceManager(baseName, resourceDir, usingResourceSet));
        }
        catch (Exception)
        {
            //Registration of file resource failed!
            return false;
        }
    }

    public bool AddFileResource(string baseName, string resourceDir, string resourceKey, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type? usingResourceSet = null)
    {
        try
        {
            if (AddFileResource(baseName, resourceDir, usingResourceSet))
            {
                keyedResources.Add(baseName, resources.Last());
                Services?.AddKeyedSingleton<ILocalizationResourceManager>(resourceKey, new SpecificLocalizationResourceManager(resourceKey));
                HasKeyedResources = true;
                return true;
            }
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
    /// <param name="restore">Flag indicating if latest set culture should be restored</param>
    /// <remarks>
    /// If set to <see langword="true"/>, this will override <see cref="InitialCulture(CultureInfo)"/>!
    /// </remarks>
    public void RestoreLatestCulture(bool restore)
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

    #endregion ILocalizationSettings

    #region ILocalizationResourceManager

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

        //Init
        string? value = null;

        //Check if ResourceManager is specified
        if (text.StartsWith("rm://"))
        {
            var textValues = text[5..].Split("/");
            value = GetValue(textValues[1], textValues[0]);
        }
        else
        {
            //If supported, handle names with dots!
            text = IsNameWithDotsSupported ? text.Replace(DotSubstitution, ".") : text;

            //Attemp to get localized string with Current Culture
            value = resources?.Select(resource => resource.GetString(text, CurrentCulture)).FirstOrDefault(output => output is not null);
        }

        //Return Result
        return value ?? (suppressTextNotFoundException ? (usePlaceholder ? string.Format(placeholderText, text) : string.Empty) : throw new NullReferenceException($"{nameof(text)}: '{text}' not found!"));
    }

    public string GetValue(string text, string resourceManager)
    {
        //Init
        string? value = null;

        if (keyedResources?.TryGetValue(resourceManager, out var resource) ?? false)
        {
            //If supported, handle names with dots!
            text = IsNameWithDotsSupported ? text.Replace(DotSubstitution, ".") : text;

            //Attemp to get localized string with Current Culture
            value = resource.GetString(text, CurrentCulture);
        }

        //Return Result
        return value ?? (suppressTextNotFoundException ? (usePlaceholder ? string.Format(placeholderText, $"{resourceManager}/{text}") : string.Empty) : throw new NullReferenceException($"{nameof(text)}: '{$"{resourceManager}/{text}"}' not found!"));
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

    public string GetValue(string text, string resourceManager, params object[] arguments) => string.Format(GetValue(text, resourceManager), arguments);

    /// <summary>
    /// Indexer property to Get resource text value for <see cref="CurrentCulture"/>.
    /// </summary>
    /// <param name="text">Resource name.</param>
    /// <remarks>Will search all registered resources, in the order they were added, until first match is found!</remarks>
    /// <returns>Found resource text value.</returns>
    public string this[string text] => GetValue(text);

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

    public string this[string text, string resourceManager, params object[] arguments] => GetValue(text, resourceManager, arguments);

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

    private string DefaultCultureName => Preferences.Get(nameof(DefaultCulture), CultureInfo.CurrentCulture.Name);

    /// <summary>
    /// Get/Set Default / System culture.
    /// </summary>
    public CultureInfo DefaultCulture
    {
        get => CultureInfo.GetCultureInfo(DefaultCultureName);
        private set => Preferences.Set(nameof(DefaultCulture), value.Name);
    }

    /// <summary>
    /// Release All Resources for All registered resources.
    /// </summary>
    public void ReleaseAllResources()
    {
        resources?.ForEach(resource => resource?.ReleaseAllResources());
    }

    #endregion ILocalizationResourceManager

    private string LatestCultureName => Preferences.Get(nameof(LatestCulture), DefaultCulture.Name);

    /// <summary>
    /// Get/Set Current / Latest culture.
    /// </summary>
    /// <remarks>
    /// Latest CultureInfo is stored in Preferrences and updated everytime CurrentCulture is updated.
    /// </remarks>
    private CultureInfo LatestCulture
    {
        get => CultureInfo.GetCultureInfo(LatestCultureName);
        set => Preferences.Set(nameof(LatestCulture), value.Name);
    }
}