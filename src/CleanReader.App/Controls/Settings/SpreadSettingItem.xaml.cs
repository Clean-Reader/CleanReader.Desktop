// Copyright (c) Richasy. All rights reserved.

using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls;

/// <summary>
/// 分栏阅读设置条目.
/// </summary>
public sealed partial class SpreadSettingItem : UserControl
{
    private readonly SettingsPageViewModel _viewModel = SettingsPageViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpreadSettingItem"/> class.
    /// </summary>
    public SpreadSettingItem() => InitializeComponent();
}
