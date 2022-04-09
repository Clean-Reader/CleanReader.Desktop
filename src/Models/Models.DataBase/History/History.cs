// Copyright (c) Richasy. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace CleanReader.Models.DataBase
{
    /// <summary>
    /// 历史记录.
    /// </summary>
    public class History
    {
        /// <summary>
        /// 书籍 Id.
        /// </summary>
        [Key]
        public string BookId { get; set; }

        /// <summary>
        /// 阅读位置.
        /// </summary>
        public string? Position { get; set; }

        /// <summary>
        /// 阅读进度.
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// 总阅读时长.
        /// </summary>
        public double ReadDuration { get; set; }

        /// <summary>
        /// 定位列表.
        /// </summary>
        public string? Locations { get; set; }

        /// <summary>
        /// 阅读分段列表，记录何时阅读，阅读了多长时间.
        /// </summary>
        public List<ReadSection>? ReadSections { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is History history && BookId == history.BookId;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(BookId);
    }
}
