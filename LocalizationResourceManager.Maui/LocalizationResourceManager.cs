using LocalizationResourceManager.Maui.ComponentModel;
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

    private List<ResourceManager> resources = new List<ResourceManager>();

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

    /// <summary>
    /// Set initial/startup culture.
    /// </summary>
    /// <param name="culture">Culture to set</param>
    /// <remarks>
    /// If, RestoreLatestCulture is set to true, this will be ignored!
    /// </remarks>
    public void InitialCulture(CultureInfo culture) => CurrentCulture = (restoreLatestCulture ? CurrentCulture : culture);

    private bool restoreLatestCulture = false;

    /// <summary>
    /// Set if latest set culture should be restored. Default: false
    /// </summary>
    /// <param name="restore">Flag indicating if latest set culture should be restored</param>
    /// <remarks>
    /// If set to true, this will override InitialCulture!
    /// </remarks>
    public void RestoreLatestCulture(bool restore)
    {
        //Set state and Update Current Culture
        restoreLatestCulture = restore;
        if (restoreLatestCulture)
            CurrentCulture = LatestCulture ?? DefaultCulture;
    }

    #endregion ILocalizationSettings

    #region ILocalizationResourceManager

    public string GetValue(string text)
    {
        //Verify Resources
        if ((resources?.Count ?? 0) == 0)
            throw new InvalidOperationException($"At least one resource must be added with Settings.{nameof(AddResource)}!");

        //Attemp to get localized string with Current Culture
        var value = resources?.Select(resource => resource.GetString(text, CurrentCulture)).FirstOrDefault(output => output is not null);

        //Return Result
        return value ?? throw new NullReferenceException($"{nameof(text)}: {text} not found!");
    }

    public string GetValue(string text, params object[] arguments) => string.Format(GetValue(text), arguments);

    public string this[string text] => GetValue(text);

    public string this[string text, params object[] arguments] => GetValue(text, arguments);

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
        set => Preferences.Set(nameof(DefaultCulture), value.Name);
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