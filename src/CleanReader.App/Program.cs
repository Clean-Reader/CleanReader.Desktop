// Copyright (c) Richasy. All rights reserved.

using System;
using System.Threading;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace CleanReader.App;

/// <summary>
/// 应用入口点.
/// </summary>
internal class Program
{
    /// <summary>
    /// 介入应用启动过程，在有多实例请求时重定向到已激活实例.
    /// </summary>
    /// <param name="args">启动参数.</param>
    [STAThread]
    internal static void Main(string[] args)
    {
        var actArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        var mainAppInstance = AppInstance.FindOrRegisterForKey(App.Id);
        if (!mainAppInstance.IsCurrent)
        {
            mainAppInstance.RedirectActivationToAsync(actArgs).AsTask().Wait();
            return;
        }

        WinRT.ComWrappersSupport.InitializeComWrappers();

        Application.Start(p =>
        {
            var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            new App();
        });
    }
}
