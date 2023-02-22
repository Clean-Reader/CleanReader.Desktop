// Copyright (c) Richasy. All rights reserved.

using CleanReader.Controls.Interfaces;
using CleanReader.Models.App;
using Microsoft.UI.Xaml;

namespace CleanReader.Controls;

/// <summary>
/// 内部搜索对话框.
/// </summary>
public sealed partial class InternalSearchDialog : CustomDialog, IInternalSearchDialog
{
    private readonly ReaderViewModel _viewModel = ReaderViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalSearchDialog"/> class.
    /// </summary>
    public InternalSearchDialog() => InitializeComponent();

    /// <inheritdoc/>
    public override void InjectData(object data)
    {
        if (data is string str && !string.IsNullOrEmpty(str))
        {
            SearchBox.Text = str;
            EnterNormal();
            _viewModel.InternalSearchCommand.Execute(str).Subscribe();
        }
    }

    private void OnQuerySubmitted(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, Microsoft.UI.Xaml.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        // Do search.
        if (!string.IsNullOrEmpty(args.QueryText))
        {
            EnterNormal();
            _viewModel.InternalSearchCommand?.Execute(args.QueryText).Subscribe();
        }
    }

    private void OnSearchResultItemClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as ReaderSearchResult;
        _viewModel.ChangeLocationCommand.Execute(data.Cfi).Subscribe();
        Hide();
    }

    private void EnterNormal()
    {
        InitializeArea.Visibility = Visibility.Collapsed;
        DetailArea.Visibility = Visibility.Visible;
    }
}
