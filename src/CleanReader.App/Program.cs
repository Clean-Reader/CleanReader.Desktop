// Copyright (c) Richasy. All rights reserved.

using System;
using System.Threading;
using CleanReader.Locator.App;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace CleanReader.App;

/// <summary>
/// 应用入口点.
/// </summary>
internal class Program
{
    private static bool _isInitialized;
    private static App _app;

    /// <summary>
    /// 介入应用启动过程，在有多实例请求时重定向到已激活实例.
    /// </summary>
    /// <param name="args">启动参数.</param>
    [STAThread]
    internal static void Main(string[] args)
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();
        if (!_isInitialized)
        {
            DIFactory.RegisterAppRequiredServices();
        }

        var isRedirect = DecideRedirection();
        if (!isRedirect && !_isInitialized)
        {
            Application.Start((p) =>
            {
                var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                _app = new App();
            });

            _isInitialized = true;
        }
    }

    private static bool DecideRedirection()
    {
        var isRedirect = false;

        var args = AppInstance.GetCurrent().GetActivatedEventArgs();

        try
        {
            var keyInstance = AppInstance.FindOrRegisterForKey("CleanReader");

            if (keyInstance.IsCurrent)
            {
                keyInstance.Activated += OnActivated;
            }
            else
            {
                isRedirect = true;
                _ = keyInstance.RedirectActivationToAsync(args);
            }
        }
        catch (Exception)
        {
            isRedirect = true;
        }

        return isRedirect;
    }

    private static void OnActivated(object sender, AppActivationArguments args)
        => _app?.ActivateWindow(args);
}
