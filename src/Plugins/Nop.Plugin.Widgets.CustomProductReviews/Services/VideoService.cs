using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Widgets.CustomCustomProductReviews;
using Nop.Plugin.Widgets.CustomProductReviews.Domains;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Services.Seo;
using SkiaSharp;

namespace Nop.Plugin.Widgets.CustomProductReviews.Services
{
    /// <summary>
    /// Video service
    /// </summary>
    public partial class VideoService : IVideoService
    {
        #region Fields

        private readonly IDownloadService _downloadService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INopFileProvider _fileProvider;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IRepository<Video> _videoRepository;
        private readonly IRepository<VideoBinary> _videoBinaryRepository;
        //private readonly IRepository<ProductVideo> _productVideoRepository;
        private readonly ISettingService _settingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public VideoService(IDownloadService downloadService,
            IHttpContextAccessor httpContextAccessor,
            INopFileProvider fileProvider,
            IProductAttributeParser productAttributeParser,
            IRepository<Video> videoRepository,
            IRepository<VideoBinary> videoBinaryRepository,
            //IRepository<ProductVideo> productVideoRepository,
            ISettingService settingService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            MediaSettings mediaSettings)
        {
            _downloadService = downloadService;
            _httpContextAccessor = httpContextAccessor;
            _fileProvider = fileProvider;
            _productAttributeParser = productAttributeParser;
            _videoRepository = videoRepository;
            _videoBinaryRepository = videoBinaryRepository;
            //_productVideoRepository = productVideoRepository;
            _settingService = settingService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Loads a video from file
        /// </summary>
        /// <param name="videoId">Video identifier</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video binary
        /// </returns>
        protected virtual async Task<byte[]> LoadVideoFromFileAsync(int videoId, string mimeType)
        {
            var lastPart = await GetFileExtensionFromMimeTypeAsync(mimeType);
            var fileName = $"{videoId:0000000}_0.{lastPart}";
            var filePath = await GetVideoLocalPathAsync(fileName);

            return await _fileProvider.ReadAllBytesAsync(filePath);
        }

        /// <summary>
        /// Save video on file system
        /// </summary>
        /// <param name="videoId">Video identifier</param>
        /// <param name="videoBinary">Video binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SaveVideoInFileAsync(int videoId, byte[] videoBinary, string mimeType)
        {
            var lastPart = await GetFileExtensionFromMimeTypeAsync(mimeType);
            var fileName = $"{videoId:0000000}_0.{lastPart}";
            await _fileProvider.WriteAllBytesAsync(await GetVideoLocalPathAsync(fileName), videoBinary);
        }

        /// <summary>
        /// Delete a video on file system
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task DeleteVideoOnFileSystemAsync(Video video)
        {
            if (video == null)
                throw new ArgumentNullException(nameof(video));

            var lastPart = await GetFileExtensionFromMimeTypeAsync(video.MimeType);
            var fileName = $"{video.Id:0000000}_0.{lastPart}";
            var filePath = await GetVideoLocalPathAsync(fileName);
            _fileProvider.DeleteFile(filePath);
        }

        /// <summary>
        /// Delete video thumbs
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task DeleteVideoThumbsAsync(Video video)
        {
            var filter = $"{video.Id:0000000}*.*";
            var currentFiles = _fileProvider.GetFiles(_fileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath), filter, false);
            foreach (var currentFileName in currentFiles)
            {
                var thumbFilePath = await GetThumbLocalPathAsync(currentFileName);
                _fileProvider.DeleteFile(thumbFilePath);
            }
        }

        /// <summary>
        /// Get video (thumb) local path
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the local video thumb path
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
        /// Get video (thumb) URL 
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the local video thumb path
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
        /// Get video local path. Used when images stored on file system (not in the database)
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the local video path
        /// </returns>
        protected virtual Task<string> GetVideoLocalPathAsync(string fileName)
        {
            return Task.FromResult(_fileProvider.GetAbsolutePath("videos", fileName));
        }

