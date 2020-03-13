using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;

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

        /// <summary>
        /// Сopy the directory with the embedded files and directories
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        protected virtual void CopyDirectory(string sourcePath, string destinationPath)
        {
            var existingFiles = _fileProvider.GetFiles(sourcePath);
            var existingDirectories = _fileProvider.GetDirectories(sourcePath);

            if (!_fileProvider.DirectoryExists(destinationPath))
                _fileProvider.CreateDirectory(destinationPath);

            foreach (var file in existingFiles)
            {
                var filePath = _fileProvider.Combine(destinationPath, _fileProvider.GetFileName(file));
                if (!_fileProvider.FileExists(filePath))
                    _fileProvider.FileCopy(file, filePath);
            }

            foreach (var directory in existingDirectories)
            {
                var directoryPath = _fileProvider.Combine(destinationPath, _fileProvider.GetDirectoryName(directory));
                CopyDirectory(directory, directoryPath);
            }
        }

        /// <summary>
        /// Get files in the passed directory
        /// </summary>
        /// <param name="directoryPath">Path to the files directory</param>
        /// <param name="type">Type of the files</param>
        /// <returns>List of paths to the files</returns>
        protected virtual List<string> GetFiles(string directoryPath, string type)
        {
            if (!IsPathAllowed(directoryPath))
                return new List<string>();

            if (type == "#")
                type = string.Empty;

            var files = new List<string>();
            foreach (var fileName in _fileProvider.GetFiles(directoryPath))
            {
                if (string.IsNullOrEmpty(type) || GetFileType(_fileProvider.GetFileExtension(fileName)) == type)
                    files.Add(fileName);
            }

            return files;
        }

        /// <summary>
        /// Get the file format of the image
        /// </summary>
        /// <param name="path">Path to the image</param>
        /// <returns>Image format</returns>
        protected virtual ImageFormat GetImageFormat(string path)
        {
            var fileExtension = _fileProvider.GetFileExtension(path).ToLower();
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

            if (!_fileProvider.FileExists(sourcePath))
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
        /// <returns></returns>
        protected virtual bool IsPathAllowed(string path)
        {
            var absp = _fileProvider.GetAbsolutePath(path);

            if (string.IsNullOrEmpty(_fileRootPath))
                Configure();

            return new DirectoryInfo(absp).FullName.StartsWith(_fileRootPath);
        }

        #endregion

        #region Methods

        #region Configuration

        /// <summary>
        /// Initial service configuration
        /// </summary>
        public virtual void Configure()
        {
            CreateConfiguration();

            var existingText = _fileProvider.ReadAllText(GetConfigurationFilePath(), Encoding.UTF8);
            var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(existingText);
            _fileRootPath = _fileProvider.GetAbsolutePath(config["FILES_ROOT"]);
        }

        #endregion

        #region Directories

        /// <summary>
        /// Copy the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task CopyDirectoryAsync(string sourcePath, string destinationPath)
        {
            var directoryPath = GetFullPath(GetVirtualPath(sourcePath));

            if (!_fileProvider.DirectoryExists(directoryPath))
                throw new Exception(GetLanguageResource("E_CopyDirInvalidPath"));

            var newDirectoryPath = GetFullPath(GetVirtualPath($"{destinationPath.TrimEnd('/')}/{_fileProvider.GetDirectoryNameOnly(directoryPath)}"));

            if (_fileProvider.DirectoryExists(newDirectoryPath))
                throw new Exception(GetLanguageResource("E_DirAlreadyExists"));

            if (!IsPathAllowed(directoryPath) || !IsPathAllowed(newDirectoryPath))
                throw new Exception(GetLanguageResource("E_CopyDirInvalidPath"));

            CopyDirectory(directoryPath, newDirectoryPath);

            await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
        }

        /// <summary>
        /// Create the new directory
        /// </summary>
        /// <param name="parentDirectoryPath">Path to the parent directory</param>
        /// <param name="name">Name of the new directory</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task CreateDirectoryAsync(string parentDirectoryPath, string name)
        {
            parentDirectoryPath = GetFullPath(GetVirtualPath(parentDirectoryPath));
            if (!_fileProvider.DirectoryExists(parentDirectoryPath))
                throw new Exception(GetLanguageResource("E_CreateDirInvalidPath"));

            if (!IsPathAllowed(parentDirectoryPath))
                throw new Exception(GetLanguageResource("E_CreateDirInvalidPath"));

            try
            {
                var path = _fileProvider.Combine(parentDirectoryPath, name);
                _fileProvider.CreateDirectory(path);

                await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(GetLanguageResource("E_CreateDirFailed"));
            }
        }

        /// <summary>
        /// Delete the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task DeleteDirectoryAsync(string path)
        {
            path = GetVirtualPath(path);
            if (path == GetRootDirectory())
                throw new Exception(GetLanguageResource("E_CannotDeleteRoot"));

            path = GetFullPath(path);
            if (!_fileProvider.DirectoryExists(path))
                throw new Exception(GetLanguageResource("E_DeleteDirInvalidPath"));

            if (_fileProvider.GetDirectories(path).Length > 0 || _fileProvider.GetFiles(path).Length > 0)
                throw new Exception(GetLanguageResource("E_DeleteNonEmpty"));

            if (!IsPathAllowed(path))
                throw new Exception(GetLanguageResource("E_DeleteDirInvalidPath"));

            try
            {
                _fileProvider.DeleteDirectory(path);
                await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(GetLanguageResource("E_CannotDeleteDir"));
            }
        }

        /// <summary>
        /// Download the directory from the server as a zip archive
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public async Task DownloadDirectoryAsync(string path)
        {
            path = GetVirtualPath(path).TrimEnd('/');
            var fullPath = GetFullPath(path);
            if (!_fileProvider.DirectoryExists(fullPath))
                throw new Exception(GetLanguageResource("E_CreateArchive"));

            if (!IsPathAllowed(fullPath))
                throw new Exception(GetLanguageResource("E_CreateArchive"));

            var zipName = _fileProvider.GetFileName(fullPath) + ".zip";
            var zipPath = $"/{zipName}";
            if (path != GetRootDirectory())
                zipPath = GetVirtualPath(zipPath);
            zipPath = GetFullPath(zipPath);

            if (_fileProvider.FileExists(zipPath))
                _fileProvider.DeleteFile(zipPath);

            ZipFile.CreateFromDirectory(fullPath, zipPath, CompressionLevel.Fastest, true);

            GetHttpContext().Response.Clear();
            GetHttpContext().Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{WebUtility.UrlEncode(zipName)}\"");
            GetHttpContext().Response.ContentType = MimeTypes.ApplicationForceDownload;
            await GetHttpContext().Response.SendFileAsync(zipPath);

            _fileProvider.DeleteFile(zipPath);
        }

        /// <summary>
        /// Get all available directories as a directory tree
        /// </summary>
        /// <param name="type">Type of the file</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task GetDirectoriesAsync(string type)
        {
            var rootDirectoryPath = GetFullPath(GetVirtualPath(null));

            if (!_fileProvider.DirectoryExists(rootDirectoryPath))
                throw new Exception("Invalid files root directory. Check your configuration.");

            var allDirectories = GetDirectories(rootDirectoryPath);
            allDirectories.Insert(0, rootDirectoryPath);

            var localPath = GetFullPath(null);
            await GetHttpContext().Response.WriteAsync("[");
            for (var i = 0; i < allDirectories.Count; i++)
            {
                var directoryPath = (string)allDirectories[i];
                await GetHttpContext().Response.WriteAsync($"{{\"p\":\"/{directoryPath.Replace(localPath, string.Empty).Replace("\\", "/").TrimStart('/')}\",\"f\":\"{GetFiles(directoryPath, type).Count}\",\"d\":\"{_fileProvider.GetDirectories(directoryPath).Length}\"}}");
                if (i < allDirectories.Count - 1)
                    await GetHttpContext().Response.WriteAsync(",");
            }

            await GetHttpContext().Response.WriteAsync("]");
        }

        /// <summary>
        /// Move the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task MoveDirectoryAsync(string sourcePath, string destinationPath)
        {
            var fullSourcePath = GetFullPath(GetVirtualPath(sourcePath));

            destinationPath = GetFullPath(GetVirtualPath(_fileProvider.Combine(destinationPath, _fileProvider.GetDirectoryNameOnly(fullSourcePath))));

            if (destinationPath.IndexOf(fullSourcePath, StringComparison.InvariantCulture) == 0)
                throw new Exception(GetLanguageResource("E_CannotMoveDirToChild"));

            if (!_fileProvider.DirectoryExists(fullSourcePath))
                throw new Exception(GetLanguageResource("E_MoveDirInvalisPath"));

            if (_fileProvider.DirectoryExists(destinationPath))
                throw new Exception(GetLanguageResource("E_DirAlreadyExists"));

            if (!IsPathAllowed(fullSourcePath) || !IsPathAllowed(destinationPath))
                throw new Exception(GetLanguageResource("E_MoveDirInvalisPath"));

            try
            {
                _fileProvider.DirectoryMove(fullSourcePath, destinationPath);
                await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{GetLanguageResource("E_MoveDir")} \"{sourcePath}\"");
            }
        }

        /// <summary>
        /// Rename the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="newName">New name of the directory</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task RenameDirectoryAsync(string sourcePath, string newName)
        {
            var fullSourcePath = GetFullPath(GetVirtualPath(sourcePath));

            if (!IsPathAllowed(fullSourcePath))
                throw new Exception(GetLanguageResource("E_RenameDirInvalidPath"));

            var destinationDirectory = _fileProvider.Combine(_fileProvider.GetParentDirectory(fullSourcePath), newName);

            if (GetVirtualPath(sourcePath) == GetRootDirectory())
                throw new Exception(GetLanguageResource("E_CannotRenameRoot"));

            if (!_fileProvider.DirectoryExists(fullSourcePath))
                throw new Exception(GetLanguageResource("E_RenameDirInvalidPath"));

            if (_fileProvider.DirectoryExists(destinationDirectory))
                throw new Exception(GetLanguageResource("E_DirAlreadyExists"));

            if (!IsPathAllowed(destinationDirectory))
                throw new Exception(GetLanguageResource("E_RenameDirInvalidPath"));

            try
            {
                _fileProvider.DirectoryMove(fullSourcePath, destinationDirectory);
                await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{GetLanguageResource("E_RenameDir")} \"{sourcePath}\"");
            }
        }

        #endregion

        #region Files

        /// <summary>
        /// Copy the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="destinationPath">Path to the destination file</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            var filePath = GetFullPath(GetVirtualPath(sourcePath));

            if (!_fileProvider.FileExists(filePath))
                throw new Exception(GetLanguageResource("E_CopyFileInvalisPath"));

            destinationPath = GetFullPath(GetVirtualPath(destinationPath));

            if (!IsPathAllowed(filePath) || !IsPathAllowed(destinationPath))
                throw new Exception(GetLanguageResource("E_CopyFileInvalisPath"));

            var newFileName = GetUniqueFileName(destinationPath, _fileProvider.GetFileName(filePath));
            try
            {
                _fileProvider.FileCopy(filePath, _fileProvider.Combine(destinationPath, newFileName));
                await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(GetLanguageResource("E_CopyFile"));
            }
        }

        /// <summary>
        /// Delete the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task DeleteFileAsync(string path)
        {
            path = GetFullPath(GetVirtualPath(path));
            if (!_fileProvider.FileExists(path))
                throw new Exception(GetLanguageResource("E_DeleteFileInvalidPath"));

            if (!IsPathAllowed(path))
                throw new Exception(GetLanguageResource("E_DeleteFileInvalidPath"));

            try
            {
                _fileProvider.DeleteFile(path);
                await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception(GetLanguageResource("E_DeletеFile"));
            }
        }

        /// <summary>
        /// Download the file from the server
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task DownloadFileAsync(string path)
        {
            var filePath = GetFullPath(GetVirtualPath(path));

            if (!IsPathAllowed(path))
                throw new Exception(GetLanguageResource("E_ActionDisabled"));

            if (_fileProvider.FileExists(filePath))
            {
                GetHttpContext().Response.Clear();
                GetHttpContext().Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{WebUtility.UrlEncode(_fileProvider.GetFileName(filePath))}\"");
                GetHttpContext().Response.ContentType = MimeTypes.ApplicationForceDownload;
                await GetHttpContext().Response.SendFileAsync(filePath);
            }
        }

        /// <summary>
        /// Get files in the passed directory
        /// </summary>
        /// <param name="directoryPath">Path to the files directory</param>
        /// <param name="type">Type of the files</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task GetFilesAsync(string directoryPath, string type)
        {
            directoryPath = GetVirtualPath(directoryPath);
            var files = GetFiles(GetFullPath(directoryPath), type);

            await GetHttpContext().Response.WriteAsync("[");
            for (var i = 0; i < files.Count; i++)
            {
                var width = 0;
                var height = 0;
                var physicalPath = files[i];

                if (GetFileType(_fileProvider.GetFileExtension(files[i])) == "image")
                {
                    using var stream = new FileStream(physicalPath, FileMode.Open);
                    using var image = Image.FromStream(stream);
                    width = image.Width;
                    height = image.Height;
                }

                await GetHttpContext().Response.WriteAsync($"{{\"p\":\"{directoryPath.TrimEnd('/')}/{_fileProvider.GetFileName(physicalPath)}\",\"t\":\"{Math.Ceiling(GetTimestamp(_fileProvider.GetLastWriteTime(physicalPath)))}\",\"s\":\"{_fileProvider.FileLength(physicalPath)}\",\"w\":\"{width}\",\"h\":\"{height}\"}}");

                if (i < files.Count - 1)
                    await GetHttpContext().Response.WriteAsync(",");
            }

            await GetHttpContext().Response.WriteAsync("]");
        }

        /// <summary>
        /// Move the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="destinationPath">Path to the destination file</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task MoveFileAsync(string sourcePath, string destinationPath)
        {
            var fullSourcePath = GetFullPath(GetVirtualPath(sourcePath));

            if (!_fileProvider.FileExists(fullSourcePath))
                throw new Exception(GetLanguageResource("E_MoveFileInvalisPath"));

            destinationPath = GetFullPath(GetVirtualPath(destinationPath));

            if (_fileProvider.FileExists(destinationPath))
                throw new Exception(GetLanguageResource("E_MoveFileAlreadyExists"));

            if (!CanHandleFile(_fileProvider.GetFileName(destinationPath)))
                throw new Exception(GetLanguageResource("E_FileExtensionForbidden"));

            if (!IsPathAllowed(fullSourcePath) || !IsPathAllowed(destinationPath))
                throw new Exception(GetLanguageResource("E_MoveFileInvalisPath"));

            try
            {
                _fileProvider.FileMove(fullSourcePath, destinationPath);
                await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{GetLanguageResource("E_MoveFile")} \"{sourcePath}\"");
            }
        }

        /// <summary>
        /// Rename the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="newName">New name of the file</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task RenameFileAsync(string sourcePath, string newName)
        {
            var fullSourcePath = GetFullPath(GetVirtualPath(sourcePath));
            if (!_fileProvider.FileExists(fullSourcePath))
                throw new Exception(GetLanguageResource("E_RenameFileInvalidPath"));

            if (!CanHandleFile(newName))
                throw new Exception(GetLanguageResource("E_FileExtensionForbidden"));

            var destinationPath = _fileProvider.Combine(_fileProvider.GetDirectoryName(fullSourcePath), newName);

            if (!IsPathAllowed(fullSourcePath) || !IsPathAllowed(destinationPath))
                throw new Exception(GetLanguageResource("E_RenameFileInvalidPath"));

            try
            {
                _fileProvider.FileMove(fullSourcePath, destinationPath);

                await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
            }
            catch
            {
                throw new Exception($"{GetLanguageResource("E_RenameFile")} \"{sourcePath}\"");
            }
        }

        /// <summary>
        /// Upload files to a directory on passed path
        /// </summary>
        /// <param name="directoryPath">Path to directory to upload files</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public virtual async Task UploadFilesAsync(string directoryPath)
        {
            var result = GetSuccessResponse();
            var hasErrors = false;

            directoryPath = GetFullPath(GetVirtualPath(directoryPath));

            if (!IsPathAllowed(directoryPath))
                throw new Exception(GetLanguageResource("E_UploadNotAll"));

            try
            {
                foreach (var formFile in GetHttpContext().Request.Form.Files)
                {
                    var fileName = formFile.FileName;
                    if (CanHandleFile(fileName))
                    {
                        var uniqueFileName = GetUniqueFileName(directoryPath, _fileProvider.GetFileName(fileName));
                        var destinationFile = _fileProvider.Combine(directoryPath, uniqueFileName);

                        //A warning (SCS0018 - Path Traversal) from the "Security Code Scan" analyzer may appear at this point. 
                        //In this case, it is not relevant. The input is not supplied by user.
                        using (var stream = new FileStream(destinationFile, FileMode.OpenOrCreate))
                        {
                            formFile.CopyTo(stream);
                        }

                        if (GetFileType(new FileInfo(uniqueFileName).Extension) != "image")
                            continue;

                        int.TryParse(GetSetting("MAX_IMAGE_WIDTH"), out var w);
                        int.TryParse(GetSetting("MAX_IMAGE_HEIGHT"), out var h);
                        ImageResize(destinationFile, destinationFile, w, h);
                    }
                    else
                    {
                        hasErrors = true;
                        result = GetErrorResponse(GetLanguageResource("E_UploadNotAll"));
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
                    result = GetErrorResponse(GetLanguageResource("E_UploadNotAll"));

                await GetHttpContext().Response.WriteAsync(result);
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
        public virtual void CreateImageThumbnail(string path)
        {
            path = GetFullPath(GetVirtualPath(path));
            int.TryParse(GetHttpContext().Request.Query["width"].ToString().Replace("px", string.Empty), out var width);
            int.TryParse(GetHttpContext().Request.Query["height"].ToString().Replace("px", string.Empty), out var height);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var image = new Bitmap(Image.FromStream(stream));
            var cropX = 0;
            var cropY = 0;

            var imgRatio = image.Width / (double)image.Height;

            if (height == 0)
                height = Convert.ToInt32(Math.Floor(width / imgRatio));

            if (width > image.Width)
                width = image.Width;
            if (height > image.Height)
                height = image.Height;

            var cropRatio = width / (double)height;
            var cropWidth = Convert.ToInt32(Math.Floor(image.Height * cropRatio));
            var cropHeight = Convert.ToInt32(Math.Floor(cropWidth / cropRatio));

            if (cropWidth > image.Width)
            {
                cropWidth = image.Width;
                cropHeight = Convert.ToInt32(Math.Floor(cropWidth / cropRatio));
            }

            if (cropHeight > image.Height)
            {
                cropHeight = image.Height;
                cropWidth = Convert.ToInt32(Math.Floor(cropHeight * cropRatio));
            }

            if (cropWidth < image.Width)
                cropX = Convert.ToInt32(Math.Floor((double)(image.Width - cropWidth) / 2));
            if (cropHeight < image.Height)
                cropY = Convert.ToInt32(Math.Floor((double)(image.Height - cropHeight) / 2));

            using var cropImg = image.Clone(new Rectangle(cropX, cropY, cropWidth, cropHeight), PixelFormat.DontCare);
            GetHttpContext().Response.Headers.Add("Content-Type", MimeTypes.ImagePng);
            cropImg.GetThumbnailImage(width, height, () => false, IntPtr.Zero).Save(GetHttpContext().Response.Body, ImageFormat.Png);
            GetHttpContext().Response.Body.Close();
        }

        /// <summary>
        /// Flush all images on disk
        /// </summary>
        /// <param name="removeOriginal">Specifies whether to delete original images</param>
        public virtual void FlushAllImagesOnDisk(bool removeOriginal = true)
        {
            //do nothing
        }

        /// <summary>
        /// Flush images on disk
        /// </summary>
        /// <param name="directoryPath">Directory path to flush images</param>
        public virtual void FlushImagesOnDisk(string directoryPath)
        {
            //do nothing
        }

        #endregion

        #endregion
    }
}