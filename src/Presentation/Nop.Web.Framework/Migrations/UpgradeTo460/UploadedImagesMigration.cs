using FluentMigrator;
using Microsoft.Extensions.FileProviders;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Media;
using Nop.Services.Media.ElFinder;
using SkiaSharp;

namespace Nop.Web.Framework.Migrations.UpgradeTo460;

[NopMigration("2022-10-18 00:00:00", "Move uploaded images to disk", MigrationProcessType.Update)]
public class UploadedImagesMigration : Migration
{
    #region Fields

    protected readonly MediaSettings _mediaSettings;
    protected readonly NopFileProvider _nopFileProvider;
    protected readonly IPictureService _pictureService;
    protected readonly IRepository<Picture> _pictureRepository;

    #endregion

    public UploadedImagesMigration(MediaSettings mediaSettings,
        NopFileProvider nopFileProvider,
        IPictureService pictureService, 
        IRepository<Picture> pictureRepository)
    {
        _mediaSettings = mediaSettings;
        _nopFileProvider = nopFileProvider;
        _pictureService = pictureService;
        _pictureRepository = pictureRepository;
    }

    #region Utilities

    /// <summary>
    /// Get the absolute path for the specified path string in the root directory for this instance
    /// </summary>
    /// <param name="path">The file or directory for which to obtain absolute path information</param>
    /// <returns>The fully qualified location of path, such as "C:\MyFile.txt"</returns>
    protected virtual string GetFullPath(string path)
    {
        path = path.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(_nopFileProvider.Root, path));

