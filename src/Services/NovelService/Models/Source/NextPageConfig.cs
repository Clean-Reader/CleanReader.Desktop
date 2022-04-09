// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Models
{
    /// <summary>
    /// 下一页配置.
    /// </summary>
    public class NextPageConfig
    {
        /// <summary>
        /// 获取属性的类型.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 从Docuemnt开始，获取指定element的范围，使用css-selector语法.
        /// </summary>
        public string Range { get; set; }

        /// <summary>
        /// 需要过滤/移除的内容，可以是正则表达式.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// 文本增补规则.
        /// </summary>
        public List<Repair> Repair { get; set; }

        /// <summary>
        /// 匹配规则.
        /// </summary>
        public Match Match { get; set; }
    }
}
