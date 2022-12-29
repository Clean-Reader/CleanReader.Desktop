// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using CleanReader.Models.App;
using CleanReader.Models.DataBase;
using CleanReader.Services.Novel;
using CleanReader.Toolkit.Interfaces;
using Microsoft.UI.Dispatching;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 书库视图模型的属性部分.
/// </summary>
public sealed partial class LibraryViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IFileToolkit _fileToolkit;
    private readonly ObservableAsPropertyHelper<bool> _isOpening;
    private readonly ObservableAsPropertyHelper<bool> _isCreating;
    private readonly ObservableAsPropertyHelper<bool> _isImporting;
    private readonly ObservableAsPropertyHelper<bool> _isSpliting;
    private readonly ObservableAsPropertyHelper<bool> _isOnlineSearching;
    private readonly ObservableAsPropertyHelper<bool> _canDownloadOnlineBook;
    private DirectoryInfo _rootDirectory;
    private NovelService _novelService;
    private ICustomDialog _importDialog;
    private string _tempSelectedFilePath;
    private string _tempSplitRegex;
    private Dictionary<string, List<Services.Novel.Models.Book>> _tempOnlineSearchResult;
    private bool _disposedValue;

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
    /// 静态实例.
    /// </summary>
    public static LibraryViewModel Instance { get; } = new Lazy<LibraryViewModel>(() => new LibraryViewModel()).Value;

    /// <summary>
    /// 书库数据库上下文.
    /// </summary>
    public LibraryDbContext LibraryContext { get; private set; }

    /// <summary>
    /// 打开书库的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> OpenLibraryFolderCommand { get; }

    /// <summary>
    /// 创建书库的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> CreateLibraryFolderCommand { get; }

    /// <summary>
    /// 显示导入对话框的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ShowImportDialogCommand { get; }

    /// <summary>
    /// 清除当前书库缓存的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }

    /// <summary>
    /// 导入本地书籍的命令.
    /// </summary>
    public ReactiveCommand<Unit, string> ImportLocalBookCommand { get; }

    /// <summary>
    /// 显示在线搜索对话框的命令.
    /// </summary>
    public ReactiveCommand<string, Unit> ShowOnlineSearchDialogCommand { get; }

    /// <summary>
    /// 显示替换书源对话框的命令.
    /// </summary>
    public ReactiveCommand<Book, Unit> ShowReplaceSourceDialogCommand { get; }

    /// <summary>
    /// 拆分章节的命令.
    /// </summary>
    public ReactiveCommand<string, Unit> SplitChapterCommand { get; }

    /// <summary>
    /// 导入本地 Txt 文件并生成书籍条目的命令.
    /// </summary>
    public ReactiveCommand<string, Book> GetBookEntryFromTxtFileCommand { get; }

    /// <summary>
    /// 导入本地 Epub 文件并生成书籍条目的命令.
    /// </summary>
    public ReactiveCommand<string, Book> GetBookEntryFromEpubFileCommand { get; }

    /// <summary>
    /// 在线搜索命令.
    /// </summary>
    public ReactiveCommand<string, Unit> OnlineSearchCommand { get; }

    /// <summary>
    /// 选中在线搜索结果的命令.
    /// </summary>
    public ReactiveCommand<OnlineBookViewModel, Unit> SelectOnlineSearchResultCommand { get; }

    /// <summary>
    /// 下载在线书籍并生成或替换书籍条目的命令.
    /// </summary>
    public ReactiveCommand<OnlineBookViewModel, Book> InsertOrUpdateBookEntryFromOnlineBookCommand { get; }

    /// <summary>
    /// 初始化主题的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> InitializeThemesCommand { get; }

    /// <summary>
    /// 创建书架的命令.
    /// </summary>
    public ReactiveCommand<string, Unit> CreateShelfCommand { get; }

    /// <summary>
    /// 更新书架的命令.
    /// </summary>
    public ReactiveCommand<Shelf, Unit> UpdateShelfCommand { get; }

    /// <summary>
    /// 同步命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SyncCommand { get; }

    /// <summary>
    /// 初始化书源命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> InitializeBookSourceCommand { get; }

    /// <summary>
    /// 显示错误的命令.
    /// </summary>
    public ReactiveCommand<Exception, Unit> DisplayExceptionCommand { get; }

    /// <summary>
    /// 是否显示错误信息.
    /// </summary>
    [Reactive]
    public bool IsShowException { get; set; }

    /// <summary>
    /// 是否显示错误处理按钮.
    /// </summary>
    [Reactive]
    public bool IsShowExceptionAction { get; set; }

    /// <summary>
    /// 错误信息.
    /// </summary>
    [Reactive]
    public string ExceptionMessage { get; set; }

    /// <summary>
    /// 错误处理链接.
    /// </summary>
    [Reactive]
    public string ExceptionActionUrl { get; set; }

    /// <summary>
    /// 错误处理链接内容.
    /// </summary>
    [Reactive]
    public string ExceptionActionContent { get; set; }

    /// <summary>
    /// 是否正在加载.
    /// </summary>
    public bool IsLoading => _isCreating.Value || _isOpening.Value;

    /// <summary>
    /// 是否正在导入.
    /// </summary>
    public bool IsImporting => _isImporting.Value;

    /// <summary>
    /// 是否正在拆分章节.
    /// </summary>
    public bool IsSpliting => _isSpliting.Value;

    /// <summary>
    /// 是否正在在线搜索.
    /// </summary>
    public bool IsOnlineSearching => _isOnlineSearching.Value;

    /// <summary>
    /// 是否显示无有效分章.
    /// </summary>
    [Reactive]
    public bool IsSplitEmptyShown { get; set; }

    /// <summary>
    /// 是否显示没有搜索结果.
    /// </summary>
    [Reactive]
    public bool IsOnlineSearchEmptyShown { get; set; }

    /// <summary>
    /// 是否显示初次打开在线搜索对话框的提示.
    /// </summary>
    [Reactive]
    public bool IsFirstOnlineSearchTipShown { get; set; }

    /// <summary>
    /// 是否显示初次打开替换书源对话框的提示.
    /// </summary>
    [Reactive]
    public bool IsFirstReplaceSourceTipShown { get; set; }

    /// <summary>
    /// 拆分后的章节集合.
    /// </summary>
    public ObservableCollection<SplitChapter> SplitChapters { get; }

    /// <summary>
    /// 书源集合.
    /// </summary>
    public ObservableCollection<Services.Novel.Models.BookSource> BookSources { get; }

    /// <summary>
    /// 用于替换的书源集合.
    /// </summary>
    public ObservableCollection<Services.Novel.Models.BookSource> ReplaceBookSources { get; }

    /// <summary>
    /// 在线搜索的书籍结果.
    /// </summary>
    public ObservableCollection<OnlineBookViewModel> OnlineSearchBooks { get; }

    /// <summary>
    /// 主题.
    /// </summary>
    public List<ReaderThemeConfig> Themes { get; }

    /// <summary>
    /// 已选中的书源.
    /// </summary>
    [Reactive]
    public Services.Novel.Models.BookSource SelectedBookSource { get; set; }

    /// <summary>
    /// 原始书源 (用于替换书源).
    /// </summary>
    [Reactive]
    public Services.Novel.Models.BookSource OriginSource { get; set; }

    /// <summary>
    /// 原始书籍 (用于替换书源).
    /// </summary>
    [Reactive]
    public Book OriginalBook { get; set; }

    /// <summary>
    /// 已选中的在线搜索结果.
    /// </summary>
    [Reactive]
    public Services.Novel.Models.Book SelectedSearchBook { get; set; }

    /// <summary>
    /// 文件访问权限是否受限.
    /// </summary>
    [Reactive]
    public bool IsFileSystemLimited { get; set; }

    /// <summary>
    /// 是否可以下载在线书籍.
    /// </summary>
    public bool CanDownloadOnlineBook => _canDownloadOnlineBook.Value;

    private static DispatcherQueue DispatcherQueue => AppViewModel.Instance.DispatcherQueue;
}
