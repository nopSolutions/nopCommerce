using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Services.Caching;
using Nop.Services.Events;
using Nop.Services.Orders;

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
        public async Task HandleEvent(CustomerPasswordChangedEvent eventMessage)
        {
            await Remove(_cacheKeyService.PrepareKey(NopCustomerServicesDefaults.CustomerPasswordLifetimeCacheKey, eventMessage.Password.CustomerId));
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(Customer entity)
        {
            await RemoveByPrefix(NopCustomerServicesDefaults.CustomerCustomerRolesPrefixCacheKey);
            await RemoveByPrefix(NopCustomerServicesDefaults.CustomerAddressesPrefixCacheKey);
            await RemoveByPrefix(NopOrderDefaults.ShoppingCartPrefixCacheKey);
        }

        #endregion
    }
}