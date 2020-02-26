using Nop.Core.Domain.Customers;
using Nop.Services.Caching.CachingDefaults;
using Nop.Services.Events;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
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
        public void HandleEvent(CustomerPasswordChangedEvent eventMessage)
        {
            Remove(string.Format(NopCustomerServiceCachingDefaults.CustomerPasswordLifetimeCacheKey, eventMessage.Password.CustomerId));
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Customer entity)
        {
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerCustomerRolesPrefixCacheKey, false);
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAddressesPrefixCacheKey, false);
            RemoveByPrefix(NopOrderCachingDefaults.ShoppingCartPrefixCacheKey, false);
        }

        #endregion
    }
}