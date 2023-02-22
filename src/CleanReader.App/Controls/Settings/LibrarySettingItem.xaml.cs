// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 库设置.
    /// </summary>
    public sealed partial class LibrarySettingItem : UserControl
    {
        private readonly SettingsPageViewModel _viewModel = SettingsPageViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="LibrarySettingItem"/> class.
        /// </summary>
        public LibrarySettingItem() => InitializeComponent();

        private async void OnCardClickAsync(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var path = _viewModel.LibraryPath;
            await Launcher.LaunchFolderPathAsync(path);
        }
    }
}
