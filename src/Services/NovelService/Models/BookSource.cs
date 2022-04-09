// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Models
{
    /// <summary>
    /// 书源.
    /// </summary>
    public class BookSource
    {
        /// <summary>
        /// 书源标识符.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 书源名称.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 网页地址.
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        /// 字符集.
        /// </summary>
        public string Charset { get; set; }

        /// <summary>
        /// 是否进入书籍详情页获取全部书籍信息.
        /// </summary>
        public bool IsBookDetailEnabled { get; set; }

        /// <summary>
        /// 是否支持发现模块.
        /// </summary>
        public bool IsExploreEnabled { get; set; }

        /// <summary>
        /// 搜索模块.
        /// </summary>
        public SearchConfig Search { get; set; }

        /// <summary>
        /// 书籍详情模块.
        /// </summary>
        public BookDetailConfig BookDetail { get; set; }

        /// <summary>
        /// 章节目录模块.
        /// </summary>
        public ChapterConfig Chapter { get; set; }

        /// <summary>
        /// 章节内容模块.
        /// </summary>
        public ChapterContentConfig ChapterContent { get; set; }

        /// <summary>
        /// 发现模块.
        /// </summary>
        public ExploreConfig Explore { get; set; }
    }
}
