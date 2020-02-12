using Nop.Core.Domain.Catalog;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a specification attribute option cache event consumer
    /// </summary>
    public partial class SpecificationAttributeOptionCacheEventConsumer : CacheEventConsumer<SpecificationAttributeOption>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(SpecificationAttributeOption entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductSpecificationAttributePrefixCacheKey);
        }
    }
}
