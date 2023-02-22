// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Services.Interfaces;
using CleanReader.Toolkit.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 应用视图模型的属性.
/// </summary>
public sealed partial class AppViewModel
{
    private const string LatestReleaseUrl = "https://api.github.com/repos/Clean-Reader/CleanReader.Desktop/releases/latest";
    private readonly IAppToolkit _appToolkit;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IEpubService _epubService;

    [ObservableProperty]
    private object _mainWindow;

    [ObservableProperty]
    private IntPtr _mainWindowHandle;

    [ObservableProperty]
    private object _appWindow;

    [ObservableProperty]
    private string _initializeFilePath;

    [ObservableProperty]
    private bool _isMaskShown;

    [ObservableProperty]
    private bool _isFullScreen;

    [ObservableProperty]
    private bool _isMiniView;

    [ObservableProperty]
    private bool _isInitializing;

    /// <inheritdoc/>
    public event EventHandler<NavigationEventArgs> NavigationRequested;

    /// <inheritdoc/>
    public event EventHandler<ReadRequestEventArgs> ReadRequested;

    /// <inheritdoc/>
    public event EventHandler StartupRequested;

    /// <inheritdoc/>
    public event EventHandler<MigrationResult> MigrationRequested;

    /// <inheritdoc/>
    public event EventHandler<AppTipNotificationEventArgs> RequestShowTip;

    /// <inheritdoc/>
    public List<NavigationItem> NavigationList { get; }
}
