// Copyright (c) Richasy. All rights reserved.

using System.Linq;
using CleanReader.Models.Constants;
using CleanReader.ViewModels.Desktop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// 主题设置条目.
    /// </summary>
    public sealed partial class ThemeSettingItem : UserControl
    {
        private readonly SettingsPageViewModel _viewModel = SettingsPageViewModel.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeSettingItem"/> class.
        /// </summary>
        public ThemeSettingItem() => InitializeComponent();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            switch (_viewModel.Theme)
            {
                case AppConstants.ThemeDefault:
                    SystemThemeRadioButton.IsChecked = true;
                    break;
                case AppConstants.ThemeLight:
                    LightThemeRadioButton.IsChecked = true;
                    break;
                case AppConstants.ThemeDark:
                    DarkThemeRadioButton.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private void OnThemeRadioButtonSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Any() && e.AddedItems.First() is RadioButton btn)
            {
                var selectTheme = string.Empty;
                if (btn == LightThemeRadioButton)
                {
                    selectTheme = AppConstants.ThemeLight;
                }
                else if (btn == DarkThemeRadioButton)
                {
                    selectTheme = AppConstants.ThemeDark;
                }
                else
                {
                    selectTheme = AppConstants.ThemeDefault;
                }

                _viewModel.Theme = selectTheme;
            }
        }
    }
}
