// Copyright (c) Richasy. All rights reserved.

using CleanReader.ViewModels.Desktop;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 书籍导入方式对话框.
    /// </summary>
    public sealed partial class ImportWayDialog : CustomDialog
    {
        private readonly LibraryViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportWayDialog"/> class.
        /// </summary>
        public ImportWayDialog()
        {
            _viewModel = LibraryViewModel.Instance;
            InitializeComponent();
        }
    }
}
