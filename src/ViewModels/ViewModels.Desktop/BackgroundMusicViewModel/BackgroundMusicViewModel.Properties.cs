// Copyright (c) Richasy. All rights reserved.

using System;
using System.Reactive;
using CleanReader.Toolkit.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 背景音乐视图模型.
/// </summary>
public sealed partial class BackgroundMusicViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;

    /// <summary>
    /// 单例.
    /// </summary>
    public static BackgroundMusicViewModel Instance { get; } = new Lazy<BackgroundMusicViewModel>(() => new BackgroundMusicViewModel()).Value;

    /// <summary>
    /// 播放音乐命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> PlayCommand { get; }

    /// <summary>
    /// 停止播放的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    /// <summary>
    /// Ambie是否已经安装在设备上.
    /// </summary>
    [Reactive]
    public bool IsAmbieInstalled { get; set; }

    /// <summary>
    /// 背景音乐自动播放.
    /// </summary>
    [Reactive]
    public bool IsBackgroundMusicAutoPlay { get; set; }

    /// <summary>
    /// Ambie启动时是否自动播放.
    /// </summary>
    [Reactive]
    public bool IsAmbieAutoPlay { get; set; }

    /// <summary>
    /// Ambie启动时是否进入小窗模式.
    /// </summary>
    [Reactive]
    public bool IsAmbieCompact { get; set; }
}
