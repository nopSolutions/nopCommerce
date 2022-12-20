using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nop.Services.Media.RoxyFileman
{
    /// <summary>
    /// RoxyFileman service interface
    /// </summary>
    public partial interface IRoxyFilemanService
    {
        #region Configuration

        /// <summary>
        /// Initial service configuration
        /// </summary>
        /// <param name="pathBase">The base path for the current request</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ConfigureAsync(string pathBase);

        #endregion

        #region Directories

        /// <summary>
        /// Copy the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        void CopyDirectory(string sourcePath, string destinationPath);

        /// <summary>
        /// Create the new directory
        /// </summary>
        /// <param name="parentDirectoryPath">Path to the parent directory</param>
        /// <param name="name">Name of the new directory</param>
        void CreateDirectory(string parentDirectoryPath, string name);

        /// <summary>
        /// Delete the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        void DeleteDirectory(string path);

        /// <summary>
        /// Download the directory from the server as a zip archive
        /// </summary>
        /// <param name="path">Path to the directory</param>
        byte[] DownloadDirectory(string path);

        /// <summary>
        /// Get all available directories as a directory tree
        /// </summary>
        /// <param name="type">Type of the file</param>
        /// <returns>List of directories</returns>
        IEnumerable<object> GetDirectoryList(string type);

        /// <summary>
        /// Move the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        void MoveDirectory(string sourcePath, string destinationPath);

        /// <summary>
        /// Rename the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="newName">New name of the directory</param>
        void RenameDirectory(string sourcePath, string newName);

        #endregion

        #region Files

        /// <summary>
        /// Get filename and read-only content stream
        /// </summary>
        /// <param name="path">Path to the file</param>
        (Stream stream, string name) GetFileStream(string path);

        /// <summary>
        /// Copy the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="destinationPath">Path to the destination file</param>
        void CopyFile(string sourcePath, string destinationPath);

        /// <summary>
        /// Get binary image thumbnail data
        /// </summary>
        /// <param name="path">Path to the image</param>
        /// <param name="contentType">The resulting MIME type</param>
        byte[] CreateImageThumbnail(string path, string contentType);

        /// <summary>
        /// Delete the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        void DeleteFile(string path);

        /// <summary>
        /// Get files in the passed directory
        /// </summary>
        /// <param name="directoryPath">Path to the files directory</param>
        /// <param name="type">Type of the files</param>
        IEnumerable<object> GetFiles(string directoryPath, string type);

        /// <summary>
        /// Move the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="destinationPath">Path to the destination file</param>
        void MoveFile(string sourcePath, string destinationPath);

        /// <summary>
        /// Rename the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="newName">New name of the file</param>
        void RenameFile(string sourcePath, string newName);

        /// <summary>
        /// Upload files to a directory on passed path
        /// </summary>
        /// <param name="directoryPath">Path to directory to upload files</param>
        /// <param name="files">Files sent with the HttpRequest</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UploadFilesAsync(string directoryPath, IEnumerable<IFormFile> files);

        #endregion
    }
}