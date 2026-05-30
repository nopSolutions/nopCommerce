using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.CloudflareImages.Services;

/// <summary>
/// Picture service for Cloudflare Images
/// </summary>
public partial class CloudflareThumbService : IThumbService
{
    #region Fields

    private readonly CloudflareImagesSettings _cloudflareImagesSettings;
    private readonly CloudflareImagesHttpClient _cloudflareImagesHttpClient;
    private readonly INopFileProvider _fileProvider;
    private readonly IRepository<Domain.CloudflareImages> _cloudflareImagesRepository;

    #endregion

    #region Ctor

    public CloudflareThumbService(CloudflareImagesSettings cloudflareImagesSettings,
        CloudflareImagesHttpClient cloudflareImagesHttpClient,
        INopFileProvider fileProvider,
        IRepository<Domain.CloudflareImages> cloudflareImagesRepository)
    {
        _cloudflareImagesSettings = cloudflareImagesSettings;
        _cloudflareImagesHttpClient = cloudflareImagesHttpClient;
        _fileProvider = fileProvider;
        _cloudflareImagesRepository = cloudflareImagesRepository;
    }

    #endregion

    #region Methods

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
        return await _cloudflareImagesRepository.Table.AnyAsync(i => i.ThumbFileName.Equals(thumbFileName));
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
        var dataContent = new MultipartFormDataContent
        {
            { new StreamContent(new MemoryStream(binary)), "file", thumbFileName }
        };

        var response = await _cloudflareImagesHttpClient.SaveThumbAsync(dataContent);

        if (response is not { Success: true })
            return;

        await _cloudflareImagesRepository.InsertAsync(new Domain.CloudflareImages
        {
            ImageId = response.Result.Id,
            ThumbFileName = thumbFileName
        }, false);
    }

    /// <summary>
    /// Get picture (thumb) local path
    /// </summary>
    /// <param name="thumbFileName">Filename</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the local picture thumb path
    /// </returns>
    public async Task<string> GetThumbLocalPathByFileNameAsync(string thumbFileName)
    {
        var image = await _cloudflareImagesRepository.Table.FirstOrDefaultAsync(i => i.ThumbFileName.Equals(thumbFileName));

        if (image == null)
            return null;

        return _cloudflareImagesSettings.DeliveryUrl
            .Replace(CloudflareImagesDefaults.ImageIdPattern, image.ImageId)
            .Replace(CloudflareImagesDefaults.VariantNamePattern, "public");
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
        var items = await _cloudflareImagesRepository.Table
            .Where(i => i.ThumbFileName.Contains($"_{picture.SeoFilename}.") || i.ThumbFileName.Contains($"_{picture.SeoFilename}_"))
            .ToListAsync();

        foreach (var item in items)
            await _cloudflareImagesHttpClient.DeleteThumbAsync(item.ImageId);

        await _cloudflareImagesRepository.DeleteAsync(items, false);
    }

    #endregion
}