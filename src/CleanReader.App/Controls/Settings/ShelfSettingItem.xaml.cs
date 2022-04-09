// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Models.DataBase;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 书架设置条目.
    /// </summary>
    public sealed partial class ShelfSettingItem : UserControl
    {
        private readonly ShelfPageViewModel _shelfPageViewModel = ShelfPageViewModel.Instance;
        private readonly SettingsPageViewModel _settingsPageViewModel = SettingsPageViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShelfSettingItem"/> class.
        /// </summary>
        public ShelfSettingItem() => InitializeComponent();

        private void OnItemUpdateButtonClick(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as Shelf;
            _settingsPageViewModel.ShowCreateOrUpdateShelfDialogCommand.Execute(data).Subscribe();
        }

        private void OnItemMoveToTopButtonClick(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as Shelf;
            _shelfPageViewModel.MoveShelfToTopCommand.Execute(data).Subscribe();
        }

        private void OnItemDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as Shelf;
            _shelfPageViewModel.DeleteShelfCommand.Execute(data).Subscribe();
        }
    }
}
