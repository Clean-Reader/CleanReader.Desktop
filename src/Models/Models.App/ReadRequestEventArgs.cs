// Copyright (c) Richasy. All rights reserved.

using CleanReader.Models.DataBase;

namespace CleanReader.Models.App
{
    /// <summary>
    /// 阅读请求事件参数.
    /// </summary>
    public class ReadRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadRequestEventArgs"/> class.
        /// </summary>
        /// <param name="book">书籍.</param>
        /// <param name="startCfi">起始位置.</param>
        public ReadRequestEventArgs(Book book, string startCfi)
        {
            Book = book;
            StartCfi = startCfi;
        }

        /// <summary>
        /// 书籍.
        /// </summary>
        public Book Book { get; set; }

        /// <summary>
        /// 起始位置.
        /// </summary>
        public string StartCfi { get; set; }
    }
}
