using System;
using System.Globalization;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Interface for Platform Culture Service
/// </summary>
public interface IPlatformCulture : IDisposable
{
    /// <summary>
    /// Gets the current platform culture
    /// </summary>
    CultureInfo GetPlatformCulture();

    /// <summary>
    /// Platform Culture Changed Event
    /// </summary>
    event EventHandler<PlatformCultureChangedEventArgs> PlatformCultureChanged;
}

/// <summary>
/// Event Args for Platform Culture Changed Event
/// </summary>
/// <param name="newCulture">New Culture</param>
public class PlatformCultureChangedEventArgs(CultureInfo newCulture) : EventArgs
{
    /// <summary>
    /// Culture platform changed to
    /// </summary>
    public CultureInfo Culture => newCulture;
}