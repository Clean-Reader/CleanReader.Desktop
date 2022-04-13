// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using CleanReader.Models.DataBase;
using Microsoft.UI.Dispatching;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 笔记概览页面视图模型.
    /// </summary>
    public sealed partial class NoteOverviewPageViewModel
    {
        private readonly List<Highlight> _allHighlights;
        private readonly ObservableAsPropertyHelper<bool> _isInitializing;

        private bool _disposedValue;
        private LibraryDbContext _libraryDbContext;

        /// <summary>
        /// 显示高亮对话框命令.
        /// </summary>
        public ReactiveCommand<Highlight, Unit> ShowHighlightDialogCommand { get; }

        /// <summary>
        /// 删除高亮命令.
        /// </summary>
        public ReactiveCommand<Highlight, Unit> DeleteHighlightCommand { get; }

        /// <summary>
        /// 跳转到高亮命令.
        /// </summary>
        public ReactiveCommand<Highlight, Unit> JumpToHighlightCommand { get; }

        /// <summary>
        /// 初始化命令.
        /// </summary>
        public ReactiveCommand<Unit, Unit> InitializeCommand { get; }

        /// <summary>
        /// 当前选中书籍.
        /// </summary>
        [Reactive]
        public Book CurrentBook { get; set; }

        /// <summary>
        /// 有高亮标记的书籍列表.
        /// </summary>
        public ObservableCollection<Book> Books { get; }

        /// <summary>
        /// 显示的高亮笔记.
        /// </summary>
        public ObservableCollection<Highlight> Notes { get; }

        /// <summary>
        /// 是否显示空白.
        /// </summary>
        [Reactive]
        public bool IsShowEmpty { get; set; }

        /// <summary>
        /// 是否正在初始化.
        /// </summary>
        public bool IsInitializing => _isInitializing.Value;

        private static DispatcherQueue DispatcherQueue => AppViewModel.Instance.DispatcherQueue;
    }
}
