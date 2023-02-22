// Copyright (c) Richasy. All rights reserved.

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CleanReader.Models.Constants;
using CleanReader.Toolkit.Interfaces;
using CleanReader.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel.AppService;
using Windows.System;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 背景音乐视图模型.
/// </summary>
public sealed partial class BackgroundMusicViewModel : ViewModelBase, IBackgroundMusicViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundMusicViewModel"/> class.
    /// </summary>
    public BackgroundMusicViewModel(ISettingsToolkit settingsToolkit)
    {
        _settingsToolkit = settingsToolkit;

        IsBackgroundMusicAutoPlay = _settingsToolkit.ReadLocalSetting(SettingNames.IsAutoPlayBackgroundMusic, true);
        IsAmbieAutoPlay = _settingsToolkit.ReadLocalSetting(SettingNames.AmbieAutoPlay, true);
        IsAmbieCompact = _settingsToolkit.ReadLocalSetting(SettingNames.AmbieCompact, false);
    }

    /// <summary>
    /// 检查 Ambie 是否被安装在设备上.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task CheckAmbieInstalledAsync()
    {
        var info = await Launcher.FindUriSchemeHandlersAsync("ambie");
        IsAmbieInstalled = info.Count > 0;
    }

    private static AppServiceConnection GetAmbieServiceConnection() => new AppServiceConnection
    {
        AppServiceName = VMConstants.Service.AmbieServiceName,
        PackageFamilyName = VMConstants.Service.AmbiePackageId,
    };

    [RelayCommand]
    private async Task PlayAsync()
    {
        if (!IsAmbieInstalled)
        {
            return;
        }

        var isCompact = _settingsToolkit.ReadLocalSetting(SettingNames.AmbieCompact, false);
        var isAutoPlay = _settingsToolkit.ReadLocalSetting(SettingNames.AmbieAutoPlay, true);
        await Launcher.LaunchUriAsync(new Uri($"ambie://launch?compact={isCompact}&autoPlay={isAutoPlay}"));
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        if (!IsAmbieInstalled)
        {
            return;
        }

        var msg = new Windows.Foundation.Collections.ValueSet
            {
                { "command", "pause" },
            };

        using var connection = GetAmbieServiceConnection();
        var status = await connection.OpenAsync();
        if (status == AppServiceConnectionStatus.Success)
        {
            await connection.SendMessageAsync(msg);
        }
    }

    partial void OnIsBackgroundMusicAutoPlayChanged(bool value)
        => _settingsToolkit.WriteLocalSetting(SettingNames.IsAutoPlayBackgroundMusic, value);

    partial void OnIsAmbieAutoPlayChanged(bool value)
        => _settingsToolkit.WriteLocalSetting(SettingNames.AmbieAutoPlay, value);

    partial void OnIsAmbieCompactChanged(bool value)
        => _settingsToolkit.WriteLocalSetting(SettingNames.AmbieCompact, value);
}
