using Nop.Core.Domain.Shipping;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a product availability range cache event consumer
    /// </summary>
    public partial class ProductAvailabilityRangeCacheEventConsumer : CacheEventConsumer<ProductAvailabilityRange>
    {
    }
}