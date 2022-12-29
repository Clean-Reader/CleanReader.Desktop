// Copyright (c) Richasy. All rights reserved.

using System.Globalization;
using System.Linq;
using System.Text;
using CleanReader.App.Controls;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Toolkit.Interfaces;
using CleanReader.ViewModels.Desktop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.Storage;

namespace CleanReader.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private Window _mainWindow;
    private DispatcherQueue _dispatcherQueue;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _ = AppViewModel.Instance;
        ServiceLocator.Instance.GetService<IAppToolkit>()
            .InitializeTheme();
        InitializeViewService();
        var sysLan = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("zh") ? "zh-CN" : "en-US";
        var customLan = ServiceLocator.Instance.GetService<ISettingsToolkit>().ReadLocalSetting(Models.Constants.SettingNames.AppLanguage, sysLan);
        ApplicationLanguages.PrimaryLanguageOverride = customLan;
        UnhandledException += OnUnhandledException;
    }

    /// <summary>
    /// 激活窗口.
    /// </summary>
    public void ActivateWindow(AppActivationArguments activationArguments)
        => _dispatcherQueue.TryEnqueue(() =>
        {
            _mainWindow?.Activate();
        });

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _mainWindow = new MainWindow();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        var activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        if (activatedEventArgs.Kind == ExtendedActivationKind.File)
        {
            var fileArgs = activatedEventArgs.Data as FileActivatedEventArgs;
            var file = fileArgs.Files.FirstOrDefault();
            if (file is StorageFile f)
            {
                AppViewModel.Instance.InitializeFilePath = f.Path;
            }
        }

        AppViewModel.Instance.SetMainWindow(_mainWindow);
        _mainWindow.Activate();
    }

    private static void InitializeViewService() => ServiceLocator.Instance.ServiceCollection.AddTransient<ICustomDialog, ImportWayDialog>()
            .AddTransient<ICustomDialog, TxtSplitDialog>()
            .AddTransient<ICustomDialog, ProgressDialog>()
            .AddTransient<ICustomDialog, OnlineSearchDialog>()
            .AddTransient<ICustomDialog, ConfirmDialog>()
            .AddTransient<ICustomDialog, BookInformationDialog>()
            .AddTransient<ICustomDialog, ReaderStyleOptionsDialog>()
            .AddTransient<ICustomDialog, ReaderHighlightDialog>()
            .AddTransient<ICustomDialog, InternalSearchDialog>()
            .AddTransient<ICustomDialog, ShelfTransferDialog>()
            .AddTransient<ICustomDialog, CreateOrUpdateShelfDialog>()
            .AddTransient<ICustomDialog, CreateBookSourceDialog>()
            .AddTransient<ICustomDialog, ReplaceSourceDialog>()
            .AddTransient<ICustomDialog, GithubUpdateDialog>()
            .AddTransient<ICustomDialog, ReadDurationDetailDialog>()
            .RebuildServiceProvider();

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) => e.Handled = true;
}
