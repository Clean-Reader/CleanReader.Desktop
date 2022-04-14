// Copyright (c) Richasy. All rights reserved.

using System;
using System.Reactive;
using Microsoft.UI.Dispatching;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 背景音乐视图模型.
/// </summary>
public sealed partial class BackgroundMusicViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;

    /// <summary>
    /// 单例.
    /// </summary>
    public static BackgroundMusicViewModel Instance { get; } = new Lazy<BackgroundMusicViewModel>(() => new BackgroundMusicViewModel()).Value;

    /// <summary>
    /// 开启服务连接的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> OpenConnectionCommand { get; }

    /// <summary>
    /// 播放音乐命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> PlayCommand { get; }

    /// <summary>
    /// 停止播放的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    /// <summary>
    /// 应用服务连接是否已经开启.
    /// </summary>
    [Reactive]
    public bool IsConnectionOpened { get; set; }

    /// <summary>
    /// Ambie是否已经安装在设备上.
    /// </summary>
    [Reactive]
    public bool IsAmbieInstalled { get; set; }
}
