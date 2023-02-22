// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CleanReader.Core;

/// <summary>
/// A layout style that displays icon, title, and description text on the head of <see cref="ExpanderEx"/>.
/// </summary>
public partial class ExpanderExDescriptor : Control
{
    private const string RootGridName = "RootGrid";

    private Grid _rootGrid;
    private FrameworkElement _parent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpanderExDescriptor"/> class.
    /// </summary>
    public ExpanderExDescriptor() => this.DefaultStyleKey = typeof(ExpanderExDescriptor);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        this._rootGrid = (Grid)this.GetTemplateChild(RootGridName);
        this._parent = this.FindAscendant(typeof(Microsoft.UI.Xaml.Controls.Expander), typeof(ExpanderEx)) as FrameworkElement;
        this.SizeChanged += this.OnSizeChanged;
        this.CheckIconVisibility();
    }

    private static void OnIconVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as ExpanderExDescriptor;
        if (e.NewValue is Visibility visibility)
        {
            instance.CheckIconVisibility();
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (this._parent != null && this.IsAutoHideIcon)
        {
            this.IconVisibility = this._parent.ActualWidth < this.AutoHideIconThreshold ?
                Visibility.Collapsed : Visibility.Visible;
        }
    }

    private void CheckIconVisibility()
    {
        if (this._rootGrid != null)
        {
            this._rootGrid.ColumnSpacing = this.IconVisibility == Visibility.Visible ? 16 : 0;
        }
    }
}
