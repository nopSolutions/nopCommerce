using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Customer cache event consumer (used for caching of current customer password)
    /// </summary>
    public partial class CustomerCacheEventConsumer : CacheEventConsumer<Customer>, IConsumer<CustomerPasswordChangedEvent>
    {
        #region Methods

        //password changed
        public void HandleEvent(CustomerPasswordChangedEvent eventMessage)
        {
            Remove(string.Format(NopCustomerServiceCachingDefaults.CustomerPasswordLifetimeCacheKey, eventMessage.Password.CustomerId));
        }

        public override void ClearCashe(Customer entity)
        {
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerCustomerRolesPrefixCacheKey, false);
            RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAddressesPrefixCacheKey, false);
            RemoveByPrefix(NopOrderCachingDefaults.ShoppingCartPrefixCacheKey);
        }

        #endregion
    }
}