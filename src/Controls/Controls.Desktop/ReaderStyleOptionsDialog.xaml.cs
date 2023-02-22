// Copyright (c) Richasy. All rights reserved.

using CleanReader.Controls.Interfaces;
using CleanReader.Models.App;
using CleanReader.ViewModels.Desktop;
using CommunityToolkit.WinUI.Helpers;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace CleanReader.Controls;

/// <summary>
/// 阅读器样式设置.
/// </summary>
public sealed partial class ReaderStyleOptionsDialog : CustomDialog, IReaderStyleOptionsDialog
{
    private readonly ReaderViewModel _viewModel = ReaderViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderStyleOptionsDialog"/> class.
    /// </summary>
    public ReaderStyleOptionsDialog() => InitializeComponent();

    /// <inheritdoc/>
    public override void OnPrimaryButtonClick()
    {
        if (FontComboBox.SelectedItem != null)
        {
            _viewModel.FontFamily = FontComboBox.SelectedItem.ToString();
        }

        _viewModel.FontSize = FontSizeBox.Value;
        _viewModel.LineHeight = LineHeightSlider.Value;
        _viewModel.Background = BackgroundBox.Text;
        _viewModel.Foreground = ForegroundBox.Text;
        _viewModel.AdditionalStyle = AdditionalStyleBox.Text;
    }

    private void OnLineHeightValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (HolderBlock != null)
        {
            var grid = new DataGrid();
            HolderBlock.LineHeight = LineHeightSlider.Value * FontSizeBox.Value;
        }
    }

    private void OnBackgroundRectTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        => BackgroundPickerFlyout.ShowAt(BackgroundRect);

    private void OnForegroundRectTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        => ForegroundPickerFlyout.ShowAt(ForegroundRect);

    private void OnBackgroundColorChanged(Microsoft.UI.Xaml.Controls.ColorPicker sender, Microsoft.UI.Xaml.Controls.ColorChangedEventArgs args)
    {
        HolderContainer.Background = new SolidColorBrush(args.NewColor);
        BackgroundRect.Fill = new SolidColorBrush(args.NewColor);
        BackgroundBox.Text = args.NewColor.ToHex().Replace("#FF", "#");
    }

    private void OnForegroundColorChanged(Microsoft.UI.Xaml.Controls.ColorPicker sender, Microsoft.UI.Xaml.Controls.ColorChangedEventArgs args)
    {
        HolderBlock.Foreground = new SolidColorBrush(args.NewColor);
        ForegroundRect.Fill = new SolidColorBrush(args.NewColor);
        ForegroundBox.Text = args.NewColor.ToHex().Replace("#FF", "#");
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var background = _viewModel.Background.ToColor();
        var foreground = _viewModel.Foreground.ToColor();
        BackgroundPicker.Color = background;
        ForegroundPicker.Color = foreground;

        BackgroundRect.Fill = new SolidColorBrush(background);
        BackgroundBox.Text = _viewModel.Background;
        ForegroundRect.Fill = new SolidColorBrush(foreground);
        ForegroundBox.Text = _viewModel.Foreground;
        HolderContainer.Background = new SolidColorBrush(background);
        HolderBlock.Foreground = new SolidColorBrush(foreground);
        AdditionalStyleBox.Text = _viewModel.AdditionalStyle;
    }

    private void OnThemeClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as ReaderThemeConfig;
        FontComboBox.SelectedItem = data.FontFamily;
        FontSizeBox.Value = data.FontSize;
        LineHeightSlider.Value = data.LineHeight;
        BackgroundPicker.Color = data.Background.ToColor();
        ForegroundPicker.Color = data.Foreground.ToColor();
        AdditionalStyleBox.Text = data.AdditionalStyle;
    }

    private void OnEditStyleButtonClick(object sender, RoutedEventArgs e)
        => AdditionalStyleEditFlyout.ShowAt(sender as FrameworkElement);
}
