// Copyright (c) Richasy. All rights reserved.

using CleanReader.Controls.Interfaces;

namespace CleanReader.Controls;

/// <summary>
/// 确认对话框.
/// </summary>
public sealed partial class ConfirmDialog : CustomDialog, IConfirmDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfirmDialog"/> class.
    /// </summary>
    public ConfirmDialog() => InitializeComponent();

    /// <inheritdoc/>
    public override void InjectData(object data)
    {
        if (data is string content)
        {
            ContentBlock.Text = content;
        }
    }
}
