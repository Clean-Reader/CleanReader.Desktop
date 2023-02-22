// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using CleanReader.Controls.Interfaces;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using CleanReader.Services.Novel;
using CleanReader.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 书库视图模型的在线搜索部分.
/// </summary>
public sealed partial class LibraryViewModel
{
    /// <inheritdoc/>
    public async Task<Book> GenerateBookEntryFromOnlineBookAsync(Models.Services.Book book = null)
    {
        book ??= SelectedSearchBook;

        try
        {
            if (book != null)
            {
                Book entry = null;
                var dialog = Locator.Lib.Locator.Instance.GetService<IProgressDialog>();
                var cancelSource = new CancellationTokenSource();
                dialog.InjectTask(
                    Task.Run(async () =>
                    {
                        _epubService.ClearCache();
                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            dialog.InjectData(StringResources.GettingChapters);
                        });

                        var chapters = await _novelService.GetBookChaptersAsync(book.SourceId, book.Url, cancelSource);
                        var chapterContents = new List<Tuple<int, string, string>>();
                        var progress = 0;
                        var total = chapters.Count;
                        var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 12, BoundedCapacity = total };
                        options.CancellationToken = cancelSource.Token;
                        var action = new ActionBlock<Models.Services.Chapter>(
                        async chapter =>
                        {
                            if (cancelSource.Token.IsCancellationRequested)
                            {
                                return;
                            }

                            var content = await _novelService.GetChapterContentAsync(book.SourceId, chapter, cancelSource);
                            chapterContents.Add(new Tuple<int, string, string>(content.ChapterIndex, chapter.Title, content.Content));
                            progress++;
                            _dispatcherQueue.TryEnqueue(() =>
                            {
                                dialog.InjectData(new Tuple<int, int, string>(progress, total, string.Format(StringResources.ChapterDownloadingProgress, progress, total)));
                            });
                        },
                        options);

                        foreach (var chapter in chapters)
                        {
                            action.Post(chapter);
                        }

                        action.Complete();
                        await action.Completion;

                        _dispatcherQueue.TryEnqueue(() =>
                        {
                            dialog.InjectData(StringResources.ConvertingAndMovingFile);
                        });
                        var configure = new Models.Services.EpubServiceConfiguration()
                        {
                            Title = book.BookName,
                            Author = book.Author,
                            Language = "zh",
                            OutputFileName = $"{book.BookName}.epub",
                            OutputFolderPath = Path.Combine(_rootDirectory.FullName, VMConstants.Library.BooksFolder),
                        };

                        try
                        {
                            configure = await _epubService.InitializeSplitedBookAsync(chapterContents, configure);
                            _epubService.SetConfiguration(configure);
                            await _epubService.CreateAsync();
                        }
                        catch (Exception ex)
                        {
                            _appViewModel.ShowTip(ex.Message, InfoType.Error);
                        }

                        entry = GetBookEntryFromOnlineBook(book, Path.Combine(configure.OutputFolderPath, configure.OutputFileName));
                    }),
                    cancelSource);

                await dialog.ShowAsync();
                _epubService.ClearGenerated();

                if (entry != null)
                {
                    if (OriginalBook != null)
                    {
                        var sourceEntry = await LibraryContext.Books.FirstOrDefaultAsync(p => p.Id == OriginalBook.Id);
                        if (sourceEntry != null)
                        {
                            await UpdateBookEntryAsync(sourceEntry, entry);
                            entry = sourceEntry;
                        }
                        else
                        {
                            await InsertBookEntryAsync(entry);
                        }

                        OriginalBook = null;
                    }
                    else
                    {
                        await InsertBookEntryAsync(entry);
                    }
                }

                if (entry != null)
                {
                    _appViewModel.ShowTip(StringResources.ExploreBookAdded, InfoType.Success);
                    _appViewModel.RequestNavigateTo(_appViewModel.NavigationList.First(p => p.Type == NavigationItemType.Item));
                }
                else
                {
                    _appViewModel.ShowTip(StringResources.FailedToDownload, InfoType.Error);
                }

