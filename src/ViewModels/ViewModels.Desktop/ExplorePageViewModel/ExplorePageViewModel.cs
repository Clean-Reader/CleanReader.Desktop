// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CleanReader.Models.Services;
using ReactiveUI;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// 探索与发现页面视图模型.
/// </summary>
public sealed partial class ExplorePageViewModel : ReactiveObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExplorePageViewModel"/> class.
    /// </summary>
    private ExplorePageViewModel()
    {
        BookSources = new ObservableCollection<BookSource>();
        Categories = new ObservableCollection<Category>();
        Books = new ObservableCollection<OnlineBookViewModel>();
        _novelService = LibraryViewModel.Instance.GetNovelService();
        _pageIndex = 0;
        LibraryViewModel.Instance.BookSources.CollectionChanged += OnLibraryBookSourcesCollectionChanged;

        var canLoadExcute = this.WhenAnyValue(x => x.IsFirstLoading, x => x.IsPagerLoading)
            .Select(p => !p.Item1 && !p.Item2);
        LoadCategoryDetailCommand = ReactiveCommand.CreateFromTask(LoadCategoryDetailAsync, canLoadExcute, RxApp.MainThreadScheduler);

        this.WhenAnyValue(x => x.ErrorMessage)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(p => IsShowError = !string.IsNullOrEmpty(p));

        this.WhenAnyValue(x => x.SelectedBookSource)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => InitializeCategories());

        this.WhenAnyValue(x => x.SelectedCategory)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x =>
            {
                _pageIndex = 1;
                Books.Clear();
                LoadCategoryDetailCommand.Execute().Subscribe();
            });

        LoadCategoryDetailCommand.ThrownExceptions
            .Subscribe(DisplayException);

        InitializeBookSources();
    }

    private void InitializeBookSources()
    {
        BookSources.Clear();
        LibraryViewModel.Instance.BookSources.Where(p => p.IsExploreEnabled && p.Explore != null && (p.Explore?.Categories?.Any() ?? false))
            .ToList()
            .ForEach(p => BookSources.Add(p));

        if (SelectedBookSource == null || !BookSources.Contains(SelectedBookSource))
        {
            SelectedBookSource = BookSources.FirstOrDefault();
        }

        IsNotSupportExplore = SelectedBookSource == null;
    }

    private void InitializeCategories()
    {
        Categories.Clear();
        SelectedCategory = null;
        if (SelectedBookSource != null)
        {
            SelectedBookSource.Explore.Categories.ForEach(p => Categories.Add(p));
            SelectedCategory = Categories.FirstOrDefault();
        }
    }

    private async Task LoadCategoryDetailAsync()
    {
        ErrorMessage = string.Empty;
        if (_exploreTokenSource != null && _exploreTokenSource.Token.CanBeCanceled)
        {
            _exploreTokenSource.Cancel();
            _exploreTokenSource.Dispose();
            _exploreTokenSource = null;
        }

        if (SelectedBookSource != null && SelectedCategory != null)
        {
            _exploreTokenSource = new System.Threading.CancellationTokenSource();
            if (Books.Count > 0)
            {
                IsPagerLoading = true;
            }
            else
            {
                await Task.Delay(100);
                IsFirstLoading = false;
                IsFirstLoading = true;
            }

            var books = new List<Book>();
            await Task.Run(async () =>
            {
                books = await _novelService.GetBooksWithCategoryAsync(SelectedBookSource.Id, SelectedCategory.Name, _pageIndex, _exploreTokenSource);
            });

            if (books.Count > 0)
            {
                foreach (var book in books)
                {
                    if (!Books.Any(p => p.Book.Equals(book)))
                    {
                        Books.Add(new OnlineBookViewModel(book));
                    }
                }
            }

            _pageIndex++;
            IsFirstLoading = IsPagerLoading = false;
        }
    }

    private void DisplayException(Exception e)
    {
        IsFirstLoading = IsPagerLoading = false;
        if (e is not TaskCanceledException)
        {
            ErrorMessage = e.Message;
        }
    }

    private void OnLibraryBookSourcesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => InitializeBookSources();
}
