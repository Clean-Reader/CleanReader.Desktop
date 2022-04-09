// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 笔记概览页面视图模型.
    /// </summary>
    public sealed partial class NoteOverviewPageViewModel : ReactiveObject, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoteOverviewPageViewModel"/> class.
        /// </summary>
        private NoteOverviewPageViewModel()
        {
            _allHighlights = new List<Highlight>();
            Books = new ObservableCollection<Book>();
            Notes = new ObservableCollection<Highlight>();

            InitializeCommand = ReactiveCommand.CreateFromTask(InitializeAsync, outputScheduler: RxApp.MainThreadScheduler);
            ShowHighlightDialogCommand = ReactiveCommand.CreateFromTask<Highlight>(ShowHighlightDialogAsync, outputScheduler: RxApp.MainThreadScheduler);
            DeleteHighlightCommand = ReactiveCommand.CreateFromTask<Highlight>(DeleteHighlightAsync, outputScheduler: RxApp.MainThreadScheduler);
            JumpToHighlightCommand = ReactiveCommand.Create<Highlight>(JumpToHighlight, outputScheduler: RxApp.MainThreadScheduler);

            _isInitializing = InitializeCommand.IsExecuting.ToProperty(this, x => x.IsInitializing, scheduler: RxApp.MainThreadScheduler);

            this.WhenAnyValue(x => x.CurrentBook)
                .WhereNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async _ => await LoadCurrentBookNotesAsync());

            Books.CollectionChanged += OnBooksCollectionChanged;
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

            _libraryDbContext = LibraryViewModel.Instance.LibraryContext;
            _allHighlights.Clear();
            DispatcherQueue.TryEnqueue(() =>
            {
                Books.Clear();
                Notes.Clear();
                CurrentBook = null;
            });

            List<Highlight> highlights = null;

            await Task.Run(async () =>
            {
                highlights = await _libraryDbContext.Highlights.ToListAsync();
            });

            if (highlights.Any())
            {
                highlights.ForEach(highlight => _allHighlights.Add(highlight));
                var bookIds = highlights.Select(p => p.BookId).Distinct();

                List<Book> books = null;

                await Task.Run(async () =>
                {
                    books = await _libraryDbContext.Books.Where(p => bookIds.Contains(p.Id)).ToListAsync();
                });

                if (books.Count > 0)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        books.ForEach(p => Books.Add(p));
                        CurrentBook = Books.First();
                    });
                }
            }
        }

        private async Task ShowHighlightDialogAsync(Highlight highlight)
        {
            var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.ReaderHighlightDialog);
            dialog.InjectData(highlight);
            await dialog.ShowAsync();
        }

        private async Task DeleteHighlightAsync(Highlight highlight)
        {
            _libraryDbContext.Highlights.Remove(highlight);
            _allHighlights.Remove(highlight);

            await Task.Run(async () =>
            {
                await _libraryDbContext.SaveChangesAsync();
                await LoadCurrentBookNotesAsync();
            });
        }

        private async Task LoadCurrentBookNotesAsync()
        {
            if (CurrentBook != null)
            {
                var notes = _allHighlights.Where(p => p.BookId == CurrentBook.Id).ToList();
                if (notes.Count > 0)
                {
                    Notes.Clear();
                    notes.ForEach(p => Notes.Add(p));
                }
                else
                {
                    await InitializeAsync();
                }
            }
        }

        private void JumpToHighlight(Highlight highlight)
        {
            if (CurrentBook != null)
            {
                AppViewModel.Instance.RequestRead(CurrentBook, highlight.CfiRange);
            }
        }

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

        private void OnBooksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => IsShowEmpty = Books.Count == 0;
    }
}
