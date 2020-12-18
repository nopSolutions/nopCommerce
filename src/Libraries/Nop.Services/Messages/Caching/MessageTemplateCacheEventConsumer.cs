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
        protected override async Task ClearCacheAsync(MessageTemplate entity)
        {
            await RemoveByPrefixAsync(NopMessageDefaults.MessageTemplatesByNamePrefix, entity.Name);
        }
    }
}
