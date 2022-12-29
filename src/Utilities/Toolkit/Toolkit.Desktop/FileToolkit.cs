// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CleanReader.Toolkit.Interfaces;
using Windows.Storage.Pickers;

namespace CleanReader.Toolkit.Desktop;

/// <summary>
/// File Toolkit.
/// </summary>
public class FileToolkit : IFileToolkit
{
    /// <inheritdoc/>
    public async Task<string> OpenLocalFileAsync(IntPtr windowHandle, params string[] types)
    {
        var picker = new FileOpenPicker();
        WinRT.Interop.InitializeWithWindow.Initialize(picker, windowHandle);
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        var typeReg = new Regex(@"^\.[a-zA-Z0-9]+$");
        foreach (var type in types)
        {
            if (type == "*" || typeReg.IsMatch(type))
            {
                picker.FileTypeFilter.Add(type);
            }
            else
            {
                throw new InvalidCastException("Invalid file extension.");
            }
        }

        var file = await picker.PickSingleFileAsync().AsTask();
        return file?.Path;
    }

    /// <inheritdoc/>
    public async Task<string> SaveFileAsync(IntPtr windowHandle, string fileName)
    {
        var picker = new FileSavePicker();
        WinRT.Interop.InitializeWithWindow.Initialize(picker, windowHandle);
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeChoices.Add("Epub file", new string[] { ".epub" });
        picker.DefaultFileExtension = ".epub";
        picker.SuggestedFileName = fileName;
        var file = await picker.PickSaveFileAsync().AsTask();
        return file?.Path;
    }

    /// <inheritdoc/>
    public Task<string> ReadFileAsync(string filePath)
    {
        EnsureCorrectFileSystemPath(filePath);
        return File.Exists(filePath) ? File.ReadAllTextAsync(filePath) : null;
    }

    /// <inheritdoc/>
    public async Task<Tuple<string, string>> OpenLocalFileAndReadAsync(IntPtr windowHandle, params string[] types)
    {
        var file = await OpenLocalFileAsync(windowHandle, types);
        var content = await ReadFileAsync(file);

        return !string.IsNullOrEmpty(content) ? new Tuple<string, string>(content, file) : null;
    }

    /// <inheritdoc/>
    public async Task<string> OpenFolderAsync(IntPtr windowHandle)
    {
        var picker = new FolderPicker();
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, windowHandle);
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        var folder = await picker.PickSingleFolderAsync();
        return folder != null ? folder.Path : string.Empty;
    }

    /// <inheritdoc/>
    public async Task CopyAsync(string sourceFileName, string destFileName, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        EnsureCorrectFileSystemPath(sourceFileName);
        EnsureCorrectFileSystemPath(destFileName);

        const int fileBufferSize = 4096;
        using (var sourceStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read, fileBufferSize, true))
        {
            var fileMode = overwrite ? FileMode.OpenOrCreate : FileMode.CreateNew;
            using (var destStream = new FileStream(destFileName, fileMode, FileAccess.Write, FileShare.None, fileBufferSize, true))
            {
                const int copyBufferSize = 81920;
                await sourceStream.CopyToAsync(destStream, copyBufferSize, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string path)
    {
        EnsureCorrectFileSystemPath(path);
        if (File.Exists(path))
        {
            const int bufferSize = 4096;
            using (var fileStream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.Delete, bufferSize, true))
            {
                await fileStream.FlushAsync();
                File.Delete(path);
            }
        }
    }

    /// <summary>
    /// Ensures the correct file system path.
    /// </summary>
    /// <param name="path">The path to file or directory.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is a zero-length string, contains only white space, or contains invalid characters as defined in <see cref="Path.GetInvalidPathChars"/>.</exception>
    /// <remarks>Throws an exception if <paramref name="path"/> is not a correct file system path, otherwise no.</remarks>
    internal static void EnsureCorrectFileSystemPath(string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path), $"{nameof(path)} is null.");
        }

        if (string.IsNullOrWhiteSpace(path) || HasSpecifiedChars(path, Path.GetInvalidPathChars()))
        {
            var message =
                $"{nameof(path)} is a zero-length string, contains only white space, or contains one or more invalid characters as defined by InvalidPathChars.";
            throw new ArgumentException(message, nameof(path));
        }
    }

    private static bool HasSpecifiedChars(string text, char[] chars)
    {
        var charsHashSet = new HashSet<char>(chars);
        for (var i = 0; i < text.Length; ++i)
        {
            if (charsHashSet.Contains(text[i]))
            {
                return true;
            }
        }

        return false;
    }
}
