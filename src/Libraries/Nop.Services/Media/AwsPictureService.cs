using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Seo;

namespace Nop.Services.Media
{
    /// <summary>
    /// Picture service for Aws
    /// </summary>
    public partial class AwsPictureService : PictureService
    {
        #region Fields

        private static AmazonS3Client _amazonS3Client;
        private static bool _isInitialized;
        private static string _region;
        private static string _bucket;
        private static string _secretAccessKey;
        private static string _accessKeyId;
        private static string _objectUrlTemplate;
        private readonly ILogger _logger;



        private readonly IStaticCacheManager _staticCacheManager;
        private readonly MediaSettings _mediaSettings;

        private readonly object _locker = new();

        #endregion

        #region Ctor

        public AwsPictureService(AppSettings appSettings,
            INopDataProvider dataProvider,
            IDownloadService downloadService,
            IHttpContextAccessor httpContextAccessor,
            INopFileProvider fileProvider,
            IProductAttributeParser productAttributeParser,
            IRepository<Picture> pictureRepository,
            IRepository<PictureBinary> pictureBinaryRepository,
            IRepository<ProductPicture> productPictureRepository,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            MediaSettings mediaSettings,
            ILogger logger)
            : base(dataProvider,
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
            _staticCacheManager = staticCacheManager;
            _mediaSettings = mediaSettings;
            _logger = logger;

            OneTimeInit(appSettings);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Initialize cloud container
        /// </summary>
        /// <param name="appSettings">App settings</param>
        protected void OneTimeInit(AppSettings appSettings)
        {
            if (_isInitialized)
                return;

            if (string.IsNullOrEmpty(appSettings.AwsS3Config.Region))
                throw new Exception("Aws region is not specified");

            if (string.IsNullOrEmpty(appSettings.AwsS3Config.Bucket))
                throw new Exception("Aws bucket name is not specified");

            if (string.IsNullOrEmpty(appSettings.AwsS3Config.AccessKeyId))
                throw new Exception("Aws access key id is not specified");

            if (string.IsNullOrEmpty(appSettings.AwsS3Config.SecretAccessKey))
                throw new Exception("Aws secret access key is not specified");

            

            lock (_locker)
            {
                if (_isInitialized)
                    return;

                _region = appSettings.AwsS3Config.Region;
                _bucket = appSettings.AwsS3Config.Bucket;
                _accessKeyId = appSettings.AwsS3Config.AccessKeyId.Trim();
                _secretAccessKey = appSettings.AwsS3Config.SecretAccessKey.Trim();
                _objectUrlTemplate = appSettings.AwsS3Config.ObjectUrlTemplate;

                _amazonS3Client = new AmazonS3Client(new BasicAWSCredentials(_accessKeyId, _secretAccessKey),
                new AmazonS3Config { RegionEndpoint = RegionEndpoint.GetBySystemName(_region) });
               
                CreateS3Bucket().GetAwaiter().GetResult();

                _isInitialized = true;
            }
        }

        /// <summary>
        /// Create S3 bucket
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task CreateS3Bucket()
        {
                try
                {
                    if (await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3Client, _bucket))
                    await _amazonS3Client.PutBucketAsync(new PutBucketRequest
                    {
                        BucketName = _bucket,
                        UseClientRegion = true,
                        CannedACL = S3CannedACL.PublicRead,
                    });
                }
                catch (Exception ex)
                {
                    await _logger.InsertLogAsync(LogLevel.Error, ex.Message, ex.StackTrace);
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
        protected override Task<string> GetThumbLocalPathAsync(string thumbFileName)
        {
            
            return Task.FromResult(_objectUrlTemplate.Replace("$region$", _region)
                .Replace("$bucket$", _bucket).Replace("$fileName$", thumbFileName));
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
        protected override async Task<string> GetThumbUrlAsync(string thumbFileName, string storeLocation = null)
        {
            return await GetThumbLocalPathAsync(thumbFileName);
        }

        /// <summary>
        /// Initiates an asynchronous operation to delete picture thumbs
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task DeletePictureThumbsAsync(Picture picture)
        {
            try
            {
                var listObjectsResult = await _amazonS3Client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = _bucket,
                    Prefix = $"{picture.Id:0000000}"

                });
                if (listObjectsResult.S3Objects.Any())
                    await _amazonS3Client.DeleteObjectsAsync(new DeleteObjectsRequest
                    {
                        BucketName = _bucket,
                        Objects = listObjectsResult.S3Objects.Select(x => new KeyVersion { Key = x.Key }).ToList()
                    });

            }
            catch(Exception ex)
            {
                await _logger.InsertLogAsync(LogLevel.Error, ex.Message, ex.StackTrace);
            }

            await _staticCacheManager.RemoveByPrefixAsync(NopMediaDefaults.ThumbsExistsPrefix);
        }

        /// <summary>
        /// Initiates an asynchronous operation to get a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected override async Task<bool> GeneratedThumbExistsAsync(string thumbFilePath, string thumbFileName)
        {
            try
            {
                var key = _staticCacheManager.PrepareKeyForDefaultCache(NopMediaDefaults.ThumbExistsCacheKey, thumbFileName);

                return await _staticCacheManager.GetAsync(key, async () =>
                {
                    var result = await _amazonS3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
                    {
                        BucketName = _bucket,
                        Key = thumbFileName,
                    });

                    return result.HttpStatusCode != HttpStatusCode.NotFound;
                });
            }
            catch(Exception ex)
            {
                await _logger.InsertLogAsync(LogLevel.Error, ex.Message, ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Initiates an asynchronous operation to save a value indicating whether some file (thumb) already exists
        /// </summary>
        /// <param name="thumbFilePath">Thumb file path</param>
        /// <param name="thumbFileName">Thumb file name</param>
        /// <param name="mimeType">MIME type</param>
        /// <param name="binary">Picture binary</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task SaveThumbAsync(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
        {
            try
            {
                await _amazonS3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = _bucket,
                    Key = thumbFileName,
                    InputStream = new MemoryStream(binary),
                    ContentType = mimeType,
                });

                await _staticCacheManager.RemoveByPrefixAsync(NopMediaDefaults.ThumbsExistsPrefix);
            }
            catch(Exception ex)
            {
                await _logger.InsertLogAsync(LogLevel.Error, ex.Message, ex.StackTrace);
            }
           
        }

        #endregion
    }
}
