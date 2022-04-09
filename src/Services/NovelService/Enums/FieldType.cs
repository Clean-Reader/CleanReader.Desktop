// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Enums
{
    /// <summary>
    /// 书源解析字段类型.
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// 书名或标题.
        /// </summary>
        Title = 0,

        /// <summary>
        /// 地址链接.
        /// </summary>
        Url = 1,

        /// <summary>
        /// 书籍封面.
        /// </summary>
        BookCover = 2,

        /// <summary>
        /// 作者.
        /// </summary>
        BookAuthor = 3,

        /// <summary>
        /// 简介.
        /// </summary>
        BookDescription = 4,

        /// <summary>
        /// 书籍状态.
        /// </summary>
        BookStatus = 5,

        /// <summary>
        /// 最新章节标题.
        /// </summary>
        LastChapterTitle = 6,

        /// <summary>
        /// 最新章节地址.
        /// </summary>
        LastChapterUrl = 7,

        /// <summary>
        /// 书籍分类.
        /// </summary>
        Category = 8,

        /// <summary>
        /// 书籍标签.
        /// </summary>
        Tag = 9,

        /// <summary>
        /// 书籍更新时间.
        /// </summary>
        UpdateTime = 10,
    }
}
