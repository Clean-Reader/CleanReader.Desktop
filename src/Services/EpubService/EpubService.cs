// Copyright (c) Richasy. All rights reserved.

using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using CleanReader.Models.Services;
using CleanReader.Services.Interfaces;
using Ionic.Zip;

namespace CleanReader.Services.Epub;

/// <summary>
/// EPUB 服务.
/// </summary>
public sealed partial class EpubService : IEpubService
{
    private static Dictionary<string, string> _chapterNames;
    private string[] _files;

    private EpubServiceConfiguration Configuration { get; set; }

    /// <inheritdoc/>
    public async Task<EpubServiceConfiguration> SplitTxtFileAsync(string filePath, Regex splitRegex = null, CancellationTokenSource cancellationTokenSource = null)
    {
        ClearCache();
        splitRegex ??= CHAPTER_DIVISION_REGEX;

        var file = new FileInfo(filePath);
        if (file.Exists)
        {
            if (!Directory.Exists(TXT_FOLDER_PATH))
            {
                Directory.CreateDirectory(TXT_FOLDER_PATH);
            }

            _chapterNames?.Clear();
            var title = Path.GetFileNameWithoutExtension(filePath);
            var encoding = EncodingHelper.DetectFileEncoding(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.UTF8);
            var lines = await File.ReadAllLinesAsync(filePath, encoding);
            var filterLines = new List<string>();
            var lastTitle = title;
            var chapterTitle = string.Empty;
            var chapterIndex = 0;

            await GenerateTitlePageFromTxtAsync(title);

            var lineAction = GetActionBlock(cancellationTokenSource);

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    filterLines.Add(line);
                    continue;
                }

                if (IsExtra(line))
                {
                    chapterTitle = NormalizeTitle(line);
                }
                else if (IsTitle(line, splitRegex))
                {
                    chapterTitle = NormalizeTitle(splitRegex.Match(line).Value);
                }

                if (!string.IsNullOrEmpty(chapterTitle))
                {
                    if (!string.IsNullOrEmpty(lastTitle))
                    {
                        var t = lastTitle.ToString();
                        var arr = filterLines.ToArray();
                        var index = chapterIndex;
                        lineAction.Post(GenerateHtmlFromTxtAsync(t, arr, index));
                        chapterIndex++;
                        filterLines.Clear();
                    }

                    lastTitle = chapterTitle;
                    chapterTitle = string.Empty;
                }
                else
                {
                    filterLines.Add(line);
                }
            }

            lineAction.Complete();
            await lineAction.Completion;

            if (filterLines.Count > 0 && !string.IsNullOrEmpty(chapterTitle))
            {
                await GenerateHtmlFromTxtAsync(chapterTitle, filterLines.ToArray(), chapterIndex);
            }

            if (!Directory.Exists(GENERATE_FOLDER_PATH))
            {
                Directory.CreateDirectory(GENERATE_FOLDER_PATH);
            }

            var configuration = new EpubServiceConfiguration()
            {
                Title = title,
                Author = string.Empty,
                SourceFolderPath = TXT_FOLDER_PATH,
                Language = "zh",
                OutputFileName = $"{title}.epub",
                OutputFolderPath = GENERATE_FOLDER_PATH,
                TitlePagePath = $"{TXT_FOLDER_PATH}titlepage.html",
            };
            return configuration;
        }
        else
        {
            throw new ArgumentException($"指定文件 {filePath} 不存在.");
        }
    }

    /// <inheritdoc/>
    public async Task<List<Tuple<string, int>>> GenerateTxtChaptersAsync(string filePath, Regex splitRegex = null)
    {
        if (splitRegex == null)
        {
            splitRegex = CHAPTER_DIVISION_REGEX;
        }

        var file = new FileInfo(filePath);
        if (file.Exists)
        {
            var title = Path.GetFileNameWithoutExtension(filePath);
            var encoding = EncodingHelper.DetectFileEncoding(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.UTF8);
            var lines = await File.ReadAllLinesAsync(filePath, encoding);
            var filterLines = new List<string>();
            var lastTitle = title;
            var chapterTitle = string.Empty;
            var chapterIndex = 0;
            var result = new List<Tuple<string, int>>();

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    filterLines.Add(line);
                    continue;
                }

                if (IsExtra(line))
                {
                    chapterTitle = NormalizeTitle(line);
                }
                else if (IsTitle(line, splitRegex))
                {
                    chapterTitle = NormalizeTitle(splitRegex.Match(line).Value);
                }

                if (!string.IsNullOrEmpty(chapterTitle))
                {
                    if (!string.IsNullOrEmpty(lastTitle))
                    {
                        var t = lastTitle.ToString();
                        var arr = filterLines.ToArray();
                        var index = chapterIndex;
                        result.Add(new Tuple<string, int>(t, arr.Sum(p => p.Length)));
                        chapterIndex++;
                        filterLines.Clear();
                    }

                    lastTitle = chapterTitle;
                    chapterTitle = string.Empty;
                }
                else
                {
                    filterLines.Add(line);
                }
            }

            if (filterLines.Count > 0 && !string.IsNullOrEmpty(chapterTitle))
            {
                result.Add(new Tuple<string, int>(chapterTitle, filterLines.Sum(p => p.Length)));
            }

            return result;
        }
        else
        {
            throw new ArgumentException($"指定文件 {filePath} 不存在.");
        }
    }

    /// <inheritdoc/>
    public async Task<EpubServiceConfiguration> InitializeSplitedBookAsync(List<Tuple<int, string, string>> data, EpubServiceConfiguration configuration)
    {
        if (!Directory.Exists(TXT_FOLDER_PATH))
        {
            Directory.CreateDirectory(TXT_FOLDER_PATH);
        }

        _chapterNames?.Clear();
        configuration.Title = NormalizeTitle(configuration.Title);
        await GenerateTitlePageFromTxtAsync(configuration.Title);

        var action = GetActionBlock();

        foreach (var item in data)
        {
            var title = NormalizeTitle(item.Item2);
            action.Post(GenerateHtmlFromTxtAsync(title, item.Item3?.Split('\n'), item.Item1));
        }

        action.Complete();
        await action.Completion;

        if (!Directory.Exists(GENERATE_FOLDER_PATH))
        {
            Directory.CreateDirectory(GENERATE_FOLDER_PATH);
        }

        await GenerateCleanReaderNamesAsync(TXT_FOLDER_PATH);
        configuration.SourceFolderPath = TXT_FOLDER_PATH;
        configuration.TitlePagePath = $"{TXT_FOLDER_PATH}titlepage.html";

        return configuration;
    }

    /// <inheritdoc/>
    public async Task<EpubServiceConfiguration> InitializeAdditionalChaptersAsync(string sourceBookPath, List<Tuple<int, string, string>> data, EpubServiceConfiguration configuration)
    {
        var guid = Guid.NewGuid().ToString("N");
        var folderPath = Path.Combine(TXT_FOLDER_PATH, guid);
        var pagesPath = Path.Combine(folderPath, "OEBPS\\Pages\\");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        using var zipFile = new ZipFile(sourceBookPath, Encoding.UTF8);
        zipFile.ExtractSelectedEntries("type = F", "OEBPS/Pages", folderPath);
        await LoadCleanReaderNamesAsync(Path.Combine(pagesPath, "crn.json"));
        var action = GetActionBlock();
        foreach (var item in data)
        {
            action.Post(GenerateHtmlFromTxtAsync(item.Item2, item.Item3?.Split('\n'), item.Item1, pagesPath));
        }

        action.Complete();
        await action.Completion;
        if (!Directory.Exists(GENERATE_FOLDER_PATH))
        {
            Directory.CreateDirectory(GENERATE_FOLDER_PATH);
        }

        await GenerateCleanReaderNamesAsync(pagesPath);

        configuration.SourceFolderPath = pagesPath;
        configuration.TitlePagePath = $"{Path.Combine(folderPath, "OEBPS/Pages/titlepage.html")}";

        zipFile.Dispose();
        return configuration;
    }

    /// <inheritdoc/>
    public void SetConfiguration(EpubServiceConfiguration configuration)
        => Configuration = configuration;

    /// <inheritdoc/>
    public bool NeedRegenerate(string sourceFile)
    {
        using var zipFile = new ZipFile(sourceFile, Encoding.UTF8);
        var result = !zipFile.EntryFileNames.Any(p => p.Contains("crn.json"));
        zipFile.Dispose();
        return result;
    }

    /// <inheritdoc/>
    public void ClearCache()
    {
        if (Directory.Exists(TXT_FOLDER_PATH))
        {
            Directory.Delete(TXT_FOLDER_PATH, true);
        }

        if (Directory.Exists(TEMP_FOLDER_PATH))
        {
            Directory.Delete(TEMP_FOLDER_PATH, true);
        }
    }

    /// <inheritdoc/>
    public void ClearGenerated()
    {
        if (Directory.Exists(GENERATE_FOLDER_PATH))
        {
            Directory.Delete(GENERATE_FOLDER_PATH, true);
        }
    }

    /// <inheritdoc/>
    public async Task CreateAsync()
    {
        GetFiles();
        CreateTempFolder();
        CopyStyleSheet();
        CopyContainerFile();
        await Task.WhenAll(
            Task.Run(() => CreateMimeTypeFileAsync()),
            Task.Run(() => CreateOpfFileAsync()),
            Task.Run(() => CreateNcxFileAsync()));

        CreateZipFile();
        ClearCache();
    }

    private static ActionBlock<Task> GetActionBlock(CancellationTokenSource cancellationTokenSource = null)
    {
        var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 12, BoundedCapacity = DataflowBlockOptions.Unbounded };

        if (cancellationTokenSource != null)
        {
            options.CancellationToken = cancellationTokenSource.Token;
        }

        var action = new ActionBlock<Task>(async t =>
        {
            await t;
        });

        return action;
    }
}
