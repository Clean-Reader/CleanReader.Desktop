// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Services.Novel.Models;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Pages
{
    /// <summary>
    /// 书源管理概览界面.
    /// </summary>
    public sealed partial class BookSourceOverviewPage : Page
    {
        private readonly BookSourceOverviewPageViewModel _viewModel = BookSourceOverviewPageViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookSourceOverviewPage"/> class.
        /// </summary>
        public BookSourceOverviewPage() => InitializeComponent();

        private void OnLoaded(object sender, RoutedEventArgs e)
            => _viewModel.InitializeCommand.Execute().Subscribe();

        private void OnItemClick(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as BookSource;
            _viewModel.OpenCommand.Execute(data).Subscribe();
        }

        private void OnItemDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as BookSource;
            _viewModel.DeleteCommand.Execute(data).Subscribe();
        }

        private void OnItemOpenUrlButtonClick(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as BookSource;
            _viewModel.OpenInBroswerCommand.Execute(data).Subscribe();
        }
    }
}
