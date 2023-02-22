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
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using CleanReader.Services.Interfaces;
using CleanReader.Services.Novel;
using CleanReader.Toolkit.Interfaces;
using CleanReader.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using ReactiveUI;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 书库视图模型.
/// </summary>
public sealed partial class LibraryViewModel : ViewModelBase, ILibraryViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryViewModel"/> class.
    /// </summary>
    private LibraryViewModel(
        ISettingsToolkit settingsToolkit,
        IFileToolkit fileToolkit,
        INovelService novelService,
        IEpubService epubService,
        IAppViewModel appViewModel)
    {
        _settingsToolkit = settingsToolkit;
        _fileToolkit = fileToolkit;
        _novelService = novelService;
        _epubService = epubService;
        _appViewModel = appViewModel;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        SplitChapters = new ObservableCollection<SplitChapter>();
        BookSources = new ObservableCollection<Models.Services.BookSource>();
        ReplaceBookSources = new ObservableCollection<Models.Services.BookSource>();
        OnlineSearchBooks = new ObservableCollection<IOnlineBookViewModel>();
        Themes = new List<ReaderThemeConfig>();
        RemoveException();

        AttachExceptionHandlerForAsyncCommand(
            DisplayException,
            OpenLibraryFolderCommand,
            CreateLibraryFolderCommand,
            SplitChapterCommand,
            SearchOnlineBooksCommand);

        AttachIsRunningForAsyncCommand(
            v =>
            {
                _isOpening = v;
                IsLoading = _isOpening || _isCreating;
            },
            OpenLibraryFolderCommand);

        AttachIsRunningForAsyncCommand(
            v =>
            {
                _isOpening = v;
                IsLoading = _isOpening || _isCreating;
            },
            CreateLibraryFolderCommand);

        AttachIsRunningForAsyncCommand(
            v =>
            {
                IsSpliting = v;
            },
            SplitChapterCommand);

        AttachIsRunningForAsyncCommand(
            v =>
            {
                IsOnlineSearching = v;
            },
            SearchOnlineBooksCommand);

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

    /// <inheritdoc/>
    public async Task<object> GetLocalCoverImageAsync(string name)
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

    /// <inheritdoc/>
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
                    _appViewModel.RequestRead(book);
                }
            }
        }
    }

    /// <inheritdoc/>
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

    [RelayCommand]
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
            BookSources.Insert(0, new Models.Services.BookSource()
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

    [RelayCommand]
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

            _epubService.ClearCache();
            _epubService.ClearGenerated();
        }
    }

    private void PopupException(Exception e)
    {
        if (e is not TaskCanceledException)
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                _appViewModel.ShowTip(e.Message, InfoType.Error);
            });

            _epubService.ClearCache();
            _epubService.ClearGenerated();
        }
    }

    private void RemoveException()
        => ExceptionMessage = ExceptionActionContent = ExceptionActionUrl = string.Empty;

    [RelayCommand]
    private async Task CreateShelfAsync(string name)
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

    [RelayCommand]
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

    private void OnSplitChaptersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsSplitEmptyShown = SplitChapters.Count == 0;

    private void OnOnlineSearchBooksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsOnlineSearchEmptyShown = OnlineSearchBooks.Count == 0;

    private void OnBookSourcesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => LoadBookSourcesRequested?.Invoke(this, EventArgs.Empty);
}
