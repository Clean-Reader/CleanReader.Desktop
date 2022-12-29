// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using CleanReader.Models.App;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using CleanReader.ViewModels.Desktop;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace CleanReader.App.Controls;

/// <summary>
/// 阅读器高亮对话框.
/// </summary>
public sealed partial class ReaderHighlightDialog : CustomDialog
{
    private readonly ReaderViewModel _viewModel = ReaderViewModel.Instance;
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private Highlight _source;
    private string _range;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderHighlightDialog"/> class.
    /// </summary>
    public ReaderHighlightDialog() => InitializeComponent();

    /// <inheritdoc/>
    public override void OnPrimaryButtonClick()
    {
        if (_source == null)
        {
            _source = new Highlight();
            _source.Id = Guid.NewGuid().ToString();
            _source.Text = SelectedTextBlock.Text;
            _source.CfiRange = _range;
            _source.BookId = _viewModel.GetCurrentBookId();
            _source.CreateTime = DateTime.Now;
        }

        _source.UpdateTime = DateTime.Now;
        _source.Color = ColorBox.Text;
        _source.Comments = NoteBox.Text;
        _viewModel.AddOrUpdateHighlightCommand.Execute(_source).Subscribe();
    }

    /// <inheritdoc/>
    public override void InjectData(object data)
    {
        if (data is ReaderContextMenuArgs args)
        {
            SelectedTextBlock.Text = args.Text;
            _range = args.Range;
            _viewModel.GetHightlightFromCfiCommand
                .Execute(args.Range)
                .Subscribe(x =>
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        if (x != null)
                        {
                            InitializeHighlight(x);
                        }
                        else
                        {
                            PrimaryButtonText = StringResources.Add;
                            ChangeColor(_viewModel.Colors.First());
                        }
                    });
                });
        }
        else if (data is Highlight hl)
        {
            InitializeHighlight(hl);
        }
    }

    private void InitializeHighlight(Highlight hl)
    {
        SelectedTextBlock.Text = hl.Text;
        _source = hl;
        PrimaryButtonText = StringResources.Update;
        ChangeColor(hl.Color);
        NoteBox.Text = hl.Comments;
    }

    private void ChangeColor(string colorStr)
    {
        if (ColorPicker != null)
        {
            ColorPicker.Color = colorStr.ToColor();
        }

        if (ColorBox != null)
        {
            ColorBox.Text = colorStr;
        }

        if (ColorRect != null)
        {
            ColorRect.Fill = new SolidColorBrush(colorStr.ToColor());
        }
    }

    private void OnColorClick(object sender, RoutedEventArgs e)
    {
        var color = (sender as FrameworkElement).DataContext as string;
        ChangeColor(color);
    }

    private void OnColorRectTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        => ColorPickerFlyout.ShowAt(sender as FrameworkElement);

    private void OnColorChanged(Microsoft.UI.Xaml.Controls.ColorPicker sender, Microsoft.UI.Xaml.Controls.ColorChangedEventArgs args)
    {
        ColorBox.Text = args.NewColor.ToHex().Replace("#FF", "#");
        ColorRect.Fill = new SolidColorBrush(args.NewColor);
    }
}
