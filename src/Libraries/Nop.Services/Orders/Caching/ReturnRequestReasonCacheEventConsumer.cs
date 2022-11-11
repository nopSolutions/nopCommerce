using Nop.Core.Domain.Orders;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Orders.Caching
{
    /// <summary>
    /// Represents a return request reason cache event consumer
    /// </summary>
    public partial class ReturnRequestReasonCacheEventConsumer : CacheEventConsumer<ReturnRequestReason>
    {
    }
}
