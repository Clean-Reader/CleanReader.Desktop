// Copyright (c) Richasy. All rights reserved.

using System;
using System.Threading.Tasks;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using ReactiveUI;
using Windows.System;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 设置页面视图模型.
    /// </summary>
    public sealed partial class SettingsPageViewModel : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPageViewModel"/> class.
        /// </summary>
        private SettingsPageViewModel()
        {
            ServiceLocator.Instance.LoadService(out _settingsToolkit);

            InitializeCommand = ReactiveCommand.Create(Initialize, outputScheduler: RxApp.MainThreadScheduler);
            CloseLibraryCommand = ReactiveCommand.Create(CloseLibrary, outputScheduler: RxApp.MainThreadScheduler);
            OpenLibraryCommand = ReactiveCommand.CreateFromTask(OpenLibraryAsync, outputScheduler: RxApp.MainThreadScheduler);
            ShowCreateOrUpdateShelfDialogCommand = ReactiveCommand.CreateFromTask<Shelf>(ShowCreateOrUpdateShelfDialogAsync, outputScheduler: RxApp.MainThreadScheduler);

            Initialize();

            this.WhenAnyValue(x => x.SpreadMinWidth)
                .WhereNotNull()
                .Subscribe(x => _settingsToolkit.WriteLocalSetting(SettingNames.SpreadMinWidth, x));

            this.WhenAnyValue(x => x.IsContinueReading)
                .WhereNotNull()
                .Subscribe(x => _settingsToolkit.WriteLocalSetting(SettingNames.IsContinueReading, x));

            this.WhenAnyValue(x => x.Theme)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    _settingsToolkit.WriteLocalSetting(SettingNames.AppTheme, x);
                    IsShowThemeRestartTip = _initializeTheme != x;
                });

            this.WhenAnyValue(x => x.LanguageCode)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    if (x != _settingsToolkit.ReadLocalSetting(SettingNames.AppLanguage, string.Empty))
                    {
                        _settingsToolkit.WriteLocalSetting(SettingNames.AppLanguage, x);
                        AppViewModel.Instance.ShowTip(StringResources.LanguageRestartTip, InfoType.Success);
                    }
                });
        }

        private void Initialize()
        {
            Version = AppViewModel.GetVersioNumber();
            LanguageCode = _settingsToolkit.ReadLocalSetting(SettingNames.AppLanguage, "zh-CN");
            LibraryPath = _settingsToolkit.ReadLocalSetting(SettingNames.LibraryPath, string.Empty);
            SpreadMinWidth = _settingsToolkit.ReadLocalSetting(SettingNames.SpreadMinWidth, 1000d);
            _initializeTheme = Theme = _settingsToolkit.ReadLocalSetting(SettingNames.AppTheme, AppConstants.ThemeDefault);
            IsContinueReading = _settingsToolkit.ReadLocalSetting(SettingNames.IsContinueReading, false);
        }

        private async Task OpenLibraryAsync()
        {
            if (!string.IsNullOrEmpty(LibraryPath))
            {
                await Launcher.LaunchFolderPathAsync(LibraryPath).AsTask();
            }
        }

        private async Task ShowCreateOrUpdateShelfDialogAsync(Shelf shelf)
        {
            var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.CreateOrUpdateShelfDialog);
            dialog.InjectData(shelf);
            await dialog.ShowAsync();
        }

        private void CloseLibrary()
            => AppViewModel.Instance.RequestStartup();
    }
}
