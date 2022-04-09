// Copyright (c) Richasy. All rights reserved.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace CleanReader.App.Converters
{
    /// <summary>
    /// 布尔值到可见性的转换器.
    /// </summary>
    internal class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 是否反转结果.
        /// </summary>
        public bool IsReverse { get; set; }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var vis = Visibility.Visible;
            if (value is bool v)
            {
                vis = IsReverse == v ? Visibility.Collapsed : Visibility.Visible;
            }

            return vis;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var vis = (Visibility)value;
            var result = vis == Visibility.Visible;
            return IsReverse ? !result : result;
        }
    }
}
