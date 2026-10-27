using Nop.Plugin.Misc.News.Domain;
using Nop.Services.Caching;

namespace Nop.Plugin.Misc.News.Services.Caching;

/// <summary>
/// Represents a news comment cache event consumer
/// </summary>
public class NewsCommentCacheEventConsumer : CacheEventConsumer<NewsComment>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(NewsComment entity)
    {
        await RemoveByPrefixAsync(NewsDefaults.NewsCommentsNumberPrefix, entity.NewsItemId);
    }
}