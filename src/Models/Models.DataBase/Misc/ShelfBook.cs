// Copyright (c) Richasy. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace CleanReader.Models.DataBase
{
    /// <summary>
    /// 书架中的书籍定义.
    /// </summary>
    public class ShelfBook
    {
        /// <summary>
        /// 对应书籍Id.
        /// </summary>
        [Key]
        public string BookId { get; set; }

        /// <summary>
        /// 最近修改时间.
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ShelfBook book && BookId == book.BookId;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(BookId);
    }
}
