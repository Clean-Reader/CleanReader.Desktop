// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using CleanReader.Models.App;
using CleanReader.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 阅读时长详情视图模型.
/// </summary>
public sealed class ReadDurationDetailViewModel : ReactiveObject, IDisposable
{
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadDurationDetailViewModel"/> class.
    /// </summary>
    /// <param name="readerDuration">阅读时长.</param>
    public ReadDurationDetailViewModel(ReaderDuration readerDuration)
    {
        Data = readerDuration;
        ReadSectionCollection = new ObservableCollection<ReadSection>();
        ReadCommand = ReactiveCommand.Create(ReadBook, outputScheduler: RxApp.MainThreadScheduler);
        InitializeCommand = ReactiveCommand.CreateFromTask(InitializeAsync, outputScheduler: RxApp.MainThreadScheduler);

        _isLoading = InitializeCommand.IsExecuting.ToProperty(this, x => x.IsLoading, scheduler: RxApp.MainThreadScheduler);
    }

    /// <summary>
    /// 阅读此书的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ReadCommand { get; }

    /// <summary>
    /// 初始化数据的命令.
    /// </summary>
    public ReactiveCommand<Unit, Unit> InitializeCommand { get; }

    /// <summary>
    /// 阅读区间集合.
    /// </summary>
    public ObservableCollection<ReadSection> ReadSectionCollection { get; }

    /// <summary>
    /// 是否正在加载.
    /// </summary>
    public bool IsLoading => _isLoading.Value;

    /// <summary>
    /// 阅读记录是否为空.
    /// </summary>
    [ObservableProperty]
    public bool IsEmpty { get; set; }

    /// <summary>
    /// 阅读数据.
    /// </summary>
    [ObservableProperty]
    public ReaderDuration Data { get; set; }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _isLoading.Dispose();
            }

            _disposedValue = true;
        }
    }

    private void ReadBook()
        => AppViewModel.Instance.RequestRead(Data.Book);

    private async Task InitializeAsync()
    {
        var dbContext = LibraryViewModel.Instance.LibraryContext;
        History history = null;

        await Task.Run(async () =>
        {
            history = await dbContext.Histories.Include(p => p.ReadSections).FirstOrDefaultAsync(p => p.BookId == Data.Book.Id);
        });

        AppViewModel.Instance.DispatcherQueue.TryEnqueue(() =>
        {
            if (history.ReadSections?.Count > 0)
            {
                var sections = history.ReadSections.OrderByDescending(p => p.EndTime);
                foreach (var section in sections)
                {
                    ReadSectionCollection.Add(section);
                }
            }

            IsEmpty = ReadSectionCollection.Count == 0;
        });
    }
}
