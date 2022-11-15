using System.Threading.Tasks;
using Nop.Core.Domain.Common;
using Nop.Services.Caching;
using Nop.Services.Customers;

namespace Nop.Services.Common.Caching
{
    /// <summary>
    /// Represents a address cache event consumer
    /// </summary>
    public partial class AddressCacheEventConsumer : CacheEventConsumer<Address>
    {
        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Address entity, EntityEventType entityEventType)
        {
            if (entityEventType != EntityEventType.Insert)
                await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerAddressesPrefix);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
