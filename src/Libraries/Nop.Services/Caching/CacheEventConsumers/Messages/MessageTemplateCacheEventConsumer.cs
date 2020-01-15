using Nop.Core.Domain.Messages;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Messages
{
    /// <summary>
    /// Represents a message template cache event consumer
    /// </summary>
    public partial class MessageTemplateCacheEventConsumer : CacheEventConsumer<MessageTemplate>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(MessageTemplate entity)
        {
            RemoveByPrefix(NopMessageCachingDefaults.MessageTemplatesPrefixCacheKey);
        }
    }
}
