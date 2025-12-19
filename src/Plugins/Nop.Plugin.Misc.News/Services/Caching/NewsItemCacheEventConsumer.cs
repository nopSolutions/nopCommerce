using Nop.Plugin.Misc.News.Domain;
using Nop.Services.Caching;

namespace Nop.Plugin.Misc.News.Services.Caching;

/// <summary>
/// Represents a news item cache event consumer
/// </summary>
public class NewsItemCacheEventConsumer : CacheEventConsumer<NewsItem>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(NewsItem entity, EntityEventType entityEventType)
    {
        if (entityEventType == EntityEventType.Delete)
            await RemoveByPrefixAsync(NewsDefaults.NewsCommentsNumberPrefix, entity);

        await base.ClearCacheAsync(entity, entityEventType);
    }
}