// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml;

namespace CleanReader.Core
{
    /// <summary>
    /// A layout style that displays icon, title, and description text on the head of <see cref="ExpanderEx"/>.
    /// </summary>
    public partial class ExpanderExDescriptor
    {
        /// <summary>
        /// Gets the dependency property for <see cref="Icon"/>.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(object), typeof(ExpanderExDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Gets the dependency property for <see cref="Title"/>.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ExpanderExDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Gets the dependency property for <see cref="Description"/>.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(ExpanderExDescriptor), new PropertyMetadata(null));

        /// <summary>
        /// Gets the dependency property for <see cref="IconVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty IconVisibilityProperty =
            DependencyProperty.Register(nameof(IconVisibility), typeof(Visibility), typeof(ExpanderExDescriptor), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnIconVisibilityChanged)));

        /// <summary>
        /// Gets the dependency property for <see cref="DescriptionVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty DescriptionVisibilityProperty =
            DependencyProperty.Register(nameof(DescriptionVisibility), typeof(Visibility), typeof(ExpanderExDescriptor), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets the dependency property for <see cref="IsAutoHideIcon"/>.
        /// </summary>
        public static readonly DependencyProperty IsAutoHideIconProperty =
            DependencyProperty.Register(nameof(IsAutoHideIcon), typeof(bool), typeof(ExpanderExDescriptor), new PropertyMetadata(true));

        /// <summary>
        /// Gets the dependency property for <see cref="AutoHideIconThreshold"/>.
        /// </summary>
        public static readonly DependencyProperty AutoHideIconThresholdProperty =
            DependencyProperty.Register(nameof(AutoHideIconThreshold), typeof(double), typeof(ExpanderExDescriptor), new PropertyMetadata(500d));

        /// <summary>
        /// Gets the dependency property for <see cref="InlineSpacing"/>.
        /// </summary>
        public static readonly DependencyProperty InlineSpacingProperty =
            DependencyProperty.Register(nameof(InlineSpacing), typeof(double), typeof(ExpanderExDescriptor), new PropertyMetadata(16d));

        /// <summary>
        /// Dependency property of <see cref="TitleFontSize"/>.
        /// </summary>
        public static readonly DependencyProperty TitleFontSizeProperty =
            DependencyProperty.Register(nameof(TitleFontSize), typeof(double), typeof(ExpanderExDescriptor), new PropertyMetadata(14d));

        /// <summary>
        /// Dependency property of <see cref="DescriptionFontSize"/>.
        /// </summary>
        public static readonly DependencyProperty DescriptionFontSizeProperty =
            DependencyProperty.Register(nameof(DescriptionFontSize), typeof(double), typeof(ExpanderExDescriptor), new PropertyMetadata(12d));

        /// <summary>
        /// Gets or sets icon.
        /// </summary>
        public object Icon
        {
            get { return (object)this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Gets or sets title.
        /// </summary>
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        public string Description
        {
            get { return (string)this.GetValue(DescriptionProperty); }
            set { this.SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// Gets or sets icon visibility.
        /// </summary>
        public Visibility IconVisibility
        {
            get { return (Visibility)this.GetValue(IconVisibilityProperty); }
            set { this.SetValue(IconVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets description visibility.
        /// </summary>
        public Visibility DescriptionVisibility
        {
            get { return (Visibility)this.GetValue(DescriptionVisibilityProperty); }
            set { this.SetValue(DescriptionVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to hide the icon automatically.
        /// </summary>
        public bool IsAutoHideIcon
        {
            get { return (bool)this.GetValue(IsAutoHideIconProperty); }
            set { this.SetValue(IsAutoHideIconProperty, value); }
        }

        /// <summary>
        /// Gets or sets the threshold, when the container width is lower than the threshold, the icon is automatically hidden.
        /// </summary>
        public double AutoHideIconThreshold
        {
            get { return (double)this.GetValue(AutoHideIconThresholdProperty); }
            set { this.SetValue(AutoHideIconThresholdProperty, value); }
        }

        /// <summary>
        /// Gets or sets the spacing between icon and text area.
        /// </summary>
        public double InlineSpacing
        {
            get { return (double)GetValue(InlineSpacingProperty); }
            set { SetValue(InlineSpacingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title font size.
        /// </summary>
        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the description font size.
        /// </summary>
        public double DescriptionFontSize
        {
            get { return (double)GetValue(DescriptionFontSizeProperty); }
            set { SetValue(DescriptionFontSizeProperty, value); }
        }
    }
}
