using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a back in stock subscription cache event consumer
    /// </summary>
    public partial class BackInStockSubscriptionCacheEventConsumer : CacheEventConsumer<BackInStockSubscription>
    {
    }
}
