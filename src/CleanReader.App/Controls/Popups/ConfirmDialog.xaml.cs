// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 确认对话框.
    /// </summary>
    public sealed partial class ConfirmDialog : CustomDialog
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
}
