using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a return request reason cache event consumer
    /// </summary>
    public partial class ReturnRequestReasonCacheEventConsumer : CacheEventConsumer<ReturnRequestReason>
    {
        protected override void ClearCache(ReturnRequestReason entity)
        {
            RemoveByPrefix(NopOrderCachingDefaults.ReturnRequestReasonPrefixCacheKey);
        }
    }
}
