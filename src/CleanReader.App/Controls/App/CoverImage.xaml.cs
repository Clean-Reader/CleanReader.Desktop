// Copyright (c) Richasy. All rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 封面图片.
    /// </summary>
    public sealed partial class CoverImage : UserControl
    {
        /// <summary>
        /// <see cref="Source"/> 的依赖属性.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                nameof(Source),
                typeof(string),
                typeof(CoverImage),
                new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChangedAsync)));

        /// <summary>
        /// <see cref="IsShowCover"/> 的依赖属性.
        /// </summary>
        public static readonly DependencyProperty IsShowCoverProperty =
            DependencyProperty.Register(
                nameof(IsShowCover),
                typeof(bool),
                typeof(CoverImage),
                new PropertyMetadata(false, new PropertyChangedCallback(OnIsShowCoverChanged)));

        /// <summary>
        /// <see cref="Title"/> 的依赖属性.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(CoverImage),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// <see cref="StatusIcon"/> 的依赖属性.
        /// </summary>
        public static readonly DependencyProperty StatusIconProperty =
            DependencyProperty.Register(
                nameof(StatusIcon),
                typeof(string),
                typeof(CoverImage),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// <see cref="Placeholder"/> 的依赖属性.
        /// </summary>
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                nameof(Placeholder),
                typeof(object),
                typeof(CoverImage),
                new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverImage"/> class.
        /// </summary>
        public CoverImage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        /// <summary>
        /// 封面图片地址.
        /// </summary>
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// 是否显示封面.
        /// </summary>
        public bool IsShowCover
        {
            get { return (bool)GetValue(IsShowCoverProperty); }
            set { SetValue(IsShowCoverProperty, value); }
        }

        /// <summary>
        /// 标题.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 状态图标.
        /// </summary>
        public string StatusIcon
        {
            get { return (string)GetValue(StatusIconProperty); }
            set { SetValue(StatusIconProperty, value); }
        }

        /// <summary>
        /// 占位图片.
        /// </summary>
        public object Placeholder
        {
            get { return (object)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        /// <summary>
        /// 检查封面.
        /// </summary>
        public void CheckCoverShown()
        {
            LocalCover.Visibility = Visibility.Collapsed;
            PlaceholderCover.Visibility = Visibility.Collapsed;
            CoverContainer.Visibility = Visibility.Collapsed;
            if (!IsShowCover)
            {
                if (Placeholder != null)
                {
                    PlaceholderCover.Visibility = Visibility.Visible;
                }
                else
                {
                    LocalCover.Visibility = Visibility.Visible;
                }
            }
            else
            {
                CoverContainer.Visibility = Visibility.Visible;
            }
        }

        private static async void OnSourceChangedAsync(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (CoverImage)d;
            await instance.LoadSourceAsync();
        }

        private static void OnIsShowCoverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (CoverImage)d;
            instance.CheckCoverShown();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
            => CheckCoverShown();

        private async Task LoadSourceAsync()
        {
            Uri uri = null;
            var path = Source;
            Image.Source = null;
            if (Path.IsPathRooted(path))
            {
                path = Path.GetFileName(path);
            }
            else if (!string.IsNullOrEmpty(path) && path.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                uri = new Uri(path);
            }

            if (uri != null)
            {
                Image.Source = new BitmapImage(uri) { DecodePixelWidth = Convert.ToInt32(Width * 2), DecodePixelType = DecodePixelType.Physical };
            }
            else if (!string.IsNullOrEmpty(path))
            {
                var source = await LibraryViewModel.Instance.GetLocalCoverImageAsync(Path.GetFileName(path));
                if (source != null)
                {
                    source.DecodePixelWidth = Convert.ToInt32(Width * 2);
                    Image.Source = source;
                }
            }
        }
    }
}
