// Copyright (c) Richasy. All rights reserved.

using System;
using System.Threading.Tasks;
using CleanReader.Models.App;
using CleanReader.ViewModels.Desktop;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls;

/// <summary>
/// 阅读器目录.
/// </summary>
public sealed partial class ReaderCatalog : UserControl
{
    private readonly ReaderViewModel _viewModel = ReaderViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderCatalog"/> class.
    /// </summary>
    public ReaderCatalog()
    {
        InitializeComponent();
        CatalogShadow.Receivers.Add(ShadowHost);
        Container.Translation += new System.Numerics.Vector3(0, 0, 48);
        RegisterPropertyChangedCallback(VisibilityProperty, new DependencyPropertyChangedCallback(OnVisibilityChangedAsync));
    }

    /// <summary>
    /// 章节点击时触发.
    /// </summary>
    public event EventHandler<ReaderChapter> ChapterSelected;

    private void OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        => ChapterSelected?.Invoke(this, (args.InvokedItem as ReaderChapterViewModel).Chapter);

    private async void OnVisibilityChangedAsync(DependencyObject sender, DependencyProperty dp)
    {
        if (Visibility == Visibility.Visible)
        {
            await ScrollToSelectedChapterAsync();
        }
    }

    private async Task ScrollToSelectedChapterAsync()
    {
        var scrollViewer = ChapterTree.FindDescendant<ScrollViewer>();
        if (scrollViewer == null)
        {
            await Task.Delay(200);
            scrollViewer = ChapterTree.FindDescendant<ScrollViewer>();
        }

        if (scrollViewer != null && _viewModel.CurrentChapter != null)
        {
            var item = ChapterTree.FindDescendant<TreeViewItem>();
            if (item == null)
            {
                return;
            }

            var index = _viewModel.Chapters.IndexOf(_viewModel.CurrentChapter);
            scrollViewer.ChangeView(0, item.ActualHeight * index, 1);
        }
    }

    private async void OnTreeViewLoadedAsync(object sender, RoutedEventArgs e)
        => await ScrollToSelectedChapterAsync();
}
