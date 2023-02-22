// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml;

namespace CleanReader.Core;

/// <summary>
/// The container that display the dynamic wrap items.
/// </summary>
public partial class ExpanderExWrapper
{
    /// <summary>
    /// Gets the dependency property for <see cref="WrapThreshold"/>.
    /// </summary>
    public static readonly DependencyProperty WrapThresholdProperty =
        DependencyProperty.Register(nameof(WrapThreshold), typeof(double), typeof(ExpanderExWrapper), new PropertyMetadata(342d));

    /// <summary>
    /// Gets the dependency property for <see cref="IntermediateThreshold"/>.
    /// </summary>
    public static readonly DependencyProperty IntermediateThresholdProperty =
        DependencyProperty.Register(nameof(IntermediateThreshold), typeof(double), typeof(ExpanderExWrapper), new PropertyMetadata(500d));

    /// <summary>
    /// Gets the dependency property for <see cref="WrapContentHorizontalAlignment"/>.
    /// </summary>
    public static readonly DependencyProperty WrapContentHorizontalAlignmentProperty =
        DependencyProperty.Register(nameof(WrapContentHorizontalAlignment), typeof(HorizontalAlignment), typeof(ExpanderExWrapper), new PropertyMetadata(HorizontalAlignment.Left));

    /// <summary>
    /// Gets the dependency property for <see cref="MainContent"/>.
    /// </summary>
    public static readonly DependencyProperty MainContentProperty =
        DependencyProperty.Register(nameof(MainContent), typeof(object), typeof(ExpanderExWrapper), new PropertyMetadata(null));

    /// <summary>
    /// Gets the dependency property for <see cref="WrapContent"/>.
    /// </summary>
    public static readonly DependencyProperty WrapContentProperty =
        DependencyProperty.Register(nameof(WrapContent), typeof(object), typeof(ExpanderExWrapper), new PropertyMetadata(null));

    /// <summary>
    /// Gets the dependency property for <see cref="InlineColumnSpacing"/>.
    /// </summary>
    public static readonly DependencyProperty InlineColumnSpacingProperty =
        DependencyProperty.Register(nameof(InlineColumnSpacing), typeof(double), typeof(ExpanderExWrapper), new PropertyMetadata(16d));

    /// <summary>
    /// Gets the dependency property for <see cref="WrapRowSpacing"/>.
    /// </summary>
    public static readonly DependencyProperty WrapRowSpacingProperty =
        DependencyProperty.Register(nameof(WrapRowSpacing), typeof(double), typeof(ExpanderExWrapper), new PropertyMetadata(0d));

    /// <summary>
    /// Gets the dependency property for <see cref="WrapMargin"/>.
    /// </summary>
    public static readonly DependencyProperty WrapMarginProperty =
        DependencyProperty.Register(nameof(WrapMargin), typeof(Thickness), typeof(ExpanderExWrapper), new PropertyMetadata(new Thickness(0)));

    /// <summary>
    /// Gets the dependency property for <see cref="InlineWidePadding"/>.
    /// </summary>
    public static readonly DependencyProperty InlineWidePaddingProperty =
        DependencyProperty.Register(nameof(InlineWidePadding), typeof(Thickness), typeof(ExpanderExWrapper), new PropertyMetadata(new Thickness(16, 0, 64, 0)));

    /// <summary>
    /// Gets the dependency property for <see cref="InlineIntermediatePadding"/>.
    /// </summary>
    public static readonly DependencyProperty InlineIntermediatePaddingProperty =
        DependencyProperty.Register(nameof(InlineIntermediatePadding), typeof(Thickness), typeof(ExpanderExWrapper), new PropertyMetadata(new Thickness(16, 0, 16, 0)));

    /// <summary>
    /// Gets or sets threshold width of wrap state switching.
    /// </summary>
    public double WrapThreshold
    {
        get => (double)this.GetValue(WrapThresholdProperty);
        set => this.SetValue(WrapThresholdProperty, value);
    }

    /// <summary>
    /// Gets or sets threshold for intermediate state.
    /// </summary>
    /// <remarks>
    /// Intermediate is a preset state.
    /// The user can define a state between WrapState and NormalState
    /// to control the margins of internal elements to achieve better use.
    /// </remarks>
    public double IntermediateThreshold
    {
        get => (double)this.GetValue(IntermediateThresholdProperty);
        set => this.SetValue(IntermediateThresholdProperty, value);
    }

    /// <summary>
    /// Gets or sets horizontal alignment of wrapable content after wrap.
    /// </summary>
    public HorizontalAlignment WrapContentHorizontalAlignment
    {
        get => (HorizontalAlignment)this.GetValue(WrapContentHorizontalAlignmentProperty);
        set => this.SetValue(WrapContentHorizontalAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets main content, location fixed.
    /// </summary>
    public object MainContent
    {
        get => (object)this.GetValue(MainContentProperty);
        set => this.SetValue(MainContentProperty, value);
    }

    /// <summary>
    /// Gets or sets wrapped content, variable location.
    /// </summary>
    public object WrapContent
    {
        get => (object)this.GetValue(WrapContentProperty);
        set => this.SetValue(WrapContentProperty, value);
    }

    /// <summary>
    /// Gets or sets when in the same row, the column spacing of the container.
    /// </summary>
    public double InlineColumnSpacing
    {
        get => (double)this.GetValue(InlineColumnSpacingProperty);
        set => this.SetValue(InlineColumnSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets row spacing when displaying different rows.
    /// </summary>
    public double WrapRowSpacing
    {
        get => (double)this.GetValue(WrapRowSpacingProperty);
        set => this.SetValue(WrapRowSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets when the flow, the margin of the container.
    /// </summary>
    public Thickness WrapMargin
    {
        get => (Thickness)this.GetValue(WrapMarginProperty);
        set => this.SetValue(WrapMarginProperty, value);
    }

    /// <summary>
    /// Gets or sets the inner padding of the container when the elements are on the same line and in wide state.
    /// </summary>
    public Thickness InlineWidePadding
    {
        get => (Thickness)this.GetValue(InlineWidePaddingProperty);
        set => this.SetValue(InlineWidePaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets the inner padding of the container when the elements are on the same line and in intermediate state.
    /// </summary>
    public Thickness InlineIntermediatePadding
    {
        get => (Thickness)this.GetValue(InlineIntermediatePaddingProperty);
        set => this.SetValue(InlineIntermediatePaddingProperty, value);
    }
}
