using Nop.Core.Domain.Seo;
using Nop.Services.Caching;
using System.Threading.Tasks;

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
        protected override async Task ClearCache(UrlRecord entity)
        {
            await Remove(NopSeoDefaults.UrlRecordAllCacheKey);

            var cacheKey = _cacheKeyService.PrepareKey(NopSeoDefaults.UrlRecordActiveByIdNameLanguageCacheKey,
                entity.EntityId, entity.EntityName, entity.LanguageId);
            await Remove(cacheKey);

            await RemoveByPrefix(NopSeoDefaults.UrlRecordByIdsPrefixCacheKey);

            cacheKey = _cacheKeyService.PrepareKey(NopSeoDefaults.UrlRecordBySlugCacheKey, entity.Slug);
            await Remove(cacheKey);
        }
    }
}
