// Copyright (c) Richasy. All rights reserved.

using System.ComponentModel;
using CleanReader.Models.Services;
using CommunityToolkit.Mvvm.Input;

namespace CleanReader.ViewModels.Interfaces;

/// <summary>
/// 在线书籍视图模型的接口定义.
/// </summary>
public interface IOnlineBookViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 在浏览器中打开的命令.
    /// </summary>
    IAsyncRelayCommand OpenInBroswerCommand { get; }

    /// <summary>
    /// 在线搜索的命令.
    /// </summary>
    IRelayCommand OnlineSearchCommand { get; }

    /// <summary>
    /// 书籍内容.
    /// </summary>
    Book Book { get; }

    /// <summary>
    /// 是否允许下载.
    /// </summary>
    bool EnableDownload { get; set; }

    /// <summary>
    /// 是否被选中.
    /// </summary>
    bool IsSelected { get; set; }

    /// <summary>
    /// 注入数据.
    /// </summary>
    /// <param name="book">书籍信息.</param>
    void InjectData(Book book);
}
