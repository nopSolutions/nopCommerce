using Nop.Core.Domain.Catalog;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a stock quantity change entry
    /// </summary>
    public partial class StockQuantityHistoryCacheEventConsumer : EntityCacheEventConsumer<StockQuantityHistory>
    {
    }
}
