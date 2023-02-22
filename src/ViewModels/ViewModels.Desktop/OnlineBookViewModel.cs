// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using CleanReader.Models.Services;
using CleanReader.Services.Novel;
using CleanReader.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 在线书籍视图模型.
/// </summary>
public sealed partial class OnlineBookViewModel : ViewModelBase, IOnlineBookViewModel
{
    [ObservableProperty]
    private bool _enableDownload;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlineBookViewModel"/> class.
    /// </summary>
    /// <param name="book">书籍信息.</param>
    public OnlineBookViewModel()
    {
        var source = LibraryViewModel.Instance.BookSources.FirstOrDefault(p => p.Id == book.SourceId);
        EnableDownload = source != null && !string.IsNullOrEmpty(source.Chapter?.Range) && !string.IsNullOrEmpty(source.ChapterContent?.Range);
    }

    /// <summary>
    /// 书籍内容.
    /// </summary>
    public Book Book { get; set; }

    /// <inheritdoc/>
    public void InjectData(Book book)
    {
        Book = book;
        var libVM = Locator.Lib.Locator.Instance.GetService<ILibraryViewModel>();
        // var source = libVM.boo
    }

    [RelayCommand]
    private async Task OpenInBroswerAsync()
    {
        var url = NovelService.DecodingBase64ToString(Book.Url);
        await Launcher.LaunchUriAsync(new Uri(url));
    }

    [RelayCommand]
    private void OnlineSearch()
        => LibraryViewModel.Instance.ShowOnlineSearchDialogCommand.Execute(Book.BookName).Subscribe();
}
