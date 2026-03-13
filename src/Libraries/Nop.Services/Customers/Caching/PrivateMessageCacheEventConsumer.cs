using Nop.Core.Domain.Customers;
using Nop.Services.Caching;

namespace Nop.Services.Customers.Caching;

/// <summary>
/// Represents a private message cache event consumer
/// </summary>
public partial class PrivateMessageCacheEventConsumer : CacheEventConsumer<PrivateMessage>;