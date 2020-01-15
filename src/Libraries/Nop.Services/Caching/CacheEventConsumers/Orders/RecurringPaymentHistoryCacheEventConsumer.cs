using Nop.Core.Domain.Orders;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a recurring payment history cache event consumer
    /// </summary>
    public partial class RecurringPaymentHistoryCacheEventConsumer : CacheEventConsumer<RecurringPaymentHistory>
    {
    }
}
