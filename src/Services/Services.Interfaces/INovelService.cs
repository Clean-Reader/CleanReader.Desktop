// Copyright (c) Richasy. All rights reserved.

using CleanReader.Models.Services;

namespace CleanReader.Services.Interfaces;

/// <summary>
/// 在线网络小说服务的接口定义.
/// </summary>
public interface INovelService
{
    /// <summary>
    /// 初始化目前的可用书源.
    /// </summary>
    /// <param name="sourceFolderPath">书源文件夹地址.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeBookSourcesAsync(string sourceFolderPath);

    /// <summary>
    /// 获取全部的书源.
    /// </summary>
    /// <returns><see cref="BookSource"/> 列表.</returns>
    List<BookSource> GetBookSources();

    /// <summary>
    /// 根据书名查询书籍.
    /// </summary>
    /// <param name="sourceId">书源Id.</param>
    /// <param name="bookName">书名.</param>
    /// <returns><see cref="Book"/> 列表.</returns>
    /// <exception cref="InvalidDataException">书源定义中没有搜索模块.</exception>
    /// <exception cref="ArgumentOutOfRangeException">指定的源不存在.</exception>
    Task<List<Book>> SearchBookAsync(string sourceId, string bookName);

    /// <summary>
    /// 从不同源搜索书名.
    /// </summary>
    /// <param name="bookName">书名.</param>
    /// <returns>以书源分组的搜索结果.</returns>
    Task<Dictionary<string, List<Book>>> SearchBookAsync(string bookName);

    /// <summary>
    /// 获取书籍目录.
    /// </summary>
    /// <param name="sourceId">书源Id.</param>
    /// <param name="bookUrl">书籍地址.</param>
    /// <param name="cancellationTokenSource">中止令牌.</param>
    /// <returns>章节列表.</returns>
    /// <exception cref="ArgumentOutOfRangeException">书源不存在.</exception>
    /// <exception cref="InvalidDataException">书源定义中没有章节目录模块.</exception>
    Task<List<Chapter>> GetBookChaptersAsync(string sourceId, string bookUrl, CancellationTokenSource cancellationTokenSource);

    /// <summary>
    /// 获取章节内容.
    /// </summary>
    /// <param name="sourceId">书源Id.</param>
    /// <param name="chapter">章节实例.</param>
    /// <param name="cancellationTokenSource">中止令牌.</param>
    /// <returns>章节内容.</returns>
    /// <exception cref="ArgumentOutOfRangeException">书源不存在.</exception>
    /// <exception cref="InvalidDataException">指定源中没有章节内容模块.</exception>
    Task<ChapterContent> GetChapterContentAsync(string sourceId, Chapter chapter, CancellationTokenSource cancellationTokenSource);

    /// <summary>
    /// 初始化书籍详情.
    /// </summary>
    /// <param name="sourceId">书源Id.</param>
    /// <param name="book">书籍实例.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">书源不存在.</exception>
    /// <exception cref="InvalidOperationException">指定源中不支持获取章节详情.</exception>
    Task InitializeBookDetailAsync(string sourceId, Book book);

    /// <summary>
    /// 从分类中获取书籍列表.
    /// </summary>
    /// <param name="sourceId">源Id.</param>
    /// <param name="categoryName">分类名.</param>
    /// <param name="page">页码.</param>
    /// <param name="tokenSource">终止令牌.</param>
    /// <returns>书籍列表.</returns>
    /// <exception cref="ArgumentOutOfRangeException">源或配置不存在.</exception>
    /// <exception cref="InvalidOperationException">不支持该操作.</exception>
    Task<List<Book>> GetBooksWithCategoryAsync(string sourceId, string categoryName, int page = 1, CancellationTokenSource tokenSource = null);
}
