// Copyright (c) Richasy. All rights reserved.

using System;
using System.Reactive;
using CleanReader.Toolkit.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 背景音乐视图模型.
/// </summary>
public sealed partial class BackgroundMusicViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;

    [ObservableProperty]
    private bool _isAmbieInstalled;

    [ObservableProperty]
    private bool _isBackgroundMusicAutoPlay;

    [ObservableProperty]
    private bool _isAmbieAutoPlay;

    [ObservableProperty]
    private bool _isAmbieCompact;
}
