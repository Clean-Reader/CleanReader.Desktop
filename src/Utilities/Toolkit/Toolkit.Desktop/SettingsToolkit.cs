// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Models.Constants;
using CleanReader.Toolkit.Interfaces;
using Windows.Storage;

namespace CleanReader.Toolkit.Desktop;

/// <summary>
/// Settings toolkit.
/// </summary>
public class SettingsToolkit : ISettingsToolkit
{
    /// <inheritdoc/>
    public T ReadLocalSetting<T>(SettingNames settingName, T defaultValue)
        => ReadLocalSetting(settingName.ToString(), defaultValue);

    /// <inheritdoc/>
    public void WriteLocalSetting<T>(SettingNames settingName, T value)
        => WriteLocalSetting(settingName.ToString(), value);

    /// <inheritdoc/>
    public void DeleteLocalSetting(SettingNames settingName)
        => DeleteLocalSetting(settingName.ToString());

    /// <inheritdoc/>
    public bool IsSettingKeyExist(SettingNames settingName)
        => IsSettingKeyExist(settingName.ToString());

    /// <inheritdoc/>
    public void WriteLocalSetting<T>(string settingName, T value)
    {
        var settingContainer = ApplicationData.Current.LocalSettings.CreateContainer(AppConstants.SettingContainerName, ApplicationDataCreateDisposition.Always);

        if (value is Enum)
        {
            settingContainer.Values[settingName] = value.ToString();
        }
        else
        {
            settingContainer.Values[settingName] = value;
        }
    }

    /// <inheritdoc/>
    public T ReadLocalSetting<T>(string settingName, T defaultValue)
    {
        var settingContainer = ApplicationData.Current.LocalSettings.CreateContainer(AppConstants.SettingContainerName, ApplicationDataCreateDisposition.Always);
        if (IsSettingKeyExist(settingName))
        {
            if (defaultValue is Enum)
            {
                var tempValue = settingContainer.Values[settingName].ToString();
                Enum.TryParse(typeof(T), tempValue, out var result);
                return (T)result;
            }
            else
            {
                return (T)settingContainer.Values[settingName];
            }
        }
        else
        {
            WriteLocalSetting(settingName, defaultValue);
            return defaultValue;
        }
    }

    /// <inheritdoc/>
    public void DeleteLocalSetting(string settingName)
    {
        var settingContainer = ApplicationData.Current.LocalSettings.CreateContainer(AppConstants.SettingContainerName, ApplicationDataCreateDisposition.Always);

        if (IsSettingKeyExist(settingName))
        {
            settingContainer.Values.Remove(settingName);
        }
    }

    /// <inheritdoc/>
    public bool IsSettingKeyExist(string settingName)
        => ApplicationData.Current.LocalSettings.CreateContainer(AppConstants.SettingContainerName, ApplicationDataCreateDisposition.Always).Values.ContainsKey(settingName.ToString());
}
