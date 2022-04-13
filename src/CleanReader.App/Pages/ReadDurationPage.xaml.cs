// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.ViewModels.Desktop;
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

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
            => _viewModel.InitializeCommand.Execute().Subscribe();
    }
}
