using System;

namespace LocalizationResourceManager.Maui;

[Flags]
public enum DevicePlatform
{
    None = 0,
    Android = 1,
    iOS = 2,
    MacCatalyst = 4,
    WinUI = 8,
    All = Android | iOS | MacCatalyst | WinUI
}