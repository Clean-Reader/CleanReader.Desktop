// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Reactive;
using CleanReader.Models.Services;
using CleanReader.Toolkit.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 书源概览页面视图模型.
/// </summary>
public sealed partial class BookSourceOverviewPageViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IFileToolkit _fileToolkit;

    /// <summary>
    /// 书源是否为空.
    /// </summary>
    [ObservableProperty]
    private bool _isShowEmpty;

    /// <inheritdoc/>
    public string RootPath { get; set; }

    /// <inheritdoc/>
    public ObservableCollection<BookSource> BookSources { get; }
}
