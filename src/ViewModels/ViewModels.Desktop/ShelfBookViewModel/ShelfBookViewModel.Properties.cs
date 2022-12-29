// Copyright (c) Richasy. All rights reserved.

using System.Reactive;
using CleanReader.Models.DataBase;
using CleanReader.Toolkit.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 书架上的书籍视图模型.
/// </summary>
public sealed partial class ShelfBookViewModel
{
    private readonly LibraryDbContext _dbContext;
    private readonly IFileToolkit _fileToolkit;
    private readonly ISettingsToolkit _settingsToolkit;

    /// <summary>
    /// 删除命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    /// <summary>
    /// 通过其他应用打开的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> OpenWithCommand { get; }

    /// <summary>
    /// 显示书籍信息对话框命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ShowInformationCommand { get; }

    /// <summary>
    /// 阅读命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ReadCommand { get; }

    /// <summary>
    /// 显示书架转移对话框.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ShowShelfTransferDialogCommand { get; }

    /// <summary>
    /// 显示替换书源对话框.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ShowReplaceSourceDialogCommand { get; }

    /// <summary>
    /// 打开在线书籍对应的链接.
    /// </summary>
    public ReactiveCommand<Unit, Unit> OpenBookUrlCommand { get; }

    /// <summary>
    /// 书籍另存为.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveBookCommand { get; }

    /// <summary>
    /// 封面.
    /// </summary>
    [Reactive]
    public string Cover { get; set; }

    /// <summary>
    /// 书籍名.
    /// </summary>
    [Reactive]
    public Book Book { get; set; }

    /// <summary>
    /// 是否为在线书籍.
    /// </summary>
    [Reactive]
    public bool IsOnlineBook { get; set; }

    /// <summary>
    /// 进度.
    /// </summary>
    [Reactive]
    public string Progress { get; set; }

    /// <summary>
    /// 状态图标.
    /// </summary>
    [Reactive]
    public string StatusIcon { get; set; }

    /// <summary>
    /// 是否显示封面.
    /// </summary>
    [Reactive]
    public bool IsShowCover { get; set; }

    /// <summary>
    /// 本地路径.
    /// </summary>
    [Reactive]
    public string LocalPath { get; set; }
}
