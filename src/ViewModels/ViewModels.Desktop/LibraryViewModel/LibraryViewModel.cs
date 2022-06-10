// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using CleanReader.Services.Epub;
using CleanReader.Services.Novel;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using ReactiveUI;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 书库视图模型.
    /// </summary>
    public sealed partial class LibraryViewModel : ReactiveObject, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryViewModel"/> class.
        /// </summary>
        private LibraryViewModel()
        {
            ServiceLocator.Instance.LoadService(out _settingsToolkit)
                .LoadService(out _fileToolkit);
            SplitChapters = new ObservableCollection<SplitChapter>();
            BookSources = new ObservableCollection<Services.Novel.Models.BookSource>();
            ReplaceBookSources = new ObservableCollection<Services.Novel.Models.BookSource>();
            OnlineSearchBooks = new ObservableCollection<OnlineBookViewModel>();
            Themes = new List<ReaderThemeConfig>();
            RemoveException();

            OpenLibraryFolderCommand = ReactiveCommand.CreateFromTask(OpenLibraryFolderAsync, outputScheduler: RxApp.MainThreadScheduler);
            CreateLibraryFolderCommand = ReactiveCommand.CreateFromTask(CreateLibraryFolderAsync, outputScheduler: RxApp.MainThreadScheduler);
            InitializeThemesCommand = ReactiveCommand.CreateFromTask(InitializeThemesAsync, outputScheduler: RxApp.MainThreadScheduler);
            InitializeBookSourceCommand = ReactiveCommand.CreateFromTask(InitializeBookSourcesAsync, outputScheduler: RxApp.MainThreadScheduler);

            ShowImportDialogCommand = ReactiveCommand.CreateFromTask(ShowImportDialogAsync, outputScheduler: RxApp.MainThreadScheduler);
            ImportLocalBookCommand = ReactiveCommand.CreateFromTask(ImportLocalBookAsync, outputScheduler: RxApp.MainThreadScheduler);
            SplitChapterCommand = ReactiveCommand.CreateFromTask<string>(s => SplitChapterAsync(s), outputScheduler: RxApp.MainThreadScheduler);
            GetBookEntryFromTxtFileCommand = ReactiveCommand.CreateFromTask<string, Book>(s => GenerateBookEntryFromTxtFileAsync(s), outputScheduler: RxApp.MainThreadScheduler);
            GetBookEntryFromEpubFileCommand = ReactiveCommand.CreateFromTask<string, Book>(s => GenerateBookEntryFromEpubFileAsync(s), outputScheduler: RxApp.MainThreadScheduler);
            SyncCommand = ReactiveCommand.CreateFromTask(SyncBooksAsync, outputScheduler: RxApp.MainThreadScheduler);

            ShowOnlineSearchDialogCommand = ReactiveCommand.CreateFromTask<string>(ShowOnlineSearchDialogAsync, outputScheduler: RxApp.MainThreadScheduler);
            ShowReplaceSourceDialogCommand = ReactiveCommand.CreateFromTask<Book>(ShowReplaceSourceDialogAsync, outputScheduler: RxApp.MainThreadScheduler);
            OnlineSearchCommand = ReactiveCommand.CreateFromTask<string>(s => SearchOnlineBooksAsync(s), outputScheduler: RxApp.MainThreadScheduler);
            SelectOnlineSearchResultCommand = ReactiveCommand.Create<OnlineBookViewModel>(vm => SetSelectedSearchItem(vm), outputScheduler: RxApp.MainThreadScheduler);
            InsertOrUpdateBookEntryFromOnlineBookCommand = ReactiveCommand.CreateFromObservable<OnlineBookViewModel, Book>(
                p => GenerateBookEntryFromOnlineBook(p?.Book),
                this.WhenAnyValue(x => x.SelectedSearchBook).Select(p => p != null),
                RxApp.MainThreadScheduler);

            CreateShelfCommand = ReactiveCommand.CreateFromTask<string>(CreateNewShelfAsync, outputScheduler: RxApp.MainThreadScheduler);
            UpdateShelfCommand = ReactiveCommand.CreateFromTask<Shelf>(UpdateShelfAsync, outputScheduler: RxApp.MainThreadScheduler);

            OpenLibraryFolderCommand.ThrownExceptions
                .Merge(CreateLibraryFolderCommand.ThrownExceptions)
                .Merge(ImportLocalBookCommand.ThrownExceptions)
                .Merge(SplitChapterCommand.ThrownExceptions)
                .Merge(GetBookEntryFromTxtFileCommand.ThrownExceptions)
                .Merge(GetBookEntryFromEpubFileCommand.ThrownExceptions)
                .Merge(OnlineSearchCommand.ThrownExceptions)
                .Subscribe(DisplayException);

            InsertOrUpdateBookEntryFromOnlineBookCommand.ThrownExceptions.Subscribe(PopupException);

            ClearCommand = ReactiveCommand.Create(ClearCurentCache, outputScheduler: RxApp.MainThreadScheduler);

            _isOpening = OpenLibraryFolderCommand.IsExecuting.ToProperty(this, x => x.IsLoading, scheduler: RxApp.MainThreadScheduler);
            _isCreating = CreateLibraryFolderCommand.IsExecuting.ToProperty(this, x => x.IsLoading, scheduler: RxApp.MainThreadScheduler);
            _isImporting = ImportLocalBookCommand.IsExecuting.ToProperty(this, x => x.IsImporting, scheduler: RxApp.MainThreadScheduler);
            _isSpliting = SplitChapterCommand.IsExecuting.ToProperty(this, x => x.IsSpliting, scheduler: RxApp.MainThreadScheduler);
            _isOnlineSearching = OnlineSearchCommand.IsExecuting.ToProperty(this, x => x.IsOnlineSearching, scheduler: RxApp.MainThreadScheduler);
            _canDownloadOnlineBook = this.WhenAnyValue(x => x.SelectedSearchBook).Select(p => p != null).ToProperty(this, x => x.CanDownloadOnlineBook, scheduler: RxApp.MainThreadScheduler);

            this.WhenAnyValue(x => x.ExceptionMessage)
                .Subscribe(x => IsShowException = !string.IsNullOrEmpty(x));

            this.WhenAnyValue(x => x.ExceptionActionUrl)
                .Subscribe(x => IsShowExceptionAction = !string.IsNullOrEmpty(x));

            SplitChapters.CollectionChanged += OnSplitChaptersCollectionChanged;
            BookSources.CollectionChanged += OnBookSourcesCollectionChanged;
            OnlineSearchBooks.CollectionChanged += OnOnlineSearchBooksCollectionChanged;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 获取小说服务.
        /// </summary>
        /// <returns><see cref="NovelService"/>.</returns>
        public NovelService GetNovelService()
            => _novelService;

        /// <summary>
        /// 获取来源书架.
        /// </summary>
        /// <param name="book">书籍.</param>
        /// <returns>书架.</returns>
        public async Task<Shelf> GetSourceShelfAsync(Book book)
        {
            Shelf shelf = null;
            await Task.Run(async () =>
            {
                shelf = await LibraryContext.Shelves.Include(p => p.Books).Where(p => p.Books.Any(j => j.BookId == book.Id)).FirstOrDefaultAsync();
            });
            return shelf;
        }

        /// <summary>
        /// 初始化书库.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        /// <exception cref="InvalidOperationException">传入的书库路径有误.</exception>
        public async Task InitializeLibraryAsync()
        {
            var rootPath = _settingsToolkit.ReadLocalSetting(SettingNames.LibraryPath, string.Empty);
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                throw new DirectoryNotFoundException("Library path is empty.");
            }
            else if (!Directory.Exists(rootPath))
            {
                throw new DirectoryNotFoundException($"Path: {rootPath} not found.");
            }
            else if (!File.Exists(Path.Combine(rootPath, VMConstants.Library.DbFile)))
            {
                throw new DirectoryNotFoundException($"Database file not found.");
            }

            _rootDirectory = new DirectoryInfo(rootPath);
            if (!Directory.Exists(Path.Combine(rootPath, VMConstants.Library.BooksFolder)))
            {
                _rootDirectory.CreateSubdirectory(VMConstants.Library.BooksFolder);
            }

            // 验证用户是否可以正常连接数据库.
            var canConnect = false;
            LibraryContext = new LibraryDbContext(Path.Combine(rootPath, VMConstants.Library.DbFile));
            await Task.Run(async () =>
            {
                canConnect = await LibraryContext.Database.CanConnectAsync();
            });

            IsFileSystemLimited = !canConnect;
            if (IsFileSystemLimited)
            {
                return;
            }

            var checkResult = await CheckMigrationsAsync();
            if (checkResult == MigrationResult.Matched)
            {
                await InitializeBookSourcesAsync();
                await InitializeThemesAsync();
            }
            else
            {
                throw new LibraryInitializeException() { Result = checkResult };
            }
        }

        /// <summary>
        /// 获取本地封面图片.
        /// </summary>
        /// <param name="name">封面文件名.</param>
        /// <returns><see cref="BitmapImage"/>.</returns>
        public async Task<BitmapImage> GetLocalCoverImageAsync(string name)
        {
            var path = Path.Combine(_rootDirectory.FullName, VMConstants.Library.CoverFolder, name);
            if (File.Exists(path))
            {
                var file = new FileInfo(path);
                var bitmap = new BitmapImage();
                using (var stream = file.OpenRead().AsRandomAccessStream())
                {
                    await bitmap.SetSourceAsync(stream);
                }

                return bitmap;
            }

            return null;
        }

        /// <summary>
        /// 检查是否需要继续阅读.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        public async Task CheckContinueReadingAsync()
        {
            var shouldReading = _settingsToolkit.ReadLocalSetting(SettingNames.IsContinueReading, false);
            if (shouldReading)
            {
                var lastId = _settingsToolkit.ReadLocalSetting(SettingNames.LastReadBookId, string.Empty);
                if (!string.IsNullOrEmpty(lastId))
                {
                    Book book = null;

                    await Task.Run(async () =>
                    {
                        book = await LibraryContext.Books.FirstOrDefaultAsync(p => p.Id == lastId);
                    });

                    if (book != null)
                    {
                        AppViewModel.Instance.RequestRead(book);
                    }
                }
            }
        }

        /// <summary>
        /// 检查初始文件是直接打开还是要执行导入.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        public async Task CheckOpenFileOrImportAsync()
        {
            if (IsFileSystemLimited)
            {
                return;
            }

            var path = AppViewModel.Instance.InitializeFilePath;
            AppViewModel.Instance.InitializeFilePath = null;
            var isLibraryBook = false;

            await Task.Run(async () =>
            {
                isLibraryBook = await LibraryContext.Books.AnyAsync(p => p.Path.Equals(Path.GetFileName(path)));
            });

            if (isLibraryBook)
            {
                Book book = null;

                await Task.Run(async () =>
                {
                    book = await LibraryContext.Books.FirstOrDefaultAsync(p => p.Path.Equals(Path.GetFileName(path)));
                });

                DispatcherQueue.TryEnqueue(() =>
                {
                    AppViewModel.Instance.RequestRead(book);
                });
            }
            else
            {
                if (Path.GetExtension(path).Equals(".epub", StringComparison.OrdinalIgnoreCase))
                {
                    GetBookEntryFromEpubFileCommand.Execute(path)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(b =>
                        {
                            DispatcherQueue.TryEnqueue(async () =>
                            {
                                await InsertBookEntryAsync(b);
                                AppViewModel.Instance.RequestRead(b);
                            });
                        });
                }
                else if (Path.GetExtension(path).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    GetBookEntryFromTxtFileCommand.Execute(path)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(async b =>
                        {
                            await InsertBookEntryAsync(b);
                        });
                }
            }
        }

        private async Task InitializeBookSourcesAsync()
        {
            var rootPath = _settingsToolkit.ReadLocalSetting(SettingNames.LibraryPath, string.Empty);
            BookSources.Clear();
            if (_novelService == null)
            {
                _novelService = new NovelService();
            }

            try
            {
                await _novelService.InitializeBookSourcesAsync(Path.Combine(rootPath, VMConstants.Library.BookSourceFolder));
                var sources = _novelService.GetBookSources();
                sources.ForEach(p => BookSources.Add(p));
                BookSources.Insert(0, new Services.Novel.Models.BookSource()
                {
                    Id = "-1",
                    Name = StringResources.AllBookSources,
                });

                SelectedBookSource = BookSources.First();
            }
            catch (Exception)
            {
            }
        }

        private async Task InitializeThemesAsync()
        {
            var rootPath = _settingsToolkit.ReadLocalSetting(SettingNames.LibraryPath, string.Empty);
            Themes.Clear();
            var themeFile = Path.Combine(rootPath, VMConstants.Library.ThemeFile);
            if (File.Exists(themeFile))
            {
                var content = await _fileToolkit.ReadFileAsync(themeFile);
                JsonConvert.DeserializeObject<List<ReaderThemeConfig>>(content).ForEach(p =>
                {
                    if (!string.IsNullOrEmpty(p.AdditionalStyle))
                    {
                        p.AdditionalStyle = Regex.Unescape(p.AdditionalStyle);
                    }

                    Themes.Add(p);
                });
            }
        }

        private void DisplayException(Exception e)
        {
            if (e is not TaskCanceledException)
            {
                ExceptionMessage = e.Message;

                if (e is LibraryInitializeException libEx && libEx.Result == MigrationResult.AccessDenied)
                {
                    ExceptionActionContent = StringResources.OpenSettings;
                    ExceptionActionUrl = "ms-settings:appsfeatures-app";
                }

                EpubService.ClearCache();
                EpubService.ClearGenerated();
            }
        }

        private void PopupException(Exception e)
        {
            if (e is not TaskCanceledException)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    AppViewModel.Instance.ShowTip(e.Message, InfoType.Error);
                });

                EpubService.ClearCache();
                EpubService.ClearGenerated();
            }
        }

        private void RemoveException()
            => ExceptionMessage = ExceptionActionContent = ExceptionActionUrl = string.Empty;

        private async Task CreateNewShelfAsync(string name)
        {
            if (LibraryContext.Shelves.Any(p => p.Name.Equals(name)))
            {
                return;
            }

            var shelf = new Shelf()
            {
                Name = name,
                Id = Guid.NewGuid().ToString("N"),
                Books = new List<ShelfBook>(),
                Order = LibraryContext.Shelves.Count(),
            };
            LibraryContext.Shelves.Add(shelf);
            await LibraryContext.SaveChangesAsync();
            RequestShelfRefresh?.Invoke(this, EventArgs.Empty);
        }

        private async Task UpdateShelfAsync(Shelf shelf)
        {
            if (LibraryContext.Shelves.Any(p => p.Name.Equals(shelf.Name)))
            {
                return;
            }

            var source = await LibraryContext.Shelves.FirstOrDefaultAsync(p => p.Id == shelf.Id);
            if (source != null)
            {
                source.Name = shelf.Name;
                LibraryContext.Shelves.Update(source);
                await LibraryContext.SaveChangesAsync();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    LibraryContext?.Dispose();
                    _isCreating?.Dispose();
                    _isOpening?.Dispose();
                    _isImporting?.Dispose();
                    _isSpliting?.Dispose();
                    _isOnlineSearching?.Dispose();
                    _canDownloadOnlineBook?.Dispose();
                    LibraryContext = null;
                }

                _disposedValue = true;
            }
        }

        private void OnSplitChaptersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => IsSplitEmptyShown = SplitChapters.Count == 0;

        private void OnOnlineSearchBooksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => IsOnlineSearchEmptyShown = OnlineSearchBooks.Count == 0;

        private void OnBookSourcesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => LoadBookSourcesRequested?.Invoke(this, EventArgs.Empty);
    }
}
