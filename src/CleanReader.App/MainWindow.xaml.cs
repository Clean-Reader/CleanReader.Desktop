// Copyright (c) Richasy. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CleanReader.App.Controls;
using CleanReader.App.Pages;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Toolkit.Interfaces;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using WinRT;
using WinRT.Interop;

namespace CleanReader.App;

#pragma warning disable CA1060

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly AppViewModel _viewModel;
    private readonly WindowsSystemDispatcherQueueHelper _wsdqHelper;
    private WinProc _newWndProc = null;
    private IntPtr _oldWndProc = IntPtr.Zero;
    private MicaController _micaController;
    private SystemBackdropConfiguration _configurationSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        _wsdqHelper = new WindowsSystemDispatcherQueueHelper();
        _wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

        _viewModel = AppViewModel.Instance;
        _viewModel.ReadRequested += OnReadRequested;
        _viewModel.StartupRequested += OnStartupRequested;
        _viewModel.MigrationRequested += OnMigrationRequested;
        _viewModel.RequestShowTip += OnRequestShowTip;
        SubClassing();
        TrySetMicaBackdrop();
    }

    private delegate IntPtr WinProc(IntPtr hWnd, PInvoke.User32.WindowMessage msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// 显示顶层视图.
    /// </summary>
    /// <param name="element">要显示的元素.</param>
    public void ShowOnHolder(UIElement element)
    {
        if (!HolderContainer.Children.Contains(element))
        {
            HolderContainer.Children.Add(element);
        }

        AppViewModel.Instance.IsMaskShown = true;
        HolderContainer.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// 从顶层视图中移除元素.
    /// </summary>
    /// <param name="element">UI元素.</param>
    public void RemoveFromHolder(UIElement element)
    {
        HolderContainer.Children.Remove(element);
        if (HolderContainer.Children.Count == 0)
        {
            AppViewModel.Instance.IsMaskShown = false;
        }
    }

    [DllImport("user32.dll")]
    internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, PInvoke.User32.WindowMessage msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, PInvoke.User32.WindowLongIndexFlags nIndex, WinProc newProc);

    private void SubClassing()
    {
        var windowHandle = WindowNative.GetWindowHandle(this);
        _newWndProc = new WinProc(NewWindowProc);
        _oldWndProc = SetWindowLongPtr(windowHandle, PInvoke.User32.WindowLongIndexFlags.GWL_WNDPROC, _newWndProc);
    }

    private IntPtr NewWindowProc(IntPtr hWnd, PInvoke.User32.WindowMessage msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case PInvoke.User32.WindowMessage.WM_GETMINMAXINFO:
                {
                    var getActualPixel = ServiceLocator.Instance.GetService<IAppToolkit>().GetScalePixel;
                    var minMaxInfo = Marshal.PtrToStructure<PInvoke.User32.MINMAXINFO>(lParam);

                    if (!AppViewModel.Instance.IsMiniView)
                    {
                        var screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
                        var screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

                        var width = screenWidth < AppConstants.AppNarrowWidth ? screenWidth - 40 : AppConstants.AppNarrowWidth;
                        var height = screenHeight < AppConstants.AppNarrowHeight ? screenHeight - 40 : AppConstants.AppNarrowHeight;
                        minMaxInfo.ptMinTrackSize.x = getActualPixel(width, hWnd);
                        minMaxInfo.ptMinTrackSize.y = getActualPixel(height, hWnd);
                    }
                    else
                    {
                        minMaxInfo.ptMinTrackSize.x = getActualPixel(AppConstants.AppMiniMinWidth, hWnd);
                        minMaxInfo.ptMinTrackSize.y = getActualPixel(AppConstants.AppMiniMinHeight, hWnd);
                    }

                    Marshal.StructureToPtr(minMaxInfo, lParam, true);
                    break;
                }
        }

        return CallWindowProc(_oldWndProc, hWnd, msg, wParam, lParam);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (AppViewModel.Instance.ShouldShowStartup())
        {
            Frame.Navigate(typeof(StartupPage));
        }
        else
        {
            Frame.Navigate(typeof(MainPage));
        }
    }

    private void OnStartupRequested(object sender, EventArgs e)
    {
        LibraryViewModel.Instance.ClearCommand.Execute().Subscribe();
        Frame.Navigate(typeof(StartupPage));
    }

    private void OnMigrationRequested(object sender, MigrationResult e)
        => Frame.Navigate(typeof(MigrationPage), e.ToString(), new DrillInNavigationTransitionInfo());

    private void OnReadRequested(object sender, ReadRequestEventArgs e)
    {
        if (e.Book != null)
        {
            TitleBar.Visibility = Frame.Visibility = Visibility.Collapsed;
            Frame.Visibility = Visibility.Collapsed;
            ReaderFrame.Visibility = Visibility.Visible;
            ReaderFrame.Navigate(typeof(ReaderPage), e);
        }
        else
        {
            TitleBar.Visibility = Frame.Visibility = Visibility.Visible;
            ReaderFrame.Visibility = Visibility.Collapsed;
            Frame.Visibility = Visibility.Visible;
            ReaderFrame.Navigate(typeof(Page));
            if (ShelfPageViewModel.Instance.CurrentShelf != null)
            {
                ShelfPageViewModel.Instance.InitializeBooksCommand.Execute().Subscribe();
            }
        }
    }

    private void OnRequestShowTip(object sender, AppTipNotificationEventArgs e)
        => new TipPopup(e.Message).ShowAsync(e.Type);

    private bool TrySetMicaBackdrop()
    {
        if (MicaController.IsSupported())
        {
            // Hooking up the policy object
            _configurationSource = new SystemBackdropConfiguration();
            this.Activated += OnActivated;
            this.Closed += OnClosed;
            ((FrameworkElement)this.Content).ActualThemeChanged += OnThemeChanged;

            // Initial configuration state.
            _configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme();

            _micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

            // Enable the system backdrop.
            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
            _micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            _micaController.SetSystemBackdropConfiguration(_configurationSource);
            return true; // succeeded
        }

        return false; // Mica is not supported on this system
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
        => _configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;

    private void OnClosed(object sender, WindowEventArgs args)
    {
        // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
        // use this closed window.
        if (_micaController != null)
        {
            _micaController.Dispose();
            _micaController = null;
        }

        Activated -= OnActivated;
        _configurationSource = null;
    }

    private void OnThemeChanged(FrameworkElement sender, object args)
    {
        if (_configurationSource != null)
        {
            SetConfigurationSourceTheme();
        }
    }

    private void SetConfigurationSourceTheme()
    {
        switch (((FrameworkElement)Content).ActualTheme)
        {
            case ElementTheme.Dark:
                _configurationSource.Theme = SystemBackdropTheme.Dark;
                break;
            case ElementTheme.Light:
                _configurationSource.Theme = SystemBackdropTheme.Light;
                break;
            case ElementTheme.Default:
                _configurationSource.Theme = SystemBackdropTheme.Default;
                break;
        }
    }
}
