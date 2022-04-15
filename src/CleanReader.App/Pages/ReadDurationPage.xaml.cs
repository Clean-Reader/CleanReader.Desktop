// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Models.App;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Pages
{
    /// <summary>
    /// 阅读时长页面.
    /// </summary>
    public sealed partial class ReadDurationPage : Page
    {
        private readonly ReadDurationPageViewModel _viewModel = new ReadDurationPageViewModel();
        private readonly LibraryViewModel _libraryViewModel = LibraryViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadDurationPage"/> class.
        /// </summary>
        public ReadDurationPage() => InitializeComponent();

        private void OnLoaded(object sender, RoutedEventArgs e)
            => _viewModel.InitializeCommand.Execute().Subscribe();

        private void OnDurationCardClick(object sender, RoutedEventArgs e)
        {
            var context = (sender as FrameworkElement).DataContext as ReaderDuration;
            _viewModel.ShowDetailCommand.Execute(context).Subscribe();
        }
    }
}