        /// <summary>
        /// Gets the loaded video binary depending on video storage settings
        /// </summary>
        /// <param name="video">Video</param>
        /// <param name="fromDb">Load from database; otherwise, from file system</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video binary
        /// </returns>
        protected virtual async Task<byte[]> LoadVideoBinaryAsync(Video video, bool fromDb)
        {
            if (video == null)
                throw new ArgumentNullException(nameof(video));

            var result = fromDb
                ? (await GetVideoBinaryByVideoIdAsync(video.Id))?.BinaryData ?? Array.Empty<byte>()
                : await LoadVideoFromFileAsync(video.Id, video.MimeType);

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
        /// <param name="binary">Video binary</param>
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
        /// Updates the video binary data
        /// </summary>
        /// <param name="video"></param>
        /// <param name="binaryData">The video binary data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video binary
        /// </returns>
        protected virtual async Task<VideoBinary> UpdateVideoBinaryAsync(Video video, byte[] binaryData)
        {
            if (video == null)
                throw new ArgumentNullException(nameof(video));

            VideoBinary videoBinary = await GetVideoBinaryByVideoIdAsync(video.Id);

            var isNew = videoBinary == null;

            if (isNew)
                videoBinary = new VideoBinary
                {
                    VideoId = video.Id
                };

            videoBinary.BinaryData = binaryData;

            if (isNew)
                await _videoBinaryRepository.InsertAsync(videoBinary);
            else
                await _videoBinaryRepository.UpdateAsync(videoBinary);

            return videoBinary;
        }

        /// <summary>
        /// Get image format by mime type
        /// </summary>
        /// <param name="mimetype">Mime type</param>
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

                //In order to exclude saving pictures in low quality at the time of installation, we will set the value of this parameter to 80 (as by default)
                return cropImage.Encode(format, _mediaSettings.DefaultImageQuality > 0 ? _mediaSettings.DefaultImageQuality : 80).ToArray();
            }
            catch
            {
                return image.Bytes;
            }

        }

        #endregion

        #region Getting video local path/URL methods

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
            switch (lastPart)
            {
                case "pjpeg":
                    lastPart = "jpg";
                    break;
                case "x-png":
                    lastPart = "png";
                    break;
                case "x-icon":
                    lastPart = "ico";
                    break;
                default:
                    break;
            }

