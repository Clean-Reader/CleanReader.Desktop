// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Models
{
    /// <summary>
    /// 在线章节内容.
    /// </summary>
    public class ChapterContent
    {
        /// <summary>
        /// 章节标识符.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 内容.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 标题.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 章节索引.
        /// </summary>
        public int ChapterIndex { get; set; }
    }
}
