// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Toolkit.Interfaces
{
    /// <summary>
    /// File, IO realated toolkit.
    /// </summary>
    public interface IFileToolkit
    {
        /// <summary>
        /// Open the file chooser.
        /// </summary>
        /// <param name="windowHandle">Window handle.</param>
        /// <param name="types">Allowed file extension.</param>
        /// <returns>File path.</returns>
        Task<string> OpenLocalFileAsync(IntPtr windowHandle, params string[] types);

        /// <summary>
        /// Read the file text content.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>File content.</returns>
        Task<string> ReadFileAsync(string filePath);

        /// <summary>
        /// Open the file chooser and read the selected file.
        /// </summary>
        /// <param name="windowHandle">Window handle.</param>
        /// <param name="types">Allowed file extension.</param>
        /// <returns><c>Item1</c> represents the content of the file, <c>Item2</c> means file path.</returns>
        Task<Tuple<string, string>> OpenLocalFileAndReadAsync(IntPtr windowHandle, params string[] types);

        /// <summary>
        /// Open folder picker and return the folder path.
        /// </summary>
        /// <param name="windowHandle">Window handle.</param>
        /// <returns>Folder path.</returns>
        Task<string> OpenFolderAsync(IntPtr windowHandle);

        /// <summary>
        /// Asynchronously copies an existing file to a new file, and monitors cancellation requests. Overwriting a file of the same name is allowed.
        /// </summary>
        /// <param name="sourceFileName">The file to copy.</param>
        /// <param name="destFileName">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite"><c>true</c> if the destination file can be overwritten; otherwise, <c>false</c>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that represents asynchronous operation.</returns>
        Task CopyAsync(string sourceFileName, string destFileName, bool overwrite = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously deletes the specified file.
        /// </summary>
        /// <param name="path">The name of the file to be deleted. Wildcard characters are not supported.</param>
        /// <returns>Task that represents asynchronous operation.</returns>
        Task DeleteAsync(string path);
    }
}
