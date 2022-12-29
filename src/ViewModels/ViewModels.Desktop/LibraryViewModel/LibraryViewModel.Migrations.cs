// Copyright (c) Richasy. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using Windows.ApplicationModel;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 书库视图模型.
/// </summary>
public sealed partial class LibraryViewModel
{
    /// <summary>
    /// 迁移数据.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task MigrationAsync()
    {
        var rootPath = _settingsToolkit.ReadLocalSetting(SettingNames.LibraryPath, string.Empty);
        var context = new LibraryDbContext(Path.Combine(rootPath, VMConstants.Library.DbFile));
        var books = await context.Books.ToListAsync();
        var histories = await context.Histories.Include(p => p.ReadSections).ToListAsync();
        var shelves = await context.Shelves.Include(p => p.Books).ToListAsync();
        var highLights = await context.Highlights.ToListAsync();
        var metas = await context.Metas.ToListAsync();
        metas.FirstOrDefault(p => p.Key == "Version").Value = AppConstants.DbVersion;

        if (LibraryContext != null)
        {
            LibraryContext.Dispose();
            LibraryContext = null;
        }

        context.Database.EnsureDeleted();
        context.Dispose();
        context = null;

        await Task.Delay(1000);
        GC.WaitForPendingFinalizers();
        await _fileToolkit.CopyAsync(Path.Combine(Package.Current.InstalledPath, "Assets/LibraryInitialize/meta.db"), Path.Combine(rootPath, VMConstants.Library.DbFile), true);
        using var context2 = new LibraryDbContext(Path.Combine(rootPath, VMConstants.Library.DbFile));
        await context2.Books.AddRangeAsync(books);
        await context2.Histories.AddRangeAsync(histories);
        await context2.Shelves.AddRangeAsync(shelves);
        await context2.Highlights.AddRangeAsync(highLights);
        await context2.Metas.AddRangeAsync(metas);
        context2.SaveChanges();
        Migrated?.Invoke(this, EventArgs.Empty);
    }

    private async Task<MigrationResult> CheckMigrationsAsync()
    {
        var result = MigrationResult.Matched;
        var version = await LibraryContext.Metas.FirstOrDefaultAsync(p => p.Key == "Version");
        if (version == null)
        {
            // 添加初始版本标识.
            LibraryContext.Metas.Add(new Meta { Key = "Version", Value = AppConstants.DbVersion });
            LibraryContext.Metas.Add(new Meta { Key = "InitializeTime", Value = DateTime.Now.ToString() });
            await LibraryContext.SaveChangesAsync();
        }
        else
        {
            var versioNum = Convert.ToInt32(version.Value);
            var appNum = Convert.ToInt32(AppConstants.DbVersion);

            // 检查是否有数据更新.
            if (versioNum > appNum)
            {
                result = MigrationResult.ShouldUpdateApp;
            }
            else if (versioNum < appNum)
            {
                result = MigrationResult.ShouldUpdateDataBase;
            }
        }

        return result;
    }
}
