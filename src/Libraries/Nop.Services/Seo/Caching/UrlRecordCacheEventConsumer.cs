using Nop.Core.Caching;
using Nop.Core.Domain.Seo;
using Nop.Services.Caching;

namespace Nop.Services.Seo.Caching
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
            Remove(_staticCacheManager.PrepareKey(NopCachingDefaults.EntitiesByIdsCacheKey, entity.GetType().Name.ToLower()));

            var cacheKey = _staticCacheManager.PrepareKey(NopSeoDefaults.UrlRecordActiveByIdNameLanguageCacheKey,
                entity.EntityId, entity.EntityName, entity.LanguageId);
            Remove(cacheKey);

            Remove(_staticCacheManager.PrepareKey(NopCachingDefaults.AllEntitiesCacheKey, entity.GetType().Name.ToLower()));

            cacheKey = _staticCacheManager.PrepareKey(NopSeoDefaults.UrlRecordBySlugCacheKey, entity.Slug);
            Remove(cacheKey);
        }
    }
}
