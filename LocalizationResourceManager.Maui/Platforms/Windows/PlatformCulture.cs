using System.Globalization;
using Microsoft.Win32;

namespace LocalizationResourceManager.Maui;

public class PlatformCulture : IPlatformCulture
{
    public static IPlatformCulture Current { get; } = new PlatformCulture();

    private PlatformCulture()
    {
        //Start listening for the UserPreferenceChanged event
        SystemEvents.InvokeOnEventsThread(() =>
        {
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        });
    }

    private readonly WeakEventManager eventManager = new();
    public event EventHandler<PlatformCultureChangedEventArgs> PlatformCultureChanged
    {
        add => eventManager.AddEventHandler(value);
        remove => eventManager.RemoveEventHandler(value);
    }

    public void Dispose()
    {
        //Stop listening for the UserPreferenceChanged event
        SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
    }

    public CultureInfo GetPlatformCulture()
    {
        return CultureInfo.CurrentCulture;
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        // Regional settings have changed
        if (e.Category == UserPreferenceCategory.Locale)
        {
            // .NET also caches culture settings, so clear them
            CultureInfo.CurrentCulture.ClearCachedData();

            //Get Platform Culture and Create Event Args
            var eventArgs = new PlatformCultureChangedEventArgs(GetPlatformCulture());

            //Raise Platform Culture Changed Event
            eventManager.HandleEvent(this, eventArgs, nameof(PlatformCultureChanged));
        }
    }
}