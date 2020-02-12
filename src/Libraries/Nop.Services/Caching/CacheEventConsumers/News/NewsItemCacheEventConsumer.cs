using Nop.Core.Domain.News;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.News
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
        protected override void ClearCache(NewsItem entity)
        {
            RemoveByPrefix(NopNewsCachingDefaults.NewsCommentsPrefixCacheKey);
        }
    }
}