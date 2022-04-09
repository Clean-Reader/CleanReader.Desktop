// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 图标文本组合控件.
    /// </summary>
    public sealed partial class IconTextBlock : UserControl
    {
        /// <summary>
        /// <see cref="Icon"/>的依赖属性.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(string), typeof(IconTextBlock), new PropertyMetadata(string.Empty));

        /// <summary>
        /// <see cref="Text"/>的依赖属性.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(IconTextBlock), new PropertyMetadata(string.Empty));

        /// <summary>
        /// <see cref="Spacing"/>的依赖属性.
        /// </summary>
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(IconTextBlock), new PropertyMetadata(4d));

        /// <summary>
        /// <see cref="IconFontSize"/>的依赖属性.
        /// </summary>
        public static readonly DependencyProperty IconFontSizeProperty =
            DependencyProperty.Register(nameof(IconFontSize), typeof(double), typeof(IconTextBlock), new PropertyMetadata(12d));

        /// <summary>
        /// <see cref="MaxLines"/>的依赖属性.
        /// </summary>
        public static readonly DependencyProperty MaxLinesProperty =
            DependencyProperty.Register(nameof(MaxLines), typeof(int), typeof(IconTextBlock), new PropertyMetadata(2));

        /// <summary>
        /// Initializes a new instance of the <see cref="IconTextBlock"/> class.
        /// </summary>
        public IconTextBlock() => InitializeComponent();

        /// <summary>
        /// 图标.
        /// </summary>
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 文本.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 图标和文本的间隔.
        /// </summary>
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        /// <summary>
        /// 图标大小.
        /// </summary>
        public double IconFontSize
        {
            get { return (double)GetValue(IconFontSizeProperty); }
            set { SetValue(IconFontSizeProperty, value); }
        }

        /// <summary>
        /// 最大行数.
        /// </summary>
        public int MaxLines
        {
            get { return (int)GetValue(MaxLinesProperty); }
            set { SetValue(MaxLinesProperty, value); }
        }
    }
}
