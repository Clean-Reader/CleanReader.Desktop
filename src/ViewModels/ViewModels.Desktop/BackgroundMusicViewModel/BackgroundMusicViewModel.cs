// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using CleanReader.Models.Constants;
using ReactiveUI;
using Windows.ApplicationModel.AppService;
using Windows.Management.Deployment;

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
        _dispatcherQueue = AppViewModel.Instance.DispatcherQueue;

        // OpenConnectionCommand = ReactiveCommand.CreateFromTask(OpenAmbieConnectionAsync, outputScheduler: RxApp.MainThreadScheduler);
        PlayCommand = ReactiveCommand.CreateFromTask(PlayAsync, outputScheduler: RxApp.MainThreadScheduler);
        StopCommand = ReactiveCommand.CreateFromTask(StopAsync, outputScheduler: RxApp.MainThreadScheduler);
    }

    private static async Task TryLaunchAmbieAsync()
    {
        await Task.Run(async () =>
        {
            var pm = new PackageManager();
            var packages = pm.FindPackagesForUser(string.Empty, VMConstants.Service.AmbiePackageId);
            var first = packages.FirstOrDefault();
            if (first != null)
            {
                var entries = await first.GetAppListEntriesAsync();
                var entry = entries.FirstOrDefault();
                if (entry != null)
                {
                    await entry.LaunchAsync();
                }
            }
        });
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
        var msg = new Windows.Foundation.Collections.ValueSet
            {
                { "command", "resume" },
            };

        await TryLaunchAmbieAsync();
        await Task.Delay(200);
        using var connection = GetAmbieServiceConnection();
        var status = await connection.OpenAsync();
        if (status == AppServiceConnectionStatus.Success)
        {
            await connection.SendMessageAsync(msg);
        }
    }

    private async Task StopAsync()
    {
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
