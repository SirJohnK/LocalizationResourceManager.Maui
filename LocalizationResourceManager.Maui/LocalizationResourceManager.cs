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

    public static LocalizationResourceManager Current => currentHolder.Value;

    private ResourceManager? resourceManager;

    private LocalizationResourceManager()
    {
    }

    public void Init(ResourceManager resource) => resourceManager = resource;

    public void Init(ResourceManager resource, CultureInfo initialCulture)
    {
        CurrentCulture = initialCulture;
        Init(resource);
    }

    public string GetValue(string text)
    {
        if (resourceManager == null)
            throw new InvalidOperationException($"Must call {nameof(LocalizationResourceManager)}.{nameof(Init)} first");

        return resourceManager.GetString(text, CurrentCulture) ?? throw new NullReferenceException($"{nameof(text)}: {text} not found");
    }

    public string this[string text] => GetValue(text);

    private CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;

    /// <summary>
    /// Get/Set Current culture for resource manager.
    /// </summary>
    /// <remarks>
    /// IMPORTANT! SetProperty must set propertyNam to null to trigger update! (Due to this, ObserablePropertyAttribute does not work!)
    /// </remarks>
    public CultureInfo CurrentCulture
    {
        get => currentCulture;
        set => SetProperty(ref currentCulture, value, null);
    }
}