using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a return request action cache event consumer
    /// </summary>
    public partial class ReturnRequestActionCacheEventConsumer : CacheEventConsumer<ReturnRequestAction>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ReturnRequestAction entity)
        {
            Remove(NopOrderCachingDefaults.ReturnRequestActionAllCacheKey);
        }
    }
}
