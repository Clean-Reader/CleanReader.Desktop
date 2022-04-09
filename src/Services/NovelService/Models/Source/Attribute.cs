// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Models
{
    /// <summary>
    /// 解析规则.
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// 获取属性的类型.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 获取指定element的规则，使用css-selector语法.
        /// </summary>
        public string Rule { get; set; }

        /// <summary>
        /// 需要过滤/移除的内容，可以是正则表达式.
        /// </summary>
        public string Filter { get; set; }
    }
}
