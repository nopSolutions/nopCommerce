using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;

namespace Nop.Services.Media
{
    /// <summary>
    /// Picture service for Windows Azure
    /// </summary>
    public partial class AzurePictureService : PictureService
    {
        #region Constants

        /// <summary>
        /// Key to cache whether thumb exists
        /// </summary>
        /// <remarks>
        /// {0} : thumb file name
        /// </remarks>
        private const string THUMB_EXISTS_KEY = "Nop.azure.thumb.exists-{0}";
        
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string THUMBS_PATTERN_KEY = "Nop.azure.thumb";

        #endregion

        #region Fields

        private static CloudBlobContainer _container;
        private readonly IStaticCacheManager _cacheManager;
        private readonly MediaSettings _mediaSettings;
        private readonly NopConfig _config;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pictureRepository">Picture repository</param>
        /// <param name="productPictureRepository">Product picture repository</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="logger">Logger</param>
        /// <param name="dbContext">Database context</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="mediaSettings">Media settings</param>
        /// <param name="config">Config</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="hostingEnvironment">Hosting environment</param>
        public AzurePictureService(IRepository<Picture> pictureRepository,
            IRepository<ProductPicture> productPictureRepository,
            ISettingService settingService,
            IWebHelper webHelper,
            ILogger logger,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            IStaticCacheManager cacheManager,
            MediaSettings mediaSettings,
            NopConfig config,
            IDataProvider dataProvider,
            IHostingEnvironment hostingEnvironment)
            : base(pictureRepository,
                productPictureRepository,
                settingService,
                webHelper,
                logger,
                dbContext,
                eventPublisher,
                mediaSettings,
                dataProvider,
                hostingEnvironment)
        {
            this._cacheManager = cacheManager;
            this._mediaSettings = mediaSettings;
            this._config = config;

            if (string.IsNullOrEmpty(_config.AzureBlobStorageConnectionString))
                throw new Exception("Azure connection string for BLOB is not specified");

            if (string.IsNullOrEmpty(_config.AzureBlobStorageContainerName))
                throw new Exception("Azure container name for BLOB is not specified");

            if (string.IsNullOrEmpty(_config.AzureBlobStorageEndPoint))
                throw new Exception("Azure end point for BLOB is not specified");

            CreateCloudBlobContainer();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Create cloud blob container
        /// </summary>
        protected virtual async void CreateCloudBlobContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(_config.AzureBlobStorageConnectionString);
            if (storageAccount == null)
                throw new Exception("Azure connection string for BLOB is not wrong");

            //should we do it for each HTTP request?
            var blobClient = storageAccount.CreateCloudBlobClient();

            //GetContainerReference doesn't need to be async since it doesn't contact the server yet
            _container = blobClient.GetContainerReference(_config.AzureBlobStorageContainerName);

            await _container.CreateIfNotExistsAsync();
            await _container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
        }

        /// <summary>
        /// Delete picture thumbs
        /// </summary>
        /// <param name="picture">Picture</param>
        protected override async void DeletePictureThumbs(Picture picture)
        {
            await DeletePictureThumbsAsync(picture);
        }

        /// <summary>
        /// Get picture (thumb) local path
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <returns>Local picture thumb path</returns>
        protected override string GetThumbLocalPath(string thumbFileName)
        {
            return $"{_config.AzureBlobStorageEndPoint}{_config.AzureBlobStorageContainerName}/{thumbFileName}";
        }

        /// <summary>
        /// Get picture (thumb) URL 
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <returns>Local picture thumb path</returns>
        protected override string GetThumbUrl(string thumbFileName, string storeLocation = null)
        {
            return $"{_config.AzureBlobStorageEndPoint}{_config.AzureBlobStorageContainerName}/{thumbFileName}";
        }

        /// <summary>
        /// Get a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <returns>Result</returns>
        protected override bool GeneratedThumbExists(string thumbFilePath, string thumbFileName)
        {
            return GeneratedThumbExistsAsync(thumbFilePath, thumbFileName).Result;
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
        /// Initiates an asynchronous operation to delete picture thumbs
        /// </summary>
        /// <param name="picture">Picture</param>
        protected virtual async Task DeletePictureThumbsAsync(Picture picture)
        {
            //create a string containing the blob name prefix
            var prefix = $"{picture.Id:0000000}";

            BlobContinuationToken continuationToken = null;
            do
            {
                //get result segment
                //listing snapshots is only supported in flat mode, so set the useFlatBlobListing parameter to true.
                var resultSegment = await _container.ListBlobsSegmentedAsync(prefix, true, BlobListingDetails.All, null, continuationToken, null, null);

                //delete files in result segment
                await Task.WhenAll(resultSegment.Results.Select(blobItem => ((CloudBlockBlob)blobItem).DeleteAsync()));

                //get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

            _cacheManager.RemoveByPattern(THUMBS_PATTERN_KEY);
        }

        /// <summary>
        /// Initiates an asynchronous operation to get a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <returns>Result</returns>
        protected virtual async Task<bool> GeneratedThumbExistsAsync(string thumbFilePath, string thumbFileName)
        {
            try
            {
                var key = string.Format(THUMB_EXISTS_KEY, thumbFileName);
                return await _cacheManager.Get(key, async () =>
                {
                    //GetBlockBlobReference doesn't need to be async since it doesn't contact the server yet
                    var blockBlob = _container.GetBlockBlobReference(thumbFileName);

                    return await blockBlob.ExistsAsync();
                });
            }
            catch { return false; }
        }

        /// <summary>
        /// Initiates an asynchronous operation to save a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <param name="mimeType">MIME type</param>
        /// <param name="binary">Picture binary</param>
        protected virtual async Task SaveThumbAsync(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
        {
            //GetBlockBlobReference doesn't need to be async since it doesn't contact the server yet
            var blockBlob = _container.GetBlockBlobReference(thumbFileName);

            //set mime type
            if (!string.IsNullOrEmpty(mimeType))
                blockBlob.Properties.ContentType = mimeType;

            //set cache control
            if (!string.IsNullOrEmpty(_mediaSettings.AzureCacheControlHeader))
                blockBlob.Properties.CacheControl = _mediaSettings.AzureCacheControlHeader;

            await blockBlob.UploadFromByteArrayAsync(binary, 0, binary.Length);

            _cacheManager.RemoveByPattern(THUMBS_PATTERN_KEY);
        }

        #endregion
    }
}