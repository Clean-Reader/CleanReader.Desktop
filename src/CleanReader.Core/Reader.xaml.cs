// Copyright (c) Richasy. All rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Windows.ApplicationModel;
using Windows.Storage;

namespace CleanReader.Core
{
    /// <summary>
    /// 阅读器.
    /// </summary>
    public sealed partial class Reader : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reader"/> class.
        /// </summary>
        public Reader()
        {
            InitializeComponent();
            MainView.Loaded += OnMainViewLoadedAsync;
        }

        /// <summary>
        /// 控件加载完成.
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// 在收到网页传回的信息时发生.
        /// </summary>
        public event EventHandler<string> MessageReceived;

        /// <summary>
        /// 是否已加载好.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 加载小说文件.
        /// </summary>
        /// <param name="path">小说路径.</param>
        /// <param name="startCfi">初始位置.</param>
        /// <param name="locations">位置列表.</param>
        /// <param name="highlights">高亮列表.</param>
        /// <param name="spreadMinWidth">最小分栏宽度.</param>
        /// <param name="isSmoothScroll">是否顺滑滚动.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task LoadFileAsync(
            string path,
            string startCfi = "",
            string locations = "",
            string highlights = "",
            double spreadMinWidth = 1000d,
            bool isSmoothScroll = true)
        {
            locations = string.IsNullOrEmpty(locations) ? "null" : locations;
            var js = $"reader.exposed.setBook('https://books/{Uri.EscapeDataString(Path.GetFileName(path))}', false, '{startCfi}', {locations},'{highlights}', {spreadMinWidth}, {isSmoothScroll.ToString().ToLower()})";
            await MainView.CoreWebView2.ExecuteScriptAsync(js);
        }

        /// <summary>
        /// 设置阅读选项.
        /// </summary>
        /// <param name="isScroll">是否为滚动阅读.</param>
        /// <param name="minSpreadWidth">最小分栏宽度.</param>
        /// <param name="isSmoothScroll">是否顺滑滚动.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task SetOptionsAsync(
            bool isScroll = false,
            double minSpreadWidth = 1000,
            bool isSmoothScroll = true)
        {
            var js = $"reader.exposed.setOptions({isScroll.ToString().ToLower()}, {minSpreadWidth}, {isSmoothScroll.ToString().ToLower()})";
            await MainView.CoreWebView2.ExecuteScriptAsync(js);
        }

        /// <summary>
        /// 前往章节.
        /// </summary>
        /// <param name="href">章节地址.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task GoToChapterAsync(string href)
        {
            var js = $"reader.exposed.setChapter('{href}')";
            await MainView.CoreWebView2.ExecuteScriptAsync(js);
        }

        /// <summary>
        /// 设置高亮.
        /// </summary>
        /// <param name="cfiRange">选中范围.</param>
        /// <param name="color">颜色.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task SetHighlightAsync(string cfiRange, string color)
        {
            var js = $"reader.exposed.setHighlight('{cfiRange}','{color}')";
            await MainView.CoreWebView2.ExecuteScriptAsync(js);
        }

        /// <summary>
        /// 取消高亮.
        /// </summary>
        /// <param name="cfiRange">选中范围.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task RemoveHighlightAsync(string cfiRange)
        {
            var js = $"epubReader.exposed.offHighlight('{cfiRange}')";
            await MainView.CoreWebView2.ExecuteScriptAsync(js);
        }

        /// <summary>
        /// 设置样式.
        /// </summary>
        /// <param name="fontFamily">字体.</param>
        /// <param name="fontSize">字号.</param>
        /// <param name="lineHeight">行高.</param>
        /// <param name="background">背景色.</param>
        /// <param name="foreground">前景色.</param>
        /// <param name="additionalStyle">附加样式.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task SetStyleAsync(string fontFamily, double fontSize, double lineHeight, string background, string foreground, string additionalStyle)
        {
            if (string.IsNullOrEmpty(additionalStyle))
            {
                additionalStyle = "null";
            }

            var style = $"{{fontFamily:'{fontFamily}',fontSize:{fontSize},lineHeight:{lineHeight}, background:'{background}', foreground:'{foreground}', additionalStyle: {additionalStyle}}}";
            var js = $"reader.exposed.setStyle({style})";
            await MainView.CoreWebView2.ExecuteScriptAsync(js);
        }

        /// <summary>
        /// 搜索.
        /// </summary>
        /// <param name="text">关键词文本.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task SearchAsync(string text)
        {
            var js = $"reader.exposed.search(\"{text}\")";
            await MainView.CoreWebView2.ExecuteScriptAsync(js);
        }

        /// <summary>
        /// 跳转到指定位置.
        /// </summary>
        /// <param name="location">位置标识.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task ChangeLocationAsync(string location)
        {
            var js = $"reader.exposed.changeLocation(\"{location}\")";
            await MainView.CoreWebView2.ExecuteScriptAsync(js);
        }

        private static string GetBooksPath()
            => Path.Combine(ApplicationData.Current.LocalSettings.CreateContainer("CleanReader_winui3", ApplicationDataCreateDisposition.Always).Values["LibraryPath"].ToString(), "books");

        private async void OnMainViewLoadedAsync(object sender, RoutedEventArgs e)
        {
            await MainView.EnsureCoreWebView2Async();
            MainView.CoreWebView2.DOMContentLoaded -= OnDomContentLoaded;
            MainView.CoreWebView2.DOMContentLoaded += OnDomContentLoaded;
            var path = Path.Combine(Package.Current.InstalledPath, "Assets/Reader");
            MainView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            MainView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            MainView.CoreWebView2.Settings.IsPinchZoomEnabled = false;
            MainView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            MainView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            MainView.CoreWebView2.Settings.AreDevToolsEnabled = true;
            MainView.CoreWebView2.SetVirtualHostNameToFolderMapping("cleanreader.richasy", path, CoreWebView2HostResourceAccessKind.Allow);
            MainView.CoreWebView2.SetVirtualHostNameToFolderMapping("books", GetBooksPath(), CoreWebView2HostResourceAccessKind.Allow);
            MainView.CoreWebView2.Navigate("https://cleanreader.richasy/index.html");
        }

        private void OnDomContentLoaded(CoreWebView2 sender, CoreWebView2DOMContentLoadedEventArgs args)
        {
            IsInitialized = true;
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        private void OnMessageReceived(WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
            => MessageReceived?.Invoke(this, args.TryGetWebMessageAsString());
    }
}
