// Copyright (c) Richasy. All rights reserved.

using System;
using Microsoft.UI.Xaml;

namespace CleanReader.Core;

/// <summary>
/// Click event arguments of ExpanderEx.
/// </summary>
public class ExpanderExClickEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpanderExClickEventArgs"/> class.
    /// </summary>
    public ExpanderExClickEventArgs()
    {
    }

    internal ExpanderExClickEventArgs(bool isExpander, FrameworkElement source)
    {
        this.IsExpander = isExpander;
        this.SourceElement = source;
    }

    /// <summary>
    /// Whether the object which trigger the click event is <see cref="Microsoft.UI.Xaml.Controls.Expander"/>.
    /// </summary>
    public bool IsExpander { get; set; }

    /// <summary>
    /// The UI object that send the click event, which may be <see cref="Microsoft.UI.Xaml.Controls.Expander"/>
    /// or <see cref="ExpanderExQuadratePanel"/>.
    /// </summary>
    public FrameworkElement SourceElement { get; set; }
}
