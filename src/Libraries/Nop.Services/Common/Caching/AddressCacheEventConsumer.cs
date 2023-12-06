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
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Address entity)
        {
            await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerAddressesPrefix);
        }
    }
}
