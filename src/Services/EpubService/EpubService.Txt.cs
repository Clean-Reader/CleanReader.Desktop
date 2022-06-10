// Copyright (c) Richasy. All rights reserved.

using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CleanReader.Services.Epub
{
    /// <summary>
    /// EPUB 服务中进行 TXT 转换的部分.
    /// </summary>
    public partial class EpubService
    {
        private static bool IsTitle(string line, Regex matchRegex)
        {
            var result = matchRegex.IsMatch(line);
            if (result)
            {
                var title = matchRegex.Match(line).Value;
                if (title.Length > MAX_CHAPTER_TITLE_LENGTH)
                {
                    return false;
                }
            }

            return result;
        }

        private static bool IsExtra(string line)
        {
            if (line.Length > MAX_CHAPTER_TITLE_LENGTH)
            {
                return false;
            }

            return CHAPTER_EXTRA_KEYS.Any(k => line.StartsWith(k));
        }

        private static async Task GenerateHtmlFromTxtAsync(string title, string[] lines, int index, string folderPath = "")
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                folderPath = TXT_FOLDER_PATH;
            }

            var fileTitle = NormalizeTitle(title);
            var chapterTemplate = await File.ReadAllTextAsync(ASSETS_PAGE_PATH);
            chapterTemplate = chapterTemplate.Replace("{{title}}", fileTitle)
                                             .Replace("{{body}}", string.Join(string.Empty, lines.Select(p => $"<p>{p}</p>")));
            if (_chapterNames == null)
            {
                _chapterNames = new Dictionary<string, string>();
            }

            _chapterNames.Add(index.ToString("0000"), title);
            var filePath = Path.Combine(folderPath, $"{index:0000}.html");
            await File.WriteAllTextAsync(filePath, chapterTemplate);
        }

        private static async Task GenerateTitlePageFromTxtAsync(string title, string folderPath = "")
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                folderPath = TXT_FOLDER_PATH;
            }

            var titleTemplate = await File.ReadAllTextAsync(ASSETS_TITLE_PAGE_PATH);
            titleTemplate = titleTemplate.Replace("{{title}}", title);
            await File.WriteAllTextAsync(Path.Combine(folderPath, "titlepage.html"), titleTemplate);
        }

        private static async Task GenerateCleanReaderNamesAsync(string folderPath = "")
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                folderPath = TXT_FOLDER_PATH;
            }

            if (_chapterNames != null && _chapterNames.Count > 0)
            {
                var json = JsonConvert.SerializeObject(_chapterNames);
                var filePath = Path.Combine(folderPath, "crn.json");
                await File.WriteAllTextAsync(filePath, json);
            }
        }

        private static async Task LoadCleanReaderNamesAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                var text = await File.ReadAllTextAsync(filePath);
                _chapterNames = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
            }
            else
            {
                _chapterNames = new Dictionary<string, string>();
            }
        }

        private static string NormalizeTitle(string title)
        {
            title = title.Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("\"", "&quot;")
                    .Replace("\'", "&apos;");

            return title;
        }
    }
}
