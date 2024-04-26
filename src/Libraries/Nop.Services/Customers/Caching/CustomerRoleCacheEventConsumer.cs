using Nop.Core.Domain.Customers;
using Nop.Services.Caching;

namespace Nop.Services.Customers.Caching;

/// <summary>
/// Represents a customer role cache event consumer
/// </summary>
public partial class CustomerRoleCacheEventConsumer : CacheEventConsumer<CustomerRole>
{
    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(CustomerRole entity, EntityEventType entityEventType)
    {
        switch (entityEventType)
        {
            case EntityEventType.Update:
                await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerRolesBySystemNamePrefix);
                break;
            case EntityEventType.Delete:
                await RemoveAsync(NopCustomerServicesDefaults.CustomerRolesBySystemNameCacheKey, entity.SystemName);
                break;
        }

        if (entityEventType != EntityEventType.Insert)
            await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerCustomerRolesPrefix);

        await base.ClearCacheAsync(entity, entityEventType);
    }
}