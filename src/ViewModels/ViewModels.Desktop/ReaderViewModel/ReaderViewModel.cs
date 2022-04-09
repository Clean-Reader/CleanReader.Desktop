// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using GoogleTranslateFreeApi;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReactiveUI;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 阅读器视图模型.
    /// </summary>
    public sealed partial class ReaderViewModel : ReactiveObject, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderViewModel"/> class.
        /// </summary>
        private ReaderViewModel()
        {
            ServiceLocator.Instance.LoadService(out _settingsToolkit)
                .LoadService(out _fontToolkit);
            _libraryDbContext = LibraryViewModel.Instance.LibraryContext;
            _isEnableToggleFullScreen = _isEnableToggleMiniView = true;
            _googleTranslator = new GoogleTranslator() { TimeOut = TimeSpan.FromSeconds(5) };
            Chapters = new ObservableCollection<ReaderChapterViewModel>();
            Themes = new ObservableCollection<ReaderThemeConfig>();
            Fonts = new ObservableCollection<string>();
            Colors = new ObservableCollection<string>();
            SearchResult = new ObservableCollection<ReaderSearchResult>();
            Highlights = new ObservableCollection<Highlight>();
            SaveProgressCommand = ReactiveCommand.CreateFromTask<string>(x => SaveProgressAsync(x), outputScheduler: RxApp.MainThreadScheduler);
            StopReadingCommand = ReactiveCommand.CreateFromTask(StopReadAsync, outputScheduler: RxApp.MainThreadScheduler);
            SaveLocationsCommand = ReactiveCommand.CreateFromTask<string>(x => SaveLocationsAsync(x), outputScheduler: RxApp.MainThreadScheduler);
            InitializeTocCommand = ReactiveCommand.Create<string>(x => InitializeToc(x), outputScheduler: RxApp.MainThreadScheduler);
            ToggleCatalogCommand = ReactiveCommand.Create(ToggleCatalogVisibility, outputScheduler: RxApp.MainThreadScheduler);
            ShowNotesCommand = ReactiveCommand.Create(ToggleNotesVisibility, outputScheduler: RxApp.MainThreadScheduler);
            ShowInterfaceSettingsCommand = ReactiveCommand.CreateFromTask(ShowInterfaceSettingsAsync, outputScheduler: RxApp.MainThreadScheduler);
            ShowHighlightDialogCommand = ReactiveCommand.CreateFromTask<ReaderContextMenuArgs>(ShowHighlightDialogAsync, outputScheduler: RxApp.MainThreadScheduler);
            ClearPopupCommand = ReactiveCommand.Create(ClearPopup, outputScheduler: RxApp.MainThreadScheduler);
            BackCommand = ReactiveCommand.Create(Back, outputScheduler: RxApp.MainThreadScheduler);
            GetHightlightFromCfiCommand = ReactiveCommand.CreateFromTask<string, Highlight>(GetHighlightAsync, outputScheduler: RxApp.MainThreadScheduler);
            AddOrUpdateHighlightCommand = ReactiveCommand.CreateFromTask<Highlight>(AddOrUpdateHighlightAsync, outputScheduler: RxApp.MainThreadScheduler);
            TranslateCommand = ReactiveCommand.CreateFromTask<string>(TranslateAsync, outputScheduler: RxApp.MainThreadScheduler);
            OnlineSearchCommand = ReactiveCommand.CreateFromTask<string>(OnlineSearchAsync, outputScheduler: RxApp.MainThreadScheduler);
            DisplayInitializeErrorCommand = ReactiveCommand.Create<string>(DisplayInitializeError, outputScheduler: RxApp.MainThreadScheduler);
            ShowSearchDailogCommand = ReactiveCommand.CreateFromTask<string>(ShowSearchDialogAsync, outputScheduler: RxApp.MainThreadScheduler);
            InternalSearchCommand = ReactiveCommand.Create<string>(SearchInternal, outputScheduler: RxApp.MainThreadScheduler);
            ChangeLocationCommand = ReactiveCommand.Create<string>(ChangeLocation, outputScheduler: RxApp.MainThreadScheduler);
            InitializeSearchResultCommand = ReactiveCommand.Create<string>(InitializeSearchResult, outputScheduler: RxApp.MainThreadScheduler);
            InitializeNotesCommand = ReactiveCommand.CreateFromTask(InitializeHighlightsAsync, outputScheduler: RxApp.MainThreadScheduler);
            ToggleFullScreenCommand = ReactiveCommand.CreateFromTask(ToggleFullScreenAsync, outputScheduler: RxApp.MainThreadScheduler);
            ToggleMiniViewCommand = ReactiveCommand.CreateFromTask(ToggleMiniViewAsync, outputScheduler: RxApp.MainThreadScheduler);

            NextChapterCommand = ReactiveCommand.Create(GoToNextChapter, outputScheduler: RxApp.MainThreadScheduler);
            PreviousChapterCommand = ReactiveCommand.Create(GoToPreviousChapter, outputScheduler: RxApp.MainThreadScheduler);
            RemoveHighlightCommand = ReactiveCommand.CreateFromTask<Highlight>(RemoveHighlightAsync, outputScheduler: RxApp.MainThreadScheduler);

            _isShowCoverMask = this.WhenAnyValue(x => x.IsMenuShown, x => x.IsCatalogShown, x => x.IsNotesShown)
              .Select(x => x.Item1 || x.Item2 || x.Item3)
              .ToProperty(this, x => x.IsShowCoverMask, scheduler: RxApp.MainThreadScheduler);
            _isTranslating = TranslateCommand.IsExecuting.ToProperty(this, x => x.IsTranslating, scheduler: RxApp.MainThreadScheduler);

            FontFamily = _settingsToolkit.ReadLocalSetting(SettingNames.FontFamily, "Origin");
            FontSize = _settingsToolkit.ReadLocalSetting(SettingNames.FontSize, 22d);
            LineHeight = _settingsToolkit.ReadLocalSetting(SettingNames.LineHeight, 1.4);
            Background = _settingsToolkit.ReadLocalSetting(SettingNames.Background, "#FFFFFF");
            Foreground = _settingsToolkit.ReadLocalSetting(SettingNames.Foreground, "#111111");
            AdditionalStyle = _settingsToolkit.ReadLocalSetting(SettingNames.AdditionalStyle, string.Empty);
            IsSmoothScroll = _settingsToolkit.ReadLocalSetting(SettingNames.IsSmoothScroll, true);

            InitializeColors();

            this.WhenAnyValue(x => x.FontFamily).WhereNotNull().Subscribe(p => WriteSetting(SettingNames.FontFamily, p));
            this.WhenAnyValue(x => x.FontSize).Subscribe(p =>
            {
                if (AppViewModel.Instance.IsMiniView)
                {
                    WriteSetting(SettingNames.MiniFontSize, p);
                }
                else
                {
                    WriteSetting(SettingNames.FontSize, p);
                }
            });
            this.WhenAnyValue(x => x.LineHeight).Subscribe(p => WriteSetting(SettingNames.LineHeight, p));
            this.WhenAnyValue(x => x.Background).Subscribe(p => WriteSetting(SettingNames.Background, p));
            this.WhenAnyValue(x => x.Foreground).Subscribe(p => WriteSetting(SettingNames.Foreground, p));
            this.WhenAnyValue(x => x.AdditionalStyle).Subscribe(p => WriteSetting(SettingNames.AdditionalStyle, p));
            this.WhenAnyValue(x => x.IsSmoothScroll).Subscribe(p => WriteSetting(SettingNames.IsSmoothScroll, p));

            Themes.CollectionChanged += OnThemesCollectionChanged;
            SearchResult.CollectionChanged += OnSearchResultCollectionChanged;
            Highlights.CollectionChanged += OnHighlightsCollectionChanged;

            TranslateCommand.ThrownExceptions.Subscribe(DisplayTranslateError);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 设置书籍.
        /// </summary>
        /// <param name="book">书籍.</param>
        /// <param name="startCfi">起始位置.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task SetBookAsync(Book book, string startCfi = null)
        {
            var rootPath = _settingsToolkit.ReadLocalSetting(SettingNames.LibraryPath, string.Empty);
            _book = book;
            FilePath = Path.Combine(rootPath, VMConstants.Library.BooksFolder, book.Path);
            BookTitle = book.Title;
            _startTime = DateTime.Now;
            IsCatalogShown = true;
            Chapters.Clear();
            CurrentChapter = null;
            ClearPopupCommand.Execute().Subscribe();
            IsInitializing = true;
            IsInitializeFailed = false;
            StartCfi = startCfi;
            SpreadMinWidth = _settingsToolkit.ReadLocalSetting(SettingNames.SpreadMinWidth, 1000d);
            IsSmoothScroll = _settingsToolkit.ReadLocalSetting(SettingNames.IsSmoothScroll, true);
            _settingsToolkit.WriteLocalSetting(SettingNames.LastReadBookId, book.Id);

            var history = await _libraryDbContext.Histories.FirstOrDefaultAsync(p => p.BookId == book.Id);
            if (history != null)
            {
                StartCfi = string.IsNullOrEmpty(startCfi) ? history.Position : startCfi;
                Locations = history.Locations ?? string.Empty;
                Progresss = history.Progress.ToString("0.0") + "%";
            }

            await InitializeHighlightsAsync();
            if (Highlights.Any())
            {
                InitHighlights = JsonConvert.SerializeObject(Highlights.Select(p => new ReaderHighlight()
                {
                    CfiRange = p.CfiRange,
                    Color = p.Color,
                }));
            }

            if (Fonts.Count == 0)
            {
                var list = _fontToolkit.GetSystemFontList();
                list.ForEach(p => Fonts.Add(p));
            }

            book.LastOpenTime = DateTime.Now;
            _libraryDbContext.Books.Update(book);
            await _libraryDbContext.SaveChangesAsync();

            InitializeThemes();
        }

        /// <summary>
        /// 获取当前书籍Id.
        /// </summary>
        /// <returns>书籍Id.</returns>
        public string GetCurrentBookId() => _book?.Id ?? string.Empty;

        private static void Back()
            => AppViewModel.Instance.RequestRead(null);

        private async Task<Highlight> GetHighlightAsync(string cfiRange)
        {
            var original = await _libraryDbContext.Highlights.Where(p => p.CfiRange == cfiRange && p.BookId == _book.Id).FirstOrDefaultAsync();
            return original;
        }

        private async Task AddOrUpdateHighlightAsync(Highlight data)
        {
            var original = _libraryDbContext.Highlights.Where(p => p.CfiRange == data.CfiRange && p.BookId == _book.Id).FirstOrDefault();
            if (original != null)
            {
                _libraryDbContext.Highlights.Update(data);
            }
            else
            {
                _libraryDbContext.Highlights.Add(data);
            }

            await _libraryDbContext.SaveChangesAsync();

            DispatcherQueue.TryEnqueue(() =>
            {
                RequestHighlight?.Invoke(this, data);
            });
        }

        private async Task StopReadAsync()
        {
            if (_book == null)
            {
                return;
            }

            var sourceHistory = await _libraryDbContext.Histories.Include(p => p.ReadSections).FirstOrDefaultAsync(p => p.BookId == _book.Id);
            if (sourceHistory != null && _startTime != DateTime.MinValue)
            {
                var endTime = DateTime.Now;
                AddReadDuration(sourceHistory, endTime);
                _libraryDbContext.Histories.Update(sourceHistory);
                await _libraryDbContext.SaveChangesAsync();
            }

            _startTime = DateTime.MinValue;
            FilePath = string.Empty;
            ClearPopup();
            BookTitle = Progresss = CurrentChapterTitle = "--";
            Chapters.Clear();
            _settingsToolkit.DeleteLocalSetting(SettingNames.LastReadBookId);
        }

        private void SetChapterSelection()
        {
            foreach (var item in Chapters)
            {
                ChangeIsSelected(item, CurrentChapter);
            }

            static void ChangeIsSelected(ReaderChapterViewModel source, ReaderChapterViewModel target)
            {
                if (target != null)
                {
                    source.IsSelected = source.Chapter.Id == target.Chapter.Id;
                }

                if (source.Children?.Any() ?? false)
                {
                    foreach (var item in source.Children)
                    {
                        ChangeIsSelected(item, target);
                    }
                }
            }
        }

        private void WriteSetting(SettingNames name, object value)
            => _settingsToolkit.WriteLocalSetting(name, value);

        private void InitializeThemes()
        {
            Themes.Clear();
            LibraryViewModel.Instance.Themes.ForEach(p => Themes.Add(p));
        }

        private void InitializeColors()
        {
            Colors.Clear();

            // 紫色.
            Colors.Add("#595EFF");

            // 蓝色.
            Colors.Add("#1F7FEE");

            // 浅蓝色.
            Colors.Add("#00D8E8");

            // 绿色.
            Colors.Add("#00F878");

            // 草绿色.
            Colors.Add("#8FD243");

            // 橙色.
            Colors.Add("#FFAB00");

            // 红色.
            Colors.Add("#FF482D");

            // 粉色.
            Colors.Add("#EB3778");
        }

        private async Task InitializeHighlightsAsync()
        {
            Highlights.Clear();
            var highlights = await _libraryDbContext.Highlights.Where(p => p.BookId == _book.Id).ToListAsync();
            if (highlights.Any())
            {
                highlights.ForEach(highlight => Highlights.Add(highlight));
            }
        }

        private async Task ToggleFullScreenAsync()
        {
            if (!_isEnableToggleFullScreen)
            {
                return;
            }

            _isEnableToggleFullScreen = false;
            AppViewModel.Instance.IsFullScreen = !AppViewModel.Instance.IsFullScreen;
            await Task.Delay(300);
            _isEnableToggleFullScreen = true;
        }

        private async Task ToggleMiniViewAsync()
        {
            if (!_isEnableToggleMiniView)
            {
                return;
            }

            _isEnableToggleMiniView = false;
            AppViewModel.Instance.IsMiniView = !AppViewModel.Instance.IsMiniView;
            FontSize = AppViewModel.Instance.IsMiniView
                ? _settingsToolkit.ReadLocalSetting(SettingNames.MiniFontSize, 13d)
                : _settingsToolkit.ReadLocalSetting(SettingNames.FontSize, 22d);
            MiniViewRequested?.Invoke(this, EventArgs.Empty);
            await Task.Delay(300);
            _isEnableToggleMiniView = true;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _isShowCoverMask?.Dispose();
                    _isTranslating?.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
