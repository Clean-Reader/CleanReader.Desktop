// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Services.Novel.Models;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 替换书源对话框.
    /// </summary>
    public sealed partial class ReplaceSourceDialog : CustomDialog
    {
        private readonly LibraryViewModel _viewModel = LibraryViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceSourceDialog"/> class.
        /// </summary>
        public ReplaceSourceDialog() => InitializeComponent();

        private void OnSourceSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem is BookSource item && _viewModel.SelectedBookSource != item)
            {
                _viewModel.SelectedBookSource = item;
                _viewModel.OnlineSearchCommand.Execute(_viewModel.OriginalBook.Title).Subscribe();
            }
        }

#pragma warning disable CA1822 // 将成员标记为 static
        private void OnCardClick(object sender, OnlineBookViewModel e)
            => LibraryViewModel.Instance.SelectOnlineSearchResultCommand.Execute(e).Subscribe();
#pragma warning restore CA1822 // 将成员标记为 static
    }
}
