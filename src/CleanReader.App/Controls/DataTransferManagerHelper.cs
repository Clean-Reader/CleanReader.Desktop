// Copyright (c) Richasy. All rights reserved.

using System;
using System.Runtime.InteropServices;
using CleanReader.ViewModels.Desktop;
using Windows.ApplicationModel.DataTransfer;
using WinRT;

namespace CleanReader.App.Controls;

/// <summary>
/// 数据分享帮助类.
/// </summary>
internal static class DataTransferManagerHelper
{
    private static readonly Guid _dtm_iid = new Guid(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c);

    /// <summary>
    /// Enables access to DataTransferManager methods in a Windows Store app that manages multiple windows.
    /// </summary>
    [ComImport]
    [Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDataTransferManagerInterop
    {
        /// <summary>
        /// Gets the DataTransferManager instance for the specified window.
        /// </summary>
        /// <param name="appWindow">Window handle.</param>
        /// <param name="riid">The requested interface ID of the DataTransferManager instance.</param>
        /// <returns>Receives the DataTransferManager instance.</returns>
        IntPtr GetForWindow([In] IntPtr appWindow, [In] ref Guid riid);

        /// <summary>
        /// Displays the UI for sharing content for the specified window.
        /// </summary>
        /// <param name="appWindow">The window to show the share UI for.</param>
        void ShowShareUIForWindow(IntPtr appWindow);
    }

    private static IDataTransferManagerInterop DataTransferManagerInterop => DataTransferManager.As<IDataTransferManagerInterop>();

    public static DataTransferManager GetForWindow()
    {
        IntPtr result;
        result = DataTransferManagerInterop.GetForWindow(AppViewModel.Instance.MainWindowHandle, _dtm_iid);
        var dataTransferManager = MarshalInterface<DataTransferManager>.FromAbi(result);
        return dataTransferManager;
    }

    public static void ShowShareUIForWindow(IntPtr hwnd) => DataTransferManagerInterop.ShowShareUIForWindow(hwnd);
}
