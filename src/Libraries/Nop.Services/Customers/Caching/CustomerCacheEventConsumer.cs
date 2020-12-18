﻿using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
﻿using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Services.Caching;
using Nop.Services.Events;

namespace Nop.Services.Customers.Caching
{
    /// <summary>
    /// Represents a customer cache event consumer
    /// </summary>
    public partial class CustomerCacheEventConsumer : CacheEventConsumer<Customer>, IConsumer<CustomerPasswordChangedEvent>
    {
        #region Methods

        /// <summary>
        /// Handle password changed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public async Task HandleEventAsync(CustomerPasswordChangedEvent eventMessage)
        {
            await RemoveAsync(NopCustomerServicesDefaults.CustomerPasswordLifetimeCacheKey, eventMessage.Password.CustomerId);
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(Customer entity)
        {
            await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerCustomerRolesPrefix);
            await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerAddressesPrefix);
            await RemoveByPrefixAsync(NopEntityCacheDefaults<ShoppingCartItem>.AllPrefix);

            if (string.IsNullOrEmpty(entity.SystemName))
                return;

            await RemoveAsync(NopCustomerServicesDefaults.CustomerBySystemNameCacheKey, entity.SystemName);
            await RemoveAsync(NopCustomerServicesDefaults.CustomerByGuidCacheKey, entity.CustomerGuid);
        }

        #endregion
    }
}