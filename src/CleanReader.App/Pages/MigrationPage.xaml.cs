// Copyright (c) Richasy. All rights reserved.

using System;
using System.Windows.Forms;
using CleanReader.Locator.Lib;
using CleanReader.Models.Constants;
using CleanReader.Toolkit.Interfaces;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.System;

namespace CleanReader.App.Pages
{
    /// <summary>
    /// 迁移页面.
    /// </summary>
    public sealed partial class MigrationPage : Page
    {
        private readonly LibraryViewModel _viewModel = LibraryViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationPage"/> class.
        /// </summary>
        public MigrationPage() => InitializeComponent();

        /// <inheritdoc/>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string result)
            {
                _viewModel.Migrated += OnMigrated;
                if (result == MigrationResult.ShouldUpdateApp.ToString())
                {
                    AppUpdateContainer.Visibility = Visibility.Visible;
                }
                else if (result == MigrationResult.ShouldUpdateDataBase.ToString())
                {
                    MigrationContainer.Visibility = Visibility.Visible;
                    await _viewModel.MigrationAsync();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => _viewModel.Migrated -= OnMigrated;

        private static void Resize(double width, double height)
        {
            var f = ServiceLocator.Instance.GetService<IAppToolkit>().GetScalePixel;
            var h = AppViewModel.Instance.MainWindowHandle;
            var aW = f(width, h);
            var aH = f(height, h);
            AppViewModel.Instance.AppWindow.Resize(new Windows.Graphics.SizeInt32(aW, aH));
            var screen = Screen.FromHandle(AppViewModel.Instance.MainWindowHandle).Bounds;
            var pt = new Point(screen.Left + (screen.Width / 2) - (aW / 2), screen.Top + (screen.Height / 2) - (aH / 2));
            PInvoke.User32.SetWindowPos(AppViewModel.Instance.MainWindowHandle, PInvoke.User32.SpecialWindowHandles.HWND_NOTOPMOST, Convert.ToInt32(pt.X), Convert.ToInt32(pt.Y), aW, aH, PInvoke.User32.SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        private async void OnOpenStoreButtonClickAsync(object sender, RoutedEventArgs e)
            => await Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/?productid=9MV65L2XFCSK"));

        private void OnLoaded(object sender, RoutedEventArgs e)
            => Resize(AppConstants.AppMediumWidth, AppConstants.AppMediumHeight);

        private void OnReselectButtonClick(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearCommand.Execute().Subscribe();
            AppViewModel.Instance.RequestStartup();
        }

        private void OnMigrated(object sender, EventArgs e)
        {
            Resize(AppConstants.AppWideWidth, AppConstants.AppWideHeight);
            Frame.Navigate(typeof(MainPage));
        }
    }
}
