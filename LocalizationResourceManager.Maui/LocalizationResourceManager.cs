using LocalizationResourceManager.Maui.ComponentModel;
using System.Globalization;
using System.Resources;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Manager to track current resource manager and current culture.
/// </summary>
public partial class LocalizationResourceManager : ObservableObject, ILocalizationSettings
{
    private static readonly Lazy<LocalizationResourceManager> currentHolder = new(() => new LocalizationResourceManager());

    internal static LocalizationResourceManager Current => currentHolder.Value;

    private List<ResourceManager> resources = new List<ResourceManager>();

    private LocalizationResourceManager()
    {
        //Init
        DefaultCulture = CultureInfo.CurrentCulture;
    }

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

    #region ILocalizationSettings

    public void AddResource(ResourceManager resource) => resources.Add(resource);

    public void InitialCulture(CultureInfo culture) => CurrentCulture = (restoreLatestCulture ? CurrentCulture : culture);

    private bool restoreLatestCulture = false;

    public void RestoreLatestCulture(bool restore)
    {
        //Set state and Update Current Culture
        restoreLatestCulture = restore;
        if (restoreLatestCulture)
            CurrentCulture = LatestCulture ?? DefaultCulture;
    }

    #endregion ILocalizationSettings

    public string this[string text] => GetValue(text);

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