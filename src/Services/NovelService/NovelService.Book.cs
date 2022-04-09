// Copyright (c) Richasy. All rights reserved.

using CleanReader.Services.Novel.Models;
using HtmlAgilityPack;

namespace CleanReader.Services.Novel
{
    /// <summary>
    /// 在线小说解析服务.
    /// </summary>
    public sealed partial class NovelService
    {
        private static void InitializeBook(Book book, BookInformationConfigBase infoConfig, HtmlNode node, out string statusString)
        {
            var nameNode = GetSingleNodeFromRule(node, infoConfig.BookName?.Rule);
            var urlNode = GetSingleNodeFromRule(node, infoConfig.BookUrl?.Rule);
            var coverNode = GetSingleNodeFromRule(node, infoConfig.BookCover?.Rule);
            var authorNode = GetSingleNodeFromRule(node, infoConfig.BookAuthor?.Rule);
            var descNode = GetSingleNodeFromRule(node, infoConfig.BookDescription?.Rule);
            var statusNode = GetSingleNodeFromRule(node, infoConfig.BookStatus?.Rule);
            var lastChapterTitleNode = GetSingleNodeFromRule(node, infoConfig.LastChapterTitle?.Rule);
            var lastChapterUrlNode = GetSingleNodeFromRule(node, infoConfig.LastChapterUrl?.Rule);
            var categoryNode = GetSingleNodeFromRule(node, infoConfig.Category?.Rule);
            var tagNode = GetSingleNodeFromRule(node, infoConfig.Tag?.Rule);
            var updateTimeNode = GetSingleNodeFromRule(node, infoConfig.UpdateTime?.Rule);

            statusString = string.Empty;

            if (nameNode != null)
            {
                book.BookName = infoConfig.BookName.Type == TextNode ?
                    nameNode.InnerText :
                    nameNode.GetAttributeValue(infoConfig.BookName.Type, string.Empty);
            }

            if (urlNode != null)
            {
                book.Url = infoConfig.BookUrl.Type == TextNode ?
                    urlNode.InnerText :
                    urlNode.GetAttributeValue(infoConfig.BookUrl.Type, string.Empty);
            }

            if (coverNode != null)
            {
                book.CoverUrl = infoConfig.BookCover.Type == TextNode ?
                    coverNode.InnerText :
                    coverNode.GetAttributeValue(infoConfig.BookCover.Type, string.Empty);
            }

            if (authorNode != null)
            {
                book.Author = infoConfig.BookAuthor.Type == TextNode ?
                    authorNode.InnerText :
                    authorNode.GetAttributeValue(infoConfig.BookAuthor.Type, string.Empty);
            }

            if (descNode != null)
            {
                book.Description = infoConfig.BookDescription.Type == TextNode ?
                    descNode.InnerText :
                    descNode.GetAttributeValue(infoConfig.BookDescription.Type, string.Empty);
            }

            if (statusNode != null)
            {
                statusString = infoConfig.BookStatus.Type == TextNode ?
                    statusNode.InnerText :
                    statusNode.GetAttributeValue(infoConfig.BookStatus.Type, string.Empty);
            }

            if (lastChapterTitleNode != null)
            {
                book.LatestChapterTitle = infoConfig.LastChapterTitle.Type == TextNode ?
                    lastChapterTitleNode.InnerText :
                    lastChapterTitleNode.GetAttributeValue(infoConfig.LastChapterTitle.Type, string.Empty);
            }

            if (lastChapterUrlNode != null)
            {
                book.LatestChapterId = infoConfig.LastChapterUrl.Type == TextNode ?
                    lastChapterUrlNode.InnerText :
                    lastChapterUrlNode.GetAttributeValue(infoConfig.LastChapterUrl.Type, string.Empty);
            }

            if (categoryNode != null)
            {
                book.Category = infoConfig.Category.Type == TextNode ?
                    categoryNode.InnerText :
                    categoryNode.GetAttributeValue(infoConfig.Category.Type, string.Empty);
            }

            if (tagNode != null)
            {
                book.Tag = infoConfig.Tag.Type == TextNode ?
                    tagNode.InnerText :
                    tagNode.GetAttributeValue(infoConfig.Tag.Type, string.Empty);
            }

            if (updateTimeNode != null)
            {
                book.UpdateTime = infoConfig.UpdateTime.Type == TextNode ?
                    updateTimeNode.InnerText :
                    updateTimeNode.GetAttributeValue(infoConfig.UpdateTime.Type, string.Empty);
            }
        }

