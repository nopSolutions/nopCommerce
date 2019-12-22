using Nop.Core.Domain.Discounts;

namespace Nop.Services.Caching.CacheEventConsumers.Discounts
{
    /// <summary>
    /// Represents a discount-product mapping class
    /// </summary>
    public partial class DiscountProductMappingCacheEventConsumer : EntityCacheEventConsumer<DiscountManufacturerMapping>
    {
    }
}