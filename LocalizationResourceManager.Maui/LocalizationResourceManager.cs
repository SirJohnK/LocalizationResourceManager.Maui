using LocalizationResourceManager.Maui.ComponentModel;
using System.Globalization;
using System.Resources;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Manager to track current resource manager and current culture.
/// </summary>
public partial class LocalizationResourceManager : ObservableObject
{
    private static readonly Lazy<LocalizationResourceManager> currentHolder = new(() => new LocalizationResourceManager());

    internal static LocalizationResourceManager Current => currentHolder.Value;

    private ResourceManager? resourceManager;

    private LocalizationResourceManager()
    {
    }

    public void Init(ResourceManager resource, CultureInfo? initialCulture = null)
    {
        //Init
        resourceManager = resource;
        CurrentCulture = initialCulture ?? DefaultCulture;
    }

    public string GetValue(string text)
    {
        if (resourceManager == null)
            throw new InvalidOperationException($"Must call {nameof(LocalizationResourceManager)}.{nameof(Init)} first!");

        return resourceManager.GetString(text, CurrentCulture) ?? throw new NullReferenceException($"{nameof(text)}: {text} not found!");
    }

    public string this[string text] => GetValue(text);

    private CultureInfo currentCulture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Get/Set Current culture for resource manager.
    /// </summary>
    public CultureInfo CurrentCulture
    {
        get => currentCulture;
        set
        {
            if (SetProperty(ref currentCulture, value, null))
            {
                CultureInfo.CurrentCulture = value;
                CultureInfo.CurrentUICulture = value;
                CultureInfo.DefaultThreadCurrentCulture = value;
                CultureInfo.DefaultThreadCurrentUICulture = value;
                LatestCulture = value;
            }
        }
    }

    private static string DefaultCultureName => Preferences.Get(nameof(DefaultCulture), CultureInfo.CurrentCulture.Name);

    /// <summary>
    /// Get/Set Default / System culture.
    /// </summary>
    public static CultureInfo DefaultCulture
    {
        get => CultureInfo.GetCultureInfo(DefaultCultureName);
        set => Preferences.Set(nameof(DefaultCulture), value.Name);
    }

    private static string LatestCultureName => Preferences.Get(nameof(LatestCulture), DefaultCulture.Name);

    /// <summary>
    /// Get/Set Default / System culture.
    /// </summary>
    public static CultureInfo LatestCulture
    {
        get => CultureInfo.GetCultureInfo(LatestCultureName);
        set => Preferences.Set(nameof(LatestCulture), value.Name);
    }
}