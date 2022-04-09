// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Epub
{
    /// <summary>
    /// EPUB 服务的配置.
    /// </summary>
    public class EpubServiceConfiguration
    {
        /// <summary>
        /// 标题.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 扉页路径.
        /// </summary>
        public string TitlePagePath { get; set; }

        /// <summary>
        /// 作者.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 语言.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 来源文件夹路径.
        /// </summary>
        public string SourceFolderPath { get; set; }

        /// <summary>
        /// 导出文件夹路径.
        /// </summary>
        public string OutputFolderPath { get; set; }

        /// <summary>
        /// 导出文件名.
        /// </summary>
        public string OutputFileName { get; set; }
    }
}
