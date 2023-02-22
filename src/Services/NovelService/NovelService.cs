// Copyright (c) Richasy. All rights reserved.

using System.Text;
using System.Threading.Tasks.Dataflow;
using System.Web;
using CleanReader.Models.Services;
using CleanReader.Services.Interfaces;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace CleanReader.Services.Novel;

/// <summary>
/// 在线小说解析服务.
/// </summary>
public sealed partial class NovelService : INovelService
{
    private const string TextNode = "text";
    private const string OriginUrl = "ORIGIN_URL";
    private Dictionary<string, BookSource> _sources;

    /// <inheritdoc/>
    public async Task InitializeBookSourcesAsync(string sourceFolderPath)
    {
        _sources = new Dictionary<string, BookSource>();
        _httpClient = GetHttpClient();
        var files = Directory.GetFiles(sourceFolderPath);
        foreach (var fileName in files)
        {
            try
            {
                var content = await File.ReadAllTextAsync(fileName);
                var source = JsonConvert.DeserializeObject<BookSource>(content);
                _sources.Add(source.Id, source);
                Console.WriteLine($"已添加源：{source.Id}");
            }
            catch (Exception)
            {
                continue;
            }
        }
    }

    /// <inheritdoc/>
    public List<BookSource> GetBookSources() => _sources == null ?
            new List<BookSource>() :
            _sources.Select(p => p.Value).ToList();

