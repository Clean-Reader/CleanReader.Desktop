// Copyright (c) Richasy. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using CleanReader.Services.Novel;
using ReactiveUI;
using Windows.System;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 书架上的书籍视图模型.
    /// </summary>
    public sealed partial class ShelfBookViewModel : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShelfBookViewModel"/> class.
        /// </summary>
        /// <param name="book">书籍信息.</param>
        /// <param name="dbContextRef">书库上下文引用.</param>
        public ShelfBookViewModel(Book book, LibraryDbContext dbContextRef)
        {
            Book = book;
            Cover = book.Cover;
            _dbContext = dbContextRef;
            ServiceLocator.Instance.LoadService(out _fileToolkit)
                .LoadService(out _settingsToolkit);

            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync, outputScheduler: RxApp.MainThreadScheduler);
            OpenWithCommand = ReactiveCommand.CreateFromTask(OpenWithOtherAppAsync, outputScheduler: RxApp.MainThreadScheduler);
            ShowInformationCommand = ReactiveCommand.CreateFromTask(ShowInformationDialogAsync, outputScheduler: RxApp.MainThreadScheduler);
            ReadCommand = ReactiveCommand.Create(Read, outputScheduler: RxApp.MainThreadScheduler);
            ShowShelfTransferDialogCommand = ReactiveCommand.CreateFromTask(ShowShelfTransferDialogAsync, outputScheduler: RxApp.MainThreadScheduler);
            ShowReplaceSourceDialogCommand = ReactiveCommand.Create(ShowReplaceSourceDialog, outputScheduler: RxApp.MainThreadScheduler);
            OpenBookUrlCommand = ReactiveCommand.CreateFromTask(OpenBookUrlAsync, outputScheduler: RxApp.MainThreadScheduler);

            Initialize();
        }

        private void Initialize()
        {
            IsShowCover = !string.IsNullOrEmpty(Book.Cover) && (Book.Cover.StartsWith("http", StringComparison.OrdinalIgnoreCase) || Path.HasExtension(Book.Cover));
            IsOnlineBook = Book.Type == BookType.Online;
            var history = _dbContext.Histories.Where(p => p.BookId == Book.Id).FirstOrDefault();
            Progress = history == null ? StringResources.NotStarted : history.Progress.ToString("0.0") + "%";

            var rootPath = _settingsToolkit.ReadLocalSetting(SettingNames.LibraryPath, string.Empty);
            LocalPath = Path.Combine(rootPath, VMConstants.Library.BooksFolder, Book.Path);

            StatusIcon = Book.Status switch
            {
                BookStatus.NotStart => "\uE8BF",
                BookStatus.Reading => "\uE736",
                BookStatus.Finish => "\uE930",
                _ => throw new System.NotImplementedException(),
            };
        }

        private async Task DeleteAsync()
        {
            var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.ConfirmDialog);
            dialog.InjectData(StringResources.DeleteBookWarning);
            var result = await dialog.ShowAsync();
            if (result == 0)
            {
                var progressDialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.ProgressDialog);
                progressDialog.InjectData(StringResources.DeletingBook);
                progressDialog.InjectTask(Task.Run(async () =>
                {
                    // 删除数据库记录.
                    _dbContext.Books.Remove(_dbContext.Books.FirstOrDefault(p => p.Id == Book.Id));
                    _dbContext.Histories.RemoveRange(_dbContext.Histories.Where(p => p.BookId == Book.Id));
                    _dbContext.Highlights.RemoveRange(_dbContext.Highlights.Where(p => p.BookId == Book.Id));
                    var shelf = _dbContext.Shelves.Where(p => p.Books.Any(j => j.BookId == Book.Id)).FirstOrDefault();
                    if (shelf != null)
                    {
                        shelf.Books.Remove(shelf.Books.FirstOrDefault(p => p.BookId == Book.Id));
                        _dbContext.Shelves.Update(shelf);
                    }

                    await _dbContext.SaveChangesAsync();

                    // 删除书库内的文件.
                    await _fileToolkit.DeleteAsync(LocalPath);
                }));

                await progressDialog.ShowAsync();
                ShelfPageViewModel.Instance.InitializeBooksCommand.Execute().Subscribe();
            }
        }

        private async Task OpenWithOtherAppAsync()
        {
            var options = new LauncherOptions
            {
                DisplayApplicationPicker = true,
            };
            await Launcher.LaunchUriAsync(new Uri(LocalPath), options);
        }

        private async Task ShowInformationDialogAsync()
        {
            var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.BookInformationDialog);
            dialog.InjectData(this);
            var result = await dialog.ShowAsync();
            if (result == 0)
            {
                Book.Path = Path.GetFileName(Book.Path);
                Book.Cover = Cover;
                _dbContext.Books.Update(Book);
                await _dbContext.SaveChangesAsync();
                var newBook = _dbContext.Books.FirstOrDefault(p => p.Id == Book.Id);
                Book = newBook;
                Initialize();
            }
        }

        private void ShowReplaceSourceDialog()
            => LibraryViewModel.Instance.ShowReplaceSourceDialogCommand.Execute(Book).Subscribe();

        private async Task ShowShelfTransferDialogAsync()
        {
            var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.ShelfTransferDialog);
            dialog.InjectData(Book);
            await dialog.ShowAsync();
        }

        private async Task OpenBookUrlAsync()
        {
            var uri = NovelService.DecodingBase64ToString(Book.Url);
            if (!uri.StartsWith("http"))
            {
                if (uri.StartsWith("//"))
                {
                    uri = "https" + uri;
                }
                else
                {
                    var source = LibraryViewModel.Instance.BookSources.FirstOrDefault(p => p.Id == Book.SourceId);
                    if (source != null && !string.IsNullOrEmpty(source.WebUrl))
                    {
                        uri = source.WebUrl.TrimEnd('/') + '/' + uri.TrimStart('/');
                    }
                }
            }

            try
            {
                await Launcher.LaunchUriAsync(new Uri(uri));
            }
            catch (Exception)
            {
                return;
            }
        }

        private void Read() => AppViewModel.Instance.RequestRead(Book);
    }
}
