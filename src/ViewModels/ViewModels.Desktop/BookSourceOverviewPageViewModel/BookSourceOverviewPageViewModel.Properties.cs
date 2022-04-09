// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Reactive;
using CleanReader.Services.Novel.Models;
using CleanReader.Toolkit.Interfaces;
using Microsoft.UI.Dispatching;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 书源概览页面视图模型.
    /// </summary>
    public sealed partial class BookSourceOverviewPageViewModel
    {
        private readonly ISettingsToolkit _settingsToolkit;
        private readonly IFileToolkit _fileToolkit;

        /// <summary>
        /// 实例.
        /// </summary>
        public static BookSourceOverviewPageViewModel Instance { get; } = new Lazy<BookSourceOverviewPageViewModel>(() => new BookSourceOverviewPageViewModel()).Value;

        /// <summary>
        /// 初始化命令.
        /// </summary>
        public ReactiveCommand<Unit, Unit> InitializeCommand { get; }

        /// <summary>
        /// 重载命令.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ReloadCommand { get; }

        /// <summary>
        /// 打开命令.
        /// </summary>
        public ReactiveCommand<BookSource, Unit> OpenCommand { get; }

        /// <summary>
        /// 在浏览器中打开命令.
        /// </summary>
        public ReactiveCommand<BookSource, Unit> OpenInBroswerCommand { get; }

        /// <summary>
        /// 删除命令.
        /// </summary>
        public ReactiveCommand<BookSource, Unit> DeleteCommand { get; }

        /// <summary>
        /// 添加命令.
        /// </summary>
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }

        /// <summary>
        /// 书源根目录.
        /// </summary>
        public string RootPath { get; private set; }

        /// <summary>
        /// 书源是否为空.
        /// </summary>
        [Reactive]
        public bool IsShowEmpty { get; set; }

        /// <summary>
        /// 书源集合.
        /// </summary>
        public ObservableCollection<BookSource> BookSources { get; }

        private static DispatcherQueue DispatcherQueue => AppViewModel.Instance.DispatcherQueue;
    }
}
