// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Models
{
    /// <summary>
    /// 请求配置.
    /// </summary>
    public class RequestConfig
    {
        /// <summary>
        /// 请求方式.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 请求体数据.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 数据类型.
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 请求头.
        /// </summary>
        public List<Header> Headers { get; set; }
    }
}
