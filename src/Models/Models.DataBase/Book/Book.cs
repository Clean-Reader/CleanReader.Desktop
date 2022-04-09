// Copyright (c) Richasy. All rights reserved.

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CleanReader.Models.DataBase
{
    /// <summary>
    /// 书籍条目.
    /// </summary>
    [Index(nameof(Title), nameof(Author))]
    public class Book
    {
        /// <summary>
        /// 书籍标题.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 书籍标识符.
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 封面地址.
        /// </summary>
        public string? Cover { get; set; }

        /// <summary>
        /// 文件路径.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// 书籍描述.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 书籍作者.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// 最后打开书籍的时间.
        /// </summary>
        public DateTime? LastOpenTime { get; set; }

        /// <summary>
        /// 添加到书库的时间.
        /// </summary>
        public DateTime? AddTime { get; set; }

        /// <summary>
        /// 书源Id.
        /// </summary>
        public string? SourceId { get; set; }

        /// <summary>
        /// 书籍类型.
        /// </summary>
        public BookType Type { get; set; }

        /// <summary>
        /// 书籍状态.
        /// </summary>
        public BookStatus Status { get; set; }

        /// <summary>
        /// 最后章节Id.
        /// </summary>
        public string? LastChapterId { get; set; }

        /// <summary>
        /// 书籍地址.
        /// </summary>
        public string? Url { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Book book && Id == book.Id;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Id);
    }
}
