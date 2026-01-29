using FluentMigrator;
using Microsoft.Extensions.FileProviders;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Media;
using Nop.Services.Media.ElFinder;
using Nop.Web.Framework.Extensions;
using SkiaSharp;

namespace Nop.Web.Framework.Migrations.UpgradeTo460;

[NopMigration("2022-10-18 00:00:00", "Move uploaded images to disk", MigrationProcessType.Update)]
public class UploadedImagesMigration : Migration
{
    #region Fields

    protected readonly INopDataProvider _dataProvider;
    protected readonly INopFileProvider _nopFileProvider;
    protected readonly IRepository<Picture> _pictureRepository;
    protected readonly IRepository<PictureBinary> _pictureBinaryRepository;
    protected readonly MediaSettings _mediaSettings;

    #endregion

    public UploadedImagesMigration(INopDataProvider dataProvider,
        INopFileProvider nopFileProvider,
        IRepository<Picture> pictureRepository,
        IRepository<PictureBinary> pictureBinaryRepository,
        MediaSettings mediaSettings)
    {
        _dataProvider = dataProvider;
        _nopFileProvider = nopFileProvider;
        _pictureRepository = pictureRepository;
        _pictureBinaryRepository = pictureBinaryRepository;
        _mediaSettings = mediaSettings;
    }

    #region Utilities

    /// <summary>
    /// Save file in the root directory for this instance
    /// </summary>
    /// <param name="pictureBinary">Picture binary data</param>
    /// <param name="directoryPath">Directory path in the root</param>
    /// <param name="fileName">The file name</param>
    /// <param name="fileExt">The file extension</param>
    /// <param name="contentType">Mime type</param>
    protected void SaveFile(byte[] pictureBinary, string directoryPath, string fileName, string fileExt, string contentType)
    {
        var uniquePath = _nopFileProvider.GetAbsolutePath(directoryPath, $"{fileName}.{fileExt}");
        var i = 0;
        while (_nopFileProvider.GetFileInfo(uniquePath) is IFileInfo fileInfo && fileInfo.Exists)
            uniquePath = _nopFileProvider.GetAbsolutePath(directoryPath, $"{fileName}-Copy-{++i}.{fileExt}");

        using var memoryStream = new MemoryStream(pictureBinary);
        using var stream = _nopFileProvider.GetOrCreateFile(uniquePath);

        if (fileExt == "jpg" || fileExt == "jpeg" || fileExt == "png" || fileExt == "gif" || fileExt == "webp" || fileExt == "svg")
        {
            using var sourceStream = new SKMemoryStream(memoryStream.ToArray());
            using var inputData = SKData.Create(sourceStream);
            using var image = SKBitmap.Decode(inputData);

            var width = Math.Min(image.Width, _mediaSettings.MaximumImageSize);
            var height = Math.Min(image.Height, _mediaSettings.MaximumImageSize);
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

            var toBitmap = new SKBitmap(width, height, image.ColorType, image.AlphaType);
            var samplingOptions = new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None);

            if (!image.ScalePixels(toBitmap, samplingOptions))
                throw new Exception("Image scaling");

            var format = contentType?.ToLowerInvariant().Split('/')[^1] switch
            {
                "bmp" or "gif" or "x-png" or "png" or "x-icon" => SKEncodedImageFormat.Png,
                "webp" => SKEncodedImageFormat.Webp,
                _ => SKEncodedImageFormat.Jpeg,
            };
            var newImage = SKImage.FromBitmap(toBitmap);
            var imageData = newImage.Encode(format, _mediaSettings.DefaultImageQuality);

            newImage.Dispose();
            stream.Write(imageData.ToArray());
        }
        else
        {
            memoryStream.CopyTo(stream);
        }

        stream.Flush();
    }

    #endregion

    public override void Up()
    {
        if (!this.GetSettingByKey("Media.Images.StoreInDB", true))
            return;

        const int pageSize = 400;
        var pageIndex = 0;
        var rootPath = _nopFileProvider.Combine(_nopFileProvider.GetLocalImagesPath(_mediaSettings), NopElFinderDefaults.DefaultRootDirectory);
        var uploadRoot = _nopFileProvider.GetVirtualPath(rootPath);

        try
        {
            var query = _pictureRepository.Table
                .Where(p => p.VirtualPath != null && p.VirtualPath.StartsWith(uploadRoot))
                .OrderByDescending(p => p.Id);

            while (true)
            {
                var pictures = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                pageIndex++;

                //all pictures flushed?
                if (!pictures.Any())
                    break;

                foreach (var picture in pictures)
                {
                    if (string.IsNullOrEmpty(picture?.VirtualPath) || string.IsNullOrEmpty(picture.SeoFilename))
                        return;

                    var seoFileName = picture.SeoFilename;
                    var ext = picture.MimeType?.ToLowerInvariant().Split('/')[^1] switch
                    {
                        "pjpeg" => "jpg",
                        "jpeg" => "jpeg",
                        "bmp" => "bmp",
                        "gif" => "gif",
                        "x-png" or "png" => "png",
                        "tiff" => "tiff",
                        "x-icon" => "ico",
                        "webp" => "webp",
                        "svg+xml" => "svg",
                        _ => string.Empty,
                    };
                    var directoryPath = _nopFileProvider.Combine(rootPath, picture.VirtualPath[uploadRoot.Length..]);
                    var filePath = _nopFileProvider.GetAbsolutePath(directoryPath, $"{seoFileName}.{ext}");

                    if (picture.IsNew)
                    {
                        // delete old file if exist
                        _nopFileProvider.DeleteFile(filePath);
                    }

                    if (_nopFileProvider.GetFileInfo(filePath).Exists)
                    {
                        _dataProvider.DeleteEntity(picture);
                        continue;
                    }

                    //the named mutex helps to avoid creating the same files in different threads,
                    //and does not decrease performance significantly, because the code is blocked only for the specific file.
                    //you should be very careful, mutexes cannot be used in with the await operation
                    //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
                    using var mutex = new Mutex(false, $"{seoFileName}.{ext}");

                    mutex.WaitOne();

                    try
                    {
                        //check, if the file was created, while we were waiting for the release of the mutex.
                        if (!_nopFileProvider.GetFileInfo(filePath).Exists)
                        {
                            var pictureBinary = _pictureBinaryRepository.Table.FirstOrDefault(pb => pb.PictureId == picture.Id);
                            if (pictureBinary?.BinaryData == null || pictureBinary.BinaryData.Length == 0)
                                continue;

                            SaveFile(pictureBinary.BinaryData, directoryPath, seoFileName, ext, picture.MimeType);
                        }
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }

                    _dataProvider.DeleteEntity(picture);
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