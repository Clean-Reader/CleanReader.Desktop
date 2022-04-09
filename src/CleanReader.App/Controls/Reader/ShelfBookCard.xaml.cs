// Copyright (c) Richasy. All rights reserved.

using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 书架书籍卡片.
    /// </summary>
    public sealed partial class ShelfBookCard : UserControl
    {
        /// <summary>
        /// <see cref="Data"/>的依赖属性.
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(ShelfBookViewModel), typeof(ShelfBookCard), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="ShelfBookCard"/> class.
        /// </summary>
        public ShelfBookCard() => InitializeComponent();

        /// <summary>
        /// 书籍数据.
        /// </summary>
        public ShelfBookViewModel Data
        {
            get => (ShelfBookViewModel)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
    }
}
