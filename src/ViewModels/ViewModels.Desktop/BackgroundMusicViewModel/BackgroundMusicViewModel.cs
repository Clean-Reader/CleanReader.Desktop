// Copyright (c) Richasy. All rights reserved.

using System;
using System.Threading.Tasks;
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
        PlayCommand = ReactiveCommand.CreateFromTask(PlayAsync, outputScheduler: RxApp.MainThreadScheduler);
        StopCommand = ReactiveCommand.CreateFromTask(StopAsync, outputScheduler: RxApp.MainThreadScheduler);
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

    private static AppServiceConnection GetAmbieServiceConnection()
    {
        return new AppServiceConnection
        {
            AppServiceName = VMConstants.Service.AmbieServiceName,
            PackageFamilyName = VMConstants.Service.AmbiePackageId,
        };
    }

    private async Task PlayAsync()
    {
        if (!IsAmbieInstalled)
        {
            return;
        }

        await Launcher.LaunchUriAsync(new Uri("ambie://launch?compact=true&autoPlay=true"));
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
