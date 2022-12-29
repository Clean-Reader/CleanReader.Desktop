// Copyright (c) Richasy. All rights reserved.

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CleanReader.Locator.Lib;
using CleanReader.Models.Constants;
using ReactiveUI;
using Windows.ApplicationModel.AppService;
using Windows.System;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 背景音乐视图模型.
/// </summary>
public sealed partial class BackgroundMusicViewModel : ReactiveObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundMusicViewModel"/> class.
    /// </summary>
    public BackgroundMusicViewModel()
    {
        ServiceLocator.Instance.LoadService(out _settingsToolkit);
        PlayCommand = ReactiveCommand.CreateFromTask(PlayAsync, outputScheduler: RxApp.MainThreadScheduler);
        StopCommand = ReactiveCommand.CreateFromTask(StopAsync, outputScheduler: RxApp.MainThreadScheduler);

        IsBackgroundMusicAutoPlay = _settingsToolkit.ReadLocalSetting(SettingNames.IsAutoPlayBackgroundMusic, true);
        IsAmbieAutoPlay = _settingsToolkit.ReadLocalSetting(SettingNames.AmbieAutoPlay, true);
        IsAmbieCompact = _settingsToolkit.ReadLocalSetting(SettingNames.AmbieCompact, false);

        this.WhenAnyValue(x => x.IsBackgroundMusicAutoPlay)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => _settingsToolkit.WriteLocalSetting(SettingNames.IsAutoPlayBackgroundMusic, x));

        this.WhenAnyValue(x => x.IsAmbieAutoPlay)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => _settingsToolkit.WriteLocalSetting(SettingNames.AmbieAutoPlay, x));

        this.WhenAnyValue(x => x.IsAmbieCompact)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => _settingsToolkit.WriteLocalSetting(SettingNames.AmbieCompact, x));
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
}
