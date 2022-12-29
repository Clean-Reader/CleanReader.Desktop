// Copyright (c) Richasy. All rights reserved.

using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CleanReader.App.Controls;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CleanReader.ViewModels.Desktop;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace CleanReader.App.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ReaderPage : Page
{
    private readonly ReaderViewModel _viewModel = ReaderViewModel.Instance;
    private readonly DispatcherQueueTimer _timer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderPage"/> class.
    /// </summary>
    public ReaderPage()
    {
        InitializeComponent();
        _timer = DispatcherQueue.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
        NavigationCacheMode = NavigationCacheMode.Enabled;
        _viewModel.RequestInitializeStyle += OnRequestInitializeStyleAsync;
        _viewModel.RequestHighlight += OnRequestHightlightAsync;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _viewModel.RequestSearch += OnRequestSearchAsync;
        _viewModel.RequestChangeLocation += OnRequestChangeLocationAsync;
        _viewModel.RequestChangeChapter += OnRequestChangeChapterAsync;
        _viewModel.RequestRemoveHighlight += OnRequestRemoveHighlightAsync;
        _viewModel.MiniViewRequested += OnMiniViewRequestedAsync;
    }

    /// <inheritdoc/>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is Models.App.ReadRequestEventArgs args)
        {
            var bookPath = Path.Combine(
                SettingsPageViewModel.Instance.LibraryPath,
                VMConstants.Library.BooksFolder,
                args.Book.Path);
            if (!File.Exists(bookPath))
            {
                _viewModel.DisplayInitializeErrorCommand.Execute(VMConstants.Reader.Book).Subscribe();
                return;
            }

            await _viewModel.SetBookAsync(args.Book, args.StartCfi);
            if (Reader.IsInitialized)
            {
                await LoadFileAsync();
            }
        }

        _timer.Start();
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        _timer.Stop();
        _viewModel.StopReadingCommand.Execute().Subscribe();
        base.OnNavigatedFrom(e);
    }

    private void OnTimerTick(object sender, object e)
    {
        if (TimeBlock != null)
        {
            TimeBlock.Text = DateTime.Now.ToString("HH:mm");
        }
    }

    private async void OnInitializedAsync(object sender, EventArgs e)
        => await LoadFileAsync();

    private async Task LoadFileAsync()
    {
        if (!string.IsNullOrEmpty(_viewModel.FilePath))
        {
            InitializeTitleBar();
            await Reader.LoadFileAsync(
                _viewModel.FilePath,
                _viewModel.StartCfi,
                _viewModel.Locations,
                _viewModel.InitHighlights,
                _viewModel.SpreadMinWidth,
                _viewModel.IsSmoothScroll);
            await SetStyleAsync();
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.Background) || e.PropertyName == nameof(_viewModel.Foreground))
        {
            InitializeTitleBar();
        }
    }

    private async void OnMessageReceivedAsync(object sender, string e)
    {
        var jobj = JObject.Parse(e);
        if (jobj.ContainsKey("Name"))
        {
            var name = jobj["Name"].ToString();
            var data = jobj["Data"].ToString();

            switch (name)
            {
                case "Progress":
                    _viewModel.IsInitializing = false;
                    _viewModel.SaveProgressCommand.Execute(data).Subscribe();
                    break;
                case "Locations":
                    _viewModel.SaveLocationsCommand.Execute(data).Subscribe();
                    break;
                case "Menu":
                    _viewModel.IsMenuShown = !_viewModel.IsMenuShown;
                    break;
                case "Toc":
                    _viewModel.InitializeTocCommand.Execute(data).Subscribe();
                    break;
                case "ContextMenu":
                    ShowContextMenu(data);
                    break;
                case "Failed":
                    _viewModel.IsInitializing = false;
                    _viewModel.DisplayInitializeErrorCommand.Execute(data.Replace("\"", string.Empty)).Subscribe();
                    break;
                case "ShowHighlight":
                    _viewModel.ShowHighlightDialogCommand.Execute(new Models.App.ReaderContextMenuArgs() { Range = data.Replace("\"", string.Empty) });
                    break;
                case "Search":
                    _viewModel.InitializeSearchResultCommand.Execute(data).Subscribe();
                    break;
                default:
                    break;
            }
        }

        await Task.CompletedTask;
    }

    private async void OnRequestInitializeStyleAsync(object sender, EventArgs e)
        => await SetStyleAsync();

    private async void OnMiniViewRequestedAsync(object sender, EventArgs e)
        => await SetStyleAsync();

    private async Task SetStyleAsync()
    {
        await Reader.SetStyleAsync(
            _viewModel.FontFamily,
            _viewModel.FontSize,
            _viewModel.LineHeight,
            _viewModel.Background,
            _viewModel.Foreground,
            _viewModel.AdditionalStyle);
        await Reader.SetOptionsAsync(minSpreadWidth: _viewModel.SpreadMinWidth, isSmoothScroll: _viewModel.IsSmoothScroll);
    }

    private async void OnRequestChangeChapterAsync(object sender, Models.App.ReaderChapter e)
        => await Reader.GoToChapterAsync(e?.Href ?? string.Empty);

    private async void OnChapterSelectedAsync(object sender, Models.App.ReaderChapter e)
        => await Reader.GoToChapterAsync(e?.Href ?? string.Empty);

    private async void OnRequestRemoveHighlightAsync(object sender, Highlight e)
        => await Reader.RemoveHighlightAsync(e.CfiRange);

    private void ShowContextMenu(string argString)
    {
        var arg = JsonConvert.DeserializeObject<Models.App.ReaderContextMenuArgs>(argString);
        HighlightButton.Tag = TranslateButton.Tag = arg;
        CopyButton.Tag = ShareButton.Tag = OnlineSearchButton.Tag = arg.Text;
        if (arg.X > Reader.ActualWidth)
        {
            arg.X %= Reader.ActualWidth;
        }

        var showOptions = new FlyoutShowOptions
        {
            Position = new Windows.Foundation.Point(arg.X, arg.Y),
        };
        ContextMenuFlyout.ShowAt(Reader, showOptions);
    }

    private void InitializeTitleBar()
    {
        var bar = AppViewModel.Instance.AppWindow.TitleBar;
        bar.ButtonBackgroundColor = _viewModel.Background.ToColor();
        bar.ButtonForegroundColor = _viewModel.Foreground.ToColor();
        bar.ButtonInactiveBackgroundColor = _viewModel.Background.ToColor();
        bar.ButtonInactiveForegroundColor = _viewModel.Foreground.ToColor();
    }

    private void OnCopyTextButtonClick(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText((sender as AppBarButton).Tag.ToString());
        Clipboard.SetContent(dataPackage);
        ContextMenuFlyout.Hide();
    }

    private void OnShareButtonClick(object sender, RoutedEventArgs e)
    {
        var manager = DataTransferManagerHelper.GetForWindow();
        manager.DataRequested += OnShareDataRequested;
        DataTransferManagerHelper.ShowShareUIForWindow(AppViewModel.Instance.MainWindowHandle);
    }

    private void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        var package = args.Request.Data;
        package.Properties.Title = _viewModel.BookTitle;
        package.Properties.Description = _viewModel.CurrentChapterTitle;
        package.SetText(ShareButton.Tag.ToString());
    }

    private void OnHighlightButtonClick(object sender, RoutedEventArgs e)
    {
        ContextMenuFlyout.Hide();
        var arg = HighlightButton.Tag as Models.App.ReaderContextMenuArgs;
        _viewModel.ShowHighlightDialogCommand.Execute(arg).Subscribe();
    }

    private async void OnRequestHightlightAsync(object sender, Highlight e)
        => await Reader.SetHighlightAsync(e.CfiRange, e.Color);

    private void OnRetryTranslateButtonClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_viewModel.SourceText))
        {
            _viewModel.TranslateCommand.Execute(_viewModel.SourceText).Subscribe();
        }
    }

    private void OnTranslateButtonClick(object sender, RoutedEventArgs e)
    {
        ContextMenuFlyout.Hide();
        var args = TranslateButton.Tag as Models.App.ReaderContextMenuArgs;
        if (args.X < 300)
        {
            args.X = 300;
        }

        var showOptions = new FlyoutShowOptions
        {
            Position = new Windows.Foundation.Point(args.X, args.Y),
            Placement = FlyoutPlacementMode.Auto,
        };
        TranslateFlyout.ShowAt(Reader, showOptions);

        _viewModel.TranslateCommand.Execute(args.Text).Subscribe();
    }

    private void OnOnlineSearchButtonClick(object sender, RoutedEventArgs e)
    {
        ContextMenuFlyout.Hide();
        _viewModel.OnlineSearchCommand.Execute((sender as FrameworkElement).Tag.ToString()).Subscribe();
    }

    private async void OnRequestChangeLocationAsync(object sender, string e)
        => await Reader.ChangeLocationAsync(e);

    private async void OnRequestSearchAsync(object sender, string e)
        => await Reader.SearchAsync(e);

    private void OnFindAcceleratorInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        args.Handled = true;
        _viewModel.ShowSearchDailogCommand.Execute().Subscribe();
    }

    private void OnBackAcceleratorInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        args.Handled = true;
        _viewModel.BackCommand.Execute().Subscribe();
    }
}
