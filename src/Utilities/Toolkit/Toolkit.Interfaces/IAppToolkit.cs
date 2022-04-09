// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Toolkit.Interfaces
{
    /// <summary>
    /// Application related toolkit.
    /// </summary>
    public interface IAppToolkit
    {
        /// <summary>
        /// Initialize application theme,
        /// this method is used to switch to the specified theme when the application starts.
        /// </summary>
        /// <returns>Toolkit self.</returns>
        IAppToolkit InitializeTheme();

        /// <summary>
        /// Initialize application title bar style,
        /// this method is used to rewrite default title bar style.
        /// </summary>
        /// <param name="titleBar">Title bar.</param>
        /// <returns>Toolkit self.</returns>
        IAppToolkit InitializeTitleBar(object titleBar);

        /// <summary>
        /// Get the current environment language code.
        /// </summary>
        /// <param name="isWindowsName">
        /// Whether it is the Windows display name,
        /// for example, Simplified Chinese is CHS,
        /// if not, it is displayed as the default name,
        /// for example, Simplified Chinese is zh-Hans.
        /// </param>
        /// <returns>Language code.</returns>
        string GetLanguageCode(bool isWindowsName = false);

        /// <summary>
        /// Get the magnified pixels under the DPI of the current window.
        /// </summary>
        /// <param name="pixel">Pixel value.</param>
        /// <param name="windowHandle">Window handle.</param>
        /// <returns>Converted value.</returns>
        int GetScalePixel(double pixel, IntPtr windowHandle);

        /// <summary>
        /// Get the actual pixels under the DPI of the current window.
        /// </summary>
        /// <param name="pixel">Pixel value.</param>
        /// <param name="windowHandle">Window handle.</param>
        /// <returns>Converted value.</returns>
        int GetNormalizePixel(double pixel, IntPtr windowHandle);
    }
}
