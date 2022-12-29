// Copyright (c) Richasy. All rights reserved.

using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls;

/// <summary>
/// 自定义对话框.
/// </summary>
public partial class CustomDialog
{
#pragma warning disable SA1600 // Elements should be documented
    public static readonly DependencyProperty CloseButtonCommandProperty =
        DependencyProperty.Register(nameof(CloseButtonCommand), typeof(ICommand), typeof(CustomDialog), new PropertyMetadata(default, new PropertyChangedCallback(OnButtonCommandChanged)));

    public static readonly DependencyProperty CloseButtonStyleProperty =
        DependencyProperty.Register(nameof(CloseButtonStyle), typeof(Style), typeof(CustomDialog), new PropertyMetadata(default));

    public static readonly DependencyProperty CloseButtonTextProperty =
        DependencyProperty.Register(nameof(CloseButtonText), typeof(string), typeof(CustomDialog), new PropertyMetadata(default));

    public static readonly DependencyProperty PrimaryButtonCommandProperty =
        DependencyProperty.Register(nameof(PrimaryButtonCommand), typeof(ICommand), typeof(CustomDialog), new PropertyMetadata(default, new PropertyChangedCallback(OnButtonCommandChanged)));

    public static readonly DependencyProperty PrimaryButtonStyleProperty =
        DependencyProperty.Register(nameof(PrimaryButtonStyle), typeof(Style), typeof(CustomDialog), new PropertyMetadata(default));

    public static readonly DependencyProperty PrimaryButtonTextProperty =
        DependencyProperty.Register(nameof(PrimaryButtonText), typeof(string), typeof(CustomDialog), new PropertyMetadata(default));

    public static readonly DependencyProperty SecondaryButtonCommandProperty =
        DependencyProperty.Register(nameof(SecondaryButtonCommand), typeof(ICommand), typeof(CustomDialog), new PropertyMetadata(default, new PropertyChangedCallback(OnButtonCommandChanged)));

    public static readonly DependencyProperty SecondaryButtonStyleProperty =
        DependencyProperty.Register(nameof(SecondaryButtonStyle), typeof(Style), typeof(CustomDialog), new PropertyMetadata(default));

    public static readonly DependencyProperty SecondaryButtonTextProperty =
        DependencyProperty.Register(nameof(SecondaryButtonText), typeof(string), typeof(CustomDialog), new PropertyMetadata(default));

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(CustomDialog), new PropertyMetadata(default));

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(CustomDialog), new PropertyMetadata(default));

    public static readonly DependencyProperty DefaultButtonProperty =
        DependencyProperty.Register(nameof(DefaultButton), typeof(ContentDialogButton), typeof(CustomDialog), new PropertyMetadata(ContentDialogButton.None));

    public static readonly DependencyProperty IsPrimaryButtonEnabledProperty =
        DependencyProperty.Register(nameof(IsPrimaryButtonEnabled), typeof(bool), typeof(CustomDialog), new PropertyMetadata(true));

    public static readonly DependencyProperty IsSecondaryButtonEnabledProperty =
        DependencyProperty.Register(nameof(IsSecondaryButtonEnabled), typeof(bool), typeof(CustomDialog), new PropertyMetadata(true));

    public static readonly DependencyProperty SubtitleMaxWidthProperty =
        DependencyProperty.Register(nameof(SubtitleMaxWidth), typeof(double), typeof(CustomDialog), new PropertyMetadata(320d));

    public ICommand CloseButtonCommand
    {
        get => (ICommand)GetValue(CloseButtonCommandProperty);
        set => SetValue(CloseButtonCommandProperty, value);
    }

    public Style CloseButtonStyle
    {
        get => (Style)GetValue(CloseButtonStyleProperty);
        set => SetValue(CloseButtonStyleProperty, value);
    }

    public string CloseButtonText
    {
        get => (string)GetValue(CloseButtonTextProperty);
        set => SetValue(CloseButtonTextProperty, value);
    }

    public ICommand PrimaryButtonCommand
    {
        get => (ICommand)GetValue(PrimaryButtonCommandProperty);
        set => SetValue(PrimaryButtonCommandProperty, value);
    }

    public Style PrimaryButtonStyle
    {
        get => (Style)GetValue(PrimaryButtonStyleProperty);
        set => SetValue(PrimaryButtonStyleProperty, value);
    }

    public string PrimaryButtonText
    {
        get => (string)GetValue(PrimaryButtonTextProperty);
        set => SetValue(PrimaryButtonTextProperty, value);
    }

    public ICommand SecondaryButtonCommand
    {
        get => (ICommand)GetValue(SecondaryButtonCommandProperty);
        set => SetValue(SecondaryButtonCommandProperty, value);
    }

    public Style SecondaryButtonStyle
    {
        get => (Style)GetValue(SecondaryButtonStyleProperty);
        set => SetValue(SecondaryButtonStyleProperty, value);
    }

    public string SecondaryButtonText
    {
        get => (string)GetValue(SecondaryButtonTextProperty);
        set => SetValue(SecondaryButtonTextProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public ContentDialogButton DefaultButton
    {
        get => (ContentDialogButton)GetValue(DefaultButtonProperty);
        set => SetValue(DefaultButtonProperty, value);
    }

    public bool IsPrimaryButtonEnabled
    {
        get => (bool)GetValue(IsPrimaryButtonEnabledProperty);
        set => SetValue(IsPrimaryButtonEnabledProperty, value);
    }

    public bool IsSecondaryButtonEnabled
    {
        get => (bool)GetValue(IsSecondaryButtonEnabledProperty);
        set => SetValue(IsSecondaryButtonEnabledProperty, value);
    }

    public double SubtitleMaxWidth
    {
        get => (double)GetValue(SubtitleMaxWidthProperty);
        set => SetValue(SubtitleMaxWidthProperty, value);
    }
#pragma warning disable SA1600 // Elements should be documented
}
