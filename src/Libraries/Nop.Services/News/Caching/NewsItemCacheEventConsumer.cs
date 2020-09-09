using Nop.Core.Domain.News;
using Nop.Services.Caching;
using System.Threading.Tasks;

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
        protected override async Task ClearCache(NewsItem entity)
        {
            var prefix = _cacheKeyService.PrepareKeyPrefix(NopNewsDefaults.NewsCommentsNumberPrefixCacheKey, entity);

            await RemoveByPrefix(prefix);
        }
    }
}