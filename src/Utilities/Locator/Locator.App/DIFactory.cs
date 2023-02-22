// Copyright (c) Richasy. All rights reserved.

using CleanReader.Controls;
using CleanReader.Controls.Interfaces;
using CleanReader.Services.Epub;
using CleanReader.Services.Interfaces;
using CleanReader.Services.Novel;
using CleanReader.Toolkit.Desktop;
using CleanReader.Toolkit.Interfaces;
using CleanReader.ViewModels.Desktop;
using CleanReader.ViewModels.Interfaces;

namespace CleanReader.Locator.App;

/// <summary>
/// 依赖注入工厂.
/// </summary>
public static class DIFactory
{
    /// <summary>
    /// 注入应用所需的依赖.
    /// </summary>
    public static void RegisterAppRequiredServices()
    {
        Lib.Locator.Instance
            .RegisterSingleton<ISettingsToolkit, SettingsToolkit>()
            .RegisterSingleton<IAppToolkit, AppToolkit>()
            .RegisterSingleton<IResourceToolkit, ResourceToolkit>()
            .RegisterSingleton<IFileToolkit, FileToolkit>()
            .RegisterSingleton<IFontToolkit, FontToolkit>()
            .RegisterSingleton<ILoggerToolkit, LoggerToolkit>()

            .RegisterSingleton<INovelService, NovelService>()
            .RegisterSingleton<IEpubService, EpubService>()

            .RegisterSingleton<IAppViewModel, AppViewModel>()
            .RegisterSingleton<IBackgroundMusicViewModel, BackgroundMusicViewModel>()

            .RegisterTransient<IBookInformationDialog, BookInformationDialog>()
            .RegisterTransient<IConfirmDialog, ConfirmDialog>()
            .RegisterTransient<ICreateBookSourceDialog, CreateBookSourceDialog>()
            .RegisterTransient<ICreateOrUpdateShelfDialog, CreateOrUpdateShelfDialog>()
            .RegisterTransient<IGithubUpdateDialog, GithubUpdateDialog>()
            .RegisterTransient<IImportWayDialog, ImportWayDialog>()
            .RegisterTransient<IInternalSearchDialog, InternalSearchDialog>()
            .RegisterTransient<IOnlineSearchDialog, OnlineSearchDialog>()
            .RegisterTransient<IProgressDialog, ProgressDialog>()
            .RegisterTransient<IReadDurationDetailDialog, ReadDurationDetailDialog>()
            .RegisterTransient<IReaderHighlightDialog, ReaderHighlightDialog>()
            .RegisterTransient<IReaderStyleOptionsDialog, ReaderStyleOptionsDialog>()
            .RegisterTransient<IReplaceSourceDialog, ReplaceSourceDialog>()
            .RegisterTransient<IShelfTransferDialog, ShelfTransferDialog>()
            .RegisterTransient<ITxtSplitDialog, TxtSplitDialog>()
            .Build();
    }
}
