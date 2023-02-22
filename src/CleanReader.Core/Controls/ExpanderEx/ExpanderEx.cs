// Copyright (c) Richasy. All rights reserved.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace CleanReader.Core;

/// <summary>
/// To encapsulate the Expander in Microsoft.UI.Xaml, customize the icon, content and expand content.
/// </summary>
[TemplatePart(Name = InternalExpanderName, Type = typeof(Expander))]
[TemplatePart(Name = InternalQuadrateName, Type = typeof(ExpanderExQuadratePanel))]
public partial class ExpanderEx : Control
{
    private const string InternalExpanderName = "InternalExpander";
    private const string InternalQuadrateName = "InternalQuadratePanel";

    private Expander _expander;
    private ExpanderExQuadratePanel _quadratePanel;

    private bool _isTemplateApplied;
    private bool _isLoaded;
    private bool _isEventAttached;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpanderEx"/> class.
    /// </summary>
    public ExpanderEx()
    {
        this.DefaultStyleKey = typeof(ExpanderEx);
        this.Loading += OnLoading;
        this.Unloaded += OnUnloaded;
        RegisterPropertyChangedCallback(AutomationProperties.NameProperty, new DependencyPropertyChangedCallback(OnAutomationNameChanged));
    }

    /// <summary>
    /// Occurs when the internal <see cref="Expander.Collapsed"/> event is triggered.
    /// </summary>
    public event TypedEventHandler<Expander, ExpanderCollapsedEventArgs> Collapsed;

    /// <summary>
    /// Occurs when the internal <see cref="Expander.Expanding"/> event is triggered.
    /// </summary>
    public event TypedEventHandler<Expander, ExpanderExpandingEventArgs> Expanding;

    /// <summary>
    /// Occurs when the header of ExpanderEx is clicked.
    /// </summary>
    public event EventHandler<ExpanderExClickEventArgs> Click;

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _isTemplateApplied = true;
        InitializeExpanderEx();
    }

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as ExpanderEx;
        instance.CheckPartVisibility();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = false;
        DestoryExpanderEx();
    }

    private void OnLoading(FrameworkElement sender, object args)
    {
        _isLoaded = true;
        InitializeExpanderEx();
    }

    private void InitializeExpanderEx()
    {
        if (_isEventAttached || !_isLoaded || !_isTemplateApplied)
        {
            return;
        }

        _expander = GetTemplateChild(InternalExpanderName) as Expander;
        _quadratePanel = GetTemplateChild(InternalQuadrateName) as ExpanderExQuadratePanel;

        if (_expander != null)
        {
            _expander.Expanding += OnExpanderExpanding;
            _expander.Collapsed += OnExpanderCollapsed;
            _expander.Tapped += OnHeaderTapped;
        }

        if (_quadratePanel != null && _quadratePanel is Button headerButton)
        {
            headerButton.Click += OnHeaderClick;
        }

        CheckPartVisibility();
        _isEventAttached = true;
        InitializeAutomationName();
    }

    private void DestoryExpanderEx()
    {
        if (_expander != null)
        {
            _expander.Expanding -= OnExpanderExpanding;
            _expander.Collapsed -= OnExpanderCollapsed;
            _expander.Tapped -= OnHeaderTapped;
        }

        if (_quadratePanel != null && _quadratePanel is Button headerButton)
        {
            headerButton.Click -= OnHeaderClick;
        }

        _isEventAttached = false;
    }

    private void OnExpanderCollapsed(Expander sender, ExpanderCollapsedEventArgs args)
        => Collapsed?.Invoke(sender, args);

    private void OnExpanderExpanding(Expander sender, ExpanderExpandingEventArgs args)
        => Expanding?.Invoke(sender, args);

    private void OnInternalExpanderLoaded(object sender, RoutedEventArgs e)
    {
        if (_expander != null)
        {
            if (_expander.FindDescendantByName("ExpanderHeader") is ToggleButton expanderHeader)
            {
                expanderHeader.Click -= OnHeaderClick;
                expanderHeader.Click += OnHeaderClick;
            }
            else
            {
                _expander.Tapped -= OnHeaderTapped;
            }
        }
    }

    private void OnHeaderTapped(object sender, TappedRoutedEventArgs e)
    {
        e.Handled = true;
        Click?.Invoke(this, new ExpanderExClickEventArgs(true, _expander));
    }

    private void OnHeaderClick(object sender, RoutedEventArgs e) => Click?.Invoke(this, new ExpanderExClickEventArgs(false, _quadratePanel));

    private void CheckPartVisibility()
    {
        var hasContent = Content != null;
        if (_quadratePanel != null)
        {
            _quadratePanel.Visibility = hasContent || ForceUseExpander ? Visibility.Collapsed : Visibility.Visible;
        }

        if (_expander != null)
        {
            _expander.Visibility = hasContent || ForceUseExpander ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void InitializeAutomationName()
    {
        var name = AutomationProperties.GetName(this);
        if (_expander != null)
        {
            AutomationProperties.SetName(_expander, name);
        }

        if (_quadratePanel != null)
        {
            AutomationProperties.SetName(_quadratePanel, name);
        }
    }

    private void OnAutomationNameChanged(DependencyObject sender, DependencyProperty dp) => InitializeAutomationName();
}
