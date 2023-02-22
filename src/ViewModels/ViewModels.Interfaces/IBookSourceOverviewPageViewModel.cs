// Copyright (c) Richasy. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CleanReader.Models.Services;
using CommunityToolkit.Mvvm.Input;

namespace CleanReader.ViewModels.Interfaces;

/// <summary>
/// 书源概览页面视图模型的接口定义.
/// </summary>
public interface IBookSourceOverviewPageViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 初始化命令.
    /// </summary>
    IRelayCommand InitializeCommand { get; }

    /// <summary>
    /// 重载命令.
    /// </summary>
    IRelayCommand ReloadCommand { get; }

    /// <summary>
    /// 打开书源的命令.
    /// </summary>
    IAsyncRelayCommand<BookSource> OpenCommand { get; }

    /// <summary>
    /// 在浏览器中打开书源的命令.
    /// </summary>
    IAsyncRelayCommand<BookSource> OpenInBroswerCommand { get; }

    /// <summary>
    /// 删除书源的命令.
    /// </summary>
    IAsyncRelayCommand<BookSource> DeleteCommand { get; }

    /// <summary>
    /// 添加书源的命令.
    /// </summary>
    IAsyncRelayCommand CreateCommand { get; }

    /// <summary>
    /// 书源根目录.
    /// </summary>
    string RootPath { get; }

    /// <summary>
    /// 书源是否为空.
    /// </summary>
    bool IsShowEmpty { get; }

    /// <summary>
    /// 书源集合.
    /// </summary>
    ObservableCollection<BookSource> BookSources { get; }
}
