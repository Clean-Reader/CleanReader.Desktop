// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Models.App;
using CleanReader.ViewModels.Desktop;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 阅读时长详情.
    /// </summary>
    public sealed partial class ReadDurationDetailDialog : CustomDialog
    {
        private ReadDurationDetailViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadDurationDetailDialog"/> class.
        /// </summary>
        public ReadDurationDetailDialog() => InitializeComponent();

        /// <inheritdoc/>
        public override void InjectData(object data)
        {
            _viewModel = new ReadDurationDetailViewModel((ReaderDuration)data);
            _viewModel.InitializeCommand.Execute().Subscribe();
        }

        /// <inheritdoc/>
        public override void OnHide()
            => _viewModel.Dispose();

        /// <inheritdoc/>
        public override void OnPrimaryButtonClick()
            => _viewModel.ReadCommand.Execute().Subscribe();
    }
}
