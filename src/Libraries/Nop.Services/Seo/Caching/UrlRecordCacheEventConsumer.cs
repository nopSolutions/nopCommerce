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
        protected override async Task ClearCacheAsync(UrlRecord entity)
        {
            await RemoveAsync(NopSeoDefaults.UrlRecordCacheKey, entity.EntityId, entity.EntityName, entity.LanguageId);
            await RemoveAsync(NopSeoDefaults.UrlRecordBySlugCacheKey, entity.Slug);
        }
    }
}
