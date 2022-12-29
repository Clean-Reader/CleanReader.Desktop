// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Specialized;
using CleanReader.Models.App;
using CleanReader.Models.DataBase;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 阅读器视图模型.
/// </summary>
public sealed partial class ReaderViewModel
{
    /// <summary>
    /// 请求初始化样式.
    /// </summary>
    public event EventHandler RequestInitializeStyle;

    /// <summary>
    /// 请求高亮.
    /// </summary>
    public event EventHandler<Highlight> RequestHighlight;

    /// <summary>
    /// 请求移除高亮.
    /// </summary>
    public event EventHandler<Highlight> RequestRemoveHighlight;

    /// <summary>
    /// 请求搜索.
    /// </summary>
    public event EventHandler<string> RequestSearch;

    /// <summary>
    /// 请求跳转位置.
    /// </summary>
    public event EventHandler<string> RequestChangeLocation;

    /// <summary>
    /// 请求跳转章节.
    /// </summary>
    public event EventHandler<ReaderChapter> RequestChangeChapter;

    /// <summary>
    /// 进入/退出迷你视图事件.
    /// </summary>
    public event EventHandler MiniViewRequested;

    private void OnThemesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsThemeSelectionShown = Themes.Count > 0;

    private void OnSearchResultCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsSearchEmptyShown = SearchResult.Count == 0;

    private void OnHighlightsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsNotesEmptyShown = Highlights.Count == 0;
}
