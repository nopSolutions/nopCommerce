using Nop.Core.Domain.Shipping;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Shipping.Caching
{
    /// <summary>
    /// Represents a warehouse cache event consumer
    /// </summary>
    public partial class WarehouseCacheEventConsumer : CacheEventConsumer<Warehouse>
    {
    }
}
