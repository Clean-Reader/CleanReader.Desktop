// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 书架页面视图模型.
    /// </summary>
    public sealed partial class ShelfPageViewModel : ReactiveObject, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShelfPageViewModel"/> class.
        /// </summary>
        private ShelfPageViewModel()
        {
            LibraryViewModel.Instance.BookAdded += OnBookAdded;
            LibraryViewModel.Instance.RequestShelfRefresh += OnRequestShelfRefreshAsync;
            DisplayBooks = new ObservableCollection<ShelfBookViewModel>();
            Shelves = new ObservableCollection<Shelf>();
            ServiceLocator.Instance.LoadService(out _settingsToolkit);

            InitializeViewModelCommand = ReactiveCommand.CreateFromTask(InitializeViewModelAsync, outputScheduler: RxApp.MainThreadScheduler);
            InitializeShelvesCommand = ReactiveCommand.CreateFromTask(InitializeShelvesAsync, outputScheduler: RxApp.MainThreadScheduler);
            InitializeBooksCommand = ReactiveCommand.CreateFromTask(LoadCurrentBooksAsync, this.WhenAnyValue(x => x.CurrentShelf).Select(x => x != null), RxApp.MainThreadScheduler);
            MoveShelfToTopCommand = ReactiveCommand.CreateFromTask<Shelf>(MoveToTopAsync, outputScheduler: RxApp.MainThreadScheduler);
            DeleteShelfCommand = ReactiveCommand.CreateFromTask<Shelf>(DeleteShelfAsync, outputScheduler: RxApp.MainThreadScheduler);

            _isViewModelLoading = InitializeViewModelCommand.IsExecuting.ToProperty(this, x => x.IsInitializing, scheduler: RxApp.MainThreadScheduler);
            _isShelvesLoading = InitializeShelvesCommand.IsExecuting.ToProperty(this, x => x.IsInitializing, scheduler: RxApp.MainThreadScheduler);
            _isBooksLoading = InitializeBooksCommand.IsExecuting.ToProperty(this, x => x.IsInitializing, scheduler: RxApp.MainThreadScheduler);

            InitializeViewModelCommand.ThrownExceptions.Subscribe(ex =>
            {
                if (ex.Message.Contains("database"))
                {
                    // 连接数据库失败，表示用户可能关闭了文件权限.
                    var exception = new LibraryInitializeException(StringResources.CanNotAccessLibrary, MigrationResult.AccessDenied);
                    LibraryViewModel.Instance.DisplayExceptionCommand.Execute(exception).Subscribe();
                    AppViewModel.Instance.RequestStartup();
                }
            });

            InitializeShelvesCommand.ThrownExceptions.Subscribe(x =>
            {
                Debug.WriteLine(x.Message);
            });

            this.WhenAnyValue(x => x.CurrentShelf, x => x.CurrentBookType, x => x.CurrentSort)
                .ObserveOn(RxApp.MainThreadScheduler)
                .WhereNotNull()
                .Select(_ => Unit.Default)
                .InvokeCommand(InitializeBooksCommand);

            DisplayBooks.CollectionChanged += OnDisplayBooksCollectionChanged;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 转移到书架.
        /// </summary>
        /// <param name="book">书籍.</param>
        /// <param name="selectedShelf">选择的书架.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task TransferShelfAsync(Book book, Shelf selectedShelf)
        {
            var list = await _dbContextRef.Shelves.Include(p => p.Books).ToListAsync();
            foreach (var item in list)
            {
                if (item.Books != null)
                {
                    item.Books.RemoveAll(p => p.BookId == book.Id);
                }
            }

            if (!string.IsNullOrEmpty(selectedShelf.Id))
            {
                var shelf = list.Where(p => p.Id == selectedShelf.Id).FirstOrDefault();
                if (shelf.Books == null)
                {
                    shelf.Books = new List<ShelfBook>();
                }

                shelf.Books.Add(new ShelfBook()
                {
                    BookId = book.Id,
                    ModifiedTime = DateTime.Now,
                });
            }

            await Task.Run(async () =>
            {
                _dbContextRef.Shelves.UpdateRange(list);
                await _dbContextRef.SaveChangesAsync();
            });

            DispatcherQueue.TryEnqueue(async () =>
            {
                await LoadCurrentBooksAsync();
            });
        }

        private async Task InitializeViewModelAsync()
        {
            if (LibraryViewModel.Instance.IsFileSystemLimited)
            {
                return;
            }

            _dbContextRef = LibraryViewModel.Instance.LibraryContext;
            await TransferBooksPathAsync();
            await InitializeShelvesAsync();
            CurrentBookType = _settingsToolkit.ReadLocalSetting(SettingNames.BookType, VMConstants.Shelf.AllType);
            CurrentSort = _settingsToolkit.ReadLocalSetting(SettingNames.SortType, VMConstants.Shelf.SortRead);
        }

        private async Task InitializeShelvesAsync()
        {
            if (LibraryViewModel.Instance.IsFileSystemLimited)
            {
                return;
            }

            Shelves.Clear();
            CurrentShelf = null;

            List<Shelf> shelves = null;
            await Task.Run(async () =>
            {
                shelves = await _dbContextRef.Shelves.Include(p => p.Books).ToListAsync();
            });

            DispatcherQueue.TryEnqueue(() =>
            {
                var defaultShelf = new Shelf
                {
                    Id = string.Empty,
                    Books = null,
                    Name = StringResources.All,
                    Order = -1,
                };
                shelves.Insert(0, defaultShelf);
                shelves = shelves.OrderBy(x => x.Order).ToList();
                shelves.ForEach(shelve => Shelves.Add(shelve));

                var shelfId = _settingsToolkit.ReadLocalSetting(SettingNames.ShelfId, string.Empty);
                var shelf = shelves.FirstOrDefault(p => p.Id == shelfId) ?? shelves.First();
                CurrentShelf = shelf;
            });
        }

        private async Task LoadCurrentBooksAsync()
        {
            if (CurrentShelf == null)
            {
                return;
            }

            DisplayBooks.Clear();
            if (CurrentShelf.Books == null)
            {
                CurrentShelf.Books = new List<ShelfBook>();
            }

            var shelfBooks = CurrentShelf.Books.Select(p => p.BookId);
            List<Book> books = null;

            await Task.Run(async () =>
            {
                books = CurrentShelf == null || CurrentShelf.Id == string.Empty
                    ? await _dbContextRef.Books.ToListAsync()
                    : await _dbContextRef.Books.Where(p => shelfBooks.Contains(p.Id)).ToListAsync();
            });

            if (CurrentBookType == VMConstants.Shelf.LocalType)
            {
                books = books.Where(p => p.Type == BookType.Local).ToList();
            }
            else if (CurrentBookType == VMConstants.Shelf.OnlineType)
            {
                books = books.Where(p => p.Type == BookType.Online).ToList();
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                SortBooks(books);
            });

            _settingsToolkit.WriteLocalSetting(SettingNames.SortType, CurrentSort);
            _settingsToolkit.WriteLocalSetting(SettingNames.BookType, CurrentBookType);
            _settingsToolkit.WriteLocalSetting(SettingNames.ShelfId, CurrentShelf?.Id ?? string.Empty);
        }

        private void SortBooks(List<Book> source)
        {
            var list = source.Select(p => new ShelfBookViewModel(p, _dbContextRef)).ToList();
            if (CurrentSort == VMConstants.Shelf.SortType)
            {
                list = list.OrderBy(p => p.Book.Type).ToList();
            }
            else if (CurrentSort == VMConstants.Shelf.SortTime)
            {
                list = list.OrderByDescending(p => p.Book.AddTime).ToList();
            }
            else if (CurrentSort == VMConstants.Shelf.SortProgress)
            {
                list = list.OrderByDescending(p => p.Progress).ToList();
            }
            else if (CurrentSort == VMConstants.Shelf.SortName)
            {
                list = list.OrderBy(p => p.Book.Title).ToList();
            }
            else if (CurrentSort == VMConstants.Shelf.SortRead)
            {
                list = list.OrderByDescending(p => p.Book.LastOpenTime).ToList();
            }

            DisplayBooks.Clear();
            list.ForEach(p => DisplayBooks.Add(p));
        }

        private async Task MoveToTopAsync(Shelf shelf)
        {
            if (!string.IsNullOrEmpty(shelf.Id) && Shelves.Contains(shelf))
            {
                Shelves.Remove(shelf);
                Shelves.Insert(1, shelf);
                for (var i = 0; i < Shelves.Count; i++)
                {
                    Shelves[i].Order = i - 1;
                }

                await Task.Run(async () =>
                {
                    _dbContextRef.Shelves.UpdateRange(Shelves.Where(p => p.Order != -1));
                    await _dbContextRef.SaveChangesAsync();
                });
                await _dbContextRef.SaveChangesAsync();
                await InitializeShelvesAsync();
            }
        }

        private async Task DeleteShelfAsync(Shelf shelf)
        {
            if (!string.IsNullOrEmpty(shelf.Id) && Shelves.Contains(shelf))
            {
                var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.ConfirmDialog);
                dialog.InjectData(string.Format(StringResources.DeleteShelfWarning, shelf.Name));
                var dialogResult = await dialog.ShowAsync();
                if (dialogResult == 0)
                {
                    await Task.Run(async () =>
                    {
                        _dbContextRef.Shelves.Remove(shelf);
                        await _dbContextRef.SaveChangesAsync();
                    });
                    await InitializeShelvesAsync();
                }
            }
        }

        private void OnBookAdded(object sender, EventArgs e)
            => InitializeBooksCommand.Execute().Subscribe();

        private async Task TransferBooksPathAsync()
        {
            var isTransfered = _settingsToolkit.ReadLocalSetting(SettingNames.IsBookPathTransfered, false);
            if (!isTransfered)
            {
                var books = _dbContextRef.Books.ToList();

                if (books.Count > 0)
                {
                    foreach (var book in books)
                    {
                        if (Path.IsPathFullyQualified(book.Path))
                        {
                            book.Path = Path.GetFileName(book.Path);
                        }
                    }

                    await Task.Run(async () =>
                    {
                        _dbContextRef.UpdateRange(books);
                        await _dbContextRef.SaveChangesAsync();
                    });
                }

                _settingsToolkit.WriteLocalSetting(SettingNames.IsBookPathTransfered, true);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _isBooksLoading?.Dispose();
                    _isShelvesLoading?.Dispose();
                    _isViewModelLoading?.Dispose();
                    LibraryViewModel.Instance.BookAdded -= OnBookAdded;
                }

                _disposedValue = true;
            }
        }

        private void OnDisplayBooksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => IsShelfEmpty = DisplayBooks.Count == 0;

        private async void OnRequestShelfRefreshAsync(object sender, EventArgs e)
            => await InitializeShelvesAsync();
    }
}
