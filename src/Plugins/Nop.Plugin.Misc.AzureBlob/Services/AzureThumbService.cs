using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Nop.Core.Caching;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.AzureBlob.Services;

/// <summary>
/// Picture service for Windows Azure
/// </summary>
public class AzureThumbService : IThumbService
{
    #region Fields

    private static BlobContainerClient _blobContainerClient;
    private static BlobServiceClient _blobServiceClient;
    private static bool _azureBlobStorageAppendContainerName;
    private static bool _isInitialized;
    private static string _azureBlobStorageConnectionString;
    private static string _azureBlobStorageContainerName;
    private static string _azureBlobStorageEndPoint;

    private readonly AzureBlobSettings _azureBlobSettings;
    private readonly INopFileProvider _fileProvider;
    private readonly IStaticCacheManager _staticCacheManager;

    private readonly object _locker = new();

    #endregion

    #region Ctor

    public AzureThumbService(AzureBlobSettings azureBlobSettings,
        INopFileProvider fileProvider,
        IStaticCacheManager staticCacheManager)
    {
        _azureBlobSettings = azureBlobSettings;
        _fileProvider = fileProvider;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Initialize cloud container
    /// </summary>
    private void Init()
    {
        if (_isInitialized)
            return;

        if (string.IsNullOrEmpty(_azureBlobSettings.ConnectionString))
            throw new Exception("Azure connection string for Blob is not specified");

        if (string.IsNullOrEmpty(_azureBlobSettings.ContainerName))
            throw new Exception("Azure container name for Blob is not specified");

        if (string.IsNullOrEmpty(_azureBlobSettings.EndPoint))
            throw new Exception("Azure end point for Blob is not specified");

        lock (_locker)
        {
            if (_isInitialized)
                return;

            _azureBlobStorageAppendContainerName = _azureBlobSettings.AppendContainerName;
            _azureBlobStorageConnectionString = _azureBlobSettings.ConnectionString;
            _azureBlobStorageContainerName = _azureBlobSettings.ContainerName.Trim().ToLowerInvariant();
            _azureBlobStorageEndPoint = _azureBlobSettings.EndPoint.Trim().ToLowerInvariant().TrimEnd('/');

            _blobServiceClient = new BlobServiceClient(_azureBlobStorageConnectionString);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_azureBlobStorageContainerName);

            _blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob).GetAwaiter().GetResult();

            _isInitialized = true;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get picture (thumb) local path
    /// </summary>
    /// <param name="thumbFileName">Filename</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    public Task<string> GetThumbLocalPathByFileNameAsync(string thumbFileName)
    {
        Init();

        var path = _azureBlobStorageAppendContainerName ? $"{_azureBlobStorageContainerName}/" : string.Empty;

        return Task.FromResult($"{_azureBlobStorageEndPoint}/{path}{thumbFileName}");
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
    public async Task<string> GetThumbUrlAsync(string thumbFileName, string storeLocation = null)
    {
        return await GetThumbLocalPathByFileNameAsync(thumbFileName);
    }

    /// <summary>
    /// Delete picture thumbs
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeletePictureThumbsAsync(Picture picture)
    {
        Init();

        //create a string containing the Blob name prefix
        var prefix = $"{picture.Id:0000000}";

        var tasks = await _blobContainerClient
            .GetBlobsAsync(BlobTraits.All, BlobStates.All, prefix)
            .Select(blob => _blobContainerClient.DeleteBlobIfExistsAsync(blob.Name, DeleteSnapshotsOption.IncludeSnapshots))
            .Select(dummy => (Task)dummy)
            .ToListAsync();
        await Task.WhenAll(tasks);

        await _staticCacheManager.RemoveByPrefixAsync(AzureBlobDefaults.ThumbsExistsPrefix);
    }

    /// <summary>
    /// Get a picture thumb local path
    /// </summary>
    /// <param name="pictureUrl">Picture URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    public async Task<string> GetThumbLocalPathAsync(string pictureUrl)
    {
        if (string.IsNullOrEmpty(pictureUrl))
            return string.Empty;

        return await GetThumbLocalPathByFileNameAsync(_fileProvider.GetFileName(pictureUrl));
    }

    /// <summary>
    /// Get a value indicating whether some file (thumb) already exists
    /// </summary>
    /// <param name="thumbFilePath">Thumb file path</param>
    /// <param name="thumbFileName">Thumb file name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check result
    /// </returns>
    public async Task<bool> GeneratedThumbExistsAsync(string thumbFilePath, string thumbFileName)
    {
        Init();

        try
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(AzureBlobDefaults.ThumbExistsCacheKey, thumbFileName);

            return await _staticCacheManager.GetAsync(key, async () => await _blobContainerClient.GetBlobClient(thumbFileName).ExistsAsync());
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Save a picture thumb
    /// </summary>
    /// <param name="thumbFilePath">Thumb file path</param>
    /// <param name="thumbFileName">Thumb file name</param>
    /// <param name="mimeType">MIME type</param>
    /// <param name="binary">Picture binary</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SaveThumbAsync(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
    {
        Init();

        var blobClient = _blobContainerClient.GetBlobClient(thumbFileName);
        await using var ms = new MemoryStream(binary);

        //set mime type
        BlobHttpHeaders headers = null;
        if (!string.IsNullOrWhiteSpace(mimeType))
            headers = new BlobHttpHeaders { ContentType = mimeType };

        //set cache control
        if (!string.IsNullOrWhiteSpace(_azureBlobSettings.AzureCacheControlHeader))
        {
            headers ??= new BlobHttpHeaders();
            headers.CacheControl = _azureBlobSettings.AzureCacheControlHeader;
        }

        if (headers is null)
            //we must explicitly indicate through the parameter that the object needs to be overwritten if it already exists
            //see more: https://github.com/Azure/azure-sdk-for-net/issues/9470
            await blobClient.UploadAsync(ms, overwrite: true);
        else
            await blobClient.UploadAsync(ms, new BlobUploadOptions { HttpHeaders = headers });

        await _staticCacheManager.RemoveByPrefixAsync(AzureBlobDefaults.ThumbsExistsPrefix);
    }

    #endregion
}