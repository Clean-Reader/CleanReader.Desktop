// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Models.DataBase;
using CleanReader.ViewModels.Desktop;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 书架转移对话框.
    /// </summary>
    public sealed partial class ShelfTransferDialog : CustomDialog
    {
        private readonly LibraryViewModel _viewModel = LibraryViewModel.Instance;
        private readonly ShelfPageViewModel _shelfPageViewModel = ShelfPageViewModel.Instance;
        private Book _book;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShelfTransferDialog"/> class.
        /// </summary>
        public ShelfTransferDialog() => InitializeComponent();

        /// <inheritdoc/>
        public override async void InjectData(object data)
        {
            _book = data as Book;
            var shelf = await _viewModel.GetSourceShelfAsync(_book);
            if (shelf != null)
            {
                ShelfView.SelectedItem = shelf;
            }
            else
            {
                ShelfView.SelectedIndex = 0;
            }
        }

        /// <inheritdoc/>
        public override async void OnPrimaryButtonClick()
        {
            if (ShelfView.SelectedItem is Shelf shelf)
            {
                await _shelfPageViewModel.TransferShelfAsync(_book, shelf);
            }
        }

        private void OnShelfNameBoxKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && !string.IsNullOrEmpty(ShelfNameBox.Text?.Trim()))
            {
                // 创建新书架.
                _viewModel.CreateShelfCommand.Execute(ShelfNameBox.Text.Trim()).Subscribe();
            }
        }

        private void OnShelfViewSelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
            => IsPrimaryButtonEnabled = ShelfView.SelectedItem != null;
    }
}
