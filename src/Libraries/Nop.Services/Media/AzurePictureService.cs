namespace Nop.Services.Media
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;

    using Microsoft.AspNetCore.Http;

    using Nop.Core;
    using Nop.Core.Caching;
    using Nop.Core.Configuration;
    using Nop.Core.Domain.Catalog;
    using Nop.Core.Domain.Media;
    using Nop.Core.Infrastructure;

    using Nop.Data;

    using Nop.Services.Catalog;
    using Nop.Services.Configuration;
    using Nop.Services.Seo;

    /// <summary>
    /// Picture service for Windows Azure
    /// </summary>
    public partial class AzurePictureService : PictureService
    {
        private static bool _azureBlobStorageAppendContainerName;
        private static string _azureBlobStorageConnectionString;
        private static string _azureBlobStorageContainerName;
        private static string _azureBlobStorageEndPoint;
        private static bool _isInitialized;
        private readonly object _locker = new object();
        private readonly MediaSettings _mediaSettings;
        private readonly IStaticCacheManager _staticCacheManager;
        private BlobContainerClient _blobContainerClient;
        private BlobServiceClient _blobServiceClient;

        public AzurePictureService(
            NopConfig config,
            INopDataProvider dataProvider,
            IDownloadService downloadService,
            INopFileProvider fileProvider,
            IHttpContextAccessor httpContextAccessor,
            MediaSettings mediaSettings,
            IRepository<PictureBinary> pictureBinaryRepository,
            IRepository<Picture> pictureRepository,
            IProductAttributeParser productAttributeParser,
            IRepository<ProductPicture> productPictureRepository,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper)
            : base(
                  dataProvider,
                  downloadService,
                  httpContextAccessor,
                  fileProvider,
                  productAttributeParser,
                  pictureRepository,
                  pictureBinaryRepository,
                  productPictureRepository,
                  settingService,
                  urlRecordService,
                  webHelper,
                  mediaSettings)
        {
            if (config is null)
                throw new ArgumentNullException(nameof(config));
            if (dataProvider is null)
                throw new ArgumentNullException(nameof(dataProvider));
            if (downloadService is null)
                throw new ArgumentNullException(nameof(downloadService));
            if (fileProvider is null)
                throw new ArgumentNullException(nameof(fileProvider));
            if (httpContextAccessor is null)
                throw new ArgumentNullException(nameof(httpContextAccessor));
            if (mediaSettings is null)
                throw new ArgumentNullException(nameof(mediaSettings));
            if (pictureBinaryRepository is null)
                throw new ArgumentNullException(nameof(pictureBinaryRepository));
            if (pictureRepository is null)
                throw new ArgumentNullException(nameof(pictureRepository));
            if (productAttributeParser is null)
                throw new ArgumentNullException(nameof(productAttributeParser));
            if (productPictureRepository is null)
                throw new ArgumentNullException(nameof(productPictureRepository));
            if (settingService is null)
                throw new ArgumentNullException(nameof(settingService));
            if (staticCacheManager is null)
                throw new ArgumentNullException(nameof(staticCacheManager));
            if (urlRecordService is null)
                throw new ArgumentNullException(nameof(urlRecordService));
            if (webHelper is null)
                throw new ArgumentNullException(nameof(webHelper));

            _mediaSettings = mediaSettings;
            _staticCacheManager = staticCacheManager;

            OneTimeInit(config);
        }

        /// <summary>
        /// Create cloud blob container
        /// </summary>
        protected virtual async Task CreateCloudBlobContainer()
        {
            await _blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        }

        /// <summary>
        /// Delete picture thumbs
        /// </summary>
        /// <param name="picture">Picture</param>
        protected override async void DeletePictureThumbs(Picture picture)
        {
            if (picture is null)
                throw new ArgumentNullException(nameof(picture));

            // create a string containing the blob name prefix
            var prefix = $"{picture.Id:0000000}";

            var tasks = new List<Task>();
            await foreach (var blob in _blobContainerClient.GetBlobsAsync(BlobTraits.All, BlobStates.All, prefix))
            {
                tasks.Add(_blobContainerClient.DeleteBlobIfExistsAsync(blob.Name, DeleteSnapshotsOption.IncludeSnapshots));
            }

            await Task.WhenAll(tasks);

            _staticCacheManager.RemoveByPrefix(NopMediaDefaults.ThumbsExistsPrefix);
        }

        /// <summary>
        /// Get a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <returns>Result</returns>
        protected override bool GeneratedThumbExists(string thumbFilePath, string thumbFileName)
        {
            if (string.IsNullOrWhiteSpace(thumbFileName))
                return false;

            return GeneratedThumbExistsAsync(thumbFilePath, thumbFileName).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Initiates an asynchronous operation to get a value indicating whether some file (thumb)
        /// already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <returns>Result</returns>
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Ignored.")]
        protected virtual async Task<bool> GeneratedThumbExistsAsync(string thumbFilePath, string thumbFileName)
        {
            try
            {
                var key = _staticCacheManager.PrepareKeyForDefaultCache(NopMediaDefaults.ThumbExistsCacheKey, thumbFileName);

                return await _staticCacheManager.GetAsync(key, async () =>
                {
                    return await _blobContainerClient.GetBlobClient(thumbFileName).ExistsAsync();
                });
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get picture (thumb) local path
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <returns>Local picture thumb path</returns>
        protected override string GetThumbLocalPath(string thumbFileName)
        {
            if (string.IsNullOrWhiteSpace(thumbFileName))
                return string.Empty;

            var path = _azureBlobStorageAppendContainerName ? $"{_azureBlobStorageContainerName}/" : string.Empty;

            return $"{_azureBlobStorageEndPoint}/{path}{thumbFileName}";
        }

        /// <summary>
        /// Get picture (thumb) URL
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <param name="storeLocation">
        /// Store location URL; null to use determine the current store location automatically
        /// </param>
        /// <returns>Local picture thumb path</returns>
        protected override string GetThumbUrl(string thumbFileName, string storeLocation = null)
        {
            if (string.IsNullOrWhiteSpace(thumbFileName))
                return string.Empty;

            return GetThumbLocalPath(thumbFileName);
        }

        protected void OneTimeInit(NopConfig config)
        {
            if (_isInitialized)
                return;

            if (config is null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrEmpty(config.AzureBlobStorageConnectionString))
                throw new Exception("Azure connection string for BLOB is not specified");

            if (string.IsNullOrEmpty(config.AzureBlobStorageContainerName))
                throw new Exception("Azure container name for BLOB is not specified");

            if (string.IsNullOrEmpty(config.AzureBlobStorageEndPoint))
                throw new Exception("Azure end point for BLOB is not specified");

            lock (_locker)
            {
                if (_isInitialized)
                    return;

                _azureBlobStorageAppendContainerName = config.AzureBlobStorageAppendContainerName;
                _azureBlobStorageConnectionString = config.AzureBlobStorageConnectionString;
                _azureBlobStorageContainerName = config.AzureBlobStorageContainerName.Trim().ToLower();
                _azureBlobStorageEndPoint = config.AzureBlobStorageEndPoint.Trim().ToLower().TrimEnd('/');

                _blobServiceClient = new BlobServiceClient(_azureBlobStorageConnectionString);
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_azureBlobStorageContainerName);

                CreateCloudBlobContainer().GetAwaiter().GetResult(); // Run initialization synchronously.

                _isInitialized = true;
            }
        }

        /// <summary>
        /// Save a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <param name="mimeType">MIME type</param>
        /// <param name="binary">Picture binary</param>
        protected override async void SaveThumb(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
        {
            await SaveThumbAsync(thumbFilePath, thumbFileName, mimeType, binary);
        }

        /// <summary>
        /// Initiates an asynchronous operation to save a value indicating whether some file (thumb)
        /// already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <param name="mimeType">MIME type</param>
        /// <param name="binary">Picture binary</param>
        protected virtual async Task SaveThumbAsync(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
        {
            var blobClient = _blobContainerClient.GetBlobClient(thumbFileName);
            using var ms = new MemoryStream(binary);

            BlobHttpHeaders headers = null;
            if (!string.IsNullOrWhiteSpace(mimeType))
            {
                headers = new BlobHttpHeaders
                {
                    ContentType = mimeType
                };
            }

            if (!string.IsNullOrWhiteSpace(_mediaSettings.AzureCacheControlHeader))
            {
                headers ??= new BlobHttpHeaders();
                headers.CacheControl = _mediaSettings.AzureCacheControlHeader;
            }

            if (headers is null)
                await blobClient.UploadAsync(ms);
            else
                await blobClient.UploadAsync(ms, new BlobUploadOptions { HttpHeaders = headers });

            _staticCacheManager.RemoveByPrefix(NopMediaDefaults.ThumbsExistsPrefix);
        }
    }
}
