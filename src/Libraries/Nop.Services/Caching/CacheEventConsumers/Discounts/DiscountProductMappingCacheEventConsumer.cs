using Nop.Core.Domain.Discounts;

namespace Nop.Services.Caching.CacheEventConsumers.Discounts
{
    /// <summary>
    /// Represents a discount-product mapping cache event consumer
    /// </summary>
    public partial class DiscountProductMappingCacheEventConsumer : CacheEventConsumer<DiscountManufacturerMapping>
    {
    }
}