// Copyright (c) Richasy. All rights reserved.

using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls;

/// <summary>
/// 库设置.
/// </summary>
public sealed partial class LibrarySettingItem : UserControl
{
    private readonly SettingsPageViewModel _viewModel = SettingsPageViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="LibrarySettingItem"/> class.
    /// </summary>
    public LibrarySettingItem() => InitializeComponent();
}
