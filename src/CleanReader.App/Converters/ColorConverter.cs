// Copyright (c) Richasy. All rights reserved.

using System;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace CleanReader.App.Converters
{
    /// <summary>
    /// 颜色转换器.
    /// </summary>
    public class ColorConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Windows.UI.Color color;
            if (string.IsNullOrEmpty(value?.ToString()))
            {
                color = Colors.Transparent;
            }
            else
            {
                color = ((string)value).ToColor();
            }

            return targetType == typeof(Brush) ? new SolidColorBrush(color) : color;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
