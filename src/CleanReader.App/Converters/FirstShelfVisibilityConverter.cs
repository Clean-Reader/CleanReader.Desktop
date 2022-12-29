// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Models.DataBase;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace CleanReader.App.Converters;

internal sealed class FirstShelfVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var shelf = value as Shelf;
        if (targetType == typeof(Visibility))
        {
            return string.IsNullOrEmpty(shelf.Id) ? Visibility.Collapsed : (object)Visibility.Visible;
        }
        else if (targetType == typeof(bool))
        {
            return shelf.Order != 0;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
