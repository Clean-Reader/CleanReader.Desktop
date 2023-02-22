// Copyright (c) Richasy. All rights reserved.

using CleanReader.Models.Constants;

namespace CleanReader.Models.Services;

/// <summary>
/// 在线书籍.
/// </summary>
public class Book
{
    /// <summary>
    /// 书标识符，使用Base64编码[书名|作者].
    /// </summary>
    public string BookId { get; set; }

    /// <summary>
    /// 书名.
    /// </summary>
    public string BookName { get; set; }

    /// <summary>
    /// 源Id. 如有多个则使用半角逗号分割.
    /// </summary>
    public string SourceId { get; set; }

    /// <summary>
    /// 作者.
    /// </summary>
    public string Author { get; set; }

    /// <summary>
    /// 分类.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 可用源.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 当前书籍状态.
    /// </summary>
    public BookStatus Status { get; set; }

    /// <summary>
    /// 书籍标签.
    /// </summary>
    public string Tag { get; set; }

    /// <summary>
    /// 书籍描述.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 书籍封面地址.
    /// </summary>
    public string CoverUrl { get; set; }

    /// <summary>
    /// 更新时间.
    /// </summary>
    public string UpdateTime { get; set; }

    /// <summary>
    /// 最新章节标题.
    /// </summary>
    public string LatestChapterTitle { get; set; }

    /// <summary>
    /// 最新章节地址.
    /// </summary>
    public string LatestChapterId { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Book book && BookName == book.BookName && Author == book.Author;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(BookName, Author);
}
