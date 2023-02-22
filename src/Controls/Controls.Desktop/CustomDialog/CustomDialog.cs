// Copyright (c) Richasy. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using CleanReader.Controls.Interfaces;
using CleanReader.ViewModels.Interfaces;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CleanReader.Controls;

/// <summary>
/// 自定义对话框.
/// </summary>
public partial class CustomDialog : ContentControl, ICustomDialog
{
#pragma warning disable SA1600 // Elements should be documented
    private readonly IAppViewModel _appViewModel;
    private TaskCompletionSource<int> _taskSource;
    private int _hideCode;
    private Button _primaryButton;
    private Button _secondaryButton;
    private Button _closeButton;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDialog"/> class.
    /// </summary>
    public CustomDialog()
    {
        DefaultStyleKey = typeof(CustomDialog);
        _appViewModel = Locator.Lib.Locator.Instance.GetService<IAppViewModel>();
        Loaded += OnLoadedAsync;
    }

    public Task<int> ShowAsync()
    {
        _hideCode = -1;
        _taskSource = new TaskCompletionSource<int>();
        ((IMainWindow)_appViewModel.MainWindow).ShowOnHolder(this);

        OnShow();
        return _taskSource.Task;
    }

    public void Hide()
    {
        _taskSource.SetResult(_hideCode);
        VisualStateManager.GoToState(this, "DialogHidden", true);
        ((IMainWindow)_appViewModel.MainWindow).RemoveFromHolder(this);
        OnHide();
    }

    public virtual void InjectData(object data)
    {
    }

    public virtual void InjectTask(Task task, CancellationTokenSource cancellationTokenSource = null)
    {
    }

    /// <summary>
    /// 主要按钮被点击时触发.
    /// </summary>
    public virtual void OnPrimaryButtonClick()
    {
    }

    /// <summary>
    /// 次要按钮被点击时触发.
    /// </summary>
    public virtual void OnSecondaryButtonClick()
    {
    }

    /// <summary>
    /// 在隐藏时触发.
    /// </summary>
    public virtual void OnHide()
    {
    }

    /// <summary>
    /// 在显示时触发.
    /// </summary>
    public virtual void OnShow()
    {
    }

    protected override void OnApplyTemplate()
    {
        _primaryButton = GetTemplateChild("PrimaryButton") as Button;
        _primaryButton.Click += (_, _) =>
        {
            OnPrimaryButtonClick();
            _hideCode = 0;
            Hide();
        };

        _secondaryButton = GetTemplateChild("SecondaryButton") as Button;
        _secondaryButton.Click += (_, _) =>
        {
            OnSecondaryButtonClick();
            _hideCode = 1;
            Hide();
        };

        _closeButton = GetTemplateChild("CloseButton") as Button;
        _closeButton.Click += (_, _) =>
        {
            _hideCode = 2;
            Hide();
        };

        var root = GetTemplateChild("LayoutRoot") as Grid;
        var shadow = root.Resources["DialogShadow"] as ThemeShadow;
        shadow.Receivers.Add(GetTemplateChild("ShadowSpace") as UIElement);
        (GetTemplateChild("BackgroundElement") as FrameworkElement).Translation += new System.Numerics.Vector3(0, 0, 128);
    }

    private static void OnButtonCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as CustomDialog;
        instance.InitializeCommands();
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "DialogShowing", true);

        var primarySign = string.IsNullOrEmpty(PrimaryButtonText) ? 0 : 100;
        var secondarySign = string.IsNullOrEmpty(SecondaryButtonText) ? 0 : 10;
        var closeSign = string.IsNullOrEmpty(CloseButtonText) ? 0 : 1;

        var commandSign = primarySign + secondarySign + closeSign;
        var commandState = commandSign switch
        {
            0 => "NoneVisible",
            1 => "CloseVisible",
            10 => "SecondaryVisible",
            11 => "SecondaryAndCloseVisible",
            100 => "PrimaryVisible",
            101 => "PrimaryAndCloseVisible",
            110 => "PrimaryAndSecondaryVisible",
            111 => "AllVisible",
            _ => "NoneVisible",
        };

        VisualStateManager.GoToState(this, commandState, false);

        if (DefaultButton != ContentDialogButton.None)
        {
            var buttonStyleState = DefaultButton switch
            {
                ContentDialogButton.Primary => "PrimaryAsDefaultButton",
                ContentDialogButton.Secondary => "SecondaryAsDefaultButton",
                ContentDialogButton.Close => "CloseAsDefaultButton",
                _ => "NoDefaultButton",
            };

            VisualStateManager.GoToState(this, buttonStyleState, false);
        }

        InitializeCommands();

        await Task.Delay(100);
        var focusEle = Microsoft.UI.Xaml.Input.FocusManager.FindFirstFocusableElement(this);
        if (focusEle != null)
        {
            await Microsoft.UI.Xaml.Input.FocusManager.TryFocusAsync(focusEle, FocusState.Programmatic).AsTask();
        }
    }

    private void InitializeCommands()
    {
        if (_primaryButton != null)
        {
            _primaryButton.Command = PrimaryButtonCommand;
        }

        if (_secondaryButton != null)
        {
            _secondaryButton.Command = SecondaryButtonCommand;
        }

        if (_closeButton != null)
        {
            _closeButton.Command = CloseButtonCommand;
        }
    }
#pragma warning restore SA1600 // Elements should be documented
}
