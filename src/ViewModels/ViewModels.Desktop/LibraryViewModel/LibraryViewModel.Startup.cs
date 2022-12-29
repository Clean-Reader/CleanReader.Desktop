// Copyright (c) Richasy. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using Windows.ApplicationModel;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 书库视图模型的启动部分.
/// </summary>
public sealed partial class LibraryViewModel
{
    private async Task CopyDefaultBookSourcesAsync(string rootPath)
    {
        var defaultSourceFolder = Path.Combine(Package.Current.InstalledPath, "Assets/LibraryInitialize/BookSources");
        var sourceFolder = new DirectoryInfo(defaultSourceFolder);
        var destFolder = new DirectoryInfo(Path.Combine(rootPath, VMConstants.Library.BookSourceFolder));
        if (!destFolder.Exists)
        {
            Directory.CreateDirectory(destFolder.FullName);
        }

        foreach (var file in sourceFolder.GetFiles())
        {
            await _fileToolkit.CopyAsync(file.FullName, Path.Combine(destFolder.FullName, file.Name), true);
        }
    }

    private async Task CopyDefaultThemeFileAsync(string rootPath)
    {
        var defaultFilePath = Path.Combine(Package.Current.InstalledPath, "Assets/LibraryInitialize/theme.json");
        var destFilePath = Path.Combine(rootPath, VMConstants.Library.ThemeFile);
        await _fileToolkit.CopyAsync(defaultFilePath, destFilePath, true);
    }

    private async Task OpenLibraryFolderAsync()
    {
        RemoveException();
        var folder = await _fileToolkit.OpenFolderAsync(AppViewModel.Instance.MainWindowHandle);
        if (!string.IsNullOrEmpty(folder))
        {
            var directory = new DirectoryInfo(folder);
            var files = directory.GetFiles();
            var hasDb = files.Any(p => p.Name.Equals(VMConstants.Library.DbFile));
            if (!hasDb)
            {
                throw new Exception(StringResources.LibraryDoNotHaveRequiredFiles);
            }

            var hasTheme = files.Any(p => p.Name.Equals(VMConstants.Library.ThemeFile));
            if (!hasTheme)
            {
                await CopyDefaultThemeFileAsync(folder);
            }

            var subDirectories = directory.GetDirectories();
            var hasSourceFolder = subDirectories.Any(p => p.Name.Equals(VMConstants.Library.BookSourceFolder));
            if (!hasSourceFolder)
            {
                await CopyDefaultBookSourcesAsync(folder);
            }

            var hasBooksFolder = subDirectories.Any(p => p.Name.Equals(VMConstants.Library.BooksFolder));
            if (!hasBooksFolder)
            {
                directory.CreateSubdirectory(VMConstants.Library.BooksFolder);
            }

            if (LibraryContext != null)
            {
                await LibraryContext.DisposeAsync();
            }

            LibraryContext = new LibraryDbContext(Path.Combine(folder, VMConstants.Library.DbFile));
            _settingsToolkit.WriteLocalSetting(SettingNames.LibraryPath, folder);

            DispatcherQueue.TryEnqueue(() =>
            {
                LibraryInitialized?.Invoke(this, EventArgs.Empty);
            });
        }
    }

    private async Task CreateLibraryFolderAsync()
    {
        RemoveException();
        var folder = await _fileToolkit.OpenFolderAsync(AppViewModel.Instance.MainWindowHandle);

        if (Directory.GetFiles(folder).Length > 0 || Directory.GetDirectories(folder).Length > 0)
        {
            throw new Exception(StringResources.TargetFolderShouldEmpty);
        }

        await Task.WhenAll(
            CopyDefaultThemeFileAsync(folder),
            CopyDefaultBookSourcesAsync(folder),
            _fileToolkit.CopyAsync(Path.Combine(Package.Current.InstalledPath, "Assets/LibraryInitialize/meta.db"), Path.Combine(folder, VMConstants.Library.DbFile), true));
        new DirectoryInfo(folder).CreateSubdirectory(VMConstants.Library.BooksFolder);

        _settingsToolkit.WriteLocalSetting(SettingNames.LibraryPath, folder);
        DispatcherQueue.TryEnqueue(() =>
        {
            LibraryInitialized?.Invoke(this, EventArgs.Empty);
        });
    }

    private void ClearCurentCache()
    {
        _rootDirectory = null;
        _settingsToolkit.DeleteLocalSetting(SettingNames.LibraryPath);
        LibraryContext?.Dispose();
        LibraryContext = null;
        _novelService = null;
    }
}
