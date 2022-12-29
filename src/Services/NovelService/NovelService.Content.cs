// Copyright (c) Richasy. All rights reserved.

using System.Text.RegularExpressions;
using CleanReader.Services.Novel.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace CleanReader.Services.Novel;

/// <summary>
/// 在线小说解析服务.
/// </summary>
public partial class NovelService
{
    private static void InitializeChapterContent(ChapterContent content, ChapterContentConfig contentConfig, HtmlNode node)
    {
        var docNode = GetSingleNodeFromRule(node, contentConfig.Content?.Rule);

        if (docNode != null)
        {
            content.Content += contentConfig.Content.Type == TextNode ?
                NormalizeContent(docNode.InnerHtml) :
                docNode.GetAttributeValue(contentConfig.Content.Type, string.Empty);
        }
    }

    private static void FormatChapterContent(ChapterContent content, ChapterContentConfig contentConfig)
    {
        content.Content = content.Content.Replace("<br>", "\n");
        content.Content = FormatString(content.Content, contentConfig.Content?.Filter);
    }

    private static string GetNextPageUrl(ChapterContentConfig contentConfig, HtmlNode node)
    {
        if (contentConfig.NextPage != null && !string.IsNullOrEmpty(contentConfig.NextPage.Range))
        {
            var nextConfig = contentConfig.NextPage;
            var nextNode = node.QuerySelector(nextConfig.Range);
            var result = string.Empty;
            if (nextNode != null)
            {
                var value = nextConfig.Type == TextNode ?
                    nextNode.InnerText :
                    nextNode.GetAttributeValue(nextConfig.Type, string.Empty);
                FormatString(value, nextConfig.Filter);
                foreach (var repair in nextConfig.Repair)
                {
                    value = RepairString(value, repair.Position == "l", repair.Value);
                }

                if (nextConfig.Match != null)
                {
                    var matchNode = node.QuerySelector(nextConfig.Match.Rule);
                    if (matchNode != null)
                    {
                        var matchText = nextConfig.Match.Type == TextNode ?
                            matchNode.InnerText :
                            matchNode.GetAttributeValue(nextConfig.Match.Type, string.Empty);
                        FormatString(value, nextConfig.Match.Filter);

                        try
                        {
                            if (Regex.IsMatch(matchText, nextConfig.Match.MatchRule))
                            {
                                result = value;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else
                {
                    result = value;
                }
            }

            return result;
        }

        return string.Empty;
    }
}
