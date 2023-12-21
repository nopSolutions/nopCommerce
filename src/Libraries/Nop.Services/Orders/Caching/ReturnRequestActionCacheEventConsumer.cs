using Nop.Core.Domain.Orders;
using Nop.Services.Caching;

namespace Nop.Services.Orders.Caching;

/// <summary>
/// Represents a return request action cache event consumer
/// </summary>
public partial class ReturnRequestActionCacheEventConsumer : CacheEventConsumer<ReturnRequestAction>
{
}