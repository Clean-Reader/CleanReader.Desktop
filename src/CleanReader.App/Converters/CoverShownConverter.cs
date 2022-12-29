// Copyright (c) Richasy. All rights reserved.

using System;
using System.IO;
using Microsoft.UI.Xaml.Data;

namespace CleanReader.App.Converters;

internal class CoverShownConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => value is string cover
            ? !string.IsNullOrEmpty(cover) && (cover.StartsWith("http", StringComparison.OrdinalIgnoreCase) || Path.HasExtension(cover))
            : (object)false;

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
