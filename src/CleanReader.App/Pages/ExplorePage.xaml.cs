// Copyright (c) Richasy. All rights reserved.

using System;
using System.Reactive.Linq;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Pages
{
    /// <summary>
    /// 发现与探索页面.
    /// </summary>
    public sealed partial class ExplorePage : Page
    {
        private readonly ExplorePageViewModel _viewModel = ExplorePageViewModel.Instance;
        private readonly LibraryViewModel _libraryViewModel = LibraryViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplorePage"/> class.
        /// </summary>
        public ExplorePage() => InitializeComponent();

        private void OnLoaded(object sender, RoutedEventArgs e) => FindName("DetailViewer");

        private void OnShelfItemClick(object sender, RoutedEventArgs e)
        {
            var dispatcher = DispatcherQueue;
            var vm = (sender as FrameworkElement).DataContext as OnlineBookViewModel;
            LibraryViewModel.Instance.InsertOrUpdateBookEntryFromOnlineBookCommand.Execute(vm).Subscribe();
        }

#pragma warning disable CA1822 // 将成员标记为 static
        private void OnCardClick(object sender, OnlineBookViewModel e)
        {
            var vm = (sender as FrameworkElement).DataContext as OnlineBookViewModel;
            vm.OpenInBroswerCommand.Execute().Subscribe();
        }
#pragma warning restore CA1822 // 将成员标记为 static
    }
}
