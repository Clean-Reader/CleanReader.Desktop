// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace CleanReader.Core;

/// <summary>
/// Tools.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Find first visual ascendant control of a possible type.
    /// </summary>
    /// <param name="element">Child element.</param>
    /// <param name="types">Type arry of ascendant to look for.</param>
    /// <returns>Ascendant control or null if not found.</returns>
    public static object FindAscendant(this DependencyObject element, params Type[] types)
    {
        var parent = VisualTreeHelper.GetParent(element);

        if (parent == null)
        {
            return null;
        }

        var parentType = parent.GetType();
        if (types.Any(p => parentType == p))
        {
            return parent;
        }

        return parent.FindAscendant(types);
    }

    /// <summary>
    /// Find descendant <see cref="FrameworkElement"/> control using its name.
    /// </summary>
    /// <param name="element">Parent element.</param>
    /// <param name="name">Name of the control to find.</param>
    /// <returns>Descendant control or null if not found.</returns>
    public static FrameworkElement FindDescendantByName(this DependencyObject element, string name)
    {
        if (element == null || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        if (name.Equals((element as FrameworkElement)?.Name, StringComparison.OrdinalIgnoreCase))
        {
            return element as FrameworkElement;
        }

        var childCount = VisualTreeHelper.GetChildrenCount(element);
        for (var i = 0; i < childCount; i++)
        {
            var result = VisualTreeHelper.GetChild(element, i).FindDescendantByName(name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// Find all descendant <see cref="FrameworkElement"/> controls using its type.
    /// </summary>
    /// <typeparam name="T">Control type.</typeparam>
    /// <param name="element">Parent control.</param>
    /// <returns>Control list.</returns>
    public static IEnumerable<T> FindDescendantElements<T>(this DependencyObject element)
        where T : DependencyObject
    {
        if (element == null)
        {
            yield break;
        }

        var childCount = VisualTreeHelper.GetChildrenCount(element);
        for (var i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            if (child.GetType() == typeof(T))
            {
                yield return (T)child;
            }
            else
            {
                foreach (var item in FindDescendantElements<T>(child))
                {
                    yield return item;
                }
            }
        }
    }
}
