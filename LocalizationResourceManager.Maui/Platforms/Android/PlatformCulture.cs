using Android.Content;
using System.Globalization;
using Application = Android.App.Application;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Android specific implementation of the Platform Culture Service
/// </summary>
public class PlatformCulture : BroadcastReceiver, IPlatformCulture
{
    /// <summary>
    /// Gets the current platform culture
    /// </summary>
    public static IPlatformCulture Current { get; } = new PlatformCulture();

    /// <summary>
    /// Private Platform Culture Service Constructor, to ensure singleton
    /// </summary>
    /// <remarks>Start listening for the ActionLocaleChanged notification</remarks>
    private PlatformCulture()
    {
        //Start listening for the ActionLocaleChanged notification
        Application.Context.RegisterReceiver(this, new IntentFilter(Intent.ActionLocaleChanged));
    }

    private readonly WeakEventManager eventManager = new();

    /// <inheritdoc/>
    public event EventHandler<PlatformCultureChangedEventArgs> PlatformCultureChanged
    {
        add => eventManager.AddEventHandler(value);
        remove => eventManager.RemoveEventHandler(value);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        //Ensure we are not already listening!
        if (disposing)
        {
            //Stop listening for the ActionLocaleChanged notification
            Application.Context.UnregisterReceiver(this);
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    public CultureInfo GetPlatformCulture()
    {
        //Build Culture Name from Android Locale
        var cultureName = $"{Java.Util.Locale.Default.Language}-{Java.Util.Locale.Default.Country}";

        //Return Culture based on Culture Name
        return CultureInfo.GetCultureInfo(cultureName);
    }

    /// <summary>
    /// Receives the Locale Changed Intent and raises the Platform Culture Changed Event
    /// </summary>
    /// <param name="context">Current context</param>
    /// <param name="intent">Locale Changed Intent</param>
    public override void OnReceive(Context? context, Intent? intent)
    {
        //Ensure Intent is Locale Changed
        if (string.Equals(intent?.Action, Intent.ActionLocaleChanged))
        {
            //Get Platform Culture and Create Event Args
            var eventArgs = new PlatformCultureChangedEventArgs(GetPlatformCulture());

            //Raise Platform Culture Changed Event
            eventManager.HandleEvent(this, eventArgs, nameof(PlatformCultureChanged));
        }
    }
}