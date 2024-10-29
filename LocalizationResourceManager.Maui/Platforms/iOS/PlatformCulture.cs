using Foundation;
using System.Globalization;

namespace LocalizationResourceManager.Maui;
public class PlatformCulture : IPlatformCulture
{
    private NSObject? notification;

    public static IPlatformCulture Current { get; } = new PlatformCulture();

    private PlatformCulture()
    {
        //Start listening for the NSLocale.currentLocaleDidChangeNotification notification
        notification = NSLocale.Notifications.ObserveCurrentLocaleDidChange(Callback);
    }

    private readonly WeakEventManager eventManager = new();
    public event EventHandler<PlatformCultureChangedEventArgs> PlatformCultureChanged
    {
        add => eventManager.AddEventHandler(value);
        remove => eventManager.RemoveEventHandler(value);
    }

    public void Dispose()
    {
        //Stop listening for the NSLocale.currentLocaleDidChangeNotification notification
        notification?.Dispose();
    }

    public CultureInfo GetPlatformCulture()
    {
        //Build Culture Name from iOS Locale
        var cultureName = NSLocale.CurrentLocale.LocaleIdentifier;

        //Return Culture based on Culture Name
        return CultureInfo.GetCultureInfo(cultureName);
    }

    private void Callback(object? sender, NSNotificationEventArgs args)
    {
        //Get Platform Culture and Create Event Args
        var eventArgs = new PlatformCultureChangedEventArgs(GetPlatformCulture());

        //Raise Platform Culture Changed Event
        eventManager.HandleEvent(this, eventArgs, nameof(PlatformCultureChanged));
    }
}