using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data;
using SkiaSharp;

namespace Nop.Services.Media.RoxyFileman
{
    /// <summary>
    /// Database RoxyFileman service
    /// </summary>
    public class DatabaseRoxyFilemanService : FileRoxyFilemanService
    {
        #region Fields

        private readonly IPictureService _pictureService;
        private readonly IRepository<Picture> _pictureRepository;

        #endregion

        #region Ctor

        public DatabaseRoxyFilemanService(IPictureService pictureService,
            IRepository<Picture> pictureRepository,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            INopFileProvider fileProvider,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings) : base(webHostEnvironment, httpContextAccessor, fileProvider, webHelper, workContext, mediaSettings)
        {
            _pictureService = pictureService;
            _pictureRepository = pictureRepository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get directories in the passed parent directory
        /// </summary>
        /// <param name="parentDirectoryPath">Path to the parent directory</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the array of the paths to the directories
        /// </returns>
        protected override async Task<ArrayList> GetDirectoriesByParentDirectoryAsync(string parentDirectoryPath)
        {
            await CreateDirectoryAsync(parentDirectoryPath);

            return await base.GetDirectoriesByParentDirectoryAsync(parentDirectoryPath);
        }

        /// <summary>
        /// Gets picture from database by file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the exist picture from database or null
        /// </returns>
        protected virtual async Task<Picture> GetPictureByFileAsync(string filePath)
        {
            var sourceVirtualPath = _fileProvider.GetVirtualPath(_fileProvider.GetDirectoryName(filePath));
            var fileName = _fileProvider.GetFileNameWithoutExtension(filePath);

            var picture = (await _pictureService.GetPicturesAsync(sourceVirtualPath.TrimEnd('/')))
                       .FirstOrDefault(p => fileName.Contains(p.SeoFilename));

            return picture;
        }

        /// <summary>
        /// Create the passed directory
        /// </summary>
        /// <param name="directoryPath">Path to the parent directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task CreateDirectoryAsync(string directoryPath)
        {
            _fileProvider.CreateDirectory(directoryPath);
            var virtualPath = _fileProvider.GetVirtualPath(directoryPath).TrimEnd('/');
            var directoryNames = (await _pictureService.GetPicturesAsync($"{virtualPath}/"))
                .Where(picture => picture.VirtualPath != virtualPath)
                .Select(picture => _fileProvider.GetAbsolutePath(picture.VirtualPath.TrimStart('~').Split('/')))
                .Distinct();

            foreach (var directory in directoryNames)
                await CreateDirectoryAsync(directory);
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
        protected override async Task<List<string>> GetFilesByDirectoryAsync(string directoryPath, string type)
        {
            //store files on disk if needed
            await FlushImagesOnDiskAsync(directoryPath);

            return await base.GetFilesByDirectoryAsync(directoryPath, type);
        }

        /// <summary>
        /// Copy the directory with the embedded files and directories
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task BaseCopyDirectoryAsync(string sourcePath, string destinationPath)
        {
            var pictures = await _pictureService.GetPicturesAsync($"{_fileProvider.GetVirtualPath(sourcePath).TrimEnd('/')}/");
            var baseDestinationPathVirtualPath = _fileProvider.GetVirtualPath(destinationPath);

            foreach (var picture in pictures)
            {
                var destinationPathVirtualPath =
                    $"{baseDestinationPathVirtualPath.TrimEnd('/')}{picture.VirtualPath.Replace(_fileProvider.GetVirtualPath(sourcePath), "")}";

                await _pictureService.InsertPictureAsync(new RoxyFilemanFormFile(picture,
                    await _pictureService.GetPictureBinaryByPictureIdAsync(picture.Id),
                    await _pictureService.GetFileExtensionFromMimeTypeAsync(picture.MimeType)),
                    string.Empty,
                    destinationPathVirtualPath);
            }
        }

        /// <summary>
        /// Save picture by picture virtual path
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SavePictureByVirtualPathAsync(Picture picture, int targetSize = 0)
        {
            if (string.IsNullOrEmpty(picture?.VirtualPath) || string.IsNullOrEmpty(picture.SeoFilename))
                return;

            var pictureBinary = await _pictureService.LoadPictureBinaryAsync(picture);

            if (pictureBinary == null || pictureBinary.Length == 0)
                return;

            var seoFileName = picture.SeoFilename;

            var lastPart = await _pictureService.GetFileExtensionFromMimeTypeAsync(picture.MimeType);

            var thumbFileName = targetSize == 0 ? $"{seoFileName}.{lastPart}" : $"{seoFileName}_{targetSize}.{lastPart}";

            var thumbsDirectoryPath = _fileProvider.GetAbsolutePath(picture.VirtualPath.TrimStart('~'));

            _fileProvider.CreateDirectory(thumbsDirectoryPath);
            var thumbFilePath = _fileProvider.Combine(thumbsDirectoryPath, thumbFileName);

            if (picture.IsNew)
            {
                // delete old file if exist
                _fileProvider.DeleteFile(thumbFilePath);

                //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown
                await _pictureService.UpdatePictureAsync(picture.Id,
                    pictureBinary,
                    picture.MimeType,
                    picture.SeoFilename,
                    picture.AltAttribute,
                    picture.TitleAttribute,
                    false,
                    false);
            }

            if (_fileProvider.FileExists(thumbFilePath))
                return;

            //the named mutex helps to avoid creating the same files in different threads,
            //and does not decrease performance significantly, because the code is blocked only for the specific file.
            //you should be very careful, mutexes cannot be used in with the await operation
            //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
            using var mutex = new Mutex(false, thumbFileName);

            mutex.WaitOne();

            try
            {
                //check, if the file was created, while we were waiting for the release of the mutex.
                if (!_fileProvider.FileExists(thumbFilePath))
                {
                    byte[] pictureBinaryResized;
                    if (targetSize != 0)
                    {
                        //resizing required
                        using var image = SKBitmap.Decode(pictureBinary);
                        var imageFormat = GetImageFormatByMimeType(picture.MimeType);
                        pictureBinaryResized = ImageResize(image, imageFormat, targetSize);
                    }
                    else
                    {
                        //create a copy of pictureBinary
                        pictureBinaryResized = pictureBinary.ToArray();
                    }

                    //save
                    _fileProvider.WriteAllBytesAsync(thumbFilePath, pictureBinaryResized).Wait();
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Resize image by targetSize
        /// </summary>
        /// <param name="image">Source image</param>
        /// <param name="format">Destination format</param>
        /// <param name="targetSize">Target size</param>
        /// <returns>Image as array of byte[]</returns>
        protected virtual byte[] ImageResize(SKBitmap image, SKEncodedImageFormat format, int targetSize)
        {
            if (image == null)
                throw new ArgumentNullException("Image is null");

            float width, height;
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

            if ((int)width == 0 || (int)height == 0)
            {
                width = image.Width;
                height = image.Height;
            }
            try
            {
                using var resizedBitmap = image.Resize(new SKImageInfo((int)width, (int)height), SKFilterQuality.Medium);
                using var cropImage = SKImage.FromBitmap(resizedBitmap);

                return cropImage.Encode(format, _mediaSettings.DefaultImageQuality).ToArray();
            }
            catch
            {
                return image.Bytes;
            }
        }

        /// <summary>
        /// Get image format by mime type
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>SKEncodedImageFormat</returns>
        protected virtual SKEncodedImageFormat GetImageFormatByMimeType(string mimeType)
        {
            var format = SKEncodedImageFormat.Jpeg;
            if (string.IsNullOrEmpty(mimeType))
                return format;

            var parts = mimeType.ToLowerInvariant().Split('/');
            var lastPart = parts[^1];

            switch (lastPart)
            {
                case "webp":
                    format = SKEncodedImageFormat.Webp;
                    break;
                case "png":
                case "gif":
                case "bmp":
                case "x-icon":
                    format = SKEncodedImageFormat.Png;
                    break;
                default:
                    break;
            }

            return format;
        }

        /// <summary>
        /// Flush image on disk
        /// </summary>
        /// <param name="picture">Image to store on disk</param>
        /// <param name="maxWidth">Max image width</param>
        /// <param name="maxHeight">Max image height</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task FlushImagesAsync(Picture picture, int maxWidth, int maxHeight)
        {
            using var image = SKBitmap.Decode((await _pictureService.GetPictureBinaryByPictureIdAsync(picture.Id)).BinaryData);

            maxWidth = image.Width > maxWidth ? maxWidth : 0;
            maxHeight = image.Height > maxHeight ? maxHeight : 0;

            //save picture to folder if its not exists
            await SavePictureByVirtualPathAsync(picture, maxWidth > maxHeight ? maxWidth : maxHeight);
        }

        /// <summary>
        /// Checks if a file is an image
        /// </summary>
        /// <param name="filePath">File path to check</param>
        /// <returns>True, if a file is an image. False, in other case</returns>
        protected virtual bool IsImage(string filePath)
        {
            return GetFileType($".{filePath.Split('.').Last()}") == "image";
        }

        #endregion

        #region Methods

        #region Configuration

        /// <summary>
        /// Initial service configuration
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task ConfigureAsync()
        {
            await base.ConfigureAsync();

            foreach (var filePath in _fileProvider.GetFiles(_fileProvider.GetAbsolutePath(NopRoxyFilemanDefaults.DefaultRootDirectory.Split('/')), topDirectoryOnly: false))
            {
                var uniqueFileName = GetUniqueFileName(filePath, _fileProvider.GetFileNameWithoutExtension(filePath));

                if (await _pictureService.GetPictureSeNameAsync(uniqueFileName) != null)
                    continue;

                var picture = new Picture
                {
                    IsNew = true,
                    SeoFilename = uniqueFileName
                };

                await _pictureService.InsertPictureAsync(
                    new RoxyFilemanFormFile(picture, new PictureBinary { BinaryData = await _fileProvider.ReadAllBytesAsync(filePath) }, _fileProvider.GetFileExtension(filePath)),
                    string.Empty, _fileProvider.GetVirtualPath(filePath));
            }
        }

        #endregion

        #region Directories

        /// <summary>
        /// Move the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task MoveDirectoryAsync(string sourcePath, string destinationPath)
        {
            await base.MoveDirectoryAsync(sourcePath, destinationPath);

            var pictures = await _pictureService.GetPicturesAsync($"{_fileProvider.GetVirtualPath(sourcePath).TrimEnd('/')}/");
            var baseDestinationPathVirtualPath = _fileProvider.GetVirtualPath(destinationPath);

            foreach (var picture in pictures)
            {
                var destinationPathVirtualPath =
                    $"{baseDestinationPathVirtualPath.TrimEnd('/')}/{_fileProvider.GetDirectoryNameOnly(_fileProvider.GetAbsolutePath(sourcePath.TrimStart('~').Split('/')))}";

                picture.VirtualPath = destinationPathVirtualPath;

                await _pictureService.UpdatePictureAsync(picture);
            }
        }

        /// <summary>
        /// Rename the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="newName">New name of the directory</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task RenameDirectoryAsync(string sourcePath, string newName)
        {
            var sourceVirtualPath = _fileProvider.GetVirtualPath(sourcePath).TrimEnd('/');
            var pictures = await _pictureService.GetPicturesAsync($"{sourceVirtualPath}/");

            var destinationPath =
                $"{_fileProvider.GetVirtualPath(_fileProvider.GetParentDirectory(_fileProvider.GetAbsolutePath(sourcePath.Split('/')))).TrimEnd('/')}/{newName}";

            foreach (var picture in pictures)
            {
                picture.VirtualPath = destinationPath;

                await _pictureService.UpdatePictureAsync(picture);
            }

            await base.RenameDirectoryAsync(sourcePath, newName);
        }

        #endregion

        #region Files

        /// <summary>
        /// Copy the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="destinationPath">Path to the destination file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            if (!IsImage(sourcePath))
            {
                await base.CopyFileAsync(sourcePath, destinationPath);

                return;
            }

            var filePath = _fileProvider.GetAbsolutePath(sourcePath.Split('/'));
            var picture = await GetPictureByFileAsync(filePath);

            if (picture == null)
                throw new Exception(await GetLanguageResourceAsync("E_CopyFile"));

            await _pictureService.InsertPictureAsync(
                new RoxyFilemanFormFile(picture, await _pictureService.GetPictureBinaryByPictureIdAsync(picture.Id), _fileProvider.GetFileExtension(filePath)),
                string.Empty, _fileProvider.GetVirtualPath(destinationPath));

            await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
        }

        /// <summary>
        /// Delete the file
        /// </summary>
        /// <param name="sourcePath">Path to the file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task DeleteFileAsync(string sourcePath)
        {
            if (!IsImage(sourcePath))
            {
                await base.DeleteFileAsync(sourcePath);

                return;
            }

            var filePath = _fileProvider.GetAbsolutePath(sourcePath.Split('/'));
            var picture = await GetPictureByFileAsync(filePath);

            if (picture == null)
                throw new Exception(await GetLanguageResourceAsync("E_DeleteFile"));

            await _pictureService.DeletePictureAsync(picture);

            await base.DeleteFileAsync(sourcePath);
        }

        /// <summary>
        /// Move the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="destinationPath">Path to the destination file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task MoveFileAsync(string sourcePath, string destinationPath)
        {
            await base.MoveFileAsync(sourcePath, destinationPath);

            if (!IsImage(sourcePath)) 
                return;

            var filePath = _fileProvider.GetAbsolutePath(sourcePath.Split('/'));
            var picture = await GetPictureByFileAsync(filePath);

            if (picture == null)
                throw new Exception(await GetLanguageResourceAsync("E_MoveFile"));

            picture.VirtualPath = _fileProvider.GetVirtualPath(_fileProvider.GetVirtualPath(_fileProvider.GetDirectoryName(destinationPath)));
            await _pictureService.UpdatePictureAsync(picture);
        }

        /// <summary>
        /// Rename the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="newName">New name of the file</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task RenameFileAsync(string sourcePath, string newName)
        {
            if (!IsImage(sourcePath))
            {
                await base.RenameFileAsync(sourcePath, newName);

                return;
            }
                
            var filePath = _fileProvider.GetAbsolutePath(_fileProvider.GetVirtualPath(sourcePath));
            var picture = await GetPictureByFileAsync(filePath);

            if (picture == null)
                throw new Exception(await GetLanguageResourceAsync("E_RenameFile"));

            picture.SeoFilename = _fileProvider.GetFileNameWithoutExtension(newName);

            await _pictureService.UpdatePictureAsync(picture);

            await base.DeleteFileAsync(sourcePath);
        }


        /// <summary>
        /// Upload files to a directory on passed path
        /// </summary>
        /// <param name="directoryPath">Path to directory to upload files</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UploadFilesAsync(string directoryPath)
        {
            var result = GetSuccessResponse();
            var hasErrors = false;
            try
            {
                var fullPath = GetFullPath(await GetVirtualPathAsync(directoryPath));

                if (!await IsPathAllowedAsync(fullPath))
                    throw new Exception(await GetLanguageResourceAsync("E_UploadNotAll"));

                foreach (var formFile in GetHttpContext().Request.Form.Files)
                {
                    var fileName = formFile.FileName;
                    if (await CanHandleFileAsync(fileName))
                    {
                        var uniqueFileName = GetUniqueFileName(fullPath, _fileProvider.GetFileName(fileName));
                        var destinationFile = _fileProvider.Combine(fullPath, uniqueFileName);

                        if (IsImage(uniqueFileName))
                            await _pictureService.InsertPictureAsync(formFile, virtualPath: await GetVirtualPathAsync(directoryPath));
                        else
                        {
                            await using var stream = new FileStream(destinationFile, FileMode.OpenOrCreate);
                            await formFile.CopyToAsync(stream);
                        }
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

                await GetHttpContext().Response.WriteAsync(result);
            }
            else
                await GetHttpContext().Response.WriteAsync($"<script>parent.fileUploaded({result});</script>");
        }

        #endregion

        #region Images

        /// <summary>
        /// Flush all images on disk
        /// </summary>
        /// <param name="removeOriginal">Specifies whether to delete original images</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task FlushAllImagesOnDiskAsync(bool removeOriginal = true)
        {
            await base.FlushAllImagesOnDiskAsync(removeOriginal);

            var pageIndex = 0;
            const int pageSize = 400;
            _ = int.TryParse(await GetSettingAsync("MAX_IMAGE_WIDTH"), out var width);
            _ = int.TryParse(await GetSettingAsync("MAX_IMAGE_HEIGHT"), out var height);

            try
            {
                while (true)
                {
                    var pictures = await _pictureService.GetPicturesAsync($"~{NopRoxyFilemanDefaults.DefaultRootDirectory}/",
                        pageIndex, pageSize);
                    pageIndex++;

                    //all pictures flushed?
                    if (!pictures.Any())
                        break;

                    foreach (var picture in pictures)
                        await FlushImagesAsync(picture, width, height);

                    if (removeOriginal)
                        await _pictureRepository.DeleteAsync(pictures, false);
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Flush images on disk
        /// </summary>
        /// <param name="directoryPath">Directory path to flush images</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task FlushImagesOnDiskAsync(string directoryPath)
        {
            await base.FlushImagesOnDiskAsync(directoryPath);

            foreach (var picture in await _pictureService.GetPicturesAsync(_fileProvider.GetVirtualPath(directoryPath)))
            {
                _ = int.TryParse(await GetSettingAsync("MAX_IMAGE_WIDTH"), out var width);
                _ = int.TryParse(await GetSettingAsync("MAX_IMAGE_HEIGHT"), out var height);

                await FlushImagesAsync(picture, width, height);
            }
        }

        #endregion

        #endregion
    }
}