// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using CleanReader.Services.Epub;
using CleanReader.Toolkit.Desktop;
using CleanReader.Toolkit.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using ReactiveUI;
using Windows.ApplicationModel;
using Windows.Graphics;
using Windows.Storage;
using WinRT.Interop;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 应用视图模型，贯穿应用生命周期始终.
    /// </summary>
    public sealed partial class AppViewModel : ReactiveObject, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppViewModel"/> class.
        /// </summary>
        private AppViewModel()
        {
            IsFullScreen = false;
            RegisterServices();
            ServiceLocator.Instance.LoadService(out _appToolkit)
                .LoadService(out _settingsToolkit);

            NavigationList = new List<NavigationItem>();

            CheckGithubUpdateCommand = ReactiveCommand.CreateFromTask(CheckGithubUpdateAsync, outputScheduler: RxApp.MainThreadScheduler);

            _mainWindowSubscription = this.WhenAnyValue(x => x.MainWindow)
                .WhereNotNull()
                .Subscribe(w => InitializeMainWindow());

            this.WhenAnyValue(x => x.IsFullScreen, x => x.IsMiniView)
                .Subscribe(i =>
                {
                    if (i.Item1)
                    {
                        EnterFullScreen();
                        IsMiniView = false;
                    }
                    else if (i.Item2)
                    {
                        EnterMiniView();
                        IsFullScreen = false;
                    }
                    else
                    {
                        EnterDefaultView();
                    }
                });

            EpubService.RootPath = ApplicationData.Current.LocalFolder.Path;
            EpubService.PackagePath = Package.Current.InstalledPath;
        }

        /// <summary>
        /// 获取当前版本号.
        /// </summary>
        /// <returns>版本号.</returns>
        public static string GetVersioNumber()
        {
            var version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        /// <summary>
        /// 设置主窗口.
        /// </summary>
        /// <param name="mainWindow">主窗口对象.</param>
        public void SetMainWindow(Window mainWindow)
            => MainWindow = mainWindow;

        /// <summary>
        /// 请求导航至某页面.
        /// </summary>
        /// <param name="pageType">页面类型.</param>
        /// <param name="parameter">导航附加参数.</param>
        public void RequestNavigateTo(Type pageType, object parameter = null)
            => NavigationRequested?.Invoke(this, new NavigationEventArgs(pageType, parameter));

        /// <summary>
        /// 请求导航至某页面.
        /// </summary>
        /// <param name="navItem">导航条目.</param>
        /// <param name="parameter">导航附加参数.</param>
        public void RequestNavigateTo(NavigationItem navItem, object parameter = null)
            => NavigationRequested?.Invoke(this, new NavigationEventArgs(navItem, parameter));

        /// <summary>
        /// 请求导航到初始页面.
        /// </summary>
        public void RequestStartup()
        {
            IsFullScreen = false;
            IsMiniView = false;
            NavigationList.Clear();
            IsMaskShown = false;
            _settingsToolkit.DeleteLocalSetting(SettingNames.LastReadBookId);
            _settingsToolkit.DeleteLocalSetting(SettingNames.LibraryPath);
            StartupRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 请求导航到迁移页面.
        /// </summary>
        /// <param name="result">迁移检查结果.</param>
        public void RequestMigration(MigrationResult result)
        {
            IsFullScreen = false;
            IsMiniView = false;
            NavigationList.Clear();
            IsMaskShown = false;
            _settingsToolkit.DeleteLocalSetting(SettingNames.LastReadBookId);
            MigrationRequested?.Invoke(this, result);
        }

        /// <summary>
        /// 请求阅读某本书.
        /// </summary>
        /// <param name="book">书籍.</param>
        /// <param name="startCfi">起始位置.</param>
        public void RequestRead(Book book, string startCfi = null)
        {
            if (book == null)
            {
                IsFullScreen = false;
                IsMiniView = false;
                _appToolkit.InitializeTitleBar(AppWindow.TitleBar);
            }

            ReadRequested?.Invoke(this, new ReadRequestEventArgs(book, startCfi));
        }

        /// <summary>
        /// 是否应该显示启动引导页面.
        /// </summary>
        /// <returns><c>true</c> 表示应该显示.</returns>
        public bool ShouldShowStartup()
        {
            var localLibPath = _settingsToolkit.ReadLocalSetting(SettingNames.LibraryPath, string.Empty);
            return string.IsNullOrEmpty(localLibPath) || !Directory.Exists(localLibPath);
        }

        /// <summary>
        /// 显示提示.
        /// </summary>
        /// <param name="message">消息内容.</param>
        /// <param name="type">消息类型.</param>
        public void ShowTip(string message, InfoType type = InfoType.Information)
            => RequestShowTip?.Invoke(this, new AppTipNotificationEventArgs(message, type));

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private static void RegisterServices()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton<ISettingsToolkit, SettingsToolkit>()
                .AddSingleton<IAppToolkit, AppToolkit>()
                .AddSingleton<IResourceToolkit, ResourceToolkit>()
                .AddSingleton<IFileToolkit, FileToolkit>()
                .AddSingleton<IFontToolkit, FontToolkit>()
                .AddSingleton<ILoggerToolkit, LoggerToolkit>();

            _ = new ServiceLocator((ServiceCollection)serviceCollection);
        }

        /// <summary>
        /// 初始化主窗口内容.
        /// </summary>
        /// <param name="mainWindow">主窗口.</param>
        private void InitializeMainWindow()
        {
            MainWindowHandle = WindowNative.GetWindowHandle(MainWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(MainWindowHandle);
            AppWindow = AppWindow.GetFromWindowId(windowId);
            AppWindow.Closing += OnAppWindowClosing;
            AppWindow.Title = StringResources.AppName;
            var path = Path.Combine(Package.Current.InstalledPath, "Assets/favicon.ico");
            AppWindow.SetIcon(path);
            _appToolkit.InitializeTitleBar(AppWindow.TitleBar);
            RestoreWindowSizeAndPosition();
            DispatcherQueue = MainWindow.DispatcherQueue;
        }

        private void WriteWindowSizeAndPosition()
        {
            var width = _appToolkit.GetNormalizePixel(AppWindow.Size.Width, MainWindowHandle);
            var height = _appToolkit.GetNormalizePixel(AppWindow.Size.Height, MainWindowHandle);
            var left = _appToolkit.GetNormalizePixel(AppWindow.Position.X, MainWindowHandle);
            var top = _appToolkit.GetNormalizePixel(AppWindow.Position.Y, MainWindowHandle);
            _settingsToolkit.WriteLocalSetting(SettingNames.WindowWidth, width);
            _settingsToolkit.WriteLocalSetting(SettingNames.WindowHeight, height);
            _settingsToolkit.WriteLocalSetting(SettingNames.WindowLeft, left);
            _settingsToolkit.WriteLocalSetting(SettingNames.WindowTop, top);
        }

        private void RestoreWindowSizeAndPosition()
        {
            var width = _appToolkit.GetScalePixel(_settingsToolkit.ReadLocalSetting(SettingNames.WindowWidth, 0), MainWindowHandle);

            if (width > 0)
            {
                var height = _appToolkit.GetScalePixel(_settingsToolkit.ReadLocalSetting(SettingNames.WindowHeight, 0), MainWindowHandle);
                var left = _appToolkit.GetScalePixel(_settingsToolkit.ReadLocalSetting(SettingNames.WindowLeft, 0), MainWindowHandle);
                var top = _appToolkit.GetScalePixel(_settingsToolkit.ReadLocalSetting(SettingNames.WindowTop, 0), MainWindowHandle);
                AppWindow.Resize(new SizeInt32(width, height));
                AppWindow.Move(new PointInt32(left, top));
            }
            else
            {
                var minWidth = _appToolkit.GetScalePixel(AppConstants.AppWideWidth, MainWindowHandle);
                var minHeight = _appToolkit.GetScalePixel(AppConstants.AppWideHeight, MainWindowHandle);
                var screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
                var screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

                if (minWidth > screenWidth)
                {
                    minWidth = screenWidth - 40;
                }

                if (minHeight > screenHeight)
                {
                    minHeight = screenHeight - 40;
                }

                AppWindow.Resize(new SizeInt32(minWidth, minHeight));
            }
        }

        private void OnAppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            WriteWindowSizeAndPosition();
            Dispose();
        }

        private void EnterFullScreen()
        {
            if (AppWindow != null && AppWindow.Presenter.Kind != AppWindowPresenterKind.FullScreen)
            {
                AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            }
        }

        private void EnterDefaultView()
        {
            if (AppWindow != null && AppWindow.Presenter.Kind != AppWindowPresenterKind.Default)
            {
                AppWindow.SetPresenter(AppWindowPresenterKind.Default);
            }
        }

        private void EnterMiniView()
        {
            if (AppWindow != null && AppWindow.Presenter.Kind != AppWindowPresenterKind.CompactOverlay)
            {
                AppWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _mainWindowSubscription?.Dispose();
                }

                _disposedValue = true;
            }
        }

        private async Task CheckGithubUpdateAsync()
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.107 Safari/537.36 Edg/92.0.902.62");
            var request = new HttpRequestMessage(HttpMethod.Get, LatestReleaseUrl);
            var response = await httpClient.SendAsync(request, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<GithubReleaseResponse>(content);

            if (string.IsNullOrEmpty(data?.Url))
            {
                return;
            }

            var currentVersion = GetVersioNumber();
            var ignoreVersion = _settingsToolkit.ReadLocalSetting(SettingNames.IgnoreVersion, string.Empty);
            var version = data.TagName.Replace("v", string.Empty).Replace(".pre-release", string.Empty);
            if (version != currentVersion && version != ignoreVersion)
            {
                // Show update dialog.
                var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.GithubUpdateDialog);
                dialog.InjectData(data);
                await dialog.ShowAsync();
            }
        }
    }
}
