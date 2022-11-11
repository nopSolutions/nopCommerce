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
    }
}
