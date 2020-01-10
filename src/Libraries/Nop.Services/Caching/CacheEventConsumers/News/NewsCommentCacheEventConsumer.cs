using Nop.Core.Domain.News;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.News
{
    /// <summary>
    /// Represents a news comment
    /// </summary>
    public partial class NewsCommentCacheEventConsumer : CacheEventConsumer<NewsComment>
    {
        protected override void ClearCache(NewsComment entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
                RemoveByPrefix(NopNewsCachingDefaults.NewsCommentsPrefixCacheKey);
        }
    }
}