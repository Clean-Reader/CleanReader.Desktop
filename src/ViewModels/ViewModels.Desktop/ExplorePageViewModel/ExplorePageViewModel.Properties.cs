// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading;
using CleanReader.Services.Novel;
using CleanReader.Services.Novel.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 探索与发现页面视图模型.
    /// </summary>
    public sealed partial class ExplorePageViewModel
    {
        private readonly NovelService _novelService;
        private int _pageIndex;
        private CancellationTokenSource _exploreTokenSource;

        /// <summary>
        /// 实例.
        /// </summary>
        public static ExplorePageViewModel Instance => new Lazy<ExplorePageViewModel>(() => new ExplorePageViewModel()).Value;

        /// <summary>
        /// 书源集合.
        /// </summary>
        public ObservableCollection<BookSource> BookSources { get; }

        /// <summary>
        /// 分类集合.
        /// </summary>
        public ObservableCollection<Category> Categories { get; }

        /// <summary>
        /// 书籍列表.
        /// </summary>
        public ObservableCollection<OnlineBookViewModel> Books { get; }

        /// <summary>
        /// 加载分类详情的命令.
        /// </summary>
        public ReactiveCommand<Unit, Unit> LoadCategoryDetailCommand { get; }

        /// <summary>
        /// 已选中的书源.
        /// </summary>
        [Reactive]
        public BookSource SelectedBookSource { get; set; }

        /// <summary>
        /// 选中的分类.
        /// </summary>
        [Reactive]
        public Category SelectedCategory { get; set; }

        /// <summary>
        /// 没有支持探索模块的书源.
        /// </summary>
        [Reactive]
        public bool IsNotSupportExplore { get; set; }

        /// <summary>
        /// 是否显示错误.
        /// </summary>
        [Reactive]
        public bool IsShowError { get; set; }

        /// <summary>
        /// 错误信息.
        /// </summary>
        [Reactive]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 是否为分类下的第一次加载.
        /// </summary>
        [Reactive]
        public bool IsFirstLoading { get; set; }

        /// <summary>
        /// 是否为分页加载.
        /// </summary>
        [Reactive]
        public bool IsPagerLoading { get; set; }
    }
}
