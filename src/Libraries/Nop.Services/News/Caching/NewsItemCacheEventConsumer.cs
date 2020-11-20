using Nop.Core.Domain.News;
using Nop.Services.Caching;

namespace Nop.Services.News.Caching
{
    /// <summary>
    /// Represents a news item cache event consumer
    /// </summary>
    public partial class NewsItemCacheEventConsumer : CacheEventConsumer<NewsItem>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(NewsItem entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
                RemoveByPrefix(NopNewsDefaults.NewsCommentsNumberPrefix, entity);

            base.ClearCache(entity, entityEventType);
        }
    }
}