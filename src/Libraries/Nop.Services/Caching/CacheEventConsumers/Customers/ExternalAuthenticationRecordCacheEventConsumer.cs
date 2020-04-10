using Nop.Core.Domain.Customers;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Represents an external authentication record cache event consumer
    /// </summary>
    public partial class ExternalAuthenticationRecordCacheEventConsumer : CacheEventConsumer<ExternalAuthenticationRecord>
    {
    }
}
