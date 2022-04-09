// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using CleanReader.Models.App;
using ReactiveUI;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 阅读器章节视图模型.
    /// </summary>
    public class ReaderChapterViewModel : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderChapterViewModel"/> class.
        /// </summary>
        /// <param name="chapter">章节.</param>
        public ReaderChapterViewModel(ReaderChapter chapter)
        {
            Chapter = chapter;
            if (chapter.Children?.Any() ?? false)
            {
                Children = new List<ReaderChapterViewModel>();
                chapter.Children.ForEach(p => Children.Add(new ReaderChapterViewModel(p)));
            }
        }

        /// <summary>
        /// 章节内容.
        /// </summary>
        public ReaderChapter Chapter { get; set; }

        /// <summary>
        /// 是否被选中.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// 子项.
        /// </summary>
        public List<ReaderChapterViewModel> Children { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is ReaderChapterViewModel model && EqualityComparer<ReaderChapter>.Default.Equals(Chapter, model.Chapter);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Chapter);

        /// <inheritdoc/>
        public override string ToString() => Chapter?.ToString() ?? string.Empty;
    }
}
