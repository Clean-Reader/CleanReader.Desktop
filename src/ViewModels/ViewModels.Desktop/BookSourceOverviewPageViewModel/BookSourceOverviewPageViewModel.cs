// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.Resources;
using CleanReader.Services.Novel.Models;
using ReactiveUI;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 书源概览页面视图模型.
/// </summary>
public sealed partial class BookSourceOverviewPageViewModel : ReactiveObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookSourceOverviewPageViewModel"/> class.
    /// </summary>
    private BookSourceOverviewPageViewModel()
    {
        ServiceLocator.Instance.LoadService(out _settingsToolkit)
            .LoadService(out _fileToolkit);
        BookSources = new ObservableCollection<BookSource>();

        InitializeCommand = ReactiveCommand.Create(Initialize, outputScheduler: RxApp.MainThreadScheduler);
        ReloadCommand = ReactiveCommand.Create(Reload, outputScheduler: RxApp.MainThreadScheduler);
        OpenCommand = ReactiveCommand.CreateFromTask<BookSource>(OpenAsync, outputScheduler: RxApp.MainThreadScheduler);
        OpenInBroswerCommand = ReactiveCommand.CreateFromTask<BookSource>(OpenInBroswerAsync, outputScheduler: RxApp.MainThreadScheduler);
        DeleteCommand = ReactiveCommand.CreateFromTask<BookSource>(DeleteAsync, outputScheduler: RxApp.MainThreadScheduler);
        CreateCommand = ReactiveCommand.CreateFromTask(CreateAsync, outputScheduler: RxApp.MainThreadScheduler);

        BookSources.CollectionChanged += OnBookSourcesCollectionChanged;
    }

    private void Initialize()
    {
        BookSources.Clear();
        RootPath = Path.Combine(_settingsToolkit.ReadLocalSetting(SettingNames.LibraryPath, string.Empty), VMConstants.Library.BookSourceFolder);
        LibraryViewModel.Instance.BookSources.Where(p => p.Id != "-1")
            .ToList()
            .ForEach(p => BookSources.Add(p));
    }

    private void Reload() => LibraryViewModel.Instance.InitializeBookSourceCommand.Execute().Subscribe(_ =>
                                      {
                                          DispatcherQueue.TryEnqueue(() =>
                                          {
                                              Initialize();
                                          });
                                      });

    private async Task OpenAsync(BookSource source)
    {
        var filePath = Path.Combine(RootPath, source.Id + ".json");
        if (File.Exists(filePath))
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri($"file:///{filePath}"));
        }
    }

    private async Task OpenInBroswerAsync(BookSource souce)
        => await Windows.System.Launcher.LaunchUriAsync(new Uri(souce.WebUrl));

    private async Task DeleteAsync(BookSource source)
    {
        var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.ConfirmDialog);
        dialog.InjectData(StringResources.DeleteBookSourceWarning);
        var result = await dialog.ShowAsync();
        if (result == 0)
        {
            var filePath = Path.Combine(RootPath, source.Id + ".json");
            if (File.Exists(filePath))
            {
                await _fileToolkit.DeleteAsync(filePath);
                Reload();
            }
        }
    }

    private async Task CreateAsync()
    {
        var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.CreateBookSourceDialog);
        await dialog.ShowAsync();
    }

    private void OnBookSourcesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsShowEmpty = BookSources.Count == 0;
}
