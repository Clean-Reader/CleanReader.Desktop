// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Models
{
    /// <summary>
    /// 发现模块中的分类.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// 地址.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 名称.
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Category category && Url == category.Url;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Url);
    }
}
