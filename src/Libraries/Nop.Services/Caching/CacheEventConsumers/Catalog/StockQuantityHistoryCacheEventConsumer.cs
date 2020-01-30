using Nop.Core.Domain.Catalog;

namespace Nop.Services.Caching.CacheEventConsumers.Catalog
{
    /// <summary>
    /// Represents a stock quantity change entry cache event consumer
    /// </summary>
    public partial class StockQuantityHistoryCacheEventConsumer : CacheEventConsumer<StockQuantityHistory>
    {
    }
}
