// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Models.App;
using CleanReader.Models.DataBase;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls;

/// <summary>
/// 阅读器笔记.
/// </summary>
public sealed partial class ReaderNotes : UserControl
{
    private readonly ReaderViewModel _viewModel = ReaderViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderNotes"/> class.
    /// </summary>
    public ReaderNotes()
    {
        InitializeComponent();
        NotesShadow.Receivers.Add(ShadowHost);
        Container.Translation += new System.Numerics.Vector3(0, 0, 48);
    }

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as Highlight;
        _viewModel.ChangeLocationCommand.Execute(data.CfiRange).Subscribe();
    }

    private void OnItemModifyButtonClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as Highlight;
        var args = new ReaderContextMenuArgs()
        {
            Text = data.Text,
            Range = data.CfiRange,
        };
        _viewModel.ShowHighlightDialogCommand.Execute(args).Subscribe();
    }

    private void OnItemDeleteButtonClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as Highlight;
        _viewModel.RemoveHighlightCommand.Execute(data).Subscribe();
    }
}
