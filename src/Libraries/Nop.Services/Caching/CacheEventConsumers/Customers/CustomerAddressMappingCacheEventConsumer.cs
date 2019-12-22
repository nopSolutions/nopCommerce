using Nop.Core.Domain.Customers;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Represents a customer-address mapping class
    /// </summary>
    public partial class CustomerAddressMappingCacheEventConsumer : EntityCacheEventConsumer<CustomerAddressMapping>
    {
    }
}