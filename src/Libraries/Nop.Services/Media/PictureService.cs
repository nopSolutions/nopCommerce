using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Seo;
using SkiaSharp;
using Svg.Skia;

namespace Nop.Services.Media;

/// <summary>
/// Picture service
/// </summary>
public partial class PictureService : IPictureService
{
    #region Fields

    protected readonly IDownloadService _downloadService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILogger _logger;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IRepository<Picture> _pictureRepository;
    protected readonly IRepository<PictureBinary> _pictureBinaryRepository;
    protected readonly IRepository<ProductPicture> _productPictureRepository;
    protected readonly ISettingService _settingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly MediaSettings _mediaSettings;

    #endregion

    #region Ctor

    public PictureService(IDownloadService downloadService,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger,
        INopFileProvider fileProvider,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IRepository<Picture> pictureRepository,
        IRepository<PictureBinary> pictureBinaryRepository,
        IRepository<ProductPicture> productPictureRepository,
        ISettingService settingService,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        MediaSettings mediaSettings)
    {
        _downloadService = downloadService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _fileProvider = fileProvider;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _pictureRepository = pictureRepository;
        _pictureBinaryRepository = pictureBinaryRepository;
        _productPictureRepository = productPictureRepository;
        _settingService = settingService;
        _urlRecordService = urlRecordService;
        _webHelper = webHelper;
        _mediaSettings = mediaSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Loads a picture from file
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="mimeType">MIME type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    protected virtual async Task<byte[]> LoadPictureFromFileAsync(int pictureId, string mimeType)
    {
        var lastPart = await GetFileExtensionFromMimeTypeAsync(mimeType);
        var fileName = $"{pictureId:0000000}_0.{lastPart}";
        var filePath = await GetPictureLocalPathAsync(fileName);

        return await _fileProvider.ReadAllBytesAsync(filePath);
    }

    /// <summary>
    /// Save picture on file system
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="pictureBinary">Picture binary</param>
    /// <param name="mimeType">MIME type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SavePictureInFileAsync(int pictureId, byte[] pictureBinary, string mimeType)
    {
        var lastPart = await GetFileExtensionFromMimeTypeAsync(mimeType);
        var fileName = $"{pictureId:0000000}_0.{lastPart}";
        await _fileProvider.WriteAllBytesAsync(await GetPictureLocalPathAsync(fileName), pictureBinary);
    }

    /// <summary>
    /// Delete a picture on file system
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task DeletePictureOnFileSystemAsync(Picture picture)
    {
        ArgumentNullException.ThrowIfNull(picture);

        var lastPart = await GetFileExtensionFromMimeTypeAsync(picture.MimeType);
        var fileName = $"{picture.Id:0000000}_0.{lastPart}";
        var filePath = await GetPictureLocalPathAsync(fileName);
        _fileProvider.DeleteFile(filePath);
    }

    /// <summary>
    /// Delete picture thumbs
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task DeletePictureThumbsAsync(Picture picture)
    {
        var filter = $"{picture.Id:0000000}*.*";
        var currentFiles = _fileProvider.GetFiles(_fileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath), filter, false);
        foreach (var currentFileName in currentFiles)
        {
            var thumbFilePath = await GetThumbLocalPathAsync(currentFileName);
            _fileProvider.DeleteFile(thumbFilePath);
        }
    }

    /// <summary>
    /// Get picture (thumb) local path
    /// </summary>
    /// <param name="thumbFileName">Filename</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    protected virtual Task<string> GetThumbLocalPathAsync(string thumbFileName)
    {
        var thumbsDirectoryPath = _fileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath);

