// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Models
{
    /// <summary>
    /// 书籍信息配置项基类.
    /// </summary>
    public abstract class BookInformationConfigBase
    {
        /// <summary>
        /// HTML的查询范围.
        /// </summary>
        public string Range { get; set; }

        /// <summary>
        /// 增补文本规则.
        /// </summary>
        public List<Repair> Repair { get; set; }

        /// <summary>
        /// 替换文本规则.
        /// </summary>
        public List<Replace> Replace { get; set; }

        /// <summary>
        /// 书名规则.
        /// </summary>
        public Attribute BookName { get; set; }

        /// <summary>
        /// 书籍地址规则.
        /// </summary>
        public Attribute BookUrl { get; set; }

        /// <summary>
        /// 封面规则.
        /// </summary>
        public Attribute BookCover { get; set; }

        /// <summary>
        /// 作者规则.
        /// </summary>
        public Attribute BookAuthor { get; set; }

        /// <summary>
        /// 说明规则.
        /// </summary>
        public Attribute BookDescription { get; set; }

        /// <summary>
        /// 书籍状态规则.
        /// </summary>
        public Attribute BookStatus { get; set; }

        /// <summary>
        /// 最新章节标题规则.
        /// </summary>
        public Attribute LastChapterTitle { get; set; }

        /// <summary>
        /// 最新章节地址规则.
        /// </summary>
        public Attribute LastChapterUrl { get; set; }

        /// <summary>
        /// 分类规则，比如一本书是玄幻还是言情.
        /// </summary>
        public Attribute Category { get; set; }

        /// <summary>
        /// 标签规则，书籍所加的标签，筛选出的列表应提供分隔符.
        /// </summary>
        public Attribute Tag { get; set; }

        /// <summary>
        /// 书籍更新时间规则.
        /// </summary>
        public Attribute UpdateTime { get; set; }
    }
}
