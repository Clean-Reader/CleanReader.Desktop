// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanReader.Models.Resources;
using CleanReader.Services.Novel.Models;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Windows.System;

namespace CleanReader.App.Controls;

/// <summary>
/// 创建书源对话框.
/// </summary>
public sealed partial class CreateBookSourceDialog : CustomDialog
{
    private readonly BookSourceOverviewPageViewModel _viewModel = BookSourceOverviewPageViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateBookSourceDialog"/> class.
    /// </summary>
    public CreateBookSourceDialog() => InitializeComponent();

    /// <inheritdoc/>
    public override async void OnPrimaryButtonClick()
    {
        var bookSource = new BookSource();
        bookSource.Name = BookSourceNameBox.Text;
        bookSource.Id = IdBox.Text;
        bookSource.Charset = "utf-8";
        bookSource.WebUrl = UrlBox.Text;

        if (ExploreBox.IsChecked ?? false)
        {
            bookSource.IsExploreEnabled = true;
            bookSource.Explore = new ExploreConfig { Range = string.Empty, Repair = new List<Repair>(), Replace = new List<Replace>(), Categories = new List<Category>() };
        }

        if (SearchBox.IsChecked ?? false)
        {
            bookSource.Search = new SearchConfig { Range = string.Empty, Repair = new List<Repair>(), Replace = new List<Replace>() };
        }

        if (BookDetailBox.IsChecked ?? false)
        {
            bookSource.BookDetail = new BookDetailConfig { Range = string.Empty, Repair = new List<Repair>(), Replace = new List<Replace>() };
        }

        if (ChapterBox.IsChecked ?? false)
        {
            bookSource.Chapter = new ChapterConfig { Range = string.Empty, Repair = new List<Repair>(), Replace = new List<Replace>() };
        }

        if (ChapterContentBox.IsChecked ?? false)
        {
            bookSource.ChapterContent = new ChapterContentConfig { Range = string.Empty };
        }

        var path = Path.Combine(_viewModel.RootPath, $"{IdBox.Text}.json");
        var settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
        };
        await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(bookSource, Formatting.Indented, settings));
        await Launcher.LaunchUriAsync(new Uri($"file:///{path}"));
        _viewModel.ReloadCommand.Execute().Subscribe();
    }

    private void OnMustBoxTextChanged(object sender, TextChangedEventArgs e) => IsPrimaryButtonEnabled = !string.IsNullOrEmpty(BookSourceNameBox.Text)
            && !string.IsNullOrEmpty(IdBox.Text)
            && !string.IsNullOrEmpty(UrlBox.Text);

    private void OnIdBoxLostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(IdBox.Text))
        {
            if (_viewModel.BookSources.Any(p => p.Id.Equals(IdBox.Text)))
            {
                ErrorBlock.Text = StringResources.IdRepeat;
                ErrorBlock.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                return;
            }
        }

        ErrorBlock.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
    }
}
