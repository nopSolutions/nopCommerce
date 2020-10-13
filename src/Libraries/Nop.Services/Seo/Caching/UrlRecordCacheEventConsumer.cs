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
            await Remove(NopSeoDefaults.UrlRecordCacheKey, entity.EntityId, entity.EntityName, entity.LanguageId);
            await Remove(NopSeoDefaults.UrlRecordBySlugCacheKey, entity.Slug);
        }
    }
}
