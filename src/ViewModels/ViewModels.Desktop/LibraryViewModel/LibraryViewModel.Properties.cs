// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using CleanReader.Controls.Interfaces;
using CleanReader.Models.App;
using CleanReader.Models.DataBase;
using CleanReader.Services.Interfaces;
using CleanReader.Toolkit.Interfaces;
using CleanReader.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 书库视图模型的属性部分.
/// </summary>
public sealed partial class LibraryViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IFileToolkit _fileToolkit;
    private readonly INovelService _novelService;
    private readonly IEpubService _epubService;
    private readonly IAppViewModel _appViewModel;
    private readonly DispatcherQueue _dispatcherQueue;
    private DirectoryInfo _rootDirectory;
    private string _tempSelectedFilePath;
    private string _tempSplitRegex;
    private Dictionary<string, List<Models.Services.Book>> _tempOnlineSearchResult;
    private bool _disposedValue;
    private IImportWayDialog _importDialog;
    private bool _isOpening;
    private bool _isCreating;

    [ObservableProperty]
    private bool _isShowException;

    [ObservableProperty]
    private bool _isShowExceptionAction;

    [ObservableProperty]
    private string _exceptionMessage;

    [ObservableProperty]
    private string _exceptionActionUrl;

    [ObservableProperty]
    private string _exceptionActionContent;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isImporting;

    [ObservableProperty]
    private bool _isSpliting;

    [ObservableProperty]
    private bool _isOnlineSearching;

    [ObservableProperty]
    private bool _isSplitEmptyShown;

    [ObservableProperty]
    private bool _isOnlineSearchEmptyShown;

    [ObservableProperty]
    private bool _isFirstOnlineSearchTipShown;

    [ObservableProperty]
    private bool _isFirstReplaceSourceTipShown;

    [ObservableProperty]
    private Models.Services.BookSource _selectedBookSource;

    [ObservableProperty]
    private Models.Services.BookSource _originSource;

    [ObservableProperty]
    private Book _originalBook;

    [ObservableProperty]
    private Models.Services.Book _selectedSearchBook;

    [ObservableProperty]
    private bool _isFileSystemLimited;

    [ObservableProperty]
    private bool _canDownloadOnlineBook;

    /// <summary>
    /// 书库已经初始化完成，可以被加载时触发.
    /// </summary>
    public event EventHandler LibraryInitialized;

    /// <summary>
    /// 有加载书源请求时发生.
    /// </summary>
    public event EventHandler LoadBookSourcesRequested;

    /// <summary>
    /// 有新书导入时发生.
    /// </summary>
    public event EventHandler BookAdded;

    /// <summary>
    /// 请求刷新书架列表.
    /// </summary>
    public event EventHandler RequestShelfRefresh;

    /// <summary>
    /// 迁移已完成.
    /// </summary>
    public event EventHandler Migrated;

    /// <summary>
    /// 书库数据库上下文.
    /// </summary>
    public LibraryDbContext LibraryContext { get; set; }

    /// <inheritdoc/>
    public ObservableCollection<SplitChapter> SplitChapters { get; }

    /// <inheritdoc/>
    public ObservableCollection<Models.Services.BookSource> BookSources { get; }

    /// <inheritdoc/>
    public ObservableCollection<Models.Services.BookSource> ReplaceBookSources { get; }

    /// <inheritdoc/>
    public ObservableCollection<IOnlineBookViewModel> OnlineSearchBooks { get; }

    /// <inheritdoc/>
    public List<ReaderThemeConfig> Themes { get; }
}
