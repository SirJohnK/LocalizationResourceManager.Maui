using System.Globalization;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Generic Platform Culture Service Implementation
/// </summary>
/// <remarks>Used to support portable .net in test project.</remarks>
public class PlatformCulture : IPlatformCulture
{
    /// <summary>
    /// Current Platform Culture Service
    /// </summary>
    public static IPlatformCulture Current { get; } = new PlatformCulture();

    private readonly WeakEventManager eventManager = new();

    /// <inheritdoc/>
    public event EventHandler<PlatformCultureChangedEventArgs> PlatformCultureChanged
    {
        add => eventManager.AddEventHandler(value);
        remove => eventManager.RemoveEventHandler(value);
    }

    /// <summary>
    /// Private Platform Culture Service Constructor, to ensure singleton
    /// </summary>
    private PlatformCulture()
    {
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public CultureInfo GetPlatformCulture() => CultureInfo.CurrentCulture;
}