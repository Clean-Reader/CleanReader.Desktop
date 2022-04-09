// Copyright (c) Richasy. All rights reserved.

using Newtonsoft.Json;

namespace CleanReader.Models.App
{
    /// <summary>
    /// 阅读器进度.
    /// </summary>
    public class ReaderProgress
    {
        /// <summary>
        /// 章节Id.
        /// </summary>
        [JsonProperty("chapterId")]
        public string ChapterId { get; set; }

        /// <summary>
        /// 章节链接.
        /// </summary>
        [JsonProperty("chapterHref")]
        public string ChapterHref { get; set; }

        /// <summary>
        /// 章节名.
        /// </summary>
        [JsonProperty("chapterName")]
        public string ChapterName { get; set; }

        /// <summary>
        /// 当前页码.
        /// </summary>
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        /// <summary>
        /// 全部页面.
        /// </summary>
        [JsonProperty("totalPage")]
        public int TotalPage { get; set; }

        /// <summary>
        /// 起始标识.
        /// </summary>
        [JsonProperty("startCfi")]
        public string StartCfi { get; set; }

        /// <summary>
        /// 结束标识.
        /// </summary>
        [JsonProperty("endCfi")]
        public string EndCfi { get; set; }

        /// <summary>
        /// 基本标识.
        /// </summary>
        [JsonProperty("base")]
        public string Base { get; set; }
    }
}
