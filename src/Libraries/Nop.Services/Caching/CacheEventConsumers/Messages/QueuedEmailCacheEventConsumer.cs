using Nop.Core.Domain.Messages;

namespace Nop.Services.Caching.CacheEventConsumers.Messages
{
    /// <summary>
    /// Represents an queued email cache event consumer
    /// </summary>
    public partial class QueuedEmailCacheEventConsumer : CacheEventConsumer<QueuedEmail>
    {
    }
}