        return fullPath;
    }

    /// <summary>
    /// Get the unique name of the file (add -copy-(N) to the file name if there is already a file with that name in the directory)
    /// </summary>
    /// <param name="directoryPath">Path to the file directory</param>
    /// <param name="fileName">Original file name</param>
    /// <returns>Unique name of the file</returns>
    protected virtual string GetUniqueFileName(string directoryPath, string fileName)
    {
        var uniqueFileName = fileName;

        var i = 0;
        while (_nopFileProvider.GetFileInfo(Path.Combine(directoryPath, uniqueFileName)) is IFileInfo fileInfo && fileInfo.Exists)
        {
            uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-Copy-{++i}{Path.GetExtension(fileName)}";
        }

        return uniqueFileName;
    }

    /// <summary>
    /// Get a file type by the specified path string
    /// </summary>
    /// <param name="subpath">The path string from which to get the file type</param>
    /// <returns>File type</returns>
    protected virtual string GetFileType(string subpath)
    {
        var fileExtension = Path.GetExtension(subpath)?.ToLowerInvariant();

        return fileExtension switch
        {
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".svg" => "image",
            ".swf" or ".flv" => "flash",
            ".mp4" or ".webm" or ".ogg" or ".mov" or ".m4a" or ".mp3" or ".wav" => "media",
            _ => "file"
        };
    }

    /// <summary>
    /// Adjust image measures to target size
    /// </summary>
    /// <param name="image">Source image</param>
    /// <param name="maxWidth">Target width</param>
    /// <param name="maxHeight">Target height</param>
    /// <returns>Adjusted width and height</returns>
    protected virtual (int width, int height) ValidateImageMeasures(SKBitmap image, int maxWidth = 0, int maxHeight = 0)
    {
        ArgumentNullException.ThrowIfNull(image);

        float width = Math.Min(image.Width, maxWidth);
        float height = Math.Min(image.Height, maxHeight);

        var targetSize = Math.Max(width, height);

        if (image.Height > image.Width)
        {
            // portrait
            width = image.Width * (targetSize / image.Height);
            height = targetSize;
        }
        else
        {
            // landscape or square
            width = targetSize;
            height = image.Height * (targetSize / image.Width);
        }

        return ((int)width, (int)height);
    }

    /// <summary>
    /// Scale image to fit the destination sizes
    /// </summary>
    /// <param name="data">Image data</param>
    /// <param name="format">SkiaSharp image format</param>
    /// <param name="maxWidth">Target width</param>
    /// <param name="maxHeight">Target height</param>
    /// <returns>The byte array of resized image</returns>
    protected virtual byte[] ResizeImage(byte[] data, SKEncodedImageFormat format, int maxWidth, int maxHeight)
    {
        using var sourceStream = new SKMemoryStream(data);
        using var inputData = SKData.Create(sourceStream);
        using var image = SKBitmap.Decode(inputData);

        var (width, height) = ValidateImageMeasures(image, maxWidth, maxHeight);

        var toBitmap = new SKBitmap(width, height, image.ColorType, image.AlphaType);
        var samplingOptions = new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None);

        if (!image.ScalePixels(toBitmap, samplingOptions))
            throw new Exception("Image scaling");

        var newImage = SKImage.FromBitmap(toBitmap);
        var imageData = newImage.Encode(format, _mediaSettings.DefaultImageQuality);

        newImage.Dispose();
        return imageData.ToArray();
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
    /// Save file in the root directory for this instance
    /// </summary>
    /// <param name="directoryPath">Directory path in the root</param>
    /// <param name="fileName">The file name and extension</param>
    /// <param name="contentType">Mime type</param>
    /// <param name="fileStream">The stream to read</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SaveFileAsync(string directoryPath, string fileName, string contentType, Stream fileStream)
    {
        var uniqueFileName = GetUniqueFileName(directoryPath, Path.GetFileName(fileName));
        var destinationFile = Path.Combine(directoryPath, uniqueFileName);

        await using var stream = new FileStream(GetFullPath(destinationFile), FileMode.Create);

        if (GetFileType(Path.GetExtension(uniqueFileName)) == "image")
        {
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);


            var imageData = ResizeImage(memoryStream.ToArray(),
                GetImageFormatByMimeType(contentType),
                _mediaSettings.MaximumImageSize,
                _mediaSettings.MaximumImageSize);

            await stream.WriteAsync(imageData);
        }
        else
        {
            await fileStream.CopyToAsync(stream);
        }

        await stream.FlushAsync();
    }

    #endregion

    public override void Up()
    {
        if (!_pictureService.IsStoreInDbAsync().Result)
            return;

        const int pageSize = 400;
        var pageIndex = 0;
        var uploadRoot = $"~/{NopElFinderDefaults.DefaultRootDirectory}/";

        try
        {
            while (true)
            {
                var pictures = _pictureService.GetPicturesAsync(uploadRoot,
                    pageIndex, pageSize).Result;
                pageIndex++;

                //all pictures flushed?
                if (!pictures.Any())
                    break;

                foreach (var picture in pictures)
                {
                    if (string.IsNullOrEmpty(picture?.VirtualPath) || string.IsNullOrEmpty(picture.SeoFilename))
                        return;

                    var seoFileName = picture.SeoFilename;

                    var lastPart = _pictureService.GetFileExtensionFromMimeTypeAsync(picture.MimeType).Result;

                    var thumbFileName = $"{seoFileName}.{lastPart}";
                    var thumbDirectoryName = picture.VirtualPath[uploadRoot.Length..];
                    var thumbFilePath = Path.Combine(thumbDirectoryName, thumbFileName);

                    //create directory if not exists
                    var fullPath = GetFullPath(Path.Combine("/", thumbDirectoryName));

                    var newDirectory = new DirectoryInfo(fullPath);
                    if (!newDirectory.Exists)
                        newDirectory.Create();

                    if (picture.IsNew)
                    {
                        // delete old file if exist
                        _nopFileProvider.DeleteFile(GetFullPath(thumbFilePath));

                    }

                    if (_nopFileProvider.GetFileInfo(thumbFilePath).Exists)
                    {
                        _pictureRepository.DeleteAsync(picture, false).Wait();
                        continue;
                    }
                    //the named mutex helps to avoid creating the same files in different threads,
                    //and does not decrease performance significantly, because the code is blocked only for the specific file.
                    //you should be very careful, mutexes cannot be used in with the await operation
                    //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
                    using var mutex = new Mutex(false, thumbFileName);

                    mutex.WaitOne();

                    try
                    {
                        //check, if the file was created, while we were waiting for the release of the mutex.
                        if (!_nopFileProvider.GetFileInfo(thumbFilePath).Exists)
                        {
                            var pictureBinary = _pictureService.LoadPictureBinaryAsync(picture).Result;

                            if (pictureBinary == null || pictureBinary.Length == 0)
                                continue;

                            using var stream = new MemoryStream(pictureBinary);
                            SaveFileAsync(thumbDirectoryName, thumbFileName, picture.MimeType, stream).Wait();
                        }
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }

                    _pictureRepository.DeleteAsync(picture, false).Wait();
                }
            }
        }
        catch
        {
            // ignored
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}