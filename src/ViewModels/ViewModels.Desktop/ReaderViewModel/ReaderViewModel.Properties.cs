// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using CleanReader.Models.App;
using CleanReader.Models.DataBase;
using CleanReader.Toolkit.Interfaces;
using GoogleTranslateFreeApi;
using Microsoft.UI.Dispatching;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 阅读器视图模型.
/// </summary>
public sealed partial class ReaderViewModel
{
    private readonly LibraryDbContext _libraryDbContext;
    private readonly ObservableAsPropertyHelper<bool> _isShowCoverMask;
    private readonly ObservableAsPropertyHelper<bool> _isTranslating;
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IFontToolkit _fontToolkit;
    private readonly GoogleTranslator _googleTranslator;
    private Book _book;
    private DateTime _startTime;
    private bool _disposedValue;
    private List<ReaderChapterViewModel> _chapterList;
    private bool _isEnableToggleFullScreen;
    private bool _isEnableToggleMiniView;

    /// <summary>
    /// 实例.
    /// </summary>
    public static ReaderViewModel Instance { get; } = new Lazy<ReaderViewModel>(() => new ReaderViewModel()).Value;

    /// <summary>
    /// 保存进度命令.
    /// </summary>
    public ReactiveCommand<string, Unit> SaveProgressCommand { get; }

    /// <summary>
    /// 保存定位列表命令.
    /// </summary>
    public ReactiveCommand<string, Unit> SaveLocationsCommand { get; }

    /// <summary>
    /// 初始化目录命令.
    /// </summary>
    public ReactiveCommand<string, Unit> InitializeTocCommand { get; }

    /// <summary>
    /// 从CfiRange获取高亮标记的命令.
    /// </summary>
    public ReactiveCommand<string, Highlight> GetHightlightFromCfiCommand { get; }

    /// <summary>
    /// 添加高亮的命令.
    /// </summary>
    public ReactiveCommand<Highlight, Unit> AddOrUpdateHighlightCommand { get; }

    /// <summary>
    /// 结束阅读命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> StopReadingCommand { get; }

    /// <summary>
    /// 切换目录显示的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ToggleCatalogCommand { get; }

    /// <summary>
    /// 显示笔记面板的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ShowNotesCommand { get; }

    /// <summary>
    /// 切换界面设置的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ShowInterfaceSettingsCommand { get; }

    /// <summary>
    /// 显示搜索面板的命令.
    /// </summary>
    public ReactiveCommand<string, Unit> ShowSearchDailogCommand { get; }

    /// <summary>
    /// 内部搜索命令.
    /// </summary>
    public ReactiveCommand<string, Unit> InternalSearchCommand { get; }

    /// <summary>
    /// 初始化搜索结果命令.
    /// </summary>
    public ReactiveCommand<string, Unit> InitializeSearchResultCommand { get; }

    /// <summary>
    /// 初始化笔记命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> InitializeNotesCommand { get; }

    /// <summary>
    /// 跳转位置的命令.
    /// </summary>
    public ReactiveCommand<string, Unit> ChangeLocationCommand { get; }

    /// <summary>
    /// 显示高亮对话框命令.
    /// </summary>
    public ReactiveCommand<ReaderContextMenuArgs, Unit> ShowHighlightDialogCommand { get; }

    /// <summary>
    /// 清理浮出层的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ClearPopupCommand { get; }

    /// <summary>
    /// 返回命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> BackCommand { get; }

    /// <summary>
    /// 翻译命令.
    /// </summary>
    public ReactiveCommand<string, Unit> TranslateCommand { get; }

    /// <summary>
    /// 在线搜索命令.
    /// </summary>
    public ReactiveCommand<string, Unit> OnlineSearchCommand { get; }

    /// <summary>
    /// 显示加载失败的错误提示.
    /// </summary>
    public ReactiveCommand<string, Unit> DisplayInitializeErrorCommand { get; }

    /// <summary>
    /// 上一章节命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> PreviousChapterCommand { get; }

    /// <summary>
    /// 下一章节命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> NextChapterCommand { get; }

    /// <summary>
    /// 移除高亮命令.
    /// </summary>
    public ReactiveCommand<Highlight, Unit> RemoveHighlightCommand { get; }

    /// <summary>
    /// 进入全屏命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ToggleFullScreenCommand { get; }

    /// <summary>
    /// 进入迷你视图命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ToggleMiniViewCommand { get; }

    /// <summary>
    /// 章节列表.
    /// </summary>
    public ObservableCollection<ReaderChapterViewModel> Chapters { get; }

    /// <summary>
    /// 字体列表.
    /// </summary>
    public ObservableCollection<string> Fonts { get; }

