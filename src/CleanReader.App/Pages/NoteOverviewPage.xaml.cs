// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Models.DataBase;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Pages
{
    /// <summary>
    /// 笔记概览页面.
    /// </summary>
    public sealed partial class NoteOverviewPage : Page
    {
        private readonly NoteOverviewPageViewModel _viewModel = new NoteOverviewPageViewModel();
        private readonly LibraryViewModel _libraryViewModel = LibraryViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteOverviewPage"/> class.
        /// </summary>
        public NoteOverviewPage() => InitializeComponent();

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
            => _viewModel.InitializeCommand.Execute().Subscribe();

        private void OnItemClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as Highlight;
            _viewModel.ShowHighlightDialogCommand.Execute(data).Subscribe();
        }

        private void OnItemDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as Highlight;
            _viewModel.DeleteHighlightCommand.Execute(data).Subscribe();
        }

        private void OnItemJumpButtonClick(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as Highlight;
            _viewModel.JumpToHighlightCommand.Execute(data).Subscribe();
        }
    }
}
