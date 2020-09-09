using Nop.Core.Domain.Messages;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Messages.Caching
{
    /// <summary>
    /// Represents an email account cache event consumer
    /// </summary>
    public partial class EmailAccountCacheEventConsumer : CacheEventConsumer<EmailAccount>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(EmailAccount entity)
        {
            await Remove(NopMessageDefaults.EmailAccountsAllCacheKey);
        }
    }
}
