using Nop.Core.Domain.Messages;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Messages
{
    public partial class MessageTemplateCacheEventConsumer : CacheEventConsumer<MessageTemplate>
    {
        public override void ClearCashe(MessageTemplate entity)
        {
            RemoveByPrefix(NopMessageCachingDefaults.MessageTemplatesPrefixCacheKey);
        }
    }
}
