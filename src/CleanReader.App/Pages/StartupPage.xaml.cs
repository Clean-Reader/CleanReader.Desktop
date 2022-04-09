// Copyright (c) Richasy. All rights reserved.

using System;
using System.Windows.Forms;
using CleanReader.Locator.Lib;
using CleanReader.Models.Constants;
using CleanReader.Models.Resources;
using CleanReader.Toolkit.Interfaces;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace CleanReader.App.Pages
{
    /// <summary>
    /// 首次启动页面.
    /// </summary>
    public sealed partial class StartupPage : Page
    {
        private readonly LibraryViewModel _viewModel = LibraryViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupPage"/> class.
        /// </summary>
        public StartupPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            _viewModel.LibraryInitialized += OnLibraryInitialized;
        }

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

        private void OnLibraryInitialized(object sender, EventArgs e)
        {
            Resize(AppConstants.AppWideWidth, AppConstants.AppWideHeight);
            Frame.Navigate(typeof(MainPage));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var toolkit = ServiceLocator.Instance.GetService<ISettingsToolkit>();
            if (!string.IsNullOrEmpty(toolkit.ReadLocalSetting(SettingNames.LibraryPath, string.Empty)))
            {
                _viewModel.ExceptionMessage = StringResources.InvalidLibraryPath;
                toolkit.DeleteLocalSetting(SettingNames.LibraryPath);
            }

            Resize(AppConstants.AppMediumWidth, AppConstants.AppMediumHeight);
            VersionBlock.Text = "v." + AppViewModel.GetVersioNumber();
        }
    }
}
