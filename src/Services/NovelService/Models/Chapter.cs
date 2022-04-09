// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Models
{
    /// <summary>
    /// 在线章节.
    /// </summary>
    public class Chapter
    {
        /// <summary>
        /// 书源标识.
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// 章节标识符.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 章节索引.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 标题.
        /// </summary>
        public string Title { get; set; }
    }
}
