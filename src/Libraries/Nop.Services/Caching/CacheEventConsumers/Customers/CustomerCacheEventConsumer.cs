using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Customers
{
    /// <summary>
    /// Customer cache event consumer (used for caching of current customer password)
    /// </summary>
    public partial class CustomerCacheEventConsumer : EntityCacheEventConsumer<Customer>, IConsumer<CustomerPasswordChangedEvent>
    {
        #region Fields

        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public CustomerCacheEventConsumer(IStaticCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        //password changed
        public void HandleEvent(CustomerPasswordChangedEvent eventMessage)
        {
            _cacheManager.Remove(string.Format(NopCustomerServiceCachingDefaults.CustomerPasswordLifetimeCacheKey, eventMessage.Password.CustomerId));
        }

        public override void ClearCashe(Customer entity)
        {
            _cacheManager.RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerRoleIdsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopCustomerServiceCachingDefaults.CustomerAddressesPrefixCacheKey);

             base.ClearCashe(entity);
        }

        #endregion
    }
}