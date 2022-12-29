// Copyright (c) Richasy. All rights reserved.

using System;
using Microsoft.UI.Xaml.Data;

namespace CleanReader.App.Converters;

/// <summary>
/// 布尔值反转转换器.
/// </summary>
internal class BoolReverseConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
        => !System.Convert.ToBoolean(value);

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
