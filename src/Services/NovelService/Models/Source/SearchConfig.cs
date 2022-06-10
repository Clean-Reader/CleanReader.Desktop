// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Models
{
    /// <summary>
    /// 搜索配置.
    /// </summary>
    public class SearchConfig : BookInformationConfigBase
    {
        /// <summary>
        /// 搜索地址.
        /// </summary>
        public string SearchUrl { get; set; }

        /// <summary>
        /// 请求配置.
        /// </summary>
        public RequestConfig Request { get; set; }

        /// <summary>
        /// 是否需要进一步请求书籍详情.
        /// </summary>
        public bool NeedDetail { get; set; }

        /// <summary>
        /// 是否需要对关键词进行Url编码.
        /// </summary>
        public bool EncodingKeyword { get; set; }
    }
}