                return entry;
            }
            else
            {
                throw new Exception(StringResources.NoSelectedOnlineBook);
            }
        }
        catch (Exception ex)
        {
            PopupException(ex);
        }
    }

    private static Book GetBookEntryFromOnlineBook(Models.Services.Book book, string path)
    {
        var bookEntry = new Book
        {
            Title = book.BookName,
            Type = BookType.Online,
            AddTime = DateTime.Now,
            Status = Models.DataBase.BookStatus.NotStart,
            Id = book.BookId,
            SourceId = book.SourceId,
            Author = book.Author,
            Cover = book.CoverUrl,
            Description = book.Description,
            Path = Path.GetFileName(path),
            LastChapterId = book.LatestChapterId,
            Url = book.Url,
        };

        return bookEntry;
    }

    private static bool IsSameChapterId(string sourceId, string targetId)
    {
        var sourceUrl = NovelService.DecodingBase64ToString(sourceId);
        var targetUrl = NovelService.DecodingBase64ToString(targetId);
        var isSame = sourceUrl.Length >= targetUrl.Length
                ? sourceUrl.Contains(targetUrl)
                : targetUrl.Contains(sourceUrl);

        return isSame;
    }

    [RelayCommand]
    private async Task ShowOnlineSearchDialogAsync(string initText)
    {
        _importDialog?.Hide();
        ClearOnlineSearchCache();
        var dialog = Locator.Lib.Locator.Instance.GetService<IOnlineSearchDialog>();
        dialog.InjectData(initText);
        IsFirstOnlineSearchTipShown = true;
        await dialog.ShowAsync();
    }

    [RelayCommand]
    private async Task ShowReplaceSourceDialogAsync(Book book)
    {
        ClearOnlineSearchCache();
        ReplaceBookSources.Clear();
        SelectedBookSource = null;
        OriginalBook = book;
        OriginSource = BookSources.FirstOrDefault(p => p.Id == book.SourceId) ?? new Models.Services.BookSource()
        {
            Id = "invalid",
            Name = StringResources.InvalidSource,
        };

        BookSources.Where(p =>
                p.Id != "-1"
                && p.Id != OriginSource.Id
                && !string.IsNullOrEmpty(p.ChapterContent?.Range)
                && !string.IsNullOrEmpty(p.Chapter?.Range))
            .ToList()
            .ForEach(p => ReplaceBookSources.Add(p));
        var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.ReplaceSourceDialog);
        IsFirstReplaceSourceTipShown = true;
        await dialog.ShowAsync();
    }

    [RelayCommand]
    private async Task SearchOnlineBooksAsync(string keyword)
    {
        ClearOnlineSearchCache();
        IsFirstOnlineSearchTipShown = false;
        IsFirstReplaceSourceTipShown = false;
        if (SelectedBookSource == null || SelectedBookSource.Id == "-1")
        {
            _tempOnlineSearchResult = await _novelService.SearchBookAsync(keyword);
        }
        else
        {
            var books = await _novelService.SearchBookAsync(SelectedBookSource.Id, keyword);
            _tempOnlineSearchResult = new Dictionary<string, List<Models.Services.Book>>()
            {
                { SelectedBookSource.Id, books },
            };
        }

        if (_tempOnlineSearchResult != null && _tempOnlineSearchResult.Count > 0)
        {
            _tempOnlineSearchResult.SelectMany(p => p.Value)
                .Distinct()
                .Select(p =>
                {
                    var vm = Locator.Lib.Locator.Instance.GetService<IOnlineBookViewModel>();
                    vm.InjectData(p);
                    return vm;
                })
                .ToList()
                .ForEach(p => OnlineSearchBooks.Add(p));
        }
    }

    [RelayCommand]
    private void SelectOnlineSearchResult(IOnlineBookViewModel vm)
    {
        if (SelectedSearchBook == vm.Book)
        {
            vm = null;
        }

        foreach (var item in OnlineSearchBooks)
        {
            item.IsSelected = vm != null && item.Book.Equals(vm.Book);
        }

        SelectedSearchBook = OnlineSearchBooks.SingleOrDefault(p => p.IsSelected)?.Book;
    }

    [RelayCommand]
    private async Task SyncBooksAsync()
    {
        var progressDialog = Locator.Lib.Locator.Instance.GetService<IProgressDialog>();
        var cancellationTokenSource = new CancellationTokenSource();
        progressDialog.InjectTask(
            Task.Run(async () =>
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                progressDialog.InjectData(StringResources.LoadingOnlineBooks);
            });
            var isBookChanged = false;
            var onlineBooks = await LibraryContext.Books.Where(p => p.Type == BookType.Online && !string.IsNullOrEmpty(p.LastChapterId)).ToListAsync();
            if (onlineBooks.Any())
            {
                var bookFetchCount = 0;
                var chapterFetchCount = 0;
                var totalChapterCount = onlineBooks.Count;
                var tasks = new List<Task>();
                _dispatcherQueue.TryEnqueue(() =>
                {
                    progressDialog.InjectData(new Tuple<int, int, string>(
                        chapterFetchCount,
                        totalChapterCount,
                        string.Format(StringResources.BookSyncProgress, bookFetchCount, onlineBooks.Count)));
                });

                var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 12, BoundedCapacity = DataflowBlockOptions.Unbounded };
                options.CancellationToken = cancellationTokenSource.Token;

                var bookAction = new ActionBlock<Book>(async book =>
                {
                    var sourceBook = Path.Combine(_rootDirectory.FullName, VMConstants.Library.BooksFolder, Path.GetFileName(book.Path));
                    var needRegenerate = _epubService.NeedRegenerate(sourceBook);
                    var chapters = await _novelService.GetBookChaptersAsync(book.SourceId, book.Url, cancellationTokenSource);
                    if (chapters != null && chapters.Count > 0)
                    {
                        var isLast = IsSameChapterId(book.LastChapterId, chapters.Last().Id);
                        if (!isLast || needRegenerate)
                        {
                            isBookChanged = true;
                            var index = -1;

                            if (!needRegenerate)
                            {
                                var sourceLastChapter = chapters.FirstOrDefault(p => IsSameChapterId(p.Id, book.LastChapterId));
                                if (sourceLastChapter != null)
                                {
                                    index = chapters.IndexOf(sourceLastChapter);
                                }
                            }

                            var needUpdateChapters = chapters.Skip(index + 1).ToList();
                            var chapterContents = new List<Tuple<int, string, string>>();
                            totalChapterCount += needUpdateChapters.Count;
                            var chapterAction = new ActionBlock<Models.Services.Chapter>(
                                async chapter =>
                                {
                                    var content = await _novelService.GetChapterContentAsync(book.SourceId, chapter, cancellationTokenSource);
                                    chapterContents.Add(new Tuple<int, string, string>(content.ChapterIndex, chapter.Title, content.Content));
                                    chapterFetchCount++;
                                    _dispatcherQueue.TryEnqueue(() =>
                                    {
                                        progressDialog.InjectData(new Tuple<int, int, string>(
                                            chapterFetchCount,
                                            totalChapterCount,
                                            string.Format(
                                                StringResources.BookSyncProgress,
                                                bookFetchCount,
                                                onlineBooks.Count)));
                                    });
                                }, options);

                            foreach (var chapter in needUpdateChapters)
                            {
                                chapterAction.Post(chapter);
                            }

                            chapterAction.Complete();
                            await chapterAction.Completion;

                            if (cancellationTokenSource.IsCancellationRequested)
                            {
                                _epubService.ClearCache();
                                _epubService.ClearGenerated();
                                return;
                            }

                            // 将数据写入已存在的epub文件中.
                            var configure = new Models.Services.EpubServiceConfiguration()
                            {
                                Title = book.Title,
                                Author = book.Author,
                                Language = "zh",
                                OutputFileName = Path.GetFileName(book.Path),
                                OutputFolderPath = Path.Combine(_rootDirectory.FullName, VMConstants.Library.BooksFolder),
                            };

                            configure = needRegenerate
                                ? await _epubService.InitializeSplitedBookAsync(chapterContents, configure)
                                : await _epubService.InitializeAdditionalChaptersAsync(sourceBook, chapterContents, configure);

                            _epubService.SetConfiguration(configure);
                            await _epubService.CreateAsync();
                            book.LastChapterId = chapters.Last().Id;
                        }
                    }

                    chapterFetchCount++;
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        progressDialog.InjectData(new Tuple<int, int, string>(
                                        chapterFetchCount,
                                        totalChapterCount,
                                        string.Format(
                                            StringResources.BookSyncProgress,
                                            bookFetchCount,
                                            onlineBooks.Count)));
                    });
                });

                foreach (var onlineBook in onlineBooks)
                {
                    bookAction.Post(onlineBook);
                }

                bookAction.Complete();
                await bookAction.Completion;
                var bookIds = onlineBooks.Select(p => p.Id);
                var histories = await LibraryContext.Histories.Where(p => bookIds.Contains(p.BookId)).ToListAsync();
                var isHistoryChanged = false;
                foreach (var history in histories)
                {
                    if (!string.IsNullOrEmpty(history.Locations))
                    {
                        isHistoryChanged = true;
                    }

                    history.Locations = string.Empty;
                }

                if (isBookChanged)
                {
                    LibraryContext.Books.UpdateRange(onlineBooks);
                }

                if (isHistoryChanged)
                {
                    LibraryContext.Histories.UpdateRange(histories);
                }

                if (isBookChanged || isHistoryChanged)
                {
                    await LibraryContext.SaveChangesAsync();
                }
            }
        }), cancellationTokenSource);

        await progressDialog.ShowAsync();
        _epubService.ClearCache();
        _epubService.ClearGenerated();
    }

    private void ClearOnlineSearchCache()
    {
        OnlineSearchBooks.Clear();
        RemoveException();
        SelectedSearchBook = null;
        _tempOnlineSearchResult = null;
        OriginSource = null;
    }
}
