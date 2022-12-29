// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace CleanReader.App.Pages;

/// <summary>
/// 设置页面.
/// </summary>
public sealed partial class SettingsPage : Page
{
    private readonly SettingsPageViewModel _viewModel = SettingsPageViewModel.Instance;
    private readonly LibraryViewModel _libraryViewModel = LibraryViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
        => _viewModel.InitializeCommand.Execute().Subscribe();
}
