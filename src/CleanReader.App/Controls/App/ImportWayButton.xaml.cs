// Copyright (c) Richasy. All rights reserved.

using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls;

/// <summary>
/// 导入方式按钮.
/// </summary>
public sealed partial class ImportWayButton : UserControl
{
    /// <summary>
    /// <see cref="Icon"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(ImportWayButton), new PropertyMetadata(string.Empty));

    /// <summary>
    /// <see cref="Title"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(ImportWayButton), new PropertyMetadata(string.Empty));

    /// <summary>
    /// <see cref="Description"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(ImportWayButton), new PropertyMetadata(string.Empty));

    /// <summary>
    /// <see cref="Command"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ImportWayButton), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportWayButton"/> class.
    /// </summary>
    public ImportWayButton() => InitializeComponent();

    /// <summary>
    /// 图标.
    /// </summary>
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// 描述.
    /// </summary>
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// 命令.
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}
