using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a manufacturer template cache event consumer
    /// </summary>
    public partial class ManufacturerTemplateCacheEventConsumer : CacheEventConsumer<ManufacturerTemplate>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ManufacturerTemplate entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ManufacturerTemplatesPrefixCacheKey);
        }
    }
}
