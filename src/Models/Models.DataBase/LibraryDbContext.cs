// Copyright (c) Richasy. All rights reserved.

using Microsoft.EntityFrameworkCore;

namespace CleanReader.Models.DataBase;

/// <summary>
/// 书库数据库上下文.
/// </summary>
public class LibraryDbContext : DbContext
{
    private readonly string _dbPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryDbContext"/> class.
    /// </summary>
    public LibraryDbContext() => _dbPath = "meta.db";

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryDbContext"/> class.
    /// </summary>
    /// <param name="dbPath">数据库路径.</param>
    public LibraryDbContext(string dbPath) => _dbPath = dbPath;

    /// <summary>
    /// 书籍数据集.
    /// </summary>
    public DbSet<Book> Books { get; set; }

    /// <summary>
    /// 历史记录数据集.
    /// </summary>
    public DbSet<History> Histories { get; set; }

    /// <summary>
    /// 书架数据集.
    /// </summary>
    public DbSet<Shelf> Shelves { get; set; }

    /// <summary>
    /// 高亮数据集.
    /// </summary>
    public DbSet<Highlight> Highlights { get; set; }

    /// <summary>
    /// 元数据集.
    /// </summary>
    public DbSet<Meta> Metas { get; set; }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={_dbPath};");

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<History>()
            .HasMany(p => p.ReadSections)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Shelf>()
            .HasMany(p => p.Books)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
