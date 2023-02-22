// Copyright (c) Richasy. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CleanReader.Controls.Interfaces;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using VersOne.Epub;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 书库视图模型的导入书籍部分.
/// </summary>
public sealed partial class LibraryViewModel
{
    /// <inheritdoc/>
    public async Task<string> ImportLocalBookAsync()
    {
        IsImporting = true;
        var path = await _fileToolkit.OpenLocalFileAsync(_appViewModel.MainWindowHandle, ".txt", ".epub");
        if (!string.IsNullOrEmpty(path))
        {
            _importDialog?.Hide();
            var book = Path.GetExtension(path).Equals(".epub", StringComparison.OrdinalIgnoreCase)
                ? await GenerateBookEntryFromEpubFileAsync(path)
                : await GenerateBookEntryFromTxtFileAsync(path);
            await InsertBookEntryAsync(book);
        }

        IsImporting = false;
        return path;
    }

    /// <inheritdoc/>
    public async Task<Book> GenerateBookEntryFromTxtFileAsync(string path)
    {
        _tempSelectedFilePath = path;
        var code = await Locator.Lib.Locator.Instance.GetService<ITxtSplitDialog>().ShowAsync();
        Book entry = null;

        if (code == 0)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            // 生成Epub文件.
            var progressDialog = Locator.Lib.Locator.Instance.GetService<IProgressDialog>();
            progressDialog.InjectData(StringResources.ConvertingAndMovingFile);
            progressDialog.InjectTask(
                Task.Run(async () =>
                {
                    var r = string.IsNullOrEmpty(_tempSplitRegex) ? null : new Regex(_tempSplitRegex);
                    var configuration = await _epubService.SplitTxtFileAsync(_tempSelectedFilePath, r, cancellationTokenSource);
                    _epubService.SetConfiguration(configuration);
                    await _epubService.CreateAsync();
                    var generatePath = Path.Combine(configuration.OutputFolderPath, configuration.OutputFileName);
                    var destPath = Path.Combine(_rootDirectory.FullName, VMConstants.Library.BooksFolder, Path.GetFileName(generatePath));
                    await _fileToolkit.CopyAsync(generatePath, destPath, true);
                    entry = GetBookEntryFromLocalPath(destPath);
                }),
                cancellationTokenSource);

            await progressDialog.ShowAsync();
            _epubService.ClearGenerated();
        }

        _tempSplitRegex = null;
        _tempSelectedFilePath = null;
        return entry;
    }

    /// <inheritdoc/>
    public async Task<Book> GenerateBookEntryFromEpubFileAsync(string path)
    {
        // 直接导入.
        var destPath = Path.Combine(_rootDirectory.FullName, VMConstants.Library.BooksFolder, Path.GetFileName(path));
        var progressDialog = Locator.Lib.Locator.Instance.GetService<IProgressDialog>();
        progressDialog.InjectData(StringResources.MovingFile);
        progressDialog.InjectTask(_fileToolkit.CopyAsync(path, destPath));

        await progressDialog.ShowAsync();
        var book = GetBookEntryFromLocalPath(destPath);

        try
        {
            var epubReader = await EpubReader.ReadBookAsync(destPath);
            book.Title = epubReader.Title;
            book.Author = epubReader.Author;

            var coverImage = epubReader.CoverImage;
            book.Cover = await SaveCoverImageAsync(coverImage, $"{book.Id}.png");
        }
        catch (Exception)
        {
        }

        return book;
    }

    private static Book GetBookEntryFromLocalPath(string path)
    {
        var bookEntry = new Book
        {
            Title = Path.GetFileNameWithoutExtension(path),
            Type = BookType.Local,
            AddTime = DateTime.Now,
            Status = Models.DataBase.BookStatus.NotStart,
            Id = Guid.NewGuid().ToString("N"),
            Path = Path.GetFileName(path),
        };

        return bookEntry;
    }

    [RelayCommand]
    private async Task ShowImportDialogAsync()
    {
        _importDialog = Locator.Lib.Locator.Instance.GetService<IImportWayDialog>();
        await _importDialog.ShowAsync();
        _importDialog = null;
    }

    [RelayCommand]
    private async Task SplitChapterAsync(string regex)
    {
        SplitChapters.Clear();
        if (string.IsNullOrEmpty(_tempSelectedFilePath))
        {
            throw new Exception(StringResources.InvalidFilePath);
        }

        Regex r = null;
        if (!string.IsNullOrEmpty(regex))
        {
            r = new Regex(regex);
        }

        _tempSplitRegex = regex;
        var result = await _epubService.GenerateTxtChaptersAsync(_tempSelectedFilePath, r);
        result.Select(p => new SplitChapter() { Title = p.Item1, WordCount = p.Item2 })
            .ToList()
            .ForEach(p => SplitChapters.Add(p));
    }

    private async Task InsertBookEntryAsync(Book bookEntry)
    {
        if (!string.IsNullOrEmpty(bookEntry?.Path))
        {
            var isAlreadyInLocal = false;
            await Task.Run(async () =>
            {
                isAlreadyInLocal = await LibraryContext.Books.AnyAsync(p => p.Id == bookEntry.Id);
            });

            if (isAlreadyInLocal)
            {
                Book sourceEntry = null;

                await Task.Run(async () =>
                {
                    sourceEntry = await LibraryContext.Books.FirstOrDefaultAsync(p => p.Id == bookEntry.Id);
                });

                await UpdateBookEntryAsync(sourceEntry, bookEntry);
                return;
            }

            await Task.Run(async () =>
            {
                await LibraryContext.Books.AddAsync(bookEntry);
            });

            var shelfId = _settingsToolkit.ReadLocalSetting(SettingNames.ShelfId, string.Empty);
            if (!string.IsNullOrEmpty(shelfId))
            {
                Shelf sourceShelf = null;

                await Task.Run(async () =>
                {
                    sourceShelf = await LibraryContext.Shelves.FirstOrDefaultAsync(p => p.Id == shelfId);
                });
                if (sourceShelf != null)
                {
                    sourceShelf.Books.Add(new ShelfBook() { BookId = bookEntry.Id, ModifiedTime = DateTime.Now });
                    LibraryContext.Shelves.Update(sourceShelf);
                }
            }

            await Task.Run(async () =>
            {
                await LibraryContext.SaveChangesAsync();
            });

            _dispatcherQueue.TryEnqueue(() =>
            {
                BookAdded?.Invoke(this, EventArgs.Empty);
            });
        }
    }

    private async Task UpdateBookEntryAsync(Book sourceEntry, Book newEntry)
    {
        sourceEntry.Title = newEntry.Title;
        sourceEntry.Author = newEntry.Author;
        sourceEntry.Cover = newEntry.Cover;
        sourceEntry.Path = newEntry.Path;
        sourceEntry.Url = newEntry.Url;
        sourceEntry.LastChapterId = newEntry.LastChapterId;
        sourceEntry.Description = newEntry.Description;
        sourceEntry.SourceId = newEntry.SourceId;

        await Task.Run(async () =>
        {
            LibraryContext.Books.Update(sourceEntry);
            await LibraryContext.SaveChangesAsync();
        });
        _dispatcherQueue.TryEnqueue(() =>
        {
            BookAdded?.Invoke(this, EventArgs.Empty);
        });
    }

    private async Task<string> SaveCoverImageAsync(byte[] image, string name)
    {
        var coverFolder = Path.Combine(_rootDirectory.FullName, ".covers");
        if (!Directory.Exists(coverFolder))
        {
            Directory.CreateDirectory(coverFolder);
        }

        var destFile = Path.Combine(coverFolder, name);
        await File.WriteAllBytesAsync(destFile, image);
        return Path.GetFileName(destFile);
    }
}