    /// <summary>
    /// 样式列表.
    /// </summary>
    public ObservableCollection<ReaderThemeConfig> Themes { get; }

    /// <summary>
    /// 高亮集合.
    /// </summary>
    public ObservableCollection<Highlight> Highlights { get; }

    /// <summary>
    /// 颜色.
    /// </summary>
    public ObservableCollection<string> Colors { get; }

    /// <summary>
    /// 搜索结果.
    /// </summary>
    public ObservableCollection<ReaderSearchResult> SearchResult { get; }

    /// <summary>
    /// 当前选中章节.
    /// </summary>
    [ObservableProperty]
    public ReaderChapterViewModel CurrentChapter { get; set; }

    /// <summary>
    /// 文件路径.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// 书名.
    /// </summary>
    [ObservableProperty]
    public string BookTitle { get; set; }

    /// <summary>
    /// 当前章节名.
    /// </summary>
    [ObservableProperty]
    public string CurrentChapterTitle { get; set; }

    /// <summary>
    /// 进度.
    /// </summary>
    [ObservableProperty]
    public string Progresss { get; set; }

    /// <summary>
    /// 是否显示菜单.
    /// </summary>
    [ObservableProperty]
    public bool IsMenuShown { get; set; }

    /// <summary>
    /// 目录是否显示.
    /// </summary>
    [ObservableProperty]
    public bool IsCatalogShown { get; set; }

    /// <summary>
    /// 笔记是否显示.
    /// </summary>
    [ObservableProperty]
    public bool IsNotesShown { get; set; }

    /// <summary>
    /// 字体.
    /// </summary>
    [ObservableProperty]
    public string FontFamily { get; set; }

    /// <summary>
    /// 字体大小.
    /// </summary>
    [ObservableProperty]
    public double FontSize { get; set; }

    /// <summary>
    /// 行高.
    /// </summary>
    [ObservableProperty]
    public double LineHeight { get; set; }

    /// <summary>
    /// 背景色.
    /// </summary>
    [ObservableProperty]
    public string Background { get; set; }

    /// <summary>
    /// 前景色.
    /// </summary>
    [ObservableProperty]
    public string Foreground { get; set; }

    /// <summary>
    /// 附加样式.
    /// </summary>
    [ObservableProperty]
    public string AdditionalStyle { get; set; }

    /// <summary>
    /// 是否显式主题选择器.
    /// </summary>
    [ObservableProperty]
    public bool IsThemeSelectionShown { get; set; }

    /// <summary>
    /// 是否正在初始化.
    /// </summary>
    [ObservableProperty]
    public bool IsInitializing { get; set; }

    /// <summary>
    /// 是否加载失败.
    /// </summary>
    [ObservableProperty]
    public bool IsInitializeFailed { get; set; }

    /// <summary>
    /// 加载错误文本.
    /// </summary>
    [ObservableProperty]
    public string InitializeErrorText { get; set; }

    /// <summary>
    /// 原始文本.
    /// </summary>
    [ObservableProperty]
    public string SourceText { get; set; }

    /// <summary>
    /// 翻译后文本.
    /// </summary>
    [ObservableProperty]
    public string TranslatedText { get; set; }

    /// <summary>
    /// 翻译出错.
    /// </summary>
    [ObservableProperty]
    public bool IsTranslateError { get; set; }

    /// <summary>
    /// 是否正在搜索.
    /// </summary>
    [ObservableProperty]
    public bool IsSearcing { get; set; }

    /// <summary>
    /// 搜索结果是否为空.
    /// </summary>
    [ObservableProperty]
    public bool IsSearchEmptyShown { get; set; }

    /// <summary>
    /// 笔记是否为空.
    /// </summary>
    [ObservableProperty]
    public bool IsNotesEmptyShown { get; set; }

    /// <summary>
    /// 是否顺滑滚动.
    /// </summary>
    [ObservableProperty]
    public bool IsSmoothScroll { get; set; }

    /// <summary>
    /// 是否正在翻译.
    /// </summary>
    public bool IsTranslating => _isTranslating.Value;

    /// <summary>
    /// 起始位置.
    /// </summary>
    public string StartCfi { get; private set; }

    /// <summary>
    /// 初始化高亮.
    /// </summary>
    public string InitHighlights { get; private set; }

    /// <summary>
    /// 位置列表.
    /// </summary>
    public string Locations { get; private set; }

    /// <summary>
    /// 分栏阅读最小宽度.
    /// </summary>
    public double SpreadMinWidth { get; private set; }

    /// <summary>
    /// 是否显示覆盖阅读界面的遮罩.
    /// </summary>
    public bool IsShowCoverMask => _isShowCoverMask.Value;

    private static DispatcherQueue DispatcherQueue => AppViewModel.Instance.DispatcherQueue;
}
