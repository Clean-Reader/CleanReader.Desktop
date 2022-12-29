// Copyright (c) Richasy. All rights reserved.

using CleanReader.ViewModels.Desktop;

namespace CleanReader.App.Controls;

/// <summary>
/// 书籍信息对话框.
/// </summary>
public sealed partial class BookInformationDialog : CustomDialog
{
    private ShelfBookViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookInformationDialog"/> class.
    /// </summary>
    public BookInformationDialog() => InitializeComponent();

    /// <inheritdoc/>
    public override void InjectData(object data)
    {
        if (data is ShelfBookViewModel vm)
        {
            _viewModel = vm;
        }
    }

    /// <inheritdoc/>
    public override void OnPrimaryButtonClick()
    {
        if (!string.IsNullOrEmpty(BookNameBox.Text))
        {
            _viewModel.Book.Title = BookNameBox.Text;
        }

        if (!string.IsNullOrEmpty(AuthorBox.Text))
        {
            _viewModel.Book.Author = AuthorBox.Text;
        }

        _viewModel.Cover = CoverBox.Text;

        if (!string.IsNullOrEmpty(DescriptionBox.Text))
        {
            _viewModel.Book.Description = DescriptionBox.Text;
        }
    }
}
