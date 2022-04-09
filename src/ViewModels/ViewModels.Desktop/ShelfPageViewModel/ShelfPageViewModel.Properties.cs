// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Reactive;
using CleanReader.Models.DataBase;
using CleanReader.Toolkit.Interfaces;
using Microsoft.UI.Dispatching;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 书架页面视图模型.
    /// </summary>
    public sealed partial class ShelfPageViewModel
    {
        private readonly ObservableAsPropertyHelper<bool> _isShelvesLoading;
        private readonly ObservableAsPropertyHelper<bool> _isBooksLoading;
        private readonly ObservableAsPropertyHelper<bool> _isViewModelLoading;

        private readonly ISettingsToolkit _settingsToolkit;
        private LibraryDbContext _dbContextRef;
        private bool _disposedValue;

        /// <summary>
        /// 静态实例.
        /// </summary>
        public static ShelfPageViewModel Instance { get; } = new Lazy<ShelfPageViewModel>(() => new ShelfPageViewModel()).Value;

        /// <summary>
        /// 初始化视图模型的命令.
        /// </summary>
        public ReactiveCommand<Unit, Unit> InitializeViewModelCommand { get; }

        /// <summary>
        /// 初始化书架的命令.
        /// </summary>
        public ReactiveCommand<Unit, Unit> InitializeShelvesCommand { get; }

        /// <summary>
        /// 初始化书籍的命令.
        /// </summary>
        public ReactiveCommand<Unit, Unit> InitializeBooksCommand { get; }

        /// <summary>
        /// 置顶命令.
        /// </summary>
        public ReactiveCommand<Shelf, Unit> MoveShelfToTopCommand { get; }

        /// <summary>
        /// 删除书架命令.
        /// </summary>
        public ReactiveCommand<Shelf, Unit> DeleteShelfCommand { get; }

        /// <summary>
        /// 显示的书籍集合.
        /// </summary>
        public ObservableCollection<ShelfBookViewModel> DisplayBooks { get; }

        /// <summary>
        /// 书架集合.
        /// </summary>
        public ObservableCollection<Shelf> Shelves { get; }

        /// <summary>
        /// 当前书架.
        /// </summary>
        [Reactive]
        public Shelf CurrentShelf { get; set; }

        /// <summary>
        /// 当前书籍类型.
        /// </summary>
        [Reactive]
        public string CurrentBookType { get; set; }

        /// <summary>
        /// 当前排序方式.
        /// </summary>
        [Reactive]
        public string CurrentSort { get; set; }

        /// <summary>
        /// 当前书架是否为空.
        /// </summary>
        [Reactive]
        public bool IsShelfEmpty { get; set; }

        /// <summary>
        /// 是否正在加载.
        /// </summary>
        public bool IsInitializing => _isBooksLoading.Value || _isShelvesLoading.Value || _isViewModelLoading.Value;

        private static DispatcherQueue DispatcherQueue => AppViewModel.Instance.DispatcherQueue;
    }
}
