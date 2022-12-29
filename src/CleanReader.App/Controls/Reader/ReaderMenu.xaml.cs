// Copyright (c) Richasy. All rights reserved.

using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls;

/// <summary>
/// 阅读器菜单.
/// </summary>
public sealed partial class ReaderMenu : UserControl
{
    private readonly ReaderViewModel _viewModel = ReaderViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderMenu"/> class.
    /// </summary>
    public ReaderMenu() =>
        InitializeComponent();
}
