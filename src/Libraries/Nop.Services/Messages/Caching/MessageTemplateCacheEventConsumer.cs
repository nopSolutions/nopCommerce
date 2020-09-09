using Nop.Core.Domain.Messages;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Messages.Caching
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
        protected override async Task ClearCache(MessageTemplate entity)
        {
            await RemoveByPrefix(NopMessageDefaults.MessageTemplatesAllPrefixCacheKey);
            var prefix = _cacheKeyService.PrepareKeyPrefix(NopMessageDefaults.MessageTemplatesByNamePrefixCacheKey, entity.Name);
            await RemoveByPrefix(prefix);
        }
    }
}