        if (_mediaSettings.MultipleThumbDirectories)
        {
            //get the first two letters of the file name
            var fileNameWithoutExtension = _fileProvider.GetFileNameWithoutExtension(thumbFileName);
            if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length > NopMediaDefaults.MultipleThumbDirectoriesLength)
            {
                var subDirectoryName = fileNameWithoutExtension[0..NopMediaDefaults.MultipleThumbDirectoriesLength];
                thumbsDirectoryPath = _fileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath, subDirectoryName);
                _fileProvider.CreateDirectory(thumbsDirectoryPath);
            }
        }

        var thumbFilePath = _fileProvider.Combine(thumbsDirectoryPath, thumbFileName);
        return Task.FromResult(thumbFilePath);
    }

    /// <summary>
    /// Get images path URL 
    /// </summary>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    protected virtual Task<string> GetImagesPathUrlAsync(string storeLocation = null)
    {
        var pathBase = _httpContextAccessor.HttpContext.Request?.PathBase.Value ?? string.Empty;
        var imagesPathUrl = _mediaSettings.UseAbsoluteImagePath ? storeLocation : $"{pathBase}/";
        imagesPathUrl = string.IsNullOrEmpty(imagesPathUrl) ? _webHelper.GetStoreLocation() : imagesPathUrl;
        imagesPathUrl += "images/";

        return Task.FromResult(imagesPathUrl);
    }

    /// <summary>
    /// Get picture (thumb) URL 
    /// </summary>
    /// <param name="thumbFileName">Filename</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    protected virtual async Task<string> GetThumbUrlAsync(string thumbFileName, string storeLocation = null)
    {
        var url = await GetImagesPathUrlAsync(storeLocation) + "thumbs/";

        if (_mediaSettings.MultipleThumbDirectories)
        {
            //get the first two letters of the file name
            var fileNameWithoutExtension = _fileProvider.GetFileNameWithoutExtension(thumbFileName);
            if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length > NopMediaDefaults.MultipleThumbDirectoriesLength)
            {
                var subDirectoryName = fileNameWithoutExtension[0..NopMediaDefaults.MultipleThumbDirectoriesLength];
                url = url + subDirectoryName + "/";
            }
        }

        url += thumbFileName;
        return url;
    }

    /// <summary>
    /// Get picture local path. Used when images stored on file system (not in the database)
    /// </summary>
    /// <param name="fileName">Filename</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture path
    /// </returns>
    protected virtual Task<string> GetPictureLocalPathAsync(string fileName)
    {
        return Task.FromResult(_fileProvider.GetAbsolutePath("images", fileName));
    }

    /// <summary>
    /// Gets the loaded picture binary depending on picture storage settings
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <param name="fromDb">Load from database; otherwise, from file system</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    protected virtual async Task<byte[]> LoadPictureBinaryAsync(Picture picture, bool fromDb)
    {
        ArgumentNullException.ThrowIfNull(picture);

        var result = fromDb
            ? (await GetPictureBinaryByPictureIdAsync(picture.Id))?.BinaryData ?? Array.Empty<byte>()
            : await LoadPictureFromFileAsync(picture.Id, picture.MimeType);

        return result;
    }

    /// <summary>
    /// Get a value indicating whether some file (thumb) already exists
    /// </summary>
    /// <param name="thumbFilePath">Thumb file path</param>
    /// <param name="thumbFileName">Thumb file name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    protected virtual Task<bool> GeneratedThumbExistsAsync(string thumbFilePath, string thumbFileName)
    {
        return Task.FromResult(_fileProvider.FileExists(thumbFilePath));
    }

    /// <summary>
    /// Save a value indicating whether some file (thumb) already exists
    /// </summary>
    /// <param name="thumbFilePath">Thumb file path</param>
    /// <param name="thumbFileName">Thumb file name</param>
    /// <param name="mimeType">MIME type</param>
    /// <param name="binary">Picture binary</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SaveThumbAsync(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
    {
        //ensure \thumb directory exists
        var thumbsDirectoryPath = _fileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath);
        _fileProvider.CreateDirectory(thumbsDirectoryPath);

        //save
        await _fileProvider.WriteAllBytesAsync(thumbFilePath, binary);
    }

    /// <summary>
    /// Updates the picture binary data
    /// </summary>
    /// <param name="picture">The picture object</param>
    /// <param name="binaryData">The picture binary data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    protected virtual async Task<PictureBinary> UpdatePictureBinaryAsync(Picture picture, byte[] binaryData)
    {
        ArgumentNullException.ThrowIfNull(picture);

        var pictureBinary = await GetPictureBinaryByPictureIdAsync(picture.Id);

        var isNew = pictureBinary == null;

        if (isNew)
            pictureBinary = new PictureBinary
            {
                PictureId = picture.Id
            };

        pictureBinary.BinaryData = binaryData;

        if (isNew)
            await _pictureBinaryRepository.InsertAsync(pictureBinary);
        else
            await _pictureBinaryRepository.UpdateAsync(pictureBinary);

        return pictureBinary;
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
    /// Gets the MIME type from the file name
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    protected virtual string GetMimeTypeFromFileName(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
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
        ArgumentNullException.ThrowIfNull(image);

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

            //In order to exclude saving pictures in low quality at the time of installation, we will set the value of this parameter to 80 (as by default)
            return cropImage.Encode(format, _mediaSettings.DefaultImageQuality > 0 ? _mediaSettings.DefaultImageQuality : 80).ToArray();
        }
        catch
        {
            return image.Bytes;
        }

    }

    /// <summary>
    /// Gets pictures
    /// </summary>
    /// <param name="pictureIds">Picture identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of pictures
    /// </returns>
    protected virtual async Task<IList<Picture>> GetPicturesByIdsAsync(int[] pictureIds)
    {
        return await _pictureRepository.GetByIdsAsync(pictureIds, cache => default);
    }

    #endregion

    #region Getting picture local path/URL methods

    /// <summary>
    /// Returns the file extension from mime type.
    /// </summary>
    /// <param name="mimeType">Mime type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the file extension
    /// </returns>
    public virtual Task<string> GetFileExtensionFromMimeTypeAsync(string mimeType)
    {
        if (mimeType == null)
            return Task.FromResult<string>(null);

        var parts = mimeType.Split('/');
        var lastPart = parts[^1];
        lastPart = lastPart switch
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
            _ => "",
        };
        return Task.FromResult(lastPart);
    }

    /// <summary>
    /// Gets the loaded picture binary depending on picture storage settings
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    public virtual async Task<byte[]> LoadPictureBinaryAsync(Picture picture)
    {
        return await LoadPictureBinaryAsync(picture, await IsStoreInDbAsync());
    }

    /// <summary>
    /// Get picture SEO friendly name
    /// </summary>
    /// <param name="name">Name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<string> GetPictureSeNameAsync(string name)
    {
        return await _urlRecordService.GetSeNameAsync(name, true, false);
    }

    /// <summary>
    /// Gets the default picture URL
    /// </summary>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="defaultPictureType">Default picture type</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture URL
    /// </returns>
    public virtual async Task<string> GetDefaultPictureUrlAsync(int targetSize = 0,
        PictureType defaultPictureType = PictureType.Entity,
        string storeLocation = null)
    {
        //get overridden default image if exists
        if (defaultPictureType == PictureType.Entity && _mediaSettings.ProductDefaultImageId > 0)
            return await GetPictureUrlAsync(_mediaSettings.ProductDefaultImageId, targetSize, false, storeLocation, PictureType.Entity);

        var defaultImageFileName = defaultPictureType switch
        {
            PictureType.Avatar => await _settingService.GetSettingByKeyAsync("Media.Customer.DefaultAvatarImageName", NopMediaDefaults.DefaultAvatarFileName),
            _ => await _settingService.GetSettingByKeyAsync("Media.DefaultImageName", NopMediaDefaults.DefaultImageFileName),
        };
        var filePath = await GetPictureLocalPathAsync(defaultImageFileName);
        if (!_fileProvider.FileExists(filePath))
        {
            return string.Empty;
        }

        if (targetSize == 0)
            return await GetImagesPathUrlAsync(storeLocation) + defaultImageFileName;

        var fileExtension = _fileProvider.GetFileExtension(filePath);
        var thumbFileName = $"{_fileProvider.GetFileNameWithoutExtension(filePath)}_{targetSize}{fileExtension}";
        var thumbFilePath = await GetThumbLocalPathAsync(thumbFileName);
        if (!await GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
        {
            //the named mutex helps to avoid creating the same files in different threads,
            //and does not decrease performance significantly, because the code is blocked only for the specific file.
            //you should be very careful, mutexes cannot be used in with the await operation
            //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
            using var mutex = new Mutex(false, thumbFileName);
            mutex.WaitOne();
            try
            {
                using var image = SKBitmap.Decode(filePath);
                var codec = SKCodec.Create(filePath);
                var format = codec.EncodedFormat;
                var pictureBinary = ImageResize(image, format, targetSize);
                var mimeType = GetMimeTypeFromFileName(thumbFileName);
                SaveThumbAsync(thumbFilePath, thumbFileName, mimeType, pictureBinary).Wait();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        return await GetThumbUrlAsync(thumbFileName, storeLocation);
    }

    /// <summary>
    /// Get a picture URL
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <param name="defaultPictureType">Default picture type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture URL
    /// </returns>
    public virtual async Task<string> GetPictureUrlAsync(int pictureId,
        int targetSize = 0,
        bool showDefaultPicture = true,
        string storeLocation = null,
        PictureType defaultPictureType = PictureType.Entity)
    {
        var picture = await GetPictureByIdAsync(pictureId);
        return (await GetPictureUrlAsync(picture, targetSize, showDefaultPicture, storeLocation, defaultPictureType)).Url;
    }

    /// <summary>
    /// Get a picture URL
    /// </summary>
    /// <param name="picture">Reference instance of Picture</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <param name="defaultPictureType">Default picture type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture URL
    /// </returns>
    public virtual async Task<(string Url, Picture Picture)> GetPictureUrlAsync(Picture picture,
        int targetSize = 0,
        bool showDefaultPicture = true,
        string storeLocation = null,
        PictureType defaultPictureType = PictureType.Entity)
    {
        if (picture == null)
            return showDefaultPicture ? (await GetDefaultPictureUrlAsync(targetSize, defaultPictureType, storeLocation), null) : (string.Empty, (Picture)null);

        byte[] pictureBinary = null;
        if (picture.IsNew)
        {
            await DeletePictureThumbsAsync(picture);
            pictureBinary = await LoadPictureBinaryAsync(picture);

            if ((pictureBinary?.Length ?? 0) == 0)
                return showDefaultPicture ? (await GetDefaultPictureUrlAsync(targetSize, defaultPictureType, storeLocation), picture) : (string.Empty, picture);

            //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown
            picture = await UpdatePictureAsync(picture.Id,
                pictureBinary,
                picture.MimeType,
                picture.SeoFilename,
                picture.AltAttribute,
                picture.TitleAttribute,
                false,
                false);
        }

        var seoFileName = picture.SeoFilename; // = GetPictureSeName(picture.SeoFilename); //just for sure

        var lastPart = await GetFileExtensionFromMimeTypeAsync(picture.MimeType);

        var thumbFileName = !string.IsNullOrEmpty(seoFileName)
            ? $"{picture.Id:0000000}_{seoFileName}.{lastPart}"
            : $"{picture.Id:0000000}.{lastPart}";

        //there is no need to resize the svg image as the browser will take care of it
        if (targetSize == 0 || picture.MimeType == MimeTypes.ImageSvg)
        {
            var thumbFilePath = await GetThumbLocalPathAsync(thumbFileName);
            if (await GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
                return (await GetThumbUrlAsync(thumbFileName, storeLocation), picture);

            pictureBinary ??= await LoadPictureBinaryAsync(picture);

            //the named mutex helps to avoid creating the same files in different threads,
            //and does not decrease performance significantly, because the code is blocked only for the specific file.
            //you should be very careful, mutexes cannot be used in with the await operation
            //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
            using var mutex = new Mutex(false, thumbFileName);
            mutex.WaitOne();
            try
            {
                SaveThumbAsync(thumbFilePath, thumbFileName, picture.MimeType, pictureBinary).Wait();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
        else
        {
            thumbFileName = !string.IsNullOrEmpty(seoFileName)
                ? $"{picture.Id:0000000}_{seoFileName}_{targetSize}.{lastPart}"
                : $"{picture.Id:0000000}_{targetSize}.{lastPart}";

            var thumbFilePath = await GetThumbLocalPathAsync(thumbFileName);
            if (await GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
                return (await GetThumbUrlAsync(thumbFileName, storeLocation), picture);

            pictureBinary ??= await LoadPictureBinaryAsync(picture);

            //the named mutex helps to avoid creating the same files in different threads,
            //and does not decrease performance significantly, because the code is blocked only for the specific file.
            //you should be very careful, mutexes cannot be used in with the await operation
            //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
            using var mutex = new Mutex(false, thumbFileName);
            mutex.WaitOne();
            try
            {
                if (pictureBinary != null)
                    try
                    {
                        using var image = SKBitmap.Decode(pictureBinary);
                        var format = GetImageFormatByMimeType(picture.MimeType);
                        pictureBinary = ImageResize(image, format, targetSize);
                        SaveThumbAsync(thumbFilePath, thumbFileName, picture.MimeType, pictureBinary).Wait();
                    }
                    catch
                    {
                        // ignored
                    }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        return (await GetThumbUrlAsync(thumbFileName, storeLocation), picture);
    }

    /// <summary>
    /// Get a picture local path
    /// </summary>
    /// <param name="picture">Picture instance</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    public virtual async Task<string> GetThumbLocalPathAsync(Picture picture, int targetSize = 0, bool showDefaultPicture = true)
    {
        var (url, _) = await GetPictureUrlAsync(picture, targetSize, showDefaultPicture);
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        return await GetThumbLocalPathAsync(_fileProvider.GetFileName(url));
    }

    #endregion

    #region Convertation methods

    /// <summary>
    /// Convert image from SVG format to PNG
    /// </summary>
    /// <param name="stream">Stream for SVG file</param>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains the byte array</returns>
    public virtual Task<byte[]> ConvertSvgToPngAsync(Stream stream)
    {
        try
        {
            using var svg = new SKSvg();
            svg.Load(stream);

            using var bitmap = new SKBitmap((int)svg.Picture.CullRect.Width, (int)svg.Picture.CullRect.Height);
            var canvas = new SKCanvas(bitmap);
            canvas.DrawPicture(svg.Picture);
            canvas.Flush();
            canvas.Save();

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            // save the data to a stream
            using var memStream = new MemoryStream();
            data.SaveTo(memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            return Task.FromResult(memStream.ToArray());
        }
        catch
        {
        }

        return null;
    }

    #endregion

    #region CRUD methods

    /// <summary>
    /// Gets a picture
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> GetPictureByIdAsync(int pictureId)
    {
        return await _pictureRepository.GetByIdAsync(pictureId, cache => default);
    }

    /// <summary>
    /// Deletes a picture
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePictureAsync(Picture picture)
    {
        ArgumentNullException.ThrowIfNull(picture);

        //delete thumbs
        await DeletePictureThumbsAsync(picture);

        //delete from file system
        if (!await IsStoreInDbAsync())
            await DeletePictureOnFileSystemAsync(picture);

        //delete from database
        await _pictureRepository.DeleteAsync(picture);
    }

    /// <summary>
    /// Gets a collection of pictures
    /// </summary>
    /// <param name="virtualPath">Virtual path</param>
    /// <param name="pageIndex">Current page</param>
    /// <param name="pageSize">Items on each page</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of pictures
    /// </returns>
    public virtual async Task<IPagedList<Picture>> GetPicturesAsync(string virtualPath = "", int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _pictureRepository.Table;

        if (!string.IsNullOrEmpty(virtualPath))
            query = virtualPath.EndsWith('/') ? query.Where(p => p.VirtualPath.StartsWith(virtualPath) || p.VirtualPath == virtualPath.TrimEnd('/')) : query.Where(p => p.VirtualPath == virtualPath);

        query = query.OrderByDescending(p => p.Id);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Gets pictures by product identifier
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pictures
    /// </returns>
    public virtual async Task<IList<Picture>> GetPicturesByProductIdAsync(int productId, int recordsToReturn = 0)
    {
        if (productId == 0)
            return new List<Picture>();

        var query = from p in _pictureRepository.Table
            join pp in _productPictureRepository.Table on p.Id equals pp.PictureId
            orderby pp.DisplayOrder, pp.Id
            where pp.ProductId == productId
            select p;

        if (recordsToReturn > 0)
            query = query.Take(recordsToReturn);

        var pics = await query.ToListAsync();

        return pics;
    }

    /// <summary>
    /// Inserts a picture
    /// </summary>
    /// <param name="pictureBinary">The picture binary</param>
    /// <param name="mimeType">The picture MIME type</param>
    /// <param name="seoFilename">The SEO filename</param>
    /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
    /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
    /// <param name="isNew">A value indicating whether the picture is new</param>
    /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> InsertPictureAsync(byte[] pictureBinary, string mimeType, string seoFilename,
        string altAttribute = null, string titleAttribute = null,
        bool isNew = true, bool validateBinary = true)
    {
        mimeType = CommonHelper.EnsureNotNull(mimeType);
        mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

        seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

        if (validateBinary)
            pictureBinary = await ValidatePictureAsync(pictureBinary, mimeType, seoFilename);

        var picture = new Picture
        {
            MimeType = mimeType,
            SeoFilename = seoFilename,
            AltAttribute = altAttribute,
            TitleAttribute = titleAttribute,
            IsNew = isNew
        };
        await _pictureRepository.InsertAsync(picture);
        await UpdatePictureBinaryAsync(picture, await IsStoreInDbAsync() ? pictureBinary : Array.Empty<byte>());

        if (!await IsStoreInDbAsync())
            await SavePictureInFileAsync(picture.Id, pictureBinary, mimeType);

        return picture;
    }

    /// <summary>
    /// Inserts a picture
    /// </summary>
    /// <param name="formFile">Form file</param>
    /// <param name="defaultFileName">File name which will be use if IFormFile.FileName not present</param>
    /// <param name="virtualPath">Virtual path</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> InsertPictureAsync(IFormFile formFile, string defaultFileName = "", string virtualPath = "")
    {
        var imgExt = new List<string>
        {
            ".bmp",
            ".gif",
            ".webp",
            ".jpeg",
            ".jpg",
            ".jpe",
            ".jfif",
            ".pjpeg",
            ".pjp",
            ".png",
            ".tiff",
            ".tif",
            ".svg"
        } as IReadOnlyCollection<string>;

        var fileName = formFile.FileName;
        if (string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(defaultFileName))
            fileName = defaultFileName;

        //remove path (passed in IE)
        fileName = _fileProvider.GetFileName(fileName);

        var contentType = formFile.ContentType;

        var fileExtension = _fileProvider.GetFileExtension(fileName);
        if (!string.IsNullOrEmpty(fileExtension))
            fileExtension = fileExtension.ToLowerInvariant();

        if (imgExt.All(ext => !ext.Equals(fileExtension, StringComparison.CurrentCultureIgnoreCase)))
            return null;

        //contentType is not always available 
        //that's why we manually update it here
        //https://mimetype.io/all-types/
        if (string.IsNullOrEmpty(contentType))
            contentType = GetPictureContentTypeByFileExtension(fileExtension);

        if (contentType == MimeTypes.ImageSvg && !_mediaSettings.AllowSVGUploads)
            return null;

        var picture = await InsertPictureAsync(await _downloadService.GetDownloadBitsAsync(formFile),
            contentType,
            _fileProvider.GetFileNameWithoutExtension(fileName),
            validateBinary: contentType != MimeTypes.ImageSvg);

        if (string.IsNullOrEmpty(virtualPath))
            return picture;

        picture.VirtualPath = _fileProvider.GetVirtualPath(virtualPath);
        await UpdatePictureAsync(picture);

        return picture;
    }

    /// <summary>
    /// Updates the picture
    /// </summary>
    /// <param name="pictureId">The picture identifier</param>
    /// <param name="pictureBinary">The picture binary</param>
    /// <param name="mimeType">The picture MIME type</param>
    /// <param name="seoFilename">The SEO filename</param>
    /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
    /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
    /// <param name="isNew">A value indicating whether the picture is new</param>
    /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> UpdatePictureAsync(int pictureId, byte[] pictureBinary, string mimeType,
        string seoFilename, string altAttribute = null, string titleAttribute = null,
        bool isNew = true, bool validateBinary = true)
    {
        mimeType = CommonHelper.EnsureNotNull(mimeType);
        mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

        seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

        if (validateBinary)
            pictureBinary = await ValidatePictureAsync(pictureBinary, mimeType, seoFilename);

        var picture = await GetPictureByIdAsync(pictureId);
        if (picture == null)
            return null;

        //delete old thumbs if a picture has been changed
        if (seoFilename != picture.SeoFilename)
            await DeletePictureThumbsAsync(picture);

        picture.MimeType = mimeType;
        picture.SeoFilename = seoFilename;
        picture.AltAttribute = altAttribute;
        picture.TitleAttribute = titleAttribute;
        picture.IsNew = isNew;

        await _pictureRepository.UpdateAsync(picture);
        await UpdatePictureBinaryAsync(picture, await IsStoreInDbAsync() ? pictureBinary : Array.Empty<byte>());

        if (!await IsStoreInDbAsync())
            await SavePictureInFileAsync(picture.Id, pictureBinary, mimeType);

        return picture;
    }

    /// <summary>
    /// Updates the picture
    /// </summary>
    /// <param name="picture">The picture to update</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> UpdatePictureAsync(Picture picture)
    {
        if (picture == null)
            return null;

        var seoFilename = CommonHelper.EnsureMaximumLength(picture.SeoFilename, 100);

        //delete old thumbs if exists
        await DeletePictureThumbsAsync(picture);

        picture.SeoFilename = seoFilename;

        await _pictureRepository.UpdateAsync(picture);
        await UpdatePictureBinaryAsync(picture, await IsStoreInDbAsync() ? (await GetPictureBinaryByPictureIdAsync(picture.Id)).BinaryData : Array.Empty<byte>());

        if (!await IsStoreInDbAsync())
            await SavePictureInFileAsync(picture.Id, (await GetPictureBinaryByPictureIdAsync(picture.Id)).BinaryData, picture.MimeType);

        return picture;
    }

    /// <summary>
    /// Get product picture binary by picture identifier
    /// </summary>
    /// <param name="pictureId">The picture identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    public virtual async Task<PictureBinary> GetPictureBinaryByPictureIdAsync(int pictureId)
    {
        return await _pictureBinaryRepository.Table
            .FirstOrDefaultAsync(pb => pb.PictureId == pictureId);
    }

    /// <summary>
    /// Updates a SEO filename of a picture
    /// </summary>
    /// <param name="pictureId">The picture identifier</param>
    /// <param name="seoFilename">The SEO filename</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> SetSeoFilenameAsync(int pictureId, string seoFilename)
    {
        var picture = await GetPictureByIdAsync(pictureId) ?? throw new ArgumentException("No picture found with the specified id");

        //update if it has been changed
        if (seoFilename != picture.SeoFilename)
        {
            //update picture
            picture = await UpdatePictureAsync(picture.Id,
                await LoadPictureBinaryAsync(picture),
                picture.MimeType,
                seoFilename,
                picture.AltAttribute,
                picture.TitleAttribute,
                true,
                false);
        }

        return picture;
    }

    /// <summary>
    /// Validates input picture dimensions
    /// </summary>
    /// <param name="pictureBinary">Picture binary</param>
    /// <param name="mimeType">MIME type</param>
    /// <param name="fileName">Name of file</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary or throws an exception
    /// </returns>
    public virtual async Task<byte[]> ValidatePictureAsync(byte[] pictureBinary, string mimeType, string fileName)
    {
        try
        {
            using var image = SKBitmap.Decode(pictureBinary);

            //resize the image in accordance with the maximum size
            if (Math.Max(image.Height, image.Width) > _mediaSettings.MaximumImageSize)
            {
                var format = GetImageFormatByMimeType(mimeType);
                pictureBinary = ImageResize(image, format, _mediaSettings.MaximumImageSize);
            }
            return pictureBinary;
        }
        catch (Exception exc)
        {
            await _logger.ErrorAsync($"Cannot decode picture binary (file name: {fileName})", exc);
            return pictureBinary;
        }
    }

    /// <summary>
    /// Get product picture (for shopping cart and order details pages)
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes (in XML format)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public virtual async Task<Picture> GetProductPictureAsync(Product product, string attributesXml)
    {
        ArgumentNullException.ThrowIfNull(product);

        //first, try to get product attribute combination picture
        var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
        if (combination != null)
        {
            var combinationPicture = (await _productAttributeService.GetProductAttributeCombinationPicturesAsync(combination.Id)).FirstOrDefault();
            if (await GetPictureByIdAsync(combinationPicture?.PictureId ?? 0) is Picture picture)
                return picture;
        }

        //then, let's see whether we have attribute values with pictures
        var values = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
        foreach (var attributeValue in values)
        {
            var valuePictures = await _productAttributeService.GetProductAttributeValuePicturesAsync(attributeValue.Id);
            var attributePicture = (await GetPicturesByIdsAsync(valuePictures.Select(vp => vp.PictureId).ToArray())).FirstOrDefault();

            if (attributePicture != null)
                return attributePicture;
        }

        //now let's load the default product picture
        var productPicture = (await GetPicturesByProductIdAsync(product.Id, 1)).FirstOrDefault();
        if (productPicture != null)
            return productPicture;

        //finally, let's check whether this product has some parent "grouped" product
        if (product.VisibleIndividually || product.ParentGroupedProductId <= 0)
            return null;

        var parentGroupedProductPicture = (await GetPicturesByProductIdAsync(product.ParentGroupedProductId, 1)).FirstOrDefault();
        return parentGroupedProductPicture;
    }

    /// <summary>
    /// Gets a value indicating whether the images should be stored in data base.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<bool> IsStoreInDbAsync()
    {
        return await _settingService.GetSettingByKeyAsync("Media.Images.StoreInDB", true);
    }

    /// <summary>
    /// Sets a value indicating whether the images should be stored in data base
    /// </summary>
    /// <param name="isStoreInDb">A value indicating whether the images should be stored in data base</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetIsStoreInDbAsync(bool isStoreInDb)
    {
        //check whether it's a new value
        if (await IsStoreInDbAsync() == isStoreInDb)
            return;

        //save the new setting value
        await _settingService.SetSettingAsync("Media.Images.StoreInDB", isStoreInDb);

        var pageIndex = 0;
        const int pageSize = 400;
        try
        {
            while (true)
            {
                var pictures = await GetPicturesAsync(pageIndex: pageIndex, pageSize: pageSize);
                pageIndex++;

                //all pictures converted?
                if (!pictures.Any())
                    break;

                foreach (var picture in pictures)
                {
                    if (!string.IsNullOrEmpty(picture.VirtualPath))
                        continue;

                    var pictureBinary = await LoadPictureBinaryAsync(picture, !isStoreInDb);

                    //we used the code below before. but it's too slow
                    //let's do it manually (uncommented code) - copy some logic from "UpdatePicture" method
                    /*just update a picture (all required logic is in "UpdatePicture" method)
                    we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown when "moving" pictures
                    UpdatePicture(picture.Id,
                                  pictureBinary,
                                  picture.MimeType,
                                  picture.SeoFilename,
                                  true,
                                  false);*/
                    if (isStoreInDb)
                        //delete from file system. now it's in the database
                        await DeletePictureOnFileSystemAsync(picture);
                    else
                        //now on file system
                        await SavePictureInFileAsync(picture.Id, pictureBinary, picture.MimeType);
                    //update appropriate properties
                    await UpdatePictureBinaryAsync(picture, isStoreInDb ? pictureBinary : Array.Empty<byte>());
                    picture.IsNew = true;
                }

                //save all at once
                await _pictureRepository.UpdateAsync(pictures, false);
            }
        }
        catch
        {
            // ignored
        }
    }

    #endregion

    #region Common methods

    /// <summary>
    /// Get content type for picture by file extension
    /// </summary>
    /// <param name="fileExtension">The file extension</param>
    /// <returns>Picture's content type</returns>
    public string GetPictureContentTypeByFileExtension(string fileExtension)
    {
        string contentType = null;

        switch (fileExtension.ToLower())
        {
            case ".bmp":
                contentType = MimeTypes.ImageBmp;
                break;
            case ".gif":
                contentType = MimeTypes.ImageGif;
                break;
            case ".jpeg":
            case ".jpg":
            case ".jpe":
            case ".jfif":
            case ".pjpeg":
            case ".pjp":
                contentType = MimeTypes.ImageJpeg;
                break;
            case ".webp":
                contentType = MimeTypes.ImageWebp;
                break;
            case ".png":
                contentType = MimeTypes.ImagePng;
                break;
            case ".svg":
                contentType = MimeTypes.ImageSvg;
                break;
            case ".tiff":
            case ".tif":
                contentType = MimeTypes.ImageTiff;
                break;
            default:
                break;
        }

        return contentType;
    }

    #endregion
}