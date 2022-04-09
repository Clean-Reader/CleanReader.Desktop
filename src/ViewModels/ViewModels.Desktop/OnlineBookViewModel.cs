// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using CleanReader.Services.Novel;
using CleanReader.Services.Novel.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Windows.System;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 在线书籍视图模型.
    /// </summary>
    public class OnlineBookViewModel : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineBookViewModel"/> class.
        /// </summary>
        /// <param name="book">书籍信息.</param>
        public OnlineBookViewModel(Book book)
        {
            Book = book;
            OpenInBroswerCommand = ReactiveCommand.CreateFromTask(OpenInBroswerAsync, outputScheduler: RxApp.MainThreadScheduler);
            OnlineSearchCommand = ReactiveCommand.Create(OnlienSearch, outputScheduler: RxApp.MainThreadScheduler);
            var source = LibraryViewModel.Instance.BookSources.FirstOrDefault(p => p.Id == book.SourceId);
            EnableDownload = source != null && !string.IsNullOrEmpty(source.Chapter?.Range) && !string.IsNullOrEmpty(source.ChapterContent?.Range);
        }

        /// <summary>
        /// 书籍内容.
        /// </summary>
        public Book Book { get; }

        /// <summary>
        /// 在浏览器中打开的命令.
        /// </summary>
        public ReactiveCommand<Unit, Unit> OpenInBroswerCommand { get; }

        /// <summary>
        /// 在线搜索的命令.
        /// </summary>
        public ReactiveCommand<Unit, Unit> OnlineSearchCommand { get; }

        /// <summary>
        /// 是否允许下载.
        /// </summary>
        [Reactive]
        public bool EnableDownload { get; set; }

        /// <summary>
        /// 是否被选中.
        /// </summary>
        [Reactive]
        public bool IsSelected { get; set; }

        private async Task OpenInBroswerAsync()
        {
            var url = NovelService.DecodingBase64ToString(Book.Url);
            await Launcher.LaunchUriAsync(new Uri(url));
        }

        private void OnlienSearch()
            => LibraryViewModel.Instance.ShowOnlineSearchDialogCommand.Execute(Book.BookName).Subscribe();
    }
}
