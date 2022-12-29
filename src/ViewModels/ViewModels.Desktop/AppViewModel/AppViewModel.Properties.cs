// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reactive;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Toolkit.Interfaces;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 应用视图模型的属性.
/// </summary>
public sealed partial class AppViewModel
{
    private const string LatestReleaseUrl = "https://api.github.com/repos/Clean-Reader/CleanReader.Desktop/releases/latest";
    private readonly IAppToolkit _appToolkit;
    private readonly ISettingsToolkit _settingsToolkit;

    private readonly IDisposable _mainWindowSubscription;

    private bool _disposedValue;

    /// <summary>
    /// 有导航请求时发生.
    /// </summary>
    public event EventHandler<NavigationEventArgs> NavigationRequested;

    /// <summary>
    /// 有阅读请求时发生.
    /// </summary>
    public event EventHandler<ReadRequestEventArgs> ReadRequested;

    /// <summary>
    /// 请求导航到初始页面时发生.
    /// </summary>
    public event EventHandler StartupRequested;

    /// <summary>
    /// 请求导航到迁移页面时发生.
    /// </summary>
    public event EventHandler<MigrationResult> MigrationRequested;

    /// <summary>
    /// 请求显示提醒.
    /// </summary>
    public event EventHandler<AppTipNotificationEventArgs> RequestShowTip;

    /// <summary>
    /// 应用视图模型实例.
    /// </summary>
    public static AppViewModel Instance { get; } = new Lazy<AppViewModel>(() => new AppViewModel()).Value;

    /// <summary>
    /// 检查Github是否有更新.
    /// </summary>
    public ReactiveCommand<Unit, Unit> CheckGithubUpdateCommand { get; }

    /// <summary>
    /// 主窗口对象.
    /// </summary>
    [Reactive]
    public Window MainWindow { get; private set; }

    /// <summary>
    /// 主窗口句柄.
    /// </summary>
    public IntPtr MainWindowHandle { get; private set; }

    /// <summary>
    /// 应用窗口对象.
    /// </summary>
    [Reactive]
    public AppWindow AppWindow { get; private set; }

    /// <summary>
    /// 窗口下的Xaml Root.
    /// </summary>
    public XamlRoot XamlRoot => MainWindow.Content.XamlRoot;

    /// <summary>
    /// 导航集合.
    /// </summary>
    public List<NavigationItem> NavigationList { get; }

    /// <summary>
    /// 初始化的文件路径.
    /// </summary>
    public string InitializeFilePath { get; set; }

    /// <summary>
    /// 是否显示遮罩.
    /// </summary>
    [Reactive]
    public bool IsMaskShown { get; set; }

    /// <summary>
    /// 是否为全屏.
    /// </summary>
    [Reactive]
    public bool IsFullScreen { get; set; }

    /// <summary>
    /// 是否为迷你模式.
    /// </summary>
    [Reactive]
    public bool IsMiniView { get; set; }

    /// <summary>
    /// 是否正在初始化.
    /// </summary>
    [Reactive]
    public bool IsInitializing { get; set; }

    /// <summary>
    /// UI线程.
    /// </summary>
    public DispatcherQueue DispatcherQueue { get; private set; }
}
