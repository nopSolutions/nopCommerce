using Nop.Core.Domain.Messages;

namespace Nop.Services.Caching.CacheEventConsumers.Messages
{
    /// <summary>
    /// Represents an email item
    /// </summary>
    public partial class QueuedEmailCacheEventConsumer : CacheEventConsumer<QueuedEmail>
    {
    }
}
