// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Models.Constants;
using CleanReader.Models.Resources;
using Microsoft.UI.Xaml.Data;

namespace CleanReader.App.Converters
{
    internal class ThemeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var themeStr = value.ToString();
            var result = string.Empty;
            switch (themeStr)
            {
                case AppConstants.ThemeLight:
                    result = StringResources.Light;
                    break;
                case AppConstants.ThemeDark:
                    result = StringResources.Dark;
                    break;
                default:
                    result = StringResources.FollowSystem;
                    break;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
