// Copyright (c) Richasy. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CleanReader.ViewModels.Interfaces;

/// <summary>
/// 背景音乐视图模型的接口定义.
/// </summary>
public interface IBackgroundMusicViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 播放音乐命令.
    /// </summary>
    IAsyncRelayCommand PlayCommand { get; }

    /// <summary>
    /// 停止播放的命令.
    /// </summary>
    IAsyncRelayCommand StopCommand { get; }

    /// <summary>
    /// Ambie是否已经安装在设备上.
    /// </summary>
    bool IsAmbieInstalled { get; }

    /// <summary>
    /// 背景音乐自动播放.
    /// </summary>
    bool IsBackgroundMusicAutoPlay { get; set; }

    /// <summary>
    /// Ambie启动时是否自动播放.
    /// </summary>
    bool IsAmbieAutoPlay { get; set; }

    /// <summary>
    /// Ambie启动时是否进入小窗模式.
    /// </summary>
    bool IsAmbieCompact { get; set; }

    /// <summary>
    /// 检查 Ambie 是否被安装在设备上.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task CheckAmbieInstalledAsync();
}
