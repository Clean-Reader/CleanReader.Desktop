// Copyright (c) Richasy. All rights reserved.
using System;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using CleanReader.ViewModels.Desktop;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 创建或更新书架对话框.
    /// </summary>
    public sealed partial class CreateOrUpdateShelfDialog : CustomDialog
    {
        private readonly LibraryViewModel _libraryViewModel = LibraryViewModel.Instance;
        private Shelf _shelf;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateOrUpdateShelfDialog"/> class.
        /// </summary>
        public CreateOrUpdateShelfDialog() => InitializeComponent();

        /// <inheritdoc/>
        public override void InjectData(object data)
        {
            if (data == null)
            {
                Title = StringResources.CreateNewShelf;
                Subtitle = StringResources.CreateNewShelfTip;
            }
            else if (data is Shelf shelf)
            {
                _shelf = shelf;
                ShelfBox.Text = shelf.Name;
                Title = StringResources.UpdateShelf;
                Subtitle = StringResources.UpdateShelfTip;
            }
        }

        /// <inheritdoc/>
        public override void OnPrimaryButtonClick()
        {
            if (string.IsNullOrEmpty(ShelfBox.Text?.Trim()))
            {
                return;
            }

            var name = ShelfBox.Text;
            if (_shelf != null)
            {
                // 更新书架.
                _shelf.Name = name;
                _libraryViewModel.UpdateShelfCommand.Execute(_shelf).Subscribe();
            }
            else
            {
                // 创建书架.
                _libraryViewModel.CreateShelfCommand.Execute(name).Subscribe();
            }
        }

        private void OnTextChanged(object sender, Microsoft.UI.Xaml.Controls.TextChangedEventArgs e)
            => IsPrimaryButtonEnabled = !string.IsNullOrEmpty(ShelfBox.Text);
    }
}