    /// <inheritdoc/>
    public async Task<List<Book>> SearchBookAsync(string sourceId, string bookName)
    {
        if (_sources.ContainsKey(sourceId))
        {
            var source = _sources[sourceId];
            var searchConfig = source.Search;
            if (searchConfig == null || string.IsNullOrEmpty(searchConfig.SearchUrl))
            {
                throw new InvalidDataException($"{sourceId} 中没有搜索模块");
            }

            HtmlDocument doc;
            var encoding = string.IsNullOrEmpty(source.Charset) ? Encoding.UTF8 : Encoding.GetEncoding(source.Charset);
            if (searchConfig.Request != null)
            {
                doc = await GetHtmlDocumentAsync(searchConfig.SearchUrl, bookName, searchConfig.Request);
            }
            else
            {
                var keyword = searchConfig.EncodingKeyword
                    ? HttpUtility.UrlEncode(bookName, encoding)
                    : bookName;
                var searchUrl = searchConfig.SearchUrl + keyword;
                doc = await GetHtmlDocumentAsync(searchUrl, encoding);
            }

            var bookList = new List<Book>();
            var nodes = doc.DocumentNode.QuerySelectorAll(source.Search.Range);
            var tasks = new List<Task>();
            foreach (var node in nodes)
            {
                var book = new Book
                {
                    SourceId = sourceId,
                };
                try
                {
                    InitializeBook(book, searchConfig, node, out var statusStr);
                    if (string.IsNullOrEmpty(book.BookName))
                    {
                        continue;
                    }

                    FormatBook(book, searchConfig, ref statusStr);
                    ReplaceBook(book, searchConfig, ref statusStr);
                    RepairBook(book, searchConfig, ref statusStr);
                    EncodingBook(book, statusStr);

                    if (searchConfig.NeedDetail)
                    {
                        tasks.Add(Task.Run(() => InitializeBookDetailAsync(sourceId, book)));
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                Console.WriteLine($"已解析书籍：{book.BookName}");
                bookList.Add(book);
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }

            return bookList;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(sourceId), "没有找到指定的源.");
        }
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, List<Book>>> SearchBookAsync(string bookName)
    {
        var tasks = new List<Task>();
        var result = new Dictionary<string, List<Book>>();
        foreach (var source in _sources)
        {
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var books = await SearchBookAsync(source.Key, bookName);
                    result.Add(source.Key, books);
                }
                catch (Exception)
                {
                }
            }));
        }

        await Task.WhenAll(tasks);
        return result;
    }

    /// <inheritdoc/>
    public async Task<List<Chapter>> GetBookChaptersAsync(string sourceId, string bookUrl, CancellationTokenSource cancellationTokenSource)
    {
        var index = 0;
        var url = DecodingBase64ToString(bookUrl);
        var chapters = new List<Chapter>();

        if (!_sources.ContainsKey(sourceId))
        {
            throw new ArgumentOutOfRangeException(nameof(sourceId), "没有找到指定的源.");
        }

        var source = _sources[sourceId];
        if (source.Chapter == null)
        {
            throw new InvalidDataException($"{sourceId} 中没有章节目录模块");
        }

        var chapterConfig = source.Chapter;
        var encoding = string.IsNullOrEmpty(source.Charset) ? Encoding.UTF8 : Encoding.GetEncoding(source.Charset);
        var bookDetailHtml = await GetHtmlDocumentAsync(url, encoding, null, cancellationTokenSource);
        var nodes = bookDetailHtml.DocumentNode.QuerySelectorAll(chapterConfig.Range);
        if (chapterConfig.IsChildFiltered)
        {
            nodes = nodes.SelectMany(p => p.QuerySelectorAll(chapterConfig.ChildSelector));
        }

        foreach (var node in nodes)
        {
            var chapter = new Chapter
            {
                SourceId = source.Id,
            };
            InitializeChapter(chapter, chapterConfig, node);

            if (string.IsNullOrEmpty(chapter.Id))
            {
                continue;
            }

            RepairChapter(chapter, chapterConfig, url);
            ReplaceChapter(chapter, chapterConfig);
            FormatChapter(chapter, chapterConfig);
            if (!string.IsNullOrEmpty(chapter.Title) && !string.IsNullOrEmpty(chapter.Id))
            {
                index++;
                chapter.Index = index;
                if (chapters.Any(p => p.Id == chapter.Id))
                {
                    chapters.Remove(chapters.First(p => p.Id == chapter.Id));
                }

                chapters.Add(chapter);
            }
        }

        return chapters;
    }

    /// <inheritdoc/>
    public async Task<ChapterContent> GetChapterContentAsync(string sourceId, Chapter chapter, CancellationTokenSource cancellationTokenSource)
    {
        var url = DecodingBase64ToString(chapter.Id);

        if (!_sources.ContainsKey(sourceId))
        {
            throw new ArgumentOutOfRangeException(nameof(sourceId), "没有找到指定的源.");
        }

        var source = _sources[sourceId];
        if (source.ChapterContent == null)
        {
            throw new InvalidDataException($"{sourceId} 中没有章节内容模块");
        }

        var contentConfig = source.ChapterContent;
        var hasNextPage = false;
        var content = new ChapterContent
        {
            ChapterIndex = chapter.Index,
            Id = EncodingStringToBase64(url),
            Title = chapter.Title,
        };

        do
        {
            var encoding = string.IsNullOrEmpty(source.Charset) ? Encoding.UTF8 : Encoding.GetEncoding(source.Charset);
            var contentHtml = await GetHtmlDocumentAsync(url, encoding, null, cancellationTokenSource);
            var nodes = contentHtml.DocumentNode.QuerySelectorAll(contentConfig.Range);

            if (nodes.Any())
            {
                foreach (var node in nodes)
                {
                    InitializeChapterContent(content, contentConfig, node);
                    FormatChapterContent(content, contentConfig);
                    var nextPageUrl = GetNextPageUrl(contentConfig, contentHtml.DocumentNode);
                    hasNextPage = !string.IsNullOrEmpty(nextPageUrl);
                    if (hasNextPage)
                    {
                        url = nextPageUrl;
                    }
                }
            }
            else
            {
                content.Title = chapter.Title;
                content.Content = string.Empty;
            }
        }
        while (hasNextPage);

        return content;
    }

    /// <inheritdoc/>
    public async Task InitializeBookDetailAsync(string sourceId, Book book)
    {
        if (_sources.ContainsKey(sourceId))
        {
            var source = _sources[sourceId];
            if (source.BookDetail != null && source.IsBookDetailEnabled)
            {
                var url = DecodingBase64ToString(book.Url);
                var encoding = string.IsNullOrEmpty(source.Charset) ? Encoding.UTF8 : Encoding.GetEncoding(source.Charset);
                var doc = await GetHtmlDocumentAsync(url, encoding);
                var bookConfig = source.BookDetail;
                var node = doc.DocumentNode.QuerySelector(bookConfig.Range);
                try
                {
                    var statusStr = string.Empty;
                    var timeStr = string.Empty;
                    InitializeBook(book, bookConfig, node, out statusStr);
                    FormatBook(book, bookConfig, ref statusStr);
                    ReplaceBook(book, bookConfig, ref statusStr);
                    RepairBook(book, bookConfig, ref statusStr);
                    EncodingBook(book, statusStr, true);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                throw new InvalidOperationException("不支持获取书籍详情");
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(sourceId), "没有找到指定的源.");
        }
    }

    /// <inheritdoc/>
    public async Task<List<Book>> GetBooksWithCategoryAsync(string sourceId, string categoryName, int page = 1, CancellationTokenSource tokenSource = null)
    {
        if (!_sources.ContainsKey(sourceId))
        {
            throw new ArgumentOutOfRangeException(nameof(sourceId), "没有找到指定的源.");
        }

        var source = _sources[sourceId];
        if (source.IsExploreEnabled && source.Explore != null)
        {
            var category = source.Explore.Categories.Where(p => p.Name.Equals(categoryName)).FirstOrDefault();
            if (category == null)
            {
                throw new ArgumentOutOfRangeException($"分类 {categoryName} 不在配置中");
            }

            var url = category.Url.Replace("{{page}}", page.ToString());
            if (!url.StartsWith("http"))
            {
                url = source.WebUrl + url;
            }

            var encoding = string.IsNullOrEmpty(source.Charset) ? Encoding.UTF8 : Encoding.GetEncoding(source.Charset);
            var doc = await GetHtmlDocumentAsync(url, encoding, source.Explore.Request, tokenSource);
            var bookList = new List<Book>();
            var nodes = doc.DocumentNode.QuerySelectorAll(source.Explore.Range);
            var exploreConfig = source.Explore;

            var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 12, BoundedCapacity = DataflowBlockOptions.Unbounded };
            if (tokenSource != null)
            {
                options.CancellationToken = tokenSource.Token;
            }

            var action = new ActionBlock<Task>(async t =>
            {
                await t;
            });

            foreach (var node in nodes)
            {
                var book = new Book
                {
                    SourceId = sourceId,
                };
                var statusStr = string.Empty;
                try
                {
                    InitializeBook(book, exploreConfig, node, out statusStr);
                    if (string.IsNullOrEmpty(book.BookName))
                    {
                        continue;
                    }

                    FormatBook(book, exploreConfig, ref statusStr);
                    ReplaceBook(book, exploreConfig, ref statusStr);
                    RepairBook(book, exploreConfig, ref statusStr);
                    EncodingBook(book, statusStr);

                    if (exploreConfig.NeedDetail)
                    {
                        action.Post(InitializeBookDetailAsync(sourceId, book));
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                Console.WriteLine($"已解析书籍：{book.BookName}");
                bookList.Add(book);
            }

            action.Complete();
            if (action.InputCount > 0)
            {
                await action.Completion;
            }

            return bookList;
        }
        else
        {
            throw new InvalidOperationException($"{sourceId} 中不支持分类索引");
        }
    }
}
