// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml;

namespace CleanReader.Core
{
    /// <summary>
    /// To encapsulate the Expander in Microsoft.UI.Xaml, customize the icon, content and expand content.
    /// </summary>
    public partial class ExpanderEx
    {
        /// <summary>
        /// Gets the dependency property for <see cref="Header"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(ExpanderEx), new PropertyMetadata(null));

        /// <summary>
        /// Gets the dependency property for <see cref="Content"/>.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(ExpanderEx), new PropertyMetadata(null, new PropertyChangedCallback(OnContentChanged)));

        /// <summary>
        /// Gets the dependency property for <see cref="InternalExpanderStyle"/>.
        /// </summary>
        public static readonly DependencyProperty InternalExpanderStyleProperty =
            DependencyProperty.Register(nameof(InternalExpanderStyle), typeof(Style), typeof(ExpanderEx), new PropertyMetadata(null));

        /// <summary>
        /// Gets the dependency property for <see cref="InternalQuadrateStyle"/>.
        /// </summary>
        public static readonly DependencyProperty InternalQuadrateStyleProperty =
            DependencyProperty.Register(nameof(InternalQuadrateStyle), typeof(Style), typeof(ExpanderEx), new PropertyMetadata(null));

        /// <summary>
        /// Gets the dependency property for <see cref="IsExpanded"/>.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(ExpanderEx), new PropertyMetadata(default));

        /// <summary>
        /// Gets the dependency property for <see cref="ForceUseExpander"/>.
        /// </summary>
        public static readonly DependencyProperty ForceUseExpanderProperty =
            DependencyProperty.Register(nameof(ForceUseExpander), typeof(bool), typeof(ExpanderEx), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets header content.
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content of internal <see cref="Microsoft.UI.Xaml.Controls.Expander"/>,
        /// when it is <c>null</c>, it displays <see cref="ExpanderExQuadratePanel"/>, does not support expand/collapse behavior.
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets header content.
        /// </summary>
        public Style InternalExpanderStyle
        {
            get { return (Style)GetValue(InternalExpanderStyleProperty); }
            set { SetValue(InternalExpanderStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets header content.
        /// </summary>
        public Style InternalQuadrateStyle
        {
            get { return (Style)GetValue(InternalQuadrateStyleProperty); }
            set { SetValue(InternalQuadrateStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the internal <see cref="Microsoft.UI.Xaml.Controls.Expander.IsExpanded"/> property.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to force the use of <see cref="Microsoft.UI.Xaml.Controls.Expander.IsExpanded"/> as a display container.
        /// </summary>
        public bool ForceUseExpander
        {
            get { return (bool)GetValue(ForceUseExpanderProperty); }
            set { SetValue(ForceUseExpanderProperty, value); }
        }
    }
}
