﻿using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
﻿using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product cache event consumer
    /// </summary>
    public partial class ProductCacheEventConsumer : CacheEventConsumer<Product>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(Product entity)
        {
            await RemoveByPrefix(NopCatalogDefaults.ProductManufacturersByProductPrefix, entity);
            await Remove(NopCatalogDefaults.ProductsHomepageCacheKey);
            await RemoveByPrefix(NopCatalogDefaults.ProductPricePrefix, entity);
            await RemoveByPrefix(NopEntityCacheDefaults<ShoppingCartItem>.AllPrefix);
        }
    }
}
