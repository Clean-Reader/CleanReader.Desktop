// Copyright (c) Richasy. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 进度对话框.
    /// </summary>
    public sealed partial class ProgressDialog : CustomDialog
    {
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressDialog"/> class.
        /// </summary>
        public ProgressDialog() => InitializeComponent();

        /// <inheritdoc/>
        public override void InjectData(object data)
        {
            if (data is string text)
            {
                TipString.Text = text;
            }
            else if (data is Tuple<int, int, string> progress)
            {
                ProgressBar.IsIndeterminate = false;
                ProgressBar.Maximum = progress.Item2;
                ProgressBar.Value = progress.Item1;
                TipString.Text = progress.Item3;
            }
        }

        /// <inheritdoc/>
        public override async void InjectTask(Task task, CancellationTokenSource cancellationTokenSource = null)
        {
            _cancellationTokenSource = cancellationTokenSource;
            CancelButton.Visibility = cancellationTokenSource == null ? Microsoft.UI.Xaml.Visibility.Collapsed : Microsoft.UI.Xaml.Visibility.Visible;
            try
            {
                if (cancellationTokenSource == null)
                {
                    await task;
                }
                else
                {
                    await task.WaitAsync(_cancellationTokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;

                // LOG error.
            }
            finally
            {
                Hide();
            }
        }

        private void OnCancelButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }

            Hide();
        }
    }
}
