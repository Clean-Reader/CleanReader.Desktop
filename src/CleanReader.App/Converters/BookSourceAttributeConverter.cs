// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Services.Novel.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace CleanReader.App.Converters;

internal class BookSourceAttributeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var type = parameter.ToString();
        var source = value as BookSource;
        if (type == "Explore")
        {
            return source.IsExploreEnabled ? Visibility.Visible : Visibility.Collapsed;
        }
        else if (type == "Search")
        {
            return string.IsNullOrEmpty(source.Search?.SearchUrl) ? Visibility.Collapsed : Visibility.Visible;
        }
        else if (type == "BookDetail")
        {
            return string.IsNullOrEmpty(source.BookDetail?.Range) ? Visibility.Collapsed : Visibility.Visible;
        }
        else if (type == "Chapter")
        {
            return string.IsNullOrEmpty(source.Chapter?.Range) ? Visibility.Collapsed : Visibility.Visible;
        }
        else if (type == "ChapterContent")
        {
            return string.IsNullOrEmpty(source.ChapterContent?.Range) ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
