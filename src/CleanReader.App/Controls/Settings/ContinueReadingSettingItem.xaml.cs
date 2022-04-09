// Copyright (c) Richasy. All rights reserved.

using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 继续阅读设置条目.
    /// </summary>
    public sealed partial class ContinueReadingSettingItem : UserControl
    {
        private readonly SettingsPageViewModel _viewModel = SettingsPageViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinueReadingSettingItem"/> class.
        /// </summary>
        public ContinueReadingSettingItem() => InitializeComponent();
    }
}