            return Task.FromResult(lastPart);
        }

        /// <summary>
        /// Gets the loaded video binary depending on video storage settings
        /// </summary>
        /// <param name="video"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video binary
        /// </returns>
        public virtual async Task<byte[]> LoadVideoBinaryAsync(Video video)
        {
            return await LoadVideoBinaryAsync(video, await IsStoreInDbAsync());
        }

        /// <summary>
        /// Get video SEO friendly name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<string> GetVideoSeNameAsync(string name)
        {
            return await _urlRecordService.GetSeNameAsync(name, true, false);
        }

        /// <summary>
        /// Gets the default video URL
        /// </summary>
        /// <param name="targetSize">The target video size (longest side)</param>
        /// <param name="defaultPictureType">Default video type</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video URL
        /// </returns>
        public virtual async Task<string> GetDefaultVideoUrlAsync(int targetSize = 0,
            PictureType defaultPictureType = PictureType.Entity,
            string storeLocation = null)
        {
            var defaultImageFileName = defaultPictureType switch
            {
                PictureType.Avatar => await _settingService.GetSettingByKeyAsync("Media.Customer.DefaultAvatarImageName", NopMediaDefaults.DefaultAvatarFileName),
                _ => await _settingService.GetSettingByKeyAsync("Media.DefaultImageName", NopMediaDefaults.DefaultImageFileName),
            };
            var filePath = await GetVideoLocalPathAsync(defaultImageFileName);
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
                    var videoBinary = ImageResize(image, format, targetSize);
                    var mimeType = GetMimeTypeFromFileName(thumbFileName);
                    SaveThumbAsync(thumbFilePath, thumbFileName, mimeType, videoBinary).Wait();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }

            return await GetThumbUrlAsync(thumbFileName, storeLocation);
        }

        /// <summary>
        /// Get a video URL
        /// </summary>
        /// <param name="videoId">Video identifier</param>
        /// <param name="targetSize">The target video size (longest side)</param>
        /// <param name="showDefaultVideo">A value indicating whether the default video is shown</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <param name="defaultPictureType">Default video type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video URL
        /// </returns>
        public virtual async Task<string> GetVideoUrlAsync(int videoId,
            int targetSize = 0,
            bool showDefaultVideo = true,
            string storeLocation = null,
            PictureType defaultPictureType = PictureType.Entity)
        {
            var video = await GetVideoByIdAsync(videoId);
            return (await GetVideoUrlAsync(video, targetSize, showDefaultVideo, storeLocation, defaultPictureType)).Url;
        }

        /// <summary>
        /// Get a video URL
        /// </summary>
        /// <param name="video">Reference instance of Video</param>
        /// <param name="targetSize">The target video size (longest side)</param>
        /// <param name="showDefaultVideo">A value indicating whether the default video is shown</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <param name="defaultPictureType">Default video type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video URL
        /// </returns>
        public virtual async Task<(string Url, Video Video)> GetVideoUrlAsync(Video video,
            int targetSize = 0,
            bool showDefaultVideo = true,
            string storeLocation = null,
            PictureType defaultPictureType = PictureType.Entity)
        {
            if (video == null)
                return showDefaultVideo ? (await GetDefaultVideoUrlAsync(targetSize, defaultPictureType, storeLocation), null) : (string.Empty, (Video)null);

            byte[] videoBinary = null;
            if (video.IsNew)
            {
                await DeleteVideoThumbsAsync(video);
                videoBinary = await LoadVideoBinaryAsync(video);

                if ((videoBinary?.Length ?? 0) == 0)
                    return showDefaultVideo ? (await GetDefaultVideoUrlAsync(targetSize, defaultPictureType, storeLocation), video) : (string.Empty, video);

                //we do not validate video binary here to ensure that no exception ("Parameter is not valid") will be thrown
                video = await UpdateVideoAsync(video.Id,
                    videoBinary,
                    video.MimeType,
                    video.SeoFilename,
                    video.AltAttribute,
                    video.TitleAttribute,
                    false,
                    false);
            }

            var seoFileName = video.SeoFilename; // = GetVideoSeName(video.SeoFilename); //just for sure

            var lastPart = await GetFileExtensionFromMimeTypeAsync(video.MimeType);
            string thumbFileName;
            if (targetSize == 0)
            {
                thumbFileName = !string.IsNullOrEmpty(seoFileName)
                    ? $"{video.Id:0000000}_{seoFileName}.{lastPart}"
                    : $"{video.Id:0000000}.{lastPart}";

                var thumbFilePath = await GetThumbLocalPathAsync(thumbFileName);
                if (await GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
                    return (await GetThumbUrlAsync(thumbFileName, storeLocation), video);

                videoBinary ??= await LoadVideoBinaryAsync(video);

                //the named mutex helps to avoid creating the same files in different threads,
                //and does not decrease performance significantly, because the code is blocked only for the specific file.
                //you should be very careful, mutexes cannot be used in with the await operation
                //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
                using var mutex = new Mutex(false, thumbFileName);
                mutex.WaitOne();
                try
                {
                    SaveThumbAsync(thumbFilePath, thumbFileName, video.MimeType, videoBinary).Wait();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                thumbFileName = !string.IsNullOrEmpty(seoFileName)
                    ? $"{video.Id:0000000}_{seoFileName}_{targetSize}.{lastPart}"
                    : $"{video.Id:0000000}_{targetSize}.{lastPart}";

                var thumbFilePath = await GetThumbLocalPathAsync(thumbFileName);
                if (await GeneratedThumbExistsAsync(thumbFilePath, thumbFileName))
                    return (await GetThumbUrlAsync(thumbFileName, storeLocation), video);

                videoBinary ??= await LoadVideoBinaryAsync(video);

                //the named mutex helps to avoid creating the same files in different threads,
                //and does not decrease performance significantly, because the code is blocked only for the specific file.
                //you should be very careful, mutexes cannot be used in with the await operation
                //we can't use semaphore here, because it produces PlatformNotSupportedException exception on UNIX based systems
                using var mutex = new Mutex(false, thumbFileName);
                mutex.WaitOne();
                try
                {
                    if (videoBinary != null)
                    {
                        try
                        {
                            using var image = SKBitmap.Decode(videoBinary);
                            var format = GetImageFormatByMimeType(video.MimeType);
                            videoBinary = ImageResize(image, format, targetSize);
                        }
                        catch
                        {
                        }
                    }

                    SaveThumbAsync(thumbFilePath, thumbFileName, video.MimeType, videoBinary).Wait();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }

            return (await GetThumbUrlAsync(thumbFileName, storeLocation), video);
        }

        /// <summary>
        /// Get a video local path
        /// </summary>
        /// <param name="video">Video instance</param>
        /// <param name="targetSize">The target video size (longest side)</param>
        /// <param name="showDefaultVideo">A value indicating whether the default video is shown</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        public virtual async Task<string> GetThumbLocalPathAsync(Video video, int targetSize = 0, bool showDefaultVideo = true)
        {
            var (url, _) = await GetVideoUrlAsync(video, targetSize, showDefaultVideo);
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            return await GetThumbLocalPathAsync(_fileProvider.GetFileName(url));
        }

        #endregion

        #region CRUD methods

        /// <summary>
        /// Gets a video
        /// </summary>
        /// <param name="videoId">Video identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> GetVideoByIdAsync(int videoId)
        {
            return await _videoRepository.GetByIdAsync(videoId, cache => default);
        }

        /// <summary>
        /// Deletes a video
        /// </summary>
        /// <param name="video">Video</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteVideoAsync(Video video)
        {
            if (video == null)
                throw new ArgumentNullException(nameof(video));

            //delete thumbs
            await DeleteVideoThumbsAsync(video);

            //delete from file system
            if (!await IsStoreInDbAsync())
                await DeleteVideoOnFileSystemAsync(video);

            //delete from database
            await _videoRepository.DeleteAsync(video);
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
        public virtual async Task<IPagedList<Video>> GetVideosAsync(string virtualPath = "", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _videoRepository.Table;

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
        //public virtual async Task<IList<Video>> GetVideosByProductIdAsync(int productId, int recordsToReturn = 0)
        //{
        //    if (productId == 0)
        //        return new List<Video>();

        //    var query = from p in _videoRepository.Table
        //                join pp in _productVideoRepository.Table on p.Id equals pp.videoId
        //                orderby pp.DisplayOrder, pp.Id
        //                where pp.ProductId == productId
        //                select p;

        //    if (recordsToReturn > 0)
        //        query = query.Take(recordsToReturn);

        //    var pics = await query.ToListAsync();

        //    return pics;
        //}

        /// <summary>
        /// Inserts a video
        /// </summary>
        /// <param name="videoBinary">The video binary</param>
        /// <param name="mimeType">The video MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
        /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
        /// <param name="isNew">A value indicating whether the video is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided video binary</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> InsertVideoAsync(byte[] videoBinary, string mimeType, string seoFilename,
            string altAttribute = null, string titleAttribute = null,
            bool isNew = true, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

            if (validateBinary)
                videoBinary = await ValidateVideoAsync(videoBinary, mimeType);

            var video = new Video
            {
                MimeType = mimeType,
                SeoFilename = seoFilename,
                AltAttribute = altAttribute,
                TitleAttribute = titleAttribute,
                IsNew = isNew
            };
            await _videoRepository.InsertAsync(video);
            await UpdateVideoBinaryAsync(video, await IsStoreInDbAsync() ? videoBinary : Array.Empty<byte>());

            if (!await IsStoreInDbAsync())
                await SaveVideoInFileAsync(video.Id, videoBinary, mimeType);

            return video;
        }

        /// <summary>
        /// Inserts a video
        /// </summary>
        /// <param name="formFile">Form file</param>
        /// <param name="defaultFileName">File name which will be use if IFormFile.FileName not present</param>
        /// <param name="virtualPath">Virtual path</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> InsertVideoAsync(IFormFile formFile, string defaultFileName = "", string virtualPath = "")
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
                ".tif"
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
            //http://www.sfsu.edu/training/mimetype.htm
            if (string.IsNullOrEmpty(contentType))
            {
                switch (fileExtension)
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
                    case ".tiff":
                    case ".tif":
                        contentType = MimeTypes.ImageTiff;
                        break;
                    default:
                        break;
                }
            }

            var video = await InsertVideoAsync(await _downloadService.GetDownloadBitsAsync(formFile), contentType, _fileProvider.GetFileNameWithoutExtension(fileName));

            if (string.IsNullOrEmpty(virtualPath))
                return video;

            video.VirtualPath = _fileProvider.GetVirtualPath(virtualPath);
            await UpdateVideoAsync(video);

            return video;
        }

        /// <summary>
        /// Updates the video
        /// </summary>
        /// <param name="videoId">The video identifier</param>
        /// <param name="videoBinary">The video binary</param>
        /// <param name="mimeType">The video MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
        /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
        /// <param name="isNew">A value indicating whether the video is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided video binary</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> UpdateVideoAsync(int videoId, byte[] videoBinary, string mimeType,
            string seoFilename, string altAttribute = null, string titleAttribute = null,
            bool isNew = true, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

            if (validateBinary)
                videoBinary = await ValidateVideoAsync(videoBinary, mimeType);

            var video = await GetVideoByIdAsync(videoId);
            if (video == null)
                return null;

            //delete old thumbs if a video has been changed
            if (seoFilename != video.SeoFilename)
                await DeleteVideoThumbsAsync(video);

            video.MimeType = mimeType;
            video.SeoFilename = seoFilename;
            video.AltAttribute = altAttribute;
            video.TitleAttribute = titleAttribute;
            video.IsNew = isNew;

            await _videoRepository.UpdateAsync(video);
            await UpdateVideoBinaryAsync(video, await IsStoreInDbAsync() ? videoBinary : Array.Empty<byte>());

            if (!await IsStoreInDbAsync())
                await SaveVideoInFileAsync(video.Id, videoBinary, mimeType);

            return video;
        }

        /// <summary>
        /// Updates the video
        /// </summary>
        /// <param name="video">The video to update</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> UpdateVideoAsync(Video video)
        {
            if (video == null)
                return null;

            var seoFilename = CommonHelper.EnsureMaximumLength(video.SeoFilename, 100);

            //delete old thumbs if exists
            await DeleteVideoThumbsAsync(video);

            video.SeoFilename = seoFilename;

            await _videoRepository.UpdateAsync(video);
            await UpdateVideoBinaryAsync(video, await IsStoreInDbAsync() ? (await GetVideoBinaryByVideoIdAsync(video.Id)).BinaryData : Array.Empty<byte>());

            if (!await IsStoreInDbAsync())
                await SaveVideoInFileAsync(video.Id, (await GetVideoBinaryByVideoIdAsync(video.Id)).BinaryData, video.MimeType);

            return video;
        }

        /// <summary>
        /// Get product video binary by video identifier
        /// </summary>
        /// <param name="videoId">The video identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video binary
        /// </returns>
        public virtual async Task<VideoBinary> GetVideoBinaryByVideoIdAsync(int videoId)
        {
            return await _videoBinaryRepository.Table
                .FirstOrDefaultAsync(pb => pb.VideoId == videoId);
        }

        /// <summary>
        /// Updates a SEO filename of a video
        /// </summary>
        /// <param name="videoId">The video identifier</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        public virtual async Task<Video> SetSeoFilenameAsync(int videoId, string seoFilename)
        {
            var video = await GetVideoByIdAsync(videoId);
            if (video == null)
                throw new ArgumentException("No video found with the specified id");

            //update if it has been changed
            if (seoFilename != video.SeoFilename)
            {
                //update video
                video = await UpdateVideoAsync(video.Id,
                    await LoadVideoBinaryAsync(video),
                    video.MimeType,
                    seoFilename,
                    video.AltAttribute,
                    video.TitleAttribute,
                    true,
                    false);
            }

            return video;
        }

        /// <summary>
        /// Validates input video dimensions
        /// </summary>
        /// <param name="videoBinary">Video binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video binary or throws an exception
        /// </returns>
        public virtual Task<byte[]> ValidateVideoAsync(byte[] videoBinary, string mimeType)
        {
            try
            {
                using var image = SKBitmap.Decode(videoBinary);

                //resize the image in accordance with the maximum size
                if (Math.Max(image.Height, image.Width) > _mediaSettings.MaximumImageSize)
                {
                    var format = GetImageFormatByMimeType(mimeType);
                    videoBinary = ImageResize(image, format, _mediaSettings.MaximumImageSize);
                }
                return Task.FromResult(videoBinary);
            }
            catch
            {
                return Task.FromResult(videoBinary);
            }
        }

        /// <summary>
        /// Get product video (for shopping cart and order details pages)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes (in XML format)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the video
        /// </returns>
        //public virtual async Task<Video> GetProductVideoAsync(Product product, string attributesXml)
        //{
        //    if (product == null)
        //        throw new ArgumentNullException(nameof(product));

        //    //first, try to get product attribute combination video
        //    var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
        //    var combinationVideo = await GetVideoByIdAsync(combination?.videoId ?? 0);
        //    if (combinationVideo != null)
        //        return combinationVideo;

        //    //then, let's see whether we have attribute values with pictures
        //    var attributeVideo = await (await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml))
        //        .SelectAwait(async attributeValue => await GetVideoByIdAsync(attributeValue?.videoId ?? 0))
        //        .FirstOrDefaultAsync(video => video != null);
        //    if (attributeVideo != null)
        //        return attributeVideo;

        //    //now let's load the default product video
        //    var productVideo = (await GetVideosByProductIdAsync(product.Id, 1)).FirstOrDefault();
        //    if (productVideo != null)
        //        return productVideo;

        //    //finally, let's check whether this product has some parent "grouped" product
        //    if (product.VisibleIndividually || product.ParentGroupedProductId <= 0)
        //        return null;

        //    var parentGroupedProductVideo = (await GetVideosByProductIdAsync(product.ParentGroupedProductId, 1)).FirstOrDefault();
        //    return parentGroupedProductVideo;
        //}

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
                    var pictures = await GetVideosAsync(pageIndex: pageIndex, pageSize: pageSize);
                    pageIndex++;

                    //all pictures converted?
                    if (!pictures.Any())
                        break;

                    foreach (var video in pictures)
                    {
                        if (!string.IsNullOrEmpty(video.VirtualPath))
                            continue;

                        var videoBinary = await LoadVideoBinaryAsync(video, !isStoreInDb);

                        //we used the code below before. but it's too slow
                        //let's do it manually (uncommented code) - copy some logic from "UpdateVideo" method
                        /*just update a video (all required logic is in "UpdateVideo" method)
                        we do not validate video binary here to ensure that no exception ("Parameter is not valid") will be thrown when "moving" pictures
                        UpdateVideo(video.Id,
                                      videoBinary,
                                      video.MimeType,
                                      video.SeoFilename,
                                      true,
                                      false);*/
                        if (isStoreInDb)
                            //delete from file system. now it's in the database
                            await DeleteVideoOnFileSystemAsync(video);
                        else
                            //now on file system
                            await SaveVideoInFileAsync(video.Id, videoBinary, video.MimeType);
                        //update appropriate properties
                        await UpdateVideoBinaryAsync(video, isStoreInDb ? videoBinary : Array.Empty<byte>());
                        video.IsNew = true;
                    }

                    //save all at once
                    await _videoRepository.UpdateAsync(pictures, false);
                }
            }
            catch
            {
                // ignored
            }
        }

        #endregion
    }
}
