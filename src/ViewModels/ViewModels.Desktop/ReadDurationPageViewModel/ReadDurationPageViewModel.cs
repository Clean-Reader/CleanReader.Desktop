// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 阅读时长页面视图模型.
/// </summary>
public sealed partial class ReadDurationPageViewModel : ReactiveObject, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadDurationPageViewModel"/> class.
    /// </summary>
    public ReadDurationPageViewModel()
    {
        ReaderDurations = new ObservableCollection<ReaderDuration>();
        InitializeCommand = ReactiveCommand.CreateFromTask(InitializeAsync, outputScheduler: RxApp.MainThreadScheduler);
        ShowDetailCommand = ReactiveCommand.CreateFromTask<ReaderDuration>(ShowDetailDialogAsync, outputScheduler: RxApp.MainThreadScheduler);

        _isInitializing = InitializeCommand.IsExecuting.ToProperty(this, x => x.IsInitializing, scheduler: RxApp.MainThreadScheduler);

        ReaderDurations.CollectionChanged += OnReaderDurationsCollectionChanged;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private async Task InitializeAsync()
    {
        if (LibraryViewModel.Instance.IsFileSystemLimited)
        {
            return;
        }

        _dbContext = LibraryViewModel.Instance.LibraryContext;
        ReaderDurations.Clear();

        List<History> histories = null;

        await Task.Run(async () =>
        {
            await HistoriesSelfCheckAsync();
            histories = await _dbContext.Histories.Include(p => p.ReadSections).ToListAsync();
        });

        if (histories.Count > 0)
        {
            histories = histories.OrderByDescending(p => p.ReadDuration).ToList();
            var totalDuration = TimeSpan.FromSeconds(histories.Sum(p => p.ReadDuration));
            TotalReadHours = Math.Round(totalDuration.TotalHours, 2);
            foreach (var history in histories)
            {
                Book book = null;
                await Task.Run(async () =>
                {
                    book = await _dbContext.Books.FirstOrDefaultAsync(p => p.Id == history.BookId);
                });
                if (book != null)
                {
                    var durationItem = new ReaderDuration()
                    {
                        Book = book,
                        TotalDuration = TimeSpan.FromSeconds(history.ReadDuration),
                        Percentage = Math.Round(history.ReadDuration / totalDuration.TotalSeconds, 2),
                    };

                    ReaderDurations.Add(durationItem);
                }
            }

            await Task.Run(async () =>
            {
                _dbContext.Histories.UpdateRange(histories);
                await _dbContext.SaveChangesAsync();
            });
        }
    }

    private async Task HistoriesSelfCheckAsync()
    {
        var hasChange = false;
        var histories = await _dbContext.Histories.Include(p => p.ReadSections).ToListAsync();
        foreach (var history in histories)
        {
            var count = history.ReadSections.RemoveAll(p => p.EndTime - p.StartTime > TimeSpan.FromDays(3));
            history.ReadDuration = history.ReadSections.Sum(p => (p.EndTime - p.StartTime).TotalSeconds);

            if (count > 0)
            {
                hasChange = true;
            }
        }

        if (hasChange)
        {
            _dbContext.Histories.UpdateRange(histories);
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task ShowDetailDialogAsync(ReaderDuration duration)
    {
        var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.ReadDurationDetailDialog);
        dialog.InjectData(duration);
        await dialog.ShowAsync();
    }

    private void OnReaderDurationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsEmptyShown = ReaderDurations.Count == 0;

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _isInitializing?.Dispose();
            }

            _disposedValue = true;
        }
    }
}
