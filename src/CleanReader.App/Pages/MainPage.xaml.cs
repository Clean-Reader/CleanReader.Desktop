// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using CleanReader.Models.App;
using CleanReader.Models.Resources;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.System;

namespace CleanReader.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly AppViewModel _viewModel;
        private readonly LibraryViewModel _libraryViewModel;
        private bool _isLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            _viewModel = AppViewModel.Instance;
            _libraryViewModel = LibraryViewModel.Instance;
            _viewModel.NavigationRequested += OnNavigationRequested;
            Loaded += OnLoadedAsync;
        }

        /// <inheritdoc/>
        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (_viewModel.NavigationList.Count == 0)
            {
                _isLoaded = false;
                InitializeNavigation();
            }
        }

        /// <inheritdoc/>
        protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
            => MainNavView.MenuItems.Clear();

        private static void InitializeItems(IList<object> menuItems, IEnumerable<NavigationItem> items)
        {
            foreach (var item in items)
            {
                var uiItem = item.Type switch
                {
                    NavigationItemType.Item => (object)new NavigationViewItem() { DataContext = item, Content = item.Name, Icon = GetIcon(item.Icon) },
                    NavigationItemType.Header => (object)new NavigationViewItemHeader() { Content = item.Name },
                    NavigationItemType.Separator => (object)new NavigationViewItemSeparator(),
                    _ => throw new NotImplementedException(),
                };

                if (item.Children != null)
                {
                    InitializeItems(((NavigationViewItem)uiItem).MenuItems, item.Children);
                }

                menuItems.Add(uiItem);
            }

            IconElement GetIcon(string icon)
            {
                return !string.IsNullOrEmpty(icon)
                    ? icon.Length > 1
                        ? new FontIcon() { FontFamily = new FontFamily("Segoe UI Emoji"), Glyph = icon }
                        : new FontIcon() { FontFamily = new FontFamily("Segoe Fluent Icons"), Glyph = icon }
                    : null;
            }
        }

        private async void OnLoadedAsync(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                return;
            }

            _isLoaded = true;
            try
            {
                _viewModel.IsInitializing = true;
                await LibraryViewModel.Instance.InitializeLibraryAsync();

                // BackgroundMusicViewModel.Instance.OpenConnectionCommand.Execute().Subscribe();
                _viewModel.IsInitializing = false;
            }
            catch (LibraryInitializeException le)
            {
                _viewModel.RequestMigration(le.Result);
                return;
            }
            catch (Exception)
            {
                _viewModel.RequestStartup();
                _libraryViewModel.ExceptionMessage = StringResources.InvalidLibraryPath;
                return;
            }

            _viewModel.RequestNavigateTo(typeof(ShelfPage));

            if (string.IsNullOrEmpty(_viewModel.InitializeFilePath))
            {
                await _libraryViewModel.CheckContinueReadingAsync();
            }
            else
            {
                await _libraryViewModel.CheckOpenFileOrImportAsync();
            }

#if !DEBUG
            _viewModel.CheckGithubUpdateCommand.Execute().Subscribe();
#endif
        }

        private void OnMainNavViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                _viewModel.RequestNavigateTo(typeof(SettingsPage));
            }
            else if (args.InvokedItemContainer is NavigationViewItem navItem)
            {
                _viewModel.RequestNavigateTo(navItem.DataContext as NavigationItem);
            }
        }

        private void InitializeNavigation()
        {
            MainNavView.MenuItems.Clear();
            MainNavView.FooterMenuItems.Clear();
            var navList = _viewModel.NavigationList;
            navList.Add(new NavigationItem(NavigationItemType.Header, NavigationItemPosition.Default, StringResources.Read));
            navList.Add(new NavigationItem(NavigationItemType.Item, NavigationItemPosition.Default, StringResources.MyBookShelf, "\uE1D3", typeof(ShelfPage)));
            navList.Add(new NavigationItem(NavigationItemType.Item, NavigationItemPosition.Default, StringResources.Explore, "\uE12B", typeof(ExplorePage)));
            navList.Add(new NavigationItem(NavigationItemType.Separator, NavigationItemPosition.Default));
            navList.Add(new NavigationItem(NavigationItemType.Header, NavigationItemPosition.Default, StringResources.Tools));

            navList.Add(new NavigationItem(NavigationItemType.Item, NavigationItemPosition.Default, StringResources.NoteManagement, "\uE193", typeof(NoteOverviewPage)));
            navList.Add(new NavigationItem(NavigationItemType.Item, NavigationItemPosition.Default, StringResources.BookSourceManagement, "\uEC7A", typeof(BookSourceOverviewPage)));
            navList.Add(new NavigationItem(NavigationItemType.Item, NavigationItemPosition.Default, StringResources.ReadTime, "\uF182", typeof(ReadDurationPage)));

            navList.ForEach(p => InitializeItems(p.Position == NavigationItemPosition.Default ? MainNavView.MenuItems : MainNavView.FooterMenuItems, new List<NavigationItem>() { p }));
        }

        private void OnNavigationRequested(object sender, NavigationEventArgs e)
        {
            var navItem = _viewModel.NavigationList.Where(p => p.PageType == e.PageType).FirstOrDefault();
            if (navItem == null)
            {
                navItem = _viewModel.NavigationList.Where(p => p.Children != null).SelectMany(p => p.Children).Where(p => p.Id == e.Id).FirstOrDefault();
            }

            if (navItem != null)
            {
                foreach (var item in MainNavView.MenuItems.Concat(MainNavView.FooterMenuItems).OfType<NavigationViewItem>())
                {
                    if (item.DataContext.Equals(navItem))
                    {
                        MainNavView.SelectedItem = item;
                        break;
                    }
                    else if (item.MenuItems != null && item.MenuItems.Count > 0)
                    {
                        var childItem = item.MenuItems.OfType<NavigationViewItem>().Where(p => p.DataContext.Equals(navItem)).FirstOrDefault();
                        if (childItem != null)
                        {
                            MainNavView.SelectedItem = childItem;
                            break;
                        }
                    }
                }
            }
            else if (e.PageType == typeof(SettingsPage))
            {
                MainNavView.SelectedItem = MainNavView.SettingsItem;
            }

            MainFrame.Navigate(e.PageType, e.Parameter, new DrillInNavigationTransitionInfo());
        }

        private async void OnHelpButtonClickAsync(object sender, RoutedEventArgs e)
            => await Launcher.LaunchUriAsync(new Uri("https://docs.richasy.cn/clean-reader/desktop"));
    }
}
