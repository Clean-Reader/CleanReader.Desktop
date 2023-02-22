// Copyright (c) Richasy. All rights reserved.

using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace CleanReader.Core;

/// <summary>
/// The container is used to display a set of <see cref="ExpanderWrapper"/>.
/// </summary>
public class ExpanderExMenuPanel : StackPanel
{
    /// <summary>
    /// Gets the dependency property for <see cref="MenuItemInlineIntermediatePadding"/>.
    /// </summary>
    public static readonly DependencyProperty MenuItemInlineWidePaddingProperty =
        DependencyProperty.Register(nameof(MenuItemInlineWidePadding), typeof(Thickness), typeof(ExpanderExMenuPanel), new PropertyMetadata(new Thickness(48, 8, 60, 8), new PropertyChangedCallback(OnItemPropertyChanged)));

    /// <summary>
    /// Gets the dependency property for <see cref="MenuItemInlineIntermediatePadding"/>.
    /// </summary>
    public static readonly DependencyProperty MenuItemInlineIntermediatePaddingProperty =
        DependencyProperty.Register(nameof(MenuItemInlineIntermediatePadding), typeof(Thickness), typeof(ExpanderExMenuPanel), new PropertyMetadata(new Thickness(16, 8, 16, 8), new PropertyChangedCallback(OnItemPropertyChanged)));

    /// <summary>
    /// Gets the dependency property for <see cref="MenuItemWrapRowSpacing"/>.
    /// </summary>
    public static readonly DependencyProperty MenuItemWrapRowSpacingProperty =
        DependencyProperty.Register(nameof(MenuItemWrapRowSpacing), typeof(double), typeof(ExpanderExMenuPanel), new PropertyMetadata(8d, new PropertyChangedCallback(OnItemPropertyChanged)));

    /// <summary>
    /// Gets the dependency property for <see cref="MenuItemWrapMargin"/>.
    /// </summary>
    public static readonly DependencyProperty MenuItemWrapMarginProperty =
        DependencyProperty.Register(nameof(MenuItemWrapMargin), typeof(Thickness), typeof(ExpanderExMenuPanel), new PropertyMetadata(new Thickness(16, 8, 16, 8), new PropertyChangedCallback(OnItemPropertyChanged)));

    /// <summary>
    /// Gets or sets the inner <see cref="ExpanderExWrapper"/> padding of the container when the elements are on the same line and in wide state.
    /// </summary>
    public Thickness MenuItemInlineWidePadding
    {
        get => (Thickness)GetValue(MenuItemInlineWidePaddingProperty);
        set => SetValue(MenuItemInlineWidePaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets the inner <see cref="ExpanderExWrapper"/> padding of the container when the elements are on the same line and in intermediate state.
    /// </summary>
    public Thickness MenuItemInlineIntermediatePadding
    {
        get => (Thickness)GetValue(MenuItemInlineIntermediatePaddingProperty);
        set => SetValue(MenuItemInlineIntermediatePaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets row spacing when inner <see cref="ExpanderExWrapper"/> displaying different rows.
    /// </summary>
    public double MenuItemWrapRowSpacing
    {
        get => (double)GetValue(MenuItemWrapRowSpacingProperty);
        set => SetValue(MenuItemWrapRowSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets when the flow, the margin of the inner <see cref="ExpanderExWrapper"/> container.
    /// </summary>
    public Thickness MenuItemWrapMargin
    {
        get => (Thickness)GetValue(MenuItemWrapMarginProperty);
        set => SetValue(MenuItemWrapMarginProperty, value);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0)
        {
            return new Size(0, 0);
        }
        else
        {
            if (Children.Count == 1)
            {
                var child = Children.First();

                // If the user uses the data source control,
                // here we need to traverse the generated wrapper instance.
                if (child is ItemsRepeater repeater)
                {
                    repeater.ElementPrepared -= OnRepeaterElementPrepared;
                    repeater.ElementPrepared += OnRepeaterElementPrepared;
                    foreach (var item in repeater.FindDescendantElements<ExpanderExWrapper>())
                    {
                        UpdateInternalWrapperLayout(availableSize, item);
                    }
                }
                else if (child is ItemsControl itemsControl)
                {
                    foreach (var item in itemsControl.Items)
                    {
                        var container = itemsControl.ContainerFromItem(item);
                        if (container == null)
                        {
                            continue;
                        }

                        var wrappers = container.FindDescendantElements<ExpanderExWrapper>();
                        foreach (var wrapper in wrappers)
                        {
                            UpdateInternalWrapperLayout(availableSize, wrapper);
                        }
                    }
                }
            }
            else
            {
                foreach (var child in Children)
                {
                    if (child is ExpanderExWrapper wrapper)
                    {
                        UpdateInternalWrapperLayout(availableSize, wrapper);
                    }
                }
            }
        }

        return base.MeasureOverride(availableSize);
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize) => base.ArrangeOverride(finalSize);

    private static void OnItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ExpanderExMenuPanel).InvalidateMeasure();

    private void OnRepeaterElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        foreach (var item in sender.FindDescendantElements<ExpanderExWrapper>())
        {
            UpdateInternalWrapperLayout(this.DesiredSize, item);
        }
    }

    private void UpdateInternalWrapperLayout(Size availableSize, ExpanderExWrapper wrapper)
    {
        wrapper.WrapMargin = MenuItemWrapMargin;
        wrapper.WrapRowSpacing = MenuItemWrapRowSpacing;
        wrapper.InlineWidePadding = MenuItemInlineWidePadding;
        wrapper.InlineIntermediatePadding = MenuItemInlineIntermediatePadding;
        wrapper.Measure(availableSize);
        wrapper.CheckVisualState();
    }
}
