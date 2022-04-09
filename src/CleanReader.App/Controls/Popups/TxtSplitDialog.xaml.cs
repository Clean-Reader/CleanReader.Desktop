// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.ViewModels.Desktop;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// TXT 小说分章对话框.
    /// </summary>
    public sealed partial class TxtSplitDialog : CustomDialog
    {
        private readonly LibraryViewModel _viewModel = LibraryViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="TxtSplitDialog"/> class.
        /// </summary>
        public TxtSplitDialog() => InitializeComponent();

        private void OnRegexBoxQuerySubmitted(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, Microsoft.UI.Xaml.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
            => _viewModel.SplitChapterCommand.Execute(sender.Text).Subscribe();

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
            => _viewModel.SplitChapterCommand.Execute(null).Subscribe();
    }
}
