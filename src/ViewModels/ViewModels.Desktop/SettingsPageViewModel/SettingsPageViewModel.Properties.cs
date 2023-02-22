// Copyright (c) Richasy. All rights reserved.

using System;
using System.Reactive;
using CleanReader.Models.DataBase;
using CleanReader.Toolkit.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;
    private string _initializeTheme;

    /// <summary>
    /// 单例.
    /// </summary>
    public static SettingsPageViewModel Instance { get; } = new Lazy<SettingsPageViewModel>(() => new SettingsPageViewModel()).Value;

    /// <summary>
    /// 初始化命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> InitializeCommand { get; }

    /// <summary>
    /// 打开书库文件夹命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> OpenLibraryCommand { get; }

    /// <summary>
    /// 关闭书库命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> CloseLibraryCommand { get; }

    /// <summary>
    /// 显示创建升级书架对话框的命令.
    /// </summary>
    public ReactiveCommand<Shelf, Unit> ShowCreateOrUpdateShelfDialogCommand { get; }

    /// <summary>
    /// 应用版本号.
    /// </summary>
    [ObservableProperty]
    public string Version { get; set; }

    /// <summary>
    /// 书库路径.
    /// </summary>
    [ObservableProperty]
    public string LibraryPath { get; set; }

    /// <summary>
    /// 分栏阅读最小宽度.
    /// </summary>
    [ObservableProperty]
    public double SpreadMinWidth { get; set; }

    /// <summary>
    /// 主题.
    /// </summary>
    [ObservableProperty]
    public string Theme { get; set; }

    /// <summary>
    /// 是否显示主题更改需要重启的提示.
    /// </summary>
    [ObservableProperty]
    public bool IsShowThemeRestartTip { get; set; }

    /// <summary>
    /// 是否继续阅读.
    /// </summary>
    [ObservableProperty]
    public bool IsContinueReading { get; set; }

    /// <summary>
    /// 语言代码.
    /// </summary>
    [ObservableProperty]
    public string LanguageCode { get; set; }
}
