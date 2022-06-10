// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using Microsoft.UI.Xaml.Data;

namespace CleanReader.App.Converters;

internal sealed class ReadSectionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is ReadSection section)
        {
            var duration = section.EndTime - section.StartTime;
            if (duration.TotalHours > 1)
            {
                return $"{Math.Round(duration.TotalHours, 2)} {StringResources.Hours}";
            }
            else if (duration.TotalMinutes > 1)
            {
                return $"{Math.Round(duration.TotalMinutes)} {StringResources.Minutes}";
            }
            else
            {
                return $"{Math.Round(duration.TotalSeconds)} {StringResources.Seconds}";
            }
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
