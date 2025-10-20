﻿using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Services.Media.RoxyFileman;

public partial class RoxyFilemanService : IRoxyFilemanService
{
    #region Fields

    protected readonly IRoxyFilemanFileProvider _fileProvider;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public RoxyFilemanService(
        IRoxyFilemanFileProvider fileProvider,
        IWorkContext workContext)
    {
        _fileProvider = fileProvider;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Check whether there are any restrictions on handling the file
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <returns>
    /// The result contains true if the file can be handled; otherwise false
    /// </returns>
    protected virtual bool CanHandleFile(string path)
    {
        var result = false;

        var fileExtension = Path.GetExtension(path).Replace(".", string.Empty).ToLowerInvariant();

        var roxyConfig = Singleton<RoxyFilemanConfig>.Instance;

        var forbiddenUploads = roxyConfig.FORBIDDEN_UPLOADS.Trim().ToLowerInvariant();

        if (!string.IsNullOrEmpty(forbiddenUploads))
            result = !WhiteSpaceRegex().Split(forbiddenUploads).Contains(fileExtension);

        var allowedUploads = roxyConfig.ALLOWED_UPLOADS.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(allowedUploads))
            return result;

        return WhiteSpaceRegex().Split(allowedUploads).Contains(fileExtension);
    }

    [GeneratedRegex("\\s+")]
    private static partial Regex WhiteSpaceRegex();

    #endregion

    #region Configuration

    /// <summary>
    /// Initial service configuration
    /// </summary>
    /// <param name="pathBase">The base path for the current request</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ConfigureAsync(string pathBase)
    {
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
        await _fileProvider.GetOrCreateConfigurationAsync(pathBase, currentLanguage.UniqueSeoCode);
    }

    #endregion

    #region Directories

    /// <summary>
    /// Delete the directory
    /// </summary>
    /// <param name="path">Path to the directory</param>
    public virtual void DeleteDirectory(string path)
    {
        _fileProvider.DeleteDirectory(path);
    }

    /// <summary>
    /// Download the directory from the server as a zip archive
    /// </summary>
    /// <param name="path">Path to the directory</param>
    public virtual byte[] DownloadDirectory(string path)
    {
        return _fileProvider.CreateZipArchiveFromDirectory(path);
    }

    /// <summary>
    /// Copy the directory
    /// </summary>
    /// <param name="sourcePath">Path to the source directory</param>
    /// <param name="destinationPath">Path to the destination directory</param>
    public virtual void CopyDirectory(string sourcePath, string destinationPath)
    {
        _fileProvider.CopyDirectory(sourcePath, destinationPath);
    }

    /// <summary>
    /// Create the new directory
    /// </summary>
    /// <param name="parentDirectoryPath">Path to the parent directory</param>
    /// <param name="name">Name of the new directory</param>
    public virtual void CreateDirectory(string parentDirectoryPath, string name)
    {
        _fileProvider.CreateDirectory(parentDirectoryPath, name);
    }

    /// <summary>
    /// Get all available directories as a directory tree
    /// </summary>
    /// <param name="type">Type of the file</param>
    /// <returns>List of directories</returns>
    public virtual IEnumerable<object> GetDirectoryList(string type)
    {
        var contents = _fileProvider.GetDirectories(type);

        var result = new List<object>() { new
        {
            p = "/",
            f = _fileProvider.GetFiles("/", type).Count(),
            d = 0
        } };

        foreach (var (path, countFiles, countDirectories) in contents)
        {
            result.Add(new
            {
                p = path.Replace("\\", "/"),
                f = countFiles,
                d = countDirectories
            });
        }

        return result;
    }

    /// <summary>
    /// Move the directory
    /// </summary>
    /// <param name="sourcePath">Path to the source directory</param>
    /// <param name="destinationPath">Path to the destination directory</param>
    public virtual void MoveDirectory(string sourcePath, string destinationPath)
    {
        if (destinationPath.IndexOf(sourcePath, StringComparison.InvariantCulture) == 0)
            throw new RoxyFilemanException("E_CannotMoveDirToChild");

        _fileProvider.DirectoryMove(sourcePath, destinationPath);
    }

    /// <summary>
    /// Get files in the passed directory
    /// </summary>
    /// <param name="directoryPath">Path to the files directory</param>
    /// <param name="type">Type of the files</param>
    public virtual IEnumerable<object> GetFiles(string directoryPath, string type)
    {
        return _fileProvider.GetFiles(directoryPath, type?.Trim('#'))
            .Select(f => new
            {
                p = f.RelativePath.Replace("\\", "/"),
                t = f.LastWriteTime.ToUnixTimeSeconds(),
                s = f.FileLength,
                w = f.Width,
                h = f.Height
            });
    }

    /// <summary>
    /// Rename the directory
    /// </summary>
    /// <param name="sourcePath">Path to the source directory</param>
    /// <param name="newName">New name of the directory</param>
    public virtual void RenameDirectory(string sourcePath, string newName)
    {
        _fileProvider.RenameDirectory(sourcePath, newName);
    }

    /// <summary>
    /// Upload files to a directory on passed path
    /// </summary>
    /// <param name="directoryPath">Path to directory to upload files</param>
    /// <param name="files">Files sent with the HttpRequest</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UploadFilesAsync(string directoryPath, IEnumerable<IFormFile> files)
    {
        foreach (var formFile in files)
        {
            await _fileProvider.SaveFileAsync(directoryPath, formFile.FileName, formFile.ContentType, formFile.OpenReadStream());
        }
    }

    #endregion

    #region Files

    /// <summary>
    /// Copy the file
    /// </summary>
    /// <param name="sourcePath">Path to the source file</param>
    /// <param name="destinationPath">Path to the destination file</param>
    public virtual void CopyFile(string sourcePath, string destinationPath)
    {
        _fileProvider.CopyFile(sourcePath, destinationPath);
    }

    /// <summary>
    /// Get binary image thumbnail data
    /// </summary>
    /// <param name="path">Path to the image</param>
    /// <param name="contentType">The resulting MIME type</param>
    public virtual byte[] CreateImageThumbnail(string path, string contentType)
    {
        return _fileProvider.CreateImageThumbnail(path, contentType);
    }

    /// <summary>
    /// Delete the directory
    /// </summary>
    /// <param name="path">Path to the directory</param>
    public virtual void DeleteFile(string path)
    {
        _fileProvider.DeleteFile(path);
    }

    /// <summary>
    /// Get filename and read-only content stream
    /// </summary>
    /// <param name="path">Path to the file</param>
    public virtual (Stream stream, string name) GetFileStream(string path)
    {
        var file = _fileProvider.GetFileInfo(path);

        if (!file.Exists)
            throw new FileNotFoundException();

        return (file.CreateReadStream(), file.Name);
    }

    /// <summary>
    /// Move the file
    /// </summary>
    /// <param name="sourcePath">Path to the source file</param>
    /// <param name="destinationPath">Path to the destination file</param>
    public virtual void MoveFile(string sourcePath, string destinationPath)
    {
        if (!CanHandleFile(sourcePath) && !CanHandleFile(destinationPath))
            throw new RoxyFilemanException("E_FileExtensionForbidden");

        _fileProvider.FileMove(sourcePath, destinationPath);
    }

    /// <summary>
    /// Rename the file
    /// </summary>
    /// <param name="sourcePath">Path to the source file</param>
    /// <param name="newName">New name of the file</param>
    public virtual void RenameFile(string sourcePath, string newName)
    {
        if (!CanHandleFile(sourcePath) && !CanHandleFile(newName))
            throw new RoxyFilemanException("E_FileExtensionForbidden");

        _fileProvider.RenameFile(sourcePath, newName);
    }        

    #endregion
}