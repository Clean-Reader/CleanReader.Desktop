// Copyright (c) Richasy. All rights reserved.

using System.Collections.ObjectModel;
using System.Reactive;
using CleanReader.Models.App;
using CleanReader.Models.DataBase;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 阅读时长页面视图模型.
    /// </summary>
    public sealed partial class ReadDurationPageViewModel
    {
        private readonly ObservableAsPropertyHelper<bool> _isInitializing;
        private LibraryDbContext _dbContext;
        private bool _disposedValue;

        /// <summary>
        /// 初始化命令.
        /// </summary>
        public ReactiveCommand<Unit, Unit> InitializeCommand { get; }

        /// <summary>
        /// 显示详情对话框的命令.
        /// </summary>
        public ReactiveCommand<ReaderDuration, Unit> ShowDetailCommand { get; }

        /// <summary>
        /// 总阅读小时数.
        /// </summary>
        [Reactive]
        public double TotalReadHours { get; set; }

        /// <summary>
        /// 是否显示无时长记录.
        /// </summary>
        [Reactive]
        public bool IsEmptyShown { get; set; }

        /// <summary>
        /// 各书籍阅读时长集合.
        /// </summary>
        public ObservableCollection<ReaderDuration> ReaderDurations { get; }

        /// <summary>
        /// 是否正在加载.
        /// </summary>
        public bool IsInitializing => _isInitializing.Value;
    }
}