        private static void FormatBook(Book book, BookInformationConfigBase infoConfig, ref string statusString)
        {
            book.BookName = FormatString(book.BookName, infoConfig.BookName?.Filter);
            book.Url = FormatString(book.Url, infoConfig.BookUrl?.Filter);
            book.CoverUrl = FormatString(book.CoverUrl, infoConfig.BookCover?.Filter);
            book.Author = FormatString(book.Author, infoConfig.BookAuthor?.Filter);
            book.Description = FormatString(book.Description, infoConfig.BookDescription?.Filter);
            book.LatestChapterTitle = FormatString(book.LatestChapterTitle, infoConfig.LastChapterTitle?.Filter);
            book.LatestChapterId = FormatString(book.LatestChapterId, infoConfig.LastChapterUrl?.Filter);
            book.Category = FormatString(book.Category, infoConfig.Category?.Filter);
            book.Tag = FormatString(book.Tag, infoConfig.Tag?.Filter);
            statusString = FormatString(statusString, infoConfig.BookStatus?.Filter);
            book.UpdateTime = FormatString(book.UpdateTime, infoConfig.UpdateTime?.Filter);
        }

        private static void ReplaceBook(Book book, BookInformationConfigBase infoConfig, ref string statusString)
        {
            if (infoConfig.Replace?.Any() ?? false)
            {
                foreach (var replace in infoConfig.Replace)
                {
                    switch (replace.Field)
                    {
                        case Enums.FieldType.Title:
                            book.BookName = ReplaceString(book.BookName, replace.Old, replace.New);
                            break;
                        case Enums.FieldType.Url:
                            book.Url = ReplaceString(book.Url, replace.Old, replace.New);
                            break;
                        case Enums.FieldType.BookCover:
                            book.CoverUrl = ReplaceString(book.CoverUrl, replace.Old, replace.New);
                            break;
                        case Enums.FieldType.BookAuthor:
                            book.Author = ReplaceString(book.Author, replace.Old, replace.New);
                            break;
                        case Enums.FieldType.BookDescription:
                            book.Description = ReplaceString(book.Description, replace.Old, replace.New);
                            break;
                        case Enums.FieldType.LastChapterTitle:
                            book.LatestChapterTitle = ReplaceString(book.LatestChapterTitle, replace.Old, replace.New);
                            break;
                        case Enums.FieldType.LastChapterUrl:
                            book.LatestChapterId = ReplaceString(book.LatestChapterId, replace.Old, replace.New);
                            break;
                        case Enums.FieldType.Category:
                            book.Category = ReplaceString(book.Category, replace.Old, replace.New);
                            break;
                        case Enums.FieldType.Tag:
                            book.Tag = ReplaceString(book.Tag, replace.Old, replace.New);
                            break;
                        case Enums.FieldType.BookStatus:
                            statusString = ReplaceString(statusString, replace.Old, replace.New);
                            break;
                        case Enums.FieldType.UpdateTime:
                            book.UpdateTime = ReplaceString(book.UpdateTime, replace.Old, replace.New);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void RepairBook(Book book, BookInformationConfigBase infoConfig, ref string statusString)
        {
            if (infoConfig.Repair?.Any() ?? false)
            {
                foreach (var repair in infoConfig.Repair)
                {
                    var isLeft = repair.Position == "l";
                    switch (repair.Field)
                    {
                        case Enums.FieldType.Title:
                            book.BookName = RepairString(book.BookName, isLeft, repair.Value);
                            break;
                        case Enums.FieldType.Url:
                            book.Url = RepairString(book.Url, isLeft, repair.Value);
                            break;
                        case Enums.FieldType.BookCover:
                            book.CoverUrl = RepairString(book.CoverUrl, isLeft, repair.Value);
                            break;
                        case Enums.FieldType.BookAuthor:
                            book.Author = RepairString(book.Author, isLeft, repair.Value);
                            break;
                        case Enums.FieldType.BookDescription:
                            book.Description = RepairString(book.Description, isLeft, repair.Value);
                            break;
                        case Enums.FieldType.LastChapterTitle:
                            book.LatestChapterTitle = RepairString(book.LatestChapterTitle, isLeft, repair.Value);
                            break;
                        case Enums.FieldType.LastChapterUrl:
                            book.LatestChapterId = RepairString(book.LatestChapterId, isLeft, repair.Value);
                            break;
                        case Enums.FieldType.Category:
                            book.Category = RepairString(book.Category, isLeft, repair.Value);
                            break;
                        case Enums.FieldType.Tag:
                            book.Tag = RepairString(book.Tag, isLeft, repair.Value);
                            break;
                        case Enums.FieldType.BookStatus:
                            statusString = RepairString(statusString, isLeft, repair.Value);
                            break;
                        case Enums.FieldType.UpdateTime:
                            book.UpdateTime = RepairString(book.UpdateTime, isLeft, repair.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void EncodingBook(Book book, string statusString, bool needCheck = false)
        {
            book.Url = EncodingStringToBase64(book.Url, needCheck);
            book.BookId = EncodingStringToBase64($"{book.BookName}|{book.Author}", needCheck);
            book.LatestChapterId = EncodingStringToBase64(book.LatestChapterId, needCheck);

            if (!string.IsNullOrEmpty(statusString))
            {
                book.Status = statusString switch
                {
                    "连载" => Enums.BookStatus.Writing,
                    "完结" => Enums.BookStatus.Finish,
                    _ => Enums.BookStatus.Invalid,
                };
            }
        }
    }
}
