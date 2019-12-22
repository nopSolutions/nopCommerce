using Nop.Core.Domain.Customers;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Represents a customer password
    /// </summary>
    public partial class CustomerPasswordCacheEventConsumer : EntityCacheEventConsumer<CustomerPassword>
    {
    }
}