using Nop.Core.Domain.Customers;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Represents a customer-customer role mapping class
    /// </summary>
    public partial class CustomerCustomerRoleMappingCacheEventConsumer : EntityCacheEventConsumer<CustomerCustomerRoleMapping>
    {
    }
}