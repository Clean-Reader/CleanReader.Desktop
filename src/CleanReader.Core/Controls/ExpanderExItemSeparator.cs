// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CleanReader.Core
{
    /// <summary>
    /// Represents a line that separates items in <see cref="ExpanderEx.Content"/>.
    /// </summary>
    public sealed class ExpanderExItemSeparator : Control
    {
        /// <summary>
        /// Gets the dependency property for <see cref="StrokeBrush"/>.
        /// </summary>
        public static readonly DependencyProperty StrokeBrushProperty =
            DependencyProperty.Register(nameof(StrokeBrush), typeof(Brush), typeof(ExpanderExItemSeparator), new PropertyMetadata(default));

        /// <summary>
        /// Gets the dependency property for <see cref="StrokeThickness"/>.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(ExpanderExItemSeparator), new PropertyMetadata(1d));

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpanderExItemSeparator"/> class.
        /// </summary>
        public ExpanderExItemSeparator()
        {
            this.DefaultStyleKey = typeof(ExpanderExItemSeparator);
        }

        /// <summary>
        /// Gets or sets separator stroke color.
        /// </summary>
        public Brush StrokeBrush
        {
            get { return (Brush)GetValue(StrokeBrushProperty); }
            set { SetValue(StrokeBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets separator stroke thickness.
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
    }
}
