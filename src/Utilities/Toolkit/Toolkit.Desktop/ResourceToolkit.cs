// Copyright (c) Richasy. All rights reserved.

using System.Linq;
using CleanReader.Toolkit.Interfaces;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.ViewManagement;

namespace CleanReader.Toolkit.Desktop;

/// <summary>
/// App resource toolkit.
/// </summary>
public class ResourceToolkit : IResourceToolkit
{
    private readonly Application _app;
    private readonly ISettingsToolkit _settingsToolkit;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceToolkit"/> class.
    /// </summary>
    /// <param name="settingsToolkit">设置工具.</param>
    public ResourceToolkit(ISettingsToolkit settingsToolkit)
    {
        _app = Application.Current;
        _settingsToolkit = settingsToolkit;
    }

    /// <inheritdoc/>
    public string GetLocaleString(string languageName)
        => ResourceManager.Current.MainResourceMap[$"Models.Resources/Resources/{languageName}"].Candidates.First().ValueAsString;

    /// <inheritdoc/>
    public T GetResource<T>(string resourceName)
    {
        var isHC = new AccessibilitySettings().HighContrast;
        ResourceDictionary themeDict;
        var theme = _settingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.AppTheme, ElementTheme.Default);
        var themeStr = theme == ElementTheme.Default
            ? Application.Current.RequestedTheme.ToString()
            : theme.ToString();
        if (isHC)
        {
            themeDict = (ResourceDictionary)Application.Current.Resources.ThemeDictionaries["HighContrast"];
        }
        else
        {
            themeDict = (ResourceDictionary)Application.Current.Resources.ThemeDictionaries[themeStr];
        }

        return (T)themeDict[resourceName];
    }
}
