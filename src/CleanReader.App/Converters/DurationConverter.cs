// Copyright (c) Richasy. All rights reserved.

using System;
using Microsoft.UI.Xaml.Data;

namespace CleanReader.App.Converters;

internal class DurationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var duration = (TimeSpan)value;
        var type = parameter.ToString();
        if (type == "Hour")
        {
            return Math.Round(duration.TotalHours, 2).ToString();
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
