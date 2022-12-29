// Copyright (c) Richasy. All rights reserved.

using CleanReader.Models.App;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls;

/// <summary>
/// 阅读时长卡片.
/// </summary>
public sealed partial class ReaderDurationCard : UserControl
{
    /// <summary>
    /// <see cref="Data"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(ReaderDuration), typeof(ReaderDurationCard), new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderDurationCard"/> class.
    /// </summary>
    public ReaderDurationCard() => InitializeComponent();

    /// <summary>
    /// 数据.
    /// </summary>
    public ReaderDuration Data
    {
        get => (ReaderDuration)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }
}
