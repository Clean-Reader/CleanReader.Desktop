// Copyright (c) Richasy. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CleanReader.Models.App;
using CleanReader.Models.DataBase;
using CommunityToolkit.Mvvm.Input;

namespace CleanReader.ViewModels.Interfaces;

/// <summary>
/// 书库视图模型的接口定义.
/// </summary>
public interface ILibraryViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 书库已经初始化完成，可以被加载时触发.
    /// </summary>
    event EventHandler LibraryInitialized;

    /// <summary>
    /// 有加载书源请求时发生.
    /// </summary>
    event EventHandler LoadBookSourcesRequested;

    /// <summary>
    /// 有新书导入时发生.
    /// </summary>
    event EventHandler BookAdded;

    /// <summary>
    /// 请求刷新书架列表.
    /// </summary>
    event EventHandler RequestShelfRefresh;

    /// <summary>
    /// 迁移已完成.
    /// </summary>
    event EventHandler Migrated;

    /// <summary>
    /// 书库数据库上下文.
    /// </summary>
    LibraryDbContext LibraryContext { get; }

    /// <summary>
    /// 打开书库的命令.
    /// </summary>
    IAsyncRelayCommand OpenLibraryFolderCommand { get; }

    /// <summary>
    /// 创建书库的命令.
    /// </summary>
    IAsyncRelayCommand CreateLibraryFolderCommand { get; }

    /// <summary>
    /// 显示导入对话框的命令.
    /// </summary>
    IAsyncRelayCommand ShowImportDialogCommand { get; }

    /// <summary>
    /// 清除当前书库缓存的命令.
    /// </summary>
    IRelayCommand ClearCommand { get; }

    /// <summary>
    /// 显示在线搜索对话框的命令.
    /// </summary>
    IAsyncRelayCommand<string> ShowOnlineSearchDialogCommand { get; }

    /// <summary>
    /// 显示替换书源对话框的命令.
    /// </summary>
    IAsyncRelayCommand<Book> ShowReplaceSourceDialogCommand { get; }

    /// <summary>
    /// 拆分章节的命令.
    /// </summary>
    IRelayCommand<string> SplitChapterCommand { get; }

    /// <summary>
    /// 在线搜索命令.
    /// </summary>
    IAsyncRelayCommand<string> SearchOnlineBooksCommand { get; }

    /// <summary>
    /// 选中在线搜索结果的命令.
    /// </summary>
    IAsyncRelayCommand<IOnlineBookViewModel> SelectOnlineSearchResultCommand { get; }

    /// <summary>
    /// 初始化主题的命令.
    /// </summary>
    IAsyncRelayCommand InitializeThemesCommand { get; }

    /// <summary>
    /// 创建书架的命令.
    /// </summary>
    IRelayCommand<string> CreateShelfCommand { get; }

    /// <summary>
    /// 更新书架的命令.
    /// </summary>
    IRelayCommand<Shelf> UpdateShelfCommand { get; }

    /// <summary>
    /// 同步命令.
    /// </summary>
    IAsyncRelayCommand SyncBooksCommand { get; }

    /// <summary>
    /// 初始化书源命令.
    /// </summary>
    IAsyncRelayCommand InitializeBookSourcesCommand { get; }

    /// <summary>
    /// 显示错误的命令.
    /// </summary>
    IRelayCommand<Exception> DisplayExceptionCommand { get; }

    /// <summary>
    /// 是否显示错误信息.
    /// </summary>
    bool IsShowException { get; }

    /// <summary>
    /// 是否显示错误处理按钮.
    /// </summary>
    bool IsShowExceptionAction { get; }

    /// <summary>
    /// 错误信息.
    /// </summary>
    string ExceptionMessage { get; }

    /// <summary>
    /// 错误处理链接.
    /// </summary>
    string ExceptionActionUrl { get; }

    /// <summary>
    /// 错误处理链接内容.
    /// </summary>
    string ExceptionActionContent { get; }

    /// <summary>
    /// 是否正在加载.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// 是否正在导入.
    /// </summary>
    bool IsImporting { get; }

    /// <summary>
    /// 是否正在拆分章节.
    /// </summary>
    bool IsSpliting { get; }

    /// <summary>
    /// 是否正在在线搜索.
    /// </summary>
    bool IsOnlineSearching { get; }

    /// <summary>
    /// 是否显示无有效分章.
    /// </summary>
    bool IsSplitEmptyShown { get; }

    /// <summary>
    /// 是否显示没有搜索结果.
    /// </summary>
    bool IsOnlineSearchEmptyShown { get; }

    /// <summary>
    /// 是否显示初次打开在线搜索对话框的提示.
    /// </summary>
    bool IsFirstOnlineSearchTipShown { get; }

    /// <summary>
    /// 是否显示初次打开替换书源对话框的提示.
    /// </summary>
    bool IsFirstReplaceSourceTipShown { get; }

    /// <summary>
    /// 拆分后的章节集合.
    /// </summary>
    ObservableCollection<SplitChapter> SplitChapters { get; }

    /// <summary>
    /// 书源集合.
    /// </summary>
    ObservableCollection<Models.Services.BookSource> BookSources { get; }

    /// <summary>
    /// 用于替换的书源集合.
    /// </summary>
    ObservableCollection<Models.Services.BookSource> ReplaceBookSources { get; }

    /// <summary>
    /// 在线搜索的书籍结果.
    /// </summary>
    ObservableCollection<IOnlineBookViewModel> OnlineSearchBooks { get; }

    /// <summary>
    /// 主题.
    /// </summary>
    List<ReaderThemeConfig> Themes { get; }

    /// <summary>
    /// 已选中的书源.
    /// </summary>
    Models.Services.BookSource SelectedBookSource { get; set; }

    /// <summary>
    /// 原始书源 (用于替换书源).
    /// </summary>
    Models.Services.BookSource OriginSource { get; set; }

    /// <summary>
    /// 原始书籍 (用于替换书源).
    /// </summary>
    Book OriginalBook { get; set; }

    /// <summary>
    /// 已选中的在线搜索结果.
    /// </summary>
    Models.Services.Book SelectedSearchBook { get; set; }

    /// <summary>
    /// 文件访问权限是否受限.
    /// </summary>
    bool IsFileSystemLimited { get; set; }

    /// <summary>
    /// 是否可以下载在线书籍.
    /// </summary>
    bool CanDownloadOnlineBook { get; }

    /// <summary>
    /// 导入本地书籍的命令.
    /// </summary>
    /// <returns>书籍路径.</returns>
    Task<string> ImportLocalBookAsync();

    /// <summary>
    /// 导入本地 Txt 文件并生成书籍条目.
    /// </summary>
    /// <param name="filePath">文件路径.</param>
    /// <returns>书籍条目.</returns>
    Task<Book> GenerateBookEntryFromTxtFileAsync(string filePath);

    /// <summary>
    /// 导入本地 Epub 文件并生成书籍条目.
    /// </summary>
    /// <param name="filePath">文件路径.</param>
    /// <returns>书籍条目.</returns>
    Task<Book> GenerateBookEntryFromEpubFileAsync(string filePath);

    /// <summary>
    /// 下载在线书籍并生成或替换书籍条目.
    /// </summary>
    /// <param name="book">书籍信息.</param>
    /// <returns>书籍条目.</returns>
    Task<Book> GenerateBookEntryFromOnlineBookAsync(Models.Services.Book book = null);

    /// <summary>
    /// 初始化书库.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="InvalidOperationException">传入的书库路径有误.</exception>
    Task InitializeLibraryAsync();

    /// <summary>
    /// 获取本地封面图片.
    /// </summary>
    /// <param name="name">封面文件名.</param>
    /// <returns><see cref="BitmapImage"/>.</returns>
    Task<object> GetLocalCoverImageAsync(string name);

    /// <summary>
    /// 检查是否需要继续阅读.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task CheckContinueReadingAsync();

    /// <summary>
    /// 检查初始文件是直接打开还是要执行导入.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task CheckOpenFileOrImportAsync();
}
