// Copyright (c) Richasy. All rights reserved.

using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 应用语言设置项.
    /// </summary>
    public sealed partial class LanguageSettingItem : UserControl
    {
        private readonly SettingsPageViewModel _viewModel = SettingsPageViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageSettingItem"/> class.
        /// </summary>
        public LanguageSettingItem() => InitializeComponent();

        private void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageBox.SelectedItem is ComboBoxItem item)
            {
                var code = item.Tag.ToString();
                if (!string.IsNullOrEmpty(code))
                {
                    _viewModel.LanguageCode = code;
                }
            }
        }

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var selectedIndex = _viewModel.LanguageCode switch
            {
                "zh-CN" => 0,
                "en-US" => 1,
                _ => -1,
            };

            LanguageBox.SelectedIndex = selectedIndex;
        }
    }
}
