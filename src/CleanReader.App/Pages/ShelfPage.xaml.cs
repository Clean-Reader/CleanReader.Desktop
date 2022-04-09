// Copyright (c) Richasy. All rights reserved.

using System;
using System.ComponentModel;
using System.Linq;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Pages
{
    /// <summary>
    /// 书架页面.
    /// </summary>
    public sealed partial class ShelfPage : Page
    {
        private readonly ShelfPageViewModel _viewModel = ShelfPageViewModel.Instance;
        private readonly LibraryViewModel _libraryViewModel = LibraryViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShelfPage"/> class.
        /// </summary>
        public ShelfPage()
        {
            InitializeComponent();
            NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
            => _viewModel.PropertyChanged -= OnViewModelPropertyChanged;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            _viewModel.InitializeViewModelCommand.Execute().Subscribe();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.CurrentSort))
            {
                InitializeSortType();
            }
        }

        private void InitializeSortType()
            => SortFlyout.Items
            .OfType<ToggleMenuFlyoutItem>()
            .ToList()
            .ForEach(p => p.IsChecked = p.Tag.ToString() == _viewModel.CurrentSort);

        private void OnSortItemClick(object sender, RoutedEventArgs e)
        {
            var item = sender as ToggleMenuFlyoutItem;
            var type = item.Tag.ToString();
            if (type == _viewModel.CurrentSort)
            {
                item.IsChecked = true;
            }
            else
            {
                _viewModel.CurrentSort = type;
            }
        }
    }
}
