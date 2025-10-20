using Nop.Core.Domain.Media;
using Nop.Services.Caching;

namespace Nop.Plugin.Misc.AzureBlob.Services.Caching;

/// <summary>
/// Represents a picture cache event consumer
/// </summary>
public class PictureCacheEventConsumer : CacheEventConsumer<Picture>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Picture entity)
    {
        await RemoveByPrefixAsync(AzureBlobDefaults.ThumbsExistsPrefix);
    }
}