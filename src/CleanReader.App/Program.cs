// Copyright (c) Richasy. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;

namespace CleanReader.App;

/// <summary>
/// 应用入口点.
/// </summary>
internal class Program
{
    private static IntPtr redirectEventHandle = IntPtr.Zero;

    /// <summary>
    /// 介入应用启动过程，在有多实例请求时重定向到已激活实例.
    /// </summary>
    /// <param name="args">启动参数.</param>
    [STAThread]
    internal static void Main(string[] args)
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();
        var isRedirect = DecideRedirection();
        if (!isRedirect)
        {
            Microsoft.UI.Xaml.Application.Start((p) =>
            {
                var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                _ = new App();
            });
        }
    }

    private static bool DecideRedirection()
    {
        var isRedirect = false;

        var args = AppInstance.GetCurrent().GetActivatedEventArgs();
        var kind = args.Kind;

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
                RedirectActivationTo(args, keyInstance);
            }
        }
        catch (Exception)
        {
        }

        return isRedirect;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

    [DllImport("kernel32.dll")]
    private static extern bool SetEvent(IntPtr hEvent);

    [DllImport("ole32.dll")]
    private static extern uint CoWaitForMultipleObjects(uint dwFlags, uint dwMilliseconds, ulong nHandles, IntPtr[] pHandles, out uint dwIndex);

    // Do the redirection on another thread, and use a non-blocking
    // wait method to wait for the redirection to complete.
    private static void RedirectActivationTo(AppActivationArguments args, AppInstance keyInstance)
    {
        redirectEventHandle = CreateEvent(IntPtr.Zero, true, false, null);
        Task.Run(() =>
        {
            keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
            SetEvent(redirectEventHandle);
        });
        uint cWMO_DEFAULT = 0;
        var iNFINITE = 0xFFFFFFFF;
        _ = CoWaitForMultipleObjects(
           cWMO_DEFAULT,
           iNFINITE,
           1,
           new IntPtr[] { redirectEventHandle },
           out var handleIndex);
    }

    private static void OnActivated(object sender, AppActivationArguments args)
        => _ = args.Kind;
}
