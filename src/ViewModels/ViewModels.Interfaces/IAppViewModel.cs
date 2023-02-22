// Copyright (c) Richasy. All rights reserved.

using System.ComponentModel;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CommunityToolkit.Mvvm.Input;

namespace CleanReader.ViewModels.Interfaces;

/// <summary>
/// 应用视图模型的接口定义.
/// </summary>
public interface IAppViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 有导航请求时发生.
    /// </summary>
    event EventHandler<NavigationEventArgs> NavigationRequested;

    /// <summary>
    /// 有阅读请求时发生.
    /// </summary>
    event EventHandler<ReadRequestEventArgs> ReadRequested;

    /// <summary>
    /// 请求导航到初始页面时发生.
    /// </summary>
    event EventHandler StartupRequested;

    /// <summary>
    /// 请求导航到迁移页面时发生.
    /// </summary>
    event EventHandler<MigrationResult> MigrationRequested;

    /// <summary>
    /// 请求显示提醒.
    /// </summary>
    event EventHandler<AppTipNotificationEventArgs> RequestShowTip;

    /// <summary>
    /// 检查Github是否有更新.
    /// </summary>
    IAsyncRelayCommand CheckGithubUpdateCommand { get; }

    /// <summary>
    /// 主窗口对象.
    /// </summary>
    object MainWindow { get; }

    /// <summary>
    /// 主窗口句柄.
    /// </summary>
    IntPtr MainWindowHandle { get; }

    /// <summary>
    /// 应用窗口对象.
    /// </summary>
    object AppWindow { get; }

    /// <summary>
    /// 导航集合.
    /// </summary>
    List<NavigationItem> NavigationList { get; }

    /// <summary>
    /// 初始化的文件路径.
    /// </summary>
    string InitializeFilePath { get; set; }

    /// <summary>
    /// 是否显示遮罩.
    /// </summary>
    bool IsMaskShown { get; set; }

    /// <summary>
    /// 是否为全屏.
    /// </summary>
    bool IsFullScreen { get; }

    /// <summary>
    /// 是否为迷你模式.
    /// </summary>
    bool IsMiniView { get; }

    /// <summary>
    /// 是否正在初始化.
    /// </summary>
    bool IsInitializing { get; }

    /// <summary>
    /// 获取当前版本号.
    /// </summary>
    /// <returns>版本号.</returns>
    string GetVersioNumber();

    /// <summary>
    /// 设置主窗口.
    /// </summary>
    /// <param name="mainWindow">主窗口对象.</param>
    void SetMainWindow(object mainWindow);

    /// <summary>
    /// 请求导航至某页面.
    /// </summary>
    /// <param name="pageType">页面类型.</param>
    /// <param name="parameter">导航附加参数.</param>
    void RequestNavigateTo(Type pageType, object parameter = null);

    /// <summary>
    /// 请求导航至某页面.
    /// </summary>
    /// <param name="navItem">导航条目.</param>
    /// <param name="parameter">导航附加参数.</param>
    void RequestNavigateTo(NavigationItem navItem, object parameter = null);

    /// <summary>
    /// 请求导航到初始页面.
    /// </summary>
    void RequestStartup();

    /// <summary>
    /// 请求导航到迁移页面.
    /// </summary>
    /// <param name="result">迁移检查结果.</param>
    void RequestMigration(MigrationResult result);

    /// <summary>
    /// 请求阅读某本书.
    /// </summary>
    /// <param name="book">书籍.</param>
    /// <param name="startCfi">起始位置.</param>
    void RequestRead(Book book, string startCfi = null);

    /// <summary>
    /// 是否应该显示启动引导页面.
    /// </summary>
    /// <returns><c>true</c> 表示应该显示.</returns>
    bool ShouldShowStartup();

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="message">消息内容.</param>
    /// <param name="type">消息类型.</param>
    void ShowTip(string message, InfoType type = InfoType.Information);
}
