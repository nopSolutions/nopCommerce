using Nop.Core.Domain.Orders;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a recurring payment history
    /// </summary>
    public partial class RecurringPaymentHistoryCacheEventConsumer : EntityCacheEventConsumer<RecurringPaymentHistory>
    {
    }
}
