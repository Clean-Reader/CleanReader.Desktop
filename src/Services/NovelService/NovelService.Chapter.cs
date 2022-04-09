// Copyright (c) Richasy. All rights reserved.

using System.Text.RegularExpressions;
using CleanReader.Services.Novel.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace CleanReader.Services.Novel
{
    /// <summary>
    /// 在线小说服务.
    /// </summary>
    public partial class NovelService
    {
        private static void InitializeChapter(Chapter chapter, ChapterConfig chapterConfig, HtmlNode node)
        {
            var titleNode = node.QuerySelector(chapterConfig.Title?.Rule);

            if (titleNode != null)
            {
                chapter.Title = chapterConfig.Title.Type == TextNode ?
                    titleNode.InnerText :
                    titleNode.GetAttributeValue(chapterConfig.Title.Type, string.Empty);
            }

            if (chapterConfig.Url != null)
            {
                if (string.IsNullOrEmpty(chapterConfig.Url.Rule))
                {
                    chapter.Id = node.GetAttributeValue(chapterConfig.Url.Type, string.Empty);
                }
                else
                {
                    var urlNode = node.QuerySelector(chapterConfig.Url.Rule);
                    if (urlNode != null)
                    {
                        chapter.Id = chapterConfig.Url.Type == TextNode
                        ? urlNode.InnerText
                        : urlNode.GetAttributeValue(chapterConfig.Url.Type, string.Empty);
                    }
                }
            }
        }

        private static void RepairChapter(Chapter chapter, ChapterConfig chapterConfig, string originUrl)
        {
            if (chapterConfig.Repair != null && chapterConfig.Repair.Any())
            {
                foreach (var repair in chapterConfig.Repair)
                {
                    if (repair.Field == Enums.FieldType.Url)
                    {
                        chapter.Id = repair.Position.Equals("r", StringComparison.OrdinalIgnoreCase)
                            ? repair.Value == OriginUrl
                            ? chapter.Id + originUrl
                            : chapter.Id + repair.Value
                            : repair.Value == OriginUrl
                            ? originUrl + chapter.Id
                            : repair.Value + chapter.Id;
                    }
                    else if (repair.Field == Enums.FieldType.Title)
                    {
                        chapter.Title = repair.Position.Equals("r", StringComparison.OrdinalIgnoreCase)
                            ? repair.Value == OriginUrl
                            ? chapter.Title + originUrl
                            : chapter.Title + repair.Value
                            : repair.Value == OriginUrl
                            ? originUrl + chapter.Title
                            : repair.Value + chapter.Title;
                    }
                }
            }
        }

        private static void ReplaceChapter(Chapter chapter, ChapterConfig chapterConfig)
        {
            if (chapterConfig.Replace != null && chapterConfig.Replace.Any())
            {
                foreach (var replace in chapterConfig.Replace)
                {
                    if (replace.Field == Enums.FieldType.Url && !string.IsNullOrEmpty(replace.Old))
                    {
                        chapter.Id = Regex.Replace(chapter.Id, replace.Old, replace.New);
                    }
                    else if (replace.Field == Enums.FieldType.Title)
                    {
                        chapter.Title = Regex.Replace(chapter.Id, replace.Old, replace.New);
                    }
                }
            }
        }

        private static void FormatChapter(Chapter chapter, ChapterConfig chapterConfig)
        {
            if (chapterConfig.Title != null)
            {
                chapter.Title = FormatString(chapter.Title, chapterConfig.Title?.Filter);
            }

            if (chapterConfig.Url != null)
            {
                chapter.Id = FormatString(chapter.Id, chapterConfig.Url?.Filter);
            }

            chapter.Id = EncodingStringToBase64(chapter.Id);
        }
    }
}
