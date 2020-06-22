using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
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
            Remove(NopCatalogDefaults.SpecAttributesWithOptionsCacheKey);
            Remove(_cacheKeyService.PrepareKey(NopCatalogDefaults.SpecAttributesOptionsCacheKey, entity.SpecificationAttributeId));

            RemoveByPrefix(NopCatalogDefaults.ProductSpecificationAttributeAllByProductIdsPrefixCacheKey);
        }
    }
}
