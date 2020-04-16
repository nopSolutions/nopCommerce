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
        public void HandleEvent(CustomerPasswordChangedEvent eventMessage)
        {
            base.Remove(NopCustomerServicesDefaults.CustomerPasswordLifetimeCacheKey.FillCacheKey(eventMessage.Password.CustomerId));
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Customer entity)
        {
            base.RemoveByPrefix(NopCustomerServicesDefaults.CustomerCustomerRolesPrefixCacheKey, false);
            base.RemoveByPrefix(NopCustomerServicesDefaults.CustomerAddressesPrefixCacheKey, false);
            RemoveByPrefix(NopOrderDefaults.ShoppingCartPrefixCacheKey, false);
        }

        #endregion
    }
}