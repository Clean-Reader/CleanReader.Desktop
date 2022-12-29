// Copyright (c) Richasy. All rights reserved.

using System;
using System.Globalization;
using CleanReader.Models.Constants;
using CleanReader.Toolkit.Interfaces;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace CleanReader.Toolkit.Desktop;

/// <summary>
/// Application Toolkit.
/// </summary>
public class AppToolkit : IAppToolkit
{
    private readonly Application _app;
    private readonly ISettingsToolkit _settingsToolkit;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppToolkit"/> class.
    /// </summary>
    /// <param name="toolkit">设置工具.</param>
    public AppToolkit(ISettingsToolkit toolkit)
    {
        _app = Application.Current;
        _settingsToolkit = toolkit;
    }

    /// <inheritdoc/>
    public int GetScalePixel(double pixel, IntPtr windowHandle)
    {
        var dpi = PInvoke.User32.GetDpiForWindow(windowHandle);
        return Convert.ToInt32(pixel * (dpi / 96.0));
    }

    /// <inheritdoc/>
    public int GetNormalizePixel(double pixel, IntPtr windowHandle)
    {
        var dpi = PInvoke.User32.GetDpiForWindow(windowHandle);
        return Convert.ToInt32(pixel / (dpi / 96.0));
    }

    /// <inheritdoc/>
    public string GetLanguageCode(bool isWindowsName = false)
    {
        var culture = CultureInfo.CurrentUICulture;
        return isWindowsName ? culture.ThreeLetterWindowsLanguageName : culture.Name;
    }

    /// <inheritdoc/>
    public IAppToolkit InitializeTheme()
    {
        var localTheme = _settingsToolkit.ReadLocalSetting(SettingNames.AppTheme, AppConstants.ThemeDefault);

        if (localTheme != AppConstants.ThemeDefault)
        {
            _app.RequestedTheme = localTheme == AppConstants.ThemeLight ?
                                    ApplicationTheme.Light :
                                    ApplicationTheme.Dark;
        }

        return this;
    }

    /// <inheritdoc/>
    public IAppToolkit InitializeTitleBar(object titleBar)
    {
        var bar = (AppWindowTitleBar)titleBar;
        bar.ExtendsContentIntoTitleBar = true;
        if (_app.RequestedTheme == ApplicationTheme.Light)
        {
            bar.BackgroundColor = Colors.White;
            bar.InactiveBackgroundColor = Colors.White;
            bar.ButtonBackgroundColor = Color.FromArgb(255, 240, 243, 249);
            bar.ButtonForegroundColor = Colors.DarkGray;
            bar.ButtonHoverBackgroundColor = Colors.LightGray;
            bar.ButtonHoverForegroundColor = Colors.DarkGray;
            bar.ButtonPressedBackgroundColor = Colors.Gray;
            bar.ButtonPressedForegroundColor = Colors.DarkGray;
            bar.ButtonInactiveBackgroundColor = Color.FromArgb(255, 240, 243, 249);
            bar.ButtonInactiveForegroundColor = Colors.Gray;
        }
        else
        {
            bar.BackgroundColor = Color.FromArgb(255, 240, 243, 249);
            bar.InactiveBackgroundColor = Colors.Black;
            bar.ButtonBackgroundColor = Color.FromArgb(255, 32, 32, 32);
            bar.ButtonForegroundColor = Colors.White;
            bar.ButtonHoverBackgroundColor = Color.FromArgb(255, 20, 20, 20);
            bar.ButtonHoverForegroundColor = Colors.White;
            bar.ButtonPressedBackgroundColor = Color.FromArgb(255, 40, 40, 40);
            bar.ButtonPressedForegroundColor = Colors.White;
            bar.ButtonInactiveBackgroundColor = Color.FromArgb(255, 32, 32, 32);
            bar.ButtonInactiveForegroundColor = Colors.Gray;
        }

        return this;
    }
}
