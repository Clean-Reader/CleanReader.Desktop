// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Models.Resources;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls;

/// <summary>
/// 在线书籍卡片.
/// </summary>
public sealed partial class OnlineBookCard : UserControl
{
    /// <summary>
    /// <see cref="Data"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(OnlineBookViewModel), typeof(OnlineBookCard), new PropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlineBookCard"/> class.
    /// </summary>
    public OnlineBookCard() => InitializeComponent();

    /// <summary>
    /// 点击事件.
    /// </summary>
    public event EventHandler<OnlineBookViewModel> Click;

    /// <summary>
    /// 数据.
    /// </summary>
    public OnlineBookViewModel Data
    {
        get => (OnlineBookViewModel)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as OnlineBookCard;
        if (e.NewValue is OnlineBookViewModel vm)
        {
            var data = vm.Book;
            instance.CoverImage.Source = data.CoverUrl;
            instance.CoverImage.Title = data.BookName;
            instance.CoverImage.StatusIcon = "\uE12B";
            instance.TitleBlock.Text = data.BookName;
            instance.AuthorBlock.Text = data.Author;
            instance.LatestChapterBlock.Text = data.LatestChapterTitle;
            instance.DescriptionBlock.Text = string.IsNullOrEmpty(data.Description)
                ? StringResources.NoDescription
                : data.Description;
        }
    }

    private void OnRootClick(object sender, RoutedEventArgs e)
        => Click?.Invoke(this, Data);
}
