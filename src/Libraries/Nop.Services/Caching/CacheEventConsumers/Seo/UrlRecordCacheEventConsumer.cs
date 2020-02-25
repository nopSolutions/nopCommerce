using Nop.Core.Domain.Seo;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Seo
{
    /// <summary>
    /// Represents an URL record cache event consumer
    /// </summary>
    public partial class UrlRecordCacheEventConsumer : CacheEventConsumer<UrlRecord>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(UrlRecord entity)
        {
            Remove(NopSeoCachingDefaults.UrlRecordAllCacheKey);

            var cacheKey = NopSeoCachingDefaults.UrlRecordActiveByIdNameLanguageCacheKey.ToCacheKey(entity.EntityId,
                entity.EntityName, entity.LanguageId);
            Remove(cacheKey);

            RemoveByPrefix(NopSeoCachingDefaults.UrlRecordByIdsPrefixCacheKey);

            cacheKey = NopSeoCachingDefaults.UrlRecordBySlugCacheKey.ToCacheKey(entity.Slug);
            Remove(cacheKey);
        }
    }
}
