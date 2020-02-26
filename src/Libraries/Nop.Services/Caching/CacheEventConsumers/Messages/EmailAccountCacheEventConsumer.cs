using Nop.Core.Domain.Messages;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Messages
{
    /// <summary>
    /// Represents an email account cache event consumer
    /// </summary>
    public partial class EmailAccountCacheEventConsumer : CacheEventConsumer<EmailAccount>
    {
        protected override void ClearCache(EmailAccount entity)
        {
            RemoveByPrefix(NopMessageCachingDefaults.EmailAccountsPrefixCacheKey);
        }
    }
}
