// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Models.App
{
    /// <summary>
    /// 阅读器颜色.
    /// </summary>
    public class ReaderThemeConfig
    {
        /// <summary>
        /// 背景色.
        /// </summary>
        public string Background { get; set; }

        /// <summary>
        /// 前景色.
        /// </summary>
        public string Foreground { get; set; }

        /// <summary>
        /// 字体.
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// 字号.
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// 行高.
        /// </summary>
        public double LineHeight { get; set; }

        /// <summary>
        /// 配置名.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 附加样式.
        /// </summary>
        public string AdditionalStyle { get; set; }
    }
}
