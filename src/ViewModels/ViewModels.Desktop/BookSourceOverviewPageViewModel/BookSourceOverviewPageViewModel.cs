// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CleanReader.Controls.Interfaces;
using CleanReader.Models.Constants;
using CleanReader.Models.Resources;
using CleanReader.Models.Services;
using CleanReader.Toolkit.Interfaces;
using CleanReader.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 书源概览页面视图模型.
/// </summary>
public sealed partial class BookSourceOverviewPageViewModel : ViewModelBase, IBookSourceOverviewPageViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookSourceOverviewPageViewModel"/> class.
    /// </summary>
    private BookSourceOverviewPageViewModel(
        ISettingsToolkit settingsToolkit,
        IFileToolkit fileToolkit)
    {
        _settingsToolkit = settingsToolkit;
        _fileToolkit = fileToolkit;
        BookSources = new ObservableCollection<BookSource>();
        BookSources.CollectionChanged += OnBookSourcesCollectionChanged;
    }

    [RelayCommand]
    private void Initialize()
    {
        BookSources.Clear();
        RootPath = Path.Combine(_settingsToolkit.ReadLocalSetting(SettingNames.LibraryPath, string.Empty), VMConstants.Library.BookSourceFolder);
        LibraryViewModel.Instance.BookSources.Where(p => p.Id != "-1")
            .ToList()
            .ForEach(p => BookSources.Add(p));
    }

    [RelayCommand]
    private void Reload()
    {
        LibraryViewModel.Instance.InitializeBookSourceCommand.Execute().Subscribe(_ =>
                                      {
                                          DispatcherQueue.TryEnqueue(() =>
                                          {
                                              Initialize();
                                          });
                                      });
    }

    [RelayCommand]
    private async Task OpenAsync(BookSource source)
    {
        var filePath = Path.Combine(RootPath, source.Id + ".json");
        if (File.Exists(filePath))
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri($"file:///{filePath}"));
        }
    }

    [RelayCommand]
    private async Task OpenInBroswerAsync(BookSource souce)
        => await Windows.System.Launcher.LaunchUriAsync(new Uri(souce.WebUrl));

    [RelayCommand]
    private async Task DeleteAsync(BookSource source)
    {
        var dialog = Locator.Lib.Locator.Instance.GetService<IConfirmDialog>();
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
        var dialog = Locator.Lib.Locator.Instance.GetService<ICreateBookSourceDialog>();
        await dialog.ShowAsync();
    }

    private void OnBookSourcesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsShowEmpty = BookSources.Count == 0;
}
