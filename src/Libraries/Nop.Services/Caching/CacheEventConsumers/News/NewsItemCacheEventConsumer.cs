using Nop.Core.Domain.News;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.News
{
    /// <summary>
    /// Represents a news item
    /// </summary>
    public partial class NewsItemCacheEventConsumer : CacheEventConsumer<NewsItem>
    {
        protected override void ClearCache(NewsItem entity)
        {
            RemoveByPrefix(NopNewsCachingDefaults.NewsCommentsPrefixCacheKey);
        }
    }
}