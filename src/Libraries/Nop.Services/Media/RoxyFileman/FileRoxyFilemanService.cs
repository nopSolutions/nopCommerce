using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using SkiaSharp;

namespace Nop.Services.Media.RoxyFileman
{
    /// <summary>
    /// File RoxyFileman service
    /// </summary>
    public class FileRoxyFilemanService : BaseRoxyFilemanService, IRoxyFilemanService
    {
        #region Fields

        protected string _fileRootPath;

        #endregion

        #region Ctor

        public FileRoxyFilemanService(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            INopFileProvider fileProvider,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings) : base(webHostEnvironment, httpContextAccessor, fileProvider, webHelper, workContext, mediaSettings)
        {
            _fileRootPath = null;
        }

        #endregion

        #region Utilities

        protected virtual HttpResponse GetJsonResponse()
        {
            var response = GetHttpContext().Response;

            response.Headers.TryAdd("Content-Type", "application/json");

            return response;
        }

        /// <summary>
        /// Сopy the directory with the embedded files and directories
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task BaseCopyDirectoryAsync(string sourcePath, string destinationPath)
        {
            var existingFiles = FileProvider.GetFiles(sourcePath);
            var existingDirectories = FileProvider.GetDirectories(sourcePath);

            if (!FileProvider.DirectoryExists(destinationPath))
                FileProvider.CreateDirectory(destinationPath);

            foreach (var file in existingFiles)
            {
                var filePath = FileProvider.Combine(destinationPath, FileProvider.GetFileName(file));
                if (!FileProvider.FileExists(filePath))
                    FileProvider.FileCopy(file, filePath);
            }

            foreach (var directory in existingDirectories)
            {
                var directoryPath = FileProvider.Combine(destinationPath, FileProvider.GetDirectoryName(directory));
                await BaseCopyDirectoryAsync(directory, directoryPath);
            }
        }

        /// <summary>
        /// Get files in the passed directory
        /// </summary>
        /// <param name="directoryPath">Path to the files directory</param>
        /// <param name="type">Type of the files</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of paths to the files
        /// </returns>
        protected virtual async Task<List<string>> GetFilesByDirectoryAsync(string directoryPath, string type)
        {
            if (!await IsPathAllowedAsync(directoryPath))
                return new List<string>();

            if (type == "#")
                type = string.Empty;

            return FileProvider.EnumerateFiles(directoryPath, "*.*")
                .AsParallel()
                .Where(file => string.IsNullOrEmpty(type) || GetFileType(FileProvider.GetFileExtension(file)) == type)
                .ToList();
        }

        /// <summary>
        /// Get the file format of the image
        /// </summary>
        /// <param name="path">Path to the image</param>
        /// <returns>Image format</returns>
        protected virtual ImageFormat GetImageFormat(string path)
        {
            var fileExtension = FileProvider.GetFileExtension(path).ToLowerInvariant();
            return fileExtension switch
            {
                ".png" => ImageFormat.Png,
                ".gif" => ImageFormat.Gif,
                _ => ImageFormat.Jpeg,
            };
        }

        /// <summary>
        /// Get the Unix timestamp by passed date
        /// </summary>
        /// <param name="date">Date and time</param>
        /// <returns>Unix timestamp</returns>
        protected virtual double GetTimestamp(DateTime date)
        {
            return (date.ToLocalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime()).TotalSeconds;
        }

        /// <summary>
        /// Resize the image
        /// </summary>
        /// <param name="sourcePath">Path to the source image</param>
        /// <param name="destinstionPath">Path to the destination image</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        protected virtual void ImageResize(string sourcePath, string destinstionPath, int width, int height)
        {
            if (string.IsNullOrEmpty(destinstionPath))
                return;

            if (!FileProvider.FileExists(sourcePath))
                return;

            using var stream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
            using var image = Image.FromStream(stream);
            var ratio = image.Width / (float)image.Height;
            if (image.Width <= width && image.Height <= height)
                return;

            if (width == 0 && height == 0)
                return;

            var newWidth = width;
            int newHeight = Convert.ToInt16(Math.Floor(newWidth / ratio));
            if ((height > 0 && newHeight > height) || width == 0)
            {
                newHeight = height;
                newWidth = Convert.ToInt16(Math.Floor(newHeight * ratio));
            }

            using var newImage = new Bitmap(newWidth, newHeight);
            using var graphics = Graphics.FromImage(newImage);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            //close the stream to prevent access error if sourcePath and destinstionPath match
            stream.Close();
            newImage.Save(destinstionPath, GetImageFormat(destinstionPath));
        }

        /// <summary>
        /// Checks if the path is allowed to work on
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        protected virtual async Task<bool> IsPathAllowedAsync(string path)
        {
            var absp = FileProvider.GetAbsolutePath(path);

            if (string.IsNullOrEmpty(_fileRootPath))
                await ConfigureAsync();

            return new DirectoryInfo(absp).FullName.StartsWith(_fileRootPath);
        }

        #endregion

        #region Methods

        #region Configuration

        /// <summary>
        /// Initial service configuration
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ConfigureAsync()
        {
            await CreateConfigurationAsync();

            var existingText = await FileProvider.ReadAllTextAsync(GetConfigurationFilePath(), Encoding.UTF8);
            var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(existingText);
            _fileRootPath = FileProvider.GetAbsolutePath(config["FILES_ROOT"]);
        }

        #endregion

        #region Directories

        /// <summary>
        /// Copy the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task CopyDirectoryAsync(string sourcePath, string destinationPath)
        {
            var directoryPath = GetFullPath(await GetVirtualPathAsync(sourcePath));

            if (!FileProvider.DirectoryExists(directoryPath))
                throw new Exception(await GetLanguageResourceAsync("E_CopyDirInvalidPath"));

            var newDirectoryPath = GetFullPath(await GetVirtualPathAsync($"{destinationPath.TrimEnd('/')}/{FileProvider.GetDirectoryNameOnly(directoryPath)}"));

            if (FileProvider.DirectoryExists(newDirectoryPath))
                throw new Exception(await GetLanguageResourceAsync("E_DirAlreadyExists"));

            if (!await IsPathAllowedAsync(directoryPath) || !await IsPathAllowedAsync(newDirectoryPath))
                throw new Exception(await GetLanguageResourceAsync("E_CopyDirInvalidPath"));

            await BaseCopyDirectoryAsync(directoryPath, newDirectoryPath);

            await GetJsonResponse().WriteAsync(GetSuccessResponse());
        }

        /// <summary>
        /// Create the new directory
        /// </summary>
        /// <param name="parentDirectoryPath">Path to the parent directory</param>
        /// <param name="name">Name of the new directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task CreateDirectoryAsync(string parentDirectoryPath, string name)
        {
            parentDirectoryPath = GetFullPath(await GetVirtualPathAsync(parentDirectoryPath));
            if (!FileProvider.DirectoryExists(parentDirectoryPath))
                throw new Exception(await GetLanguageResourceAsync("E_CreateDirInvalidPath"));

            if (!await IsPathAllowedAsync(parentDirectoryPath))
                throw new Exception(await GetLanguageResourceAsync("E_CreateDirInvalidPath"));

            try
            {
                var path = FileProvider.Combine(parentDirectoryPath, name);
                FileProvider.CreateDirectory(path);

                await GetJsonResponse().WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(await GetLanguageResourceAsync("E_CreateDirFailed"));
            }
        }

        /// <summary>
        /// Delete the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteDirectoryAsync(string path)
        {
            path = await GetVirtualPathAsync(path);
            if (path == await GetRootDirectoryAsync())
                throw new Exception(await GetLanguageResourceAsync("E_CannotDeleteRoot"));

            path = GetFullPath(path);
            if (!FileProvider.DirectoryExists(path))
                throw new Exception(await GetLanguageResourceAsync("E_DeleteDirInvalidPath"));

            if (FileProvider.GetDirectories(path).Length > 0 || FileProvider.GetFiles(path).Length > 0)
                throw new Exception(await GetLanguageResourceAsync("E_DeleteNonEmpty"));

            if (!await IsPathAllowedAsync(path))
                throw new Exception(await GetLanguageResourceAsync("E_DeleteDirInvalidPath"));

            try
            {
                FileProvider.DeleteDirectory(path);
                await GetJsonResponse().WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(await GetLanguageResourceAsync("E_CannotDeleteDir"));
            }
        }

        /// <summary>
        /// Download the directory from the server as a zip archive
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DownloadDirectoryAsync(string path)
        {
            path = (await GetVirtualPathAsync(path)).TrimEnd('/');
            var fullPath = GetFullPath(path);
            if (!FileProvider.DirectoryExists(fullPath))
                throw new Exception(await GetLanguageResourceAsync("E_CreateArchive"));

            if (!await IsPathAllowedAsync(fullPath))
                throw new Exception(await GetLanguageResourceAsync("E_CreateArchive"));

            var zipName = FileProvider.GetFileName(fullPath) + ".zip";
            var zipPath = $"/{zipName}";
            if (path != await GetRootDirectoryAsync())
                zipPath = await GetVirtualPathAsync(zipPath);
            zipPath = GetFullPath(zipPath);

            if (FileProvider.FileExists(zipPath))
                FileProvider.DeleteFile(zipPath);

            ZipFile.CreateFromDirectory(fullPath, zipPath, CompressionLevel.Fastest, true);

            var response = GetHttpContext().Response;

            response.Clear();
            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{WebUtility.UrlEncode(zipName)}\"");
            response.ContentType = MimeTypes.ApplicationForceDownload;
            await response.SendFileAsync(zipPath);

            FileProvider.DeleteFile(zipPath);
        }

        /// <summary>
        /// Get all available directories as a directory tree
        /// </summary>
        /// <param name="type">Type of the file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task GetDirectoriesAsync(string type)
        {
            var rootDirectoryPath = GetFullPath(await GetVirtualPathAsync(null));

            if (!FileProvider.DirectoryExists(rootDirectoryPath))
                throw new Exception("Invalid files root directory. Check your configuration.");

            var allDirectories = await GetDirectoriesByParentDirectoryAsync(rootDirectoryPath);
            allDirectories.Insert(0, rootDirectoryPath);

            var localPath = GetFullPath(null);
            var response = GetJsonResponse();

            await response.WriteAsync("[");
            for (var i = 0; i < allDirectories.Count; i++)
            {
                var directoryPath = (string)allDirectories[i];
                await response.WriteAsync($"{{\"p\":\"/{directoryPath.Replace(localPath, string.Empty).Replace("\\", "/").TrimStart('/')}\",\"f\":\"{(await GetFilesByDirectoryAsync(directoryPath, type)).Count}\",\"d\":\"{FileProvider.GetDirectories(directoryPath).Length}\"}}");
                if (i < allDirectories.Count - 1)
                    await response.WriteAsync(",");
            }

            await response.WriteAsync("]");
        }

        /// <summary>
        /// Move the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task MoveDirectoryAsync(string sourcePath, string destinationPath)
        {
            var fullSourcePath = GetFullPath(await GetVirtualPathAsync(sourcePath));

            destinationPath = GetFullPath(await GetVirtualPathAsync(FileProvider.Combine(destinationPath, FileProvider.GetDirectoryNameOnly(fullSourcePath))));

            if (destinationPath.IndexOf(fullSourcePath, StringComparison.InvariantCulture) == 0)
                throw new Exception(await GetLanguageResourceAsync("E_CannotMoveDirToChild"));

            if (!FileProvider.DirectoryExists(fullSourcePath))
                throw new Exception(await GetLanguageResourceAsync("E_MoveDirInvalisPath"));

            if (FileProvider.DirectoryExists(destinationPath))
                throw new Exception(await GetLanguageResourceAsync("E_DirAlreadyExists"));

            if (!await IsPathAllowedAsync(fullSourcePath) || !await IsPathAllowedAsync(destinationPath))
                throw new Exception(await GetLanguageResourceAsync("E_MoveDirInvalisPath"));

            try
            {
                FileProvider.DirectoryMove(fullSourcePath, destinationPath);
                await GetJsonResponse().WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{await GetLanguageResourceAsync("E_MoveDir")} \"{sourcePath}\"");
            }
        }

        /// <summary>
        /// Rename the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="newName">New name of the directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task RenameDirectoryAsync(string sourcePath, string newName)
        {
            var fullSourcePath = GetFullPath(await GetVirtualPathAsync(sourcePath));

            if (!await IsPathAllowedAsync(fullSourcePath))
                throw new Exception(await GetLanguageResourceAsync("E_RenameDirInvalidPath"));

            var destinationDirectory = FileProvider.Combine(FileProvider.GetParentDirectory(fullSourcePath), newName);

            if (await GetVirtualPathAsync(sourcePath) == await GetRootDirectoryAsync())
                throw new Exception(await GetLanguageResourceAsync("E_CannotRenameRoot"));

            if (!FileProvider.DirectoryExists(fullSourcePath))
                throw new Exception(await GetLanguageResourceAsync("E_RenameDirInvalidPath"));

            if (FileProvider.DirectoryExists(destinationDirectory))
                throw new Exception(await GetLanguageResourceAsync("E_DirAlreadyExists"));

            if (!await IsPathAllowedAsync(destinationDirectory))
                throw new Exception(await GetLanguageResourceAsync("E_RenameDirInvalidPath"));

            try
            {
                FileProvider.DirectoryMove(fullSourcePath, destinationDirectory);
                await GetJsonResponse().WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{await GetLanguageResourceAsync("E_RenameDir")} \"{sourcePath}\"");
            }
        }

        #endregion

        #region Files

        /// <summary>
        /// Copy the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="destinationPath">Path to the destination file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            var filePath = GetFullPath(await GetVirtualPathAsync(sourcePath));

            if (!FileProvider.FileExists(filePath))
                throw new Exception(await GetLanguageResourceAsync("E_CopyFileInvalisPath"));

            destinationPath = GetFullPath(await GetVirtualPathAsync(destinationPath));

            if (!await IsPathAllowedAsync(filePath) || !await IsPathAllowedAsync(destinationPath))
                throw new Exception(await GetLanguageResourceAsync("E_CopyFileInvalisPath"));

            var newFileName = GetUniqueFileName(destinationPath, FileProvider.GetFileName(filePath));
            try
            {
                FileProvider.FileCopy(filePath, FileProvider.Combine(destinationPath, newFileName));
                await GetJsonResponse().WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(await GetLanguageResourceAsync("E_CopyFile"));
            }
        }

        /// <summary>
        /// Delete the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteFileAsync(string path)
        {
            path = GetFullPath(await GetVirtualPathAsync(path));
            if (!FileProvider.FileExists(path))
                throw new Exception(await GetLanguageResourceAsync("E_DeleteFileInvalidPath"));

            if (!await IsPathAllowedAsync(path))
                throw new Exception(await GetLanguageResourceAsync("E_DeleteFileInvalidPath"));

            try
            {
                FileProvider.DeleteFile(path);
                await GetJsonResponse().WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(await GetLanguageResourceAsync("E_DeletеFile"));
            }
        }

        /// <summary>
        /// Download the file from the server
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DownloadFileAsync(string path)
        {
            var filePath = GetFullPath(await GetVirtualPathAsync(path));

            if (!await IsPathAllowedAsync(path))
                throw new Exception(await GetLanguageResourceAsync("E_ActionDisabled"));

            if (FileProvider.FileExists(filePath))
            {
                var response = GetHttpContext().Response;

                response.Clear();
                response.Headers.Add("Content-Disposition", $"attachment; filename=\"{WebUtility.UrlEncode(FileProvider.GetFileName(filePath))}\"");
                response.ContentType = MimeTypes.ApplicationForceDownload;
                await response.SendFileAsync(filePath);
            }
        }

        /// <summary>
        /// Get files in the passed directory
        /// </summary>
        /// <param name="directoryPath">Path to the files directory</param>
        /// <param name="type">Type of the files</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task GetFilesAsync(string directoryPath, string type)
        {
            directoryPath = await GetVirtualPathAsync(directoryPath);
            var files = await GetFilesByDirectoryAsync(GetFullPath(directoryPath), type);
            var response = GetJsonResponse();

            await response.WriteAsync("[");
            for (var i = 0; i < files.Count; i++)
            {
                var width = 0;
                var height = 0;
                var physicalPath = files[i];

                if (GetFileType(FileProvider.GetFileExtension(files[i])) == "image")
                {
                    await using var stream = new FileStream(physicalPath, FileMode.Open);
                    var image = SKBitmap.DecodeBounds(stream);
                    width = image.Width;
                    height = image.Height;
                }

                await response.WriteAsync($"{{\"p\":\"{directoryPath.TrimEnd('/')}/{FileProvider.GetFileName(physicalPath)}\",\"t\":\"{Math.Ceiling(GetTimestamp(FileProvider.GetLastWriteTime(physicalPath)))}\",\"s\":\"{FileProvider.FileLength(physicalPath)}\",\"w\":\"{width}\",\"h\":\"{height}\"}}");

                if (i < files.Count - 1)
                    await response.WriteAsync(",");
            }

            await response.WriteAsync("]");
        }

        /// <summary>
        /// Move the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="destinationPath">Path to the destination file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task MoveFileAsync(string sourcePath, string destinationPath)
        {
            var fullSourcePath = GetFullPath(await GetVirtualPathAsync(sourcePath));

            if (!FileProvider.FileExists(fullSourcePath))
                throw new Exception(await GetLanguageResourceAsync("E_MoveFileInvalisPath"));

            destinationPath = GetFullPath(await GetVirtualPathAsync(destinationPath));

            if (FileProvider.FileExists(destinationPath))
                throw new Exception(await GetLanguageResourceAsync("E_MoveFileAlreadyExists"));

            if (!await CanHandleFileAsync(FileProvider.GetFileName(destinationPath)))
                throw new Exception(await GetLanguageResourceAsync("E_FileExtensionForbidden"));

            if (!await IsPathAllowedAsync(fullSourcePath) || !await IsPathAllowedAsync(destinationPath))
                throw new Exception(await GetLanguageResourceAsync("E_MoveFileInvalisPath"));

            try
            {
                FileProvider.FileMove(fullSourcePath, destinationPath);
                await GetJsonResponse().WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{await GetLanguageResourceAsync("E_MoveFile")} \"{sourcePath}\"");
            }
        }

        /// <summary>
        /// Rename the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="newName">New name of the file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task RenameFileAsync(string sourcePath, string newName)
        {
            var fullSourcePath = GetFullPath(await GetVirtualPathAsync(sourcePath));
            if (!FileProvider.FileExists(fullSourcePath))
                throw new Exception(await GetLanguageResourceAsync("E_RenameFileInvalidPath"));

            if (!await CanHandleFileAsync(newName))
                throw new Exception(await GetLanguageResourceAsync("E_FileExtensionForbidden"));

            var destinationPath = FileProvider.Combine(FileProvider.GetDirectoryName(fullSourcePath), newName);

            if (!await IsPathAllowedAsync(fullSourcePath) || !await IsPathAllowedAsync(destinationPath))
                throw new Exception(await GetLanguageResourceAsync("E_RenameFileInvalidPath"));

            try
            {
                FileProvider.FileMove(fullSourcePath, destinationPath);

                await GetJsonResponse().WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{await GetLanguageResourceAsync("E_RenameFile")} \"{sourcePath}\"");
            }
        }

        /// <summary>
        /// Upload files to a directory on passed path
        /// </summary>
        /// <param name="directoryPath">Path to directory to upload files</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UploadFilesAsync(string directoryPath)
        {
            var result = GetSuccessResponse();
            var hasErrors = false;

            directoryPath = GetFullPath(await GetVirtualPathAsync(directoryPath));

            if (!await IsPathAllowedAsync(directoryPath))
                throw new Exception(await GetLanguageResourceAsync("E_UploadNotAll"));

            try
            {
                foreach (var formFile in GetHttpContext().Request.Form.Files)
                {
                    var fileName = formFile.FileName;
                    if (await CanHandleFileAsync(fileName))
                    {
                        var uniqueFileName = GetUniqueFileName(directoryPath, FileProvider.GetFileName(fileName));
                        var destinationFile = FileProvider.Combine(directoryPath, uniqueFileName);

                        //A warning (SCS0018 - Path Traversal) from the "Security Code Scan" analyzer may appear at this point. 
                        //In this case, it is not relevant. The input is not supplied by user.
                        await using (var stream = new FileStream(destinationFile, FileMode.OpenOrCreate)) 
                            await formFile.CopyToAsync(stream);

                        if (GetFileType(new FileInfo(uniqueFileName).Extension) != "image")
                            continue;

                        int.TryParse(await GetSettingAsync("MAX_IMAGE_WIDTH"), out var w);
                        int.TryParse(await GetSettingAsync("MAX_IMAGE_HEIGHT"), out var h);
                        ImageResize(destinationFile, destinationFile, w, h);
                    }
                    else
                    {
                        hasErrors = true;
                        result = GetErrorResponse(await GetLanguageResourceAsync("E_UploadNotAll"));
                    }
                }
            }
            catch (Exception ex)
            {
                result = GetErrorResponse(ex.Message);
            }

            if (IsAjaxRequest())
            {
                if (hasErrors)
                    result = GetErrorResponse(await GetLanguageResourceAsync("E_UploadNotAll"));

                await GetJsonResponse().WriteAsync(result);
            }
            else
                await GetHttpContext().Response.WriteAsync($"<script>parent.fileUploaded({result});</script>");
        }

        #endregion

        #region Images

        /// <summary>
        /// Create the thumbnail of the image and write it to the response
        /// </summary>
        /// <param name="path">Path to the image</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task CreateImageThumbnailAsync(string path)
        {
            path = GetFullPath(await GetVirtualPathAsync(path));

            var file = File.ReadAllBytes(path);
            using var image = SKBitmap.Decode(file);


            float width, height;
            var targetSize = 120;
            if (image.Height > image.Width)
            {
                // portrait
                width = image.Width * (targetSize / (float)image.Height);
                height = targetSize;
            }
            else
            {
                // landscape or square
                width = targetSize;
                height = image.Height * (targetSize / (float)image.Width);
            }

            var response = GetHttpContext().Response;

            response.Headers.Add("Content-Type", MimeTypes.ImagePng);

            using (var bitmap = image.Resize(new SKImageInfo((int)width, (int)height), SKFilterQuality.None))
            {
                using var cropImg = SKImage.FromBitmap(bitmap);
                file = cropImg.Encode().ToArray();
            }

            await response.Body.WriteAsync(file, 0, file.Length);
            response.Body.Close();
        }

        /// <summary>
        /// Flush all images on disk
        /// </summary>
        /// <param name="removeOriginal">Specifies whether to delete original images</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual Task FlushAllImagesOnDiskAsync(bool removeOriginal = true)
        {
            //do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// Flush images on disk
        /// </summary>
        /// <param name="directoryPath">Directory path to flush images</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual Task FlushImagesOnDiskAsync(string directoryPath)
        {
            //do nothing
            return Task.CompletedTask;
        }

        #endregion

        #endregion
    }
}