// Copyright (c) Richasy. All rights reserved.

using CleanReader.Locator.Lib;
using CleanReader.Toolkit.Interfaces;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls;

/// <summary>
/// 应用标题栏.
/// </summary>
public sealed partial class AppTitleBar : UserControl
{
    private readonly LibraryViewModel _libraryViewModel = LibraryViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppTitleBar"/> class.
    /// </summary>
    public AppTitleBar()
    {
        InitializeComponent();
        Instance = this;
        ActualThemeChanged += OnActualThemeChanged;
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static AppTitleBar Instance { get; private set; }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
        => ServiceLocator.Instance.GetService<IAppToolkit>().InitializeTitleBar(AppViewModel.Instance.AppWindow.TitleBar);
}
