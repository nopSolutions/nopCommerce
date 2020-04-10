using Nop.Core.Domain.Orders;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a recurring payment cache event consumer
    /// </summary>
    public partial class RecurringPaymentCacheEventConsumer : CacheEventConsumer<RecurringPayment>
    { 
    }
}
