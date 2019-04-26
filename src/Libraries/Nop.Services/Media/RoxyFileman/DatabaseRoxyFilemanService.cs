using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Nop.Core.Data;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using static SixLabors.ImageSharp.Configuration;

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
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public DatabaseRoxyFilemanService(IPictureService pictureService,
            IRepository<Picture> pictureRepository,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor,
            INopFileProvider fileProvider,
            MediaSettings mediaSettings) : base(hostingEnvironment, httpContextAccessor, fileProvider)
        {
            _pictureService = pictureService;
            _pictureRepository = pictureRepository;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get directories in the passed parent directory
        /// </summary>
        /// <param name="parentDirectoryPath">Path to the parent directory</param>
        /// <returns>Array of the paths to the directories</returns>
        protected override ArrayList GetDirectories(string parentDirectoryPath)
        {
            CreateDirectory(parentDirectoryPath);

            return base.GetDirectories(parentDirectoryPath);
        }

        /// <summary>
        /// Gets picture from database by file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Exist picture from database or null</returns>
        protected virtual Picture GetPictureByFile(string filePath)
        {
            var sourceVirtualPath = _fileProvider.GetVirtualPath(_fileProvider.GetDirectoryName(filePath));
            var fileName = _fileProvider.GetFileNameWithoutExtension(filePath);

            var picture = _pictureService.GetPictures(sourceVirtualPath.TrimEnd('/'))
                       .FirstOrDefault(p => fileName.Contains(p.SeoFilename));

            return picture;
        }

        /// <summary>
        /// Create the passed directory
        /// </summary>
        /// <param name="directoryPath">Path to the parent directory</param>
        protected virtual void CreateDirectory(string directoryPath)
        {
            _fileProvider.CreateDirectory(directoryPath);
            var virtualPath = _fileProvider.GetVirtualPath(directoryPath).TrimEnd('/');
            var directoryNames = _pictureService.GetPictures($"{virtualPath}/")
                .Where(picture => picture.VirtualPath != virtualPath)
                .Select(picture => _fileProvider.GetAbsolutePath(picture.VirtualPath.TrimStart('~').Split('/')))
                .Distinct();

            foreach (var directory in directoryNames)
            {
                CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Get files in the passed directory
        /// </summary>
        /// <param name="directoryPath">Path to the files directory</param>
        /// <param name="type">Type of the files</param>
        /// <returns>List of paths to the files</returns>
        protected override List<string> GetFiles(string directoryPath, string type)
        {
            if (type == "#")
                type = string.Empty;

            var files = new List<string>();

            //store files on disk if needed
            FlushImagesOnDisk(directoryPath);

            foreach (var fileName in _fileProvider.GetFiles(_fileProvider.DirectoryExists(directoryPath) ? directoryPath : GetFullPath(directoryPath)))
            {
                if (string.IsNullOrEmpty(type) || GetFileType(_fileProvider.GetFileExtension(fileName)) == type)
                    files.Add(fileName);
            }

            return files;
        }

        /// <summary>
        /// Сopy the directory with the embedded files and directories
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="destinationPath">Path to the destination directory</param>
        protected override void CopyDirectory(string sourcePath, string destinationPath)
        {
            var pictures = _pictureService.GetPictures($"{_fileProvider.GetVirtualPath(sourcePath).TrimEnd('/')}/");
            var baseDestinationPathVirtualPath = _fileProvider.GetVirtualPath(destinationPath);

            foreach (var picture in pictures)
            {
                var destinationPathVirtualPath =
                    $"{baseDestinationPathVirtualPath.TrimEnd('/')}{picture.VirtualPath.Replace(_fileProvider.GetVirtualPath(sourcePath), "")}";

                _pictureService.InsertPicture(new RoxyFilemanFormFile(picture, _pictureService.GetFileExtensionFromMimeType(picture.MimeType)), string.Empty, destinationPathVirtualPath);
            }
        }

        /// <summary>
        /// Encode the image into a byte array in accordance with the specified image format
        /// </summary>
        /// <typeparam name="T">Pixel data type</typeparam>
        /// <param name="image">Image data</param>
        /// <param name="imageFormat">Image format</param>
        /// <param name="quality">Quality index that will be used to encode the image</param>
        /// <returns>Image binary data</returns>
        protected virtual byte[] EncodeImage<T>(Image<T> image, IImageFormat imageFormat, int? quality = null) where T : struct, IPixel<T>
        {
            using (var stream = new MemoryStream())
            {
                var imageEncoder = Default.ImageFormatsManager.FindEncoder(imageFormat);
                switch (imageEncoder)
                {
                    case JpegEncoder jpegEncoder:
                        jpegEncoder.IgnoreMetadata = true;
                        jpegEncoder.Quality = quality ?? _mediaSettings.DefaultImageQuality;
                        jpegEncoder.Encode(image, stream);
                        break;

                    case PngEncoder pngEncoder:
                        pngEncoder.ColorType = PngColorType.RgbWithAlpha;
                        pngEncoder.Encode(image, stream);
                        break;

                    case BmpEncoder bmpEncoder:
                        bmpEncoder.BitsPerPixel = BmpBitsPerPixel.Pixel32;
                        bmpEncoder.Encode(image, stream);
                        break;

                    case GifEncoder gifEncoder:
                        gifEncoder.IgnoreMetadata = true;
                        gifEncoder.Encode(image, stream);
                        break;

                    default:
                        imageEncoder.Encode(image, stream);
                        break;
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Calculates picture dimensions whilst maintaining aspect
        /// </summary>
        /// <param name="originalSize">The original picture size</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="resizeType">Resize type</param>
        /// <param name="ensureSizePositive">A value indicating whether we should ensure that size values are positive</param>
        /// <returns></returns>
        protected virtual Size CalculateDimensions(Size originalSize, int targetSize,
            ResizeType resizeType = ResizeType.LongestSide, bool ensureSizePositive = true)
        {
            float width, height;

            switch (resizeType)
            {
                case ResizeType.LongestSide:
                    if (originalSize.Height > originalSize.Width)
                    {
                        // portrait
                        width = originalSize.Width * (targetSize / (float)originalSize.Height);
                        height = targetSize;
                    }
                    else
                    {
                        // landscape or square
                        width = targetSize;
                        height = originalSize.Height * (targetSize / (float)originalSize.Width);
                    }

                    break;
                case ResizeType.Width:
                    width = targetSize;
                    height = originalSize.Height * (targetSize / (float)originalSize.Width);
                    break;
                case ResizeType.Height:
                    width = originalSize.Width * (targetSize / (float)originalSize.Height);
                    height = targetSize;
                    break;
                default:
                    throw new Exception("Not supported ResizeType");
            }

            if (!ensureSizePositive)
                return new Size((int)Math.Round(width), (int)Math.Round(height));

            if (width < 1)
                width = 1;
            if (height < 1)
                height = 1;

            //we invoke Math.Round to ensure that no white background is rendered - https://www.nopcommerce.com/boards/t/40616/image-resizing-bug.aspx
            return new Size((int)Math.Round(width), (int)Math.Round(height));
        }

        /// <summary>
        /// Save picture by picture virtual path
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        protected virtual void SavePictureByVirtualPath(Picture picture, int targetSize = 0)
        {
            if (string.IsNullOrEmpty(picture?.VirtualPath) || string.IsNullOrEmpty(picture.SeoFilename))
                return;

            var pictureBinary = _pictureService.LoadPictureBinary(picture);

            if (pictureBinary == null || pictureBinary.Length == 0)
                return;

            var seoFileName = picture.SeoFilename;

            var lastPart = _pictureService.GetFileExtensionFromMimeType(picture.MimeType);

            var thumbFileName = targetSize == 0 ? $"{seoFileName}.{lastPart}" : $"{seoFileName}_{targetSize}.{lastPart}";

            var thumbsDirectoryPath = _fileProvider.GetAbsolutePath(picture.VirtualPath.TrimStart('~'));

            _fileProvider.CreateDirectory(thumbsDirectoryPath);
            var thumbFilePath = _fileProvider.Combine(thumbsDirectoryPath, thumbFileName);

            if (picture.IsNew)
            {
                // delete old file if exist
                _fileProvider.DeleteFile(thumbFilePath);

                //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown
                _pictureService.UpdatePicture(picture.Id,
                    pictureBinary,
                    picture.MimeType,
                    picture.SeoFilename,
                    picture.AltAttribute,
                    picture.TitleAttribute,
                    false,
                    false);
            }

            //the named mutex helps to avoid creating the same files in different threads,
            //and does not decrease performance significantly, because the code is blocked only for the specific file.
            using (var mutex = new Mutex(false, thumbFileName))
            {
                if (_fileProvider.FileExists(thumbFilePath))
                    return;

                mutex.WaitOne();

                //check, if the file was created, while we were waiting for the release of the mutex.
                if (!_fileProvider.FileExists(thumbFilePath))
                {
                    byte[] pictureBinaryResized;
                    if (targetSize != 0)
                    {
                        //resizing required
                        using (var image = Image.Load(pictureBinary, out var imageFormat))
                        {
                            var size = image.Size();

                            image.Mutate(imageProcess => imageProcess.Resize(new ResizeOptions
                            {
                                Mode = ResizeMode.Max,
                                Size = CalculateDimensions(size, targetSize)
                            }));

                            pictureBinaryResized = EncodeImage(image, imageFormat);
                        }
                    }
                    else
                    {
                        //create a copy of pictureBinary
                        pictureBinaryResized = pictureBinary.ToArray();
                    }

                    //save
                    _fileProvider.WriteAllBytes(thumbFilePath, pictureBinaryResized);
                }

                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Flush image on disk
        /// </summary>
        /// <param name="picture">Image to store on disk</param>
        /// <param name="maxWidth">Max image width</param>
        /// <param name="maxHeight">Max image height</param>
        protected virtual void FlushImages(Picture picture, int maxWidth, int maxHeight)
        {
            var image = Image.Load(picture.PictureBinary.BinaryData);

            maxWidth = image.Width > maxWidth ? maxWidth : 0;
            maxHeight = image.Height > maxHeight ? maxHeight : 0;

            //save picture to folder if its not exists
            SavePictureByVirtualPath(picture, maxWidth > maxHeight ? maxWidth : maxHeight);
        }

        #endregion

        #region Methods

        #region Configuration

        /// <summary>
        /// Initial service configuration
        /// </summary>
        public override void Configure()
        {
            foreach (var filePath in _fileProvider.GetFiles(_fileProvider.GetAbsolutePath(NopRoxyFilemanDefaults.DefaultRootDirectory.Split('/')), topDirectoryOnly: false))
            {
                var uniqueFileName = GetUniqueFileName(filePath, _fileProvider.GetFileNameWithoutExtension(filePath));

                var picture = new Picture
                {
                    IsNew = true,
                    SeoFilename = uniqueFileName,
                    PictureBinary = new PictureBinary
                    {
                        BinaryData = _fileProvider.ReadAllBytes(filePath)
                    }
                };

                _pictureService.InsertPicture(
                    new RoxyFilemanFormFile(picture, _fileProvider.GetFileExtension(filePath)),
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
        /// <returns>A task that represents the completion of the operation</returns>
        public override async Task MoveDirectoryAsync(string sourcePath, string destinationPath)
        {
            await base.MoveDirectoryAsync(sourcePath, destinationPath);

            var pictures = _pictureService.GetPictures($"{_fileProvider.GetVirtualPath(sourcePath).TrimEnd('/')}/");
            var baseDestinationPathVirtualPath = _fileProvider.GetVirtualPath(destinationPath);

            foreach (var picture in pictures)
            {
                var destinationPathVirtualPath =
                    $"{baseDestinationPathVirtualPath.TrimEnd('/')}/{_fileProvider.GetDirectoryNameOnly(_fileProvider.GetAbsolutePath(sourcePath.TrimStart('~').Split('/')))}";

                picture.VirtualPath = destinationPathVirtualPath;

                _pictureService.UpdatePicture(picture);
            }
        }

        /// <summary>
        /// Rename the directory
        /// </summary>
        /// <param name="sourcePath">Path to the source directory</param>
        /// <param name="newName">New name of the directory</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public override async Task RenameDirectoryAsync(string sourcePath, string newName)
        {
            var sourceVirtualPath = _fileProvider.GetVirtualPath(sourcePath).TrimEnd('/');
            var pictures = _pictureService.GetPictures($"{sourceVirtualPath}/");

            var destinationPath =
                $"{_fileProvider.GetVirtualPath(_fileProvider.GetParentDirectory(_fileProvider.GetAbsolutePath(sourcePath.Split('/')))).TrimEnd('/')}/{newName}";

            foreach (var picture in pictures)
            {
                picture.VirtualPath = destinationPath;

                _pictureService.UpdatePicture(picture);
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
        /// <returns>A task that represents the completion of the operation</returns>
        public override async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            var filePath = _fileProvider.GetAbsolutePath(sourcePath.Split('/'));
            var picture = GetPictureByFile(filePath);

            if (picture == null)
                throw new Exception(GetLanguageResource("E_CopyFile"));

            _pictureService.InsertPicture(
                new RoxyFilemanFormFile(picture, _fileProvider.GetFileExtension(filePath)),
                string.Empty, _fileProvider.GetVirtualPath(destinationPath));

            await GetHttpContext().Response.WriteAsync(GetSuccessResponse());
        }

        /// <summary>
        /// Delete the file
        /// </summary>
        /// <param name="sourcePath">Path to the file</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public override async Task DeleteFileAsync(string sourcePath)
        {
            var filePath = _fileProvider.GetAbsolutePath(sourcePath.Split('/'));
            var picture = GetPictureByFile(filePath);

            if (picture == null)
                throw new Exception(GetLanguageResource("E_DeletеFile"));

            _pictureService.DeletePicture(picture);

            await base.DeleteFileAsync(sourcePath);
        }

        /// <summary>
        /// Move the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="destinationPath">Path to the destination file</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public override async Task MoveFileAsync(string sourcePath, string destinationPath)
        {
            await base.MoveFileAsync(sourcePath, destinationPath);

            var filePath = _fileProvider.GetAbsolutePath(sourcePath.Split('/'));
            var picture = GetPictureByFile(filePath);

            if (picture == null)
                throw new Exception(GetLanguageResource("E_MoveFile"));

            picture.VirtualPath = _fileProvider.GetVirtualPath(_fileProvider.GetVirtualPath(_fileProvider.GetDirectoryName(destinationPath)));
            _pictureService.UpdatePicture(picture);
        }

        /// <summary>
        /// Rename the file
        /// </summary>
        /// <param name="sourcePath">Path to the source file</param>
        /// <param name="newName">New name of the file</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public override async Task RenameFileAsync(string sourcePath, string newName)
        {
            var filePath = _fileProvider.GetAbsolutePath(_fileProvider.GetVirtualPath(sourcePath));
            var picture = GetPictureByFile(filePath);

            if (picture == null)
                throw new Exception(GetLanguageResource("E_RenameFile"));

            picture.SeoFilename = _fileProvider.GetFileNameWithoutExtension(newName);

            _pictureService.UpdatePicture(picture);

            await base.DeleteFileAsync(sourcePath);
        }

        /// <summary>
        /// Upload files to a directory on passed path
        /// </summary>
        /// <param name="directoryPath">Path to directory to upload files</param>
        /// <returns>A task that represents the completion of the operation</returns>
        public override async Task UploadFilesAsync(string directoryPath)
        {
            var result = GetSuccessResponse();
            var hasErrors = false;
            try
            {
                var fullPath = GetFullPath(GetVirtualPath(directoryPath));
                foreach (var formFile in GetHttpContext().Request.Form.Files)
                {
                    var fileName = formFile.FileName;
                    if (CanHandleFile(fileName))
                    {
                        var uniqueFileName = GetUniqueFileName(fullPath, _fileProvider.GetFileName(fileName));
                        var destinationFile = _fileProvider.Combine(fullPath, uniqueFileName);

                        if (GetFileType(new FileInfo(uniqueFileName).Extension) != "image")
                        {
                            using (var stream = new FileStream(destinationFile, FileMode.OpenOrCreate))
                            {
                                formFile.CopyTo(stream);
                            }
                        }
                        else
                        {
                            _pictureService.InsertPicture(formFile, virtualPath: GetVirtualPath(directoryPath));
                        }
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
        /// Flush all images on disk
        /// </summary>
        /// <param name="removeOriginal">Specifies whether to delete original images</param>
        public override void FlushAllImagesOnDisk(bool removeOriginal = true)
        {
            base.FlushAllImagesOnDisk(removeOriginal);

            var pageIndex = 0;
            const int pageSize = 400;
            int.TryParse(GetSetting("MAX_IMAGE_WIDTH"), out var width);
            int.TryParse(GetSetting("MAX_IMAGE_HEIGHT"), out var height);

            try
            {
                while (true)
                {
                    var pictures = _pictureService.GetPictures($"~{NopRoxyFilemanDefaults.DefaultRootDirectory}/",
                        pageIndex, pageSize);
                    pageIndex++;

                    //all pictures flushed?
                    if (!pictures.Any())
                        break;

                    foreach (var picture in pictures)
                    {
                        FlushImages(picture, width, height);
                    }

                    if (removeOriginal)
                        _pictureRepository.Delete(pictures);
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
        public override void FlushImagesOnDisk(string directoryPath)
        {
            base.FlushImagesOnDisk(directoryPath);

            foreach (var picture in _pictureService.GetPictures(_fileProvider.GetVirtualPath(directoryPath)))
            {
                int.TryParse(GetSetting("MAX_IMAGE_WIDTH"), out var width);
                int.TryParse(GetSetting("MAX_IMAGE_HEIGHT"), out var height);

                FlushImages(picture, width, height);
            }
        }

        #endregion

        #endregion
    }
}