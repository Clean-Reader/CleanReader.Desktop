// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;

namespace CleanReader.App.Controls;

/// <summary>
/// 在线搜索对话框.
/// </summary>
public sealed partial class OnlineSearchDialog : CustomDialog
{
    private readonly LibraryViewModel _viewModel = LibraryViewModel.Instance;
    private bool _isInitSearch = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlineSearchDialog"/> class.
    /// </summary>
    public OnlineSearchDialog() => InitializeComponent();

    /// <inheritdoc/>
    public override void InjectData(object data)
    {
        if (data is string text)
        {
            SearchBox.Text = text;
            _isInitSearch = true;
        }
    }

    private void OnSearchBoxQuerySubmitted(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, Microsoft.UI.Xaml.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (!string.IsNullOrEmpty(sender.Text?.Trim()))
        {
            _viewModel.OnlineSearchCommand.Execute(sender.Text.Trim()).Subscribe();
        }
    }

#pragma warning disable CA1822 // 将成员标记为 static
    private void OnCardClick(object sender, OnlineBookViewModel e)
        => LibraryViewModel.Instance.SelectOnlineSearchResultCommand.Execute(e).Subscribe();

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_isInitSearch && !string.IsNullOrEmpty(SearchBox.Text))
        {
            _viewModel.OnlineSearchCommand.Execute(SearchBox.Text.Trim()).Subscribe();
            _isInitSearch = false;
        }
    }
#pragma warning restore CA1822 // 将成员标记为 static
}
