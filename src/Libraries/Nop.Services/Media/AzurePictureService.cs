using System;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Nop.Core;
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
        #region Fields
        
        private static CloudStorageAccount _storageAccount = null;
        private static CloudBlobClient blobClient = null;
        private static CloudBlobContainer container_thumb = null;

        private readonly NopConfig _config;
        #endregion

        #region Ctor

        public AzurePictureService(IRepository<Picture> pictureRepository,
            IRepository<ProductPicture> productPictureRepository,
            ISettingService settingService,
            IWebHelper webHelper,
            ILogger logger,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            NopConfig config)
            : base(pictureRepository,
                productPictureRepository,
                settingService,
                webHelper,
                logger,
                dbContext,
                eventPublisher,
                mediaSettings)
        {
            this._config = config;

            if (String.IsNullOrEmpty(_config.AzureBlobStorageConnectionString))
                throw new Exception("Azure connection string for BLOB is not specified");
            if (String.IsNullOrEmpty(_config.AzureBlobStorageContainerName))
                throw new Exception("Azure container name for BLOB is not specified");
            if (String.IsNullOrEmpty(_config.AzureBlobStorageEndPoint))
                throw new Exception("Azure end point for BLOB is not specified");

            _storageAccount = CloudStorageAccount.Parse(_config.AzureBlobStorageConnectionString);
            if (_storageAccount == null)
                throw new Exception("Azure connection string for BLOB is not wrong");

            //should we do it for each HTTP request?
            blobClient = _storageAccount.CreateCloudBlobClient();
            BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
            containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            //container.SetPermissions(containerPermissions);
            container_thumb = blobClient.GetContainerReference(_config.AzureBlobStorageContainerName);
            container_thumb.CreateIfNotExists();
            container_thumb.SetPermissions(containerPermissions);
        }

        #endregion

        #region Utilities
        
        /// <summary>
        /// Delete picture thumbs
        /// </summary>
        /// <param name="picture">Picture</param>
        protected override void DeletePictureThumbs(Picture picture)
        {
            string filter = string.Format("{0}", picture.Id.ToString("0000000"));
            var files = container_thumb.ListBlobs(prefix: filter, useFlatBlobListing: false);
            foreach (var ff in files)
            {
                CloudBlockBlob blockBlob = (CloudBlockBlob)ff;
                blockBlob.Delete();
            }
        }

        /// <summary>
        /// Get picture (thumb) local path
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <returns>Local picture thumb path</returns>
        protected override string GetThumbLocalPath(string thumbFileName)
        {
            var thumbFilePath = _config.AzureBlobStorageEndPoint + _config.AzureBlobStorageContainerName + "/" + thumbFileName;
            return thumbFilePath;
        }

        /// <summary>
        /// Get picture (thumb) URL 
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <returns>Local picture thumb path</returns>
        protected override string GetThumbUrl(string thumbFileName, string storeLocation = null)
        {
            var url = _config.AzureBlobStorageEndPoint + _config.AzureBlobStorageContainerName + "/";

            url = url + thumbFileName;
            return url;
        }

        /// <summary>
        /// Get a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <returns>Result</returns>
        protected override bool GeneratedThumbExists(string thumbFilePath, string thumbFileName)
        {
            try
            {
                CloudBlockBlob blockBlob = container_thumb.GetBlockBlobReference(thumbFileName);
                return blockBlob.Exists();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Save a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <param name="binary">Picture binary</param>
        protected override void SaveThumb(string thumbFilePath, string thumbFileName, byte[] binary)
        {
            CloudBlockBlob blockBlob = container_thumb.GetBlockBlobReference(thumbFileName);
            blockBlob.UploadFromByteArray(binary, 0, binary.Length);

        }

        #endregion
    }
}
