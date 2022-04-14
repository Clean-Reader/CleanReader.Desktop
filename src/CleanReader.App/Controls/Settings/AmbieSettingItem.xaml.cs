// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// Ambie 集成设置.
    /// </summary>
    public sealed partial class AmbieSettingItem : UserControl
    {
        private readonly BackgroundMusicViewModel _viewModel = BackgroundMusicViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbieSettingItem"/> class.
        /// </summary>
        public AmbieSettingItem()
        {
            InitializeComponent();
            Loaded += OnLoadedAsync;
        }

        private async void OnLoadedAsync(object sender, RoutedEventArgs e)
            => await _viewModel.CheckAmbieInstalledAsync();

#pragma warning disable CA1822 // 将成员标记为 static
        private async void OnDownloadLinkClickAsync(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            var uri = new Uri("ms-windows-store://pdp/?productid=9P07XNM5CHP0");
            await Launcher.LaunchUriAsync(uri);
        }
#pragma warning restore CA1822 // 将成员标记为 static
    }
}
