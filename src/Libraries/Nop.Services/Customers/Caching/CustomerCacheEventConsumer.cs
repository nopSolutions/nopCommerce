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
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(CustomerPasswordChangedEvent eventMessage)
        {
            await RemoveAsync(NopCustomerServicesDefaults.CustomerPasswordLifetimeCacheKey, eventMessage.Password.CustomerId);
        }

        /// <summary>
        /// Clear cache by entity event type
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Customer entity, EntityEventType entityEventType)
        {
            if (entityEventType == EntityEventType.Delete)
            {
                await RemoveAsync(NopCustomerServicesDefaults.CustomerAddressesCacheKey, entity);
                await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerAddressesByCustomerPrefix, entity);
            }

            await base.ClearCacheAsync(entity, entityEventType);
        }

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Customer entity)
        {
            await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerCustomerRolesByCustomerPrefix, entity);
            await RemoveByPrefixAsync(NopOrderDefaults.ShoppingCartItemsByCustomerPrefix, entity);
            await RemoveAsync(NopCustomerServicesDefaults.CustomerByGuidCacheKey, entity.CustomerGuid);

            if (string.IsNullOrEmpty(entity.SystemName))
                return;

            await RemoveAsync(NopCustomerServicesDefaults.CustomerBySystemNameCacheKey, entity.SystemName);
        }

        #endregion
    }
}