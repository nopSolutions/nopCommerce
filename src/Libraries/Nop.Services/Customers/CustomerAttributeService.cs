using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Data;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer attribute service
    /// </summary>
    public partial class CustomerAttributeService : ICustomerAttributeService
    {
        #region Fields

        protected IRepository<CustomerAttribute> CustomerAttributeRepository { get; }
        protected IRepository<CustomerAttributeValue> CustomerAttributeValueRepository { get; }
        protected IStaticCacheManager StaticCacheManager { get; }

        #endregion

        #region Ctor

        public CustomerAttributeService(IRepository<CustomerAttribute> customerAttributeRepository,
            IRepository<CustomerAttributeValue> customerAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        {
            CustomerAttributeRepository = customerAttributeRepository;
            CustomerAttributeValueRepository = customerAttributeValueRepository;
            StaticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCustomerAttributeAsync(CustomerAttribute customerAttribute)
        {
            await CustomerAttributeRepository.DeleteAsync(customerAttribute);
        }

        /// <summary>
        /// Gets all customer attributes
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attributes
        /// </returns>
        public virtual async Task<IList<CustomerAttribute>> GetAllCustomerAttributesAsync()
        {
            return await CustomerAttributeRepository.GetAllAsync(query =>
            {
                return from ca in query
                    orderby ca.DisplayOrder, ca.Id
                    select ca;
            }, cache => default);
        }

        /// <summary>
        /// Gets a customer attribute 
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attribute
        /// </returns>
        public virtual async Task<CustomerAttribute> GetCustomerAttributeByIdAsync(int customerAttributeId)
        {
            return await CustomerAttributeRepository.GetByIdAsync(customerAttributeId, cache => default);
        }

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCustomerAttributeAsync(CustomerAttribute customerAttribute)
        {
            await CustomerAttributeRepository.InsertAsync(customerAttribute);
        }

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCustomerAttributeAsync(CustomerAttribute customerAttribute)
        {
            await CustomerAttributeRepository.UpdateAsync(customerAttribute);
        }

        /// <summary>
        /// Deletes a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue)
        {
            await CustomerAttributeValueRepository.DeleteAsync(customerAttributeValue);
        }

        /// <summary>
        /// Gets customer attribute values by customer attribute identifier
        /// </summary>
        /// <param name="customerAttributeId">The customer attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attribute values
        /// </returns>
        public virtual async Task<IList<CustomerAttributeValue>> GetCustomerAttributeValuesAsync(int customerAttributeId)
        {
            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopCustomerServicesDefaults.CustomerAttributeValuesByAttributeCacheKey, customerAttributeId);

            var query = from cav in CustomerAttributeValueRepository.Table
                orderby cav.DisplayOrder, cav.Id
                where cav.CustomerAttributeId == customerAttributeId
                select cav;

            var customerAttributeValues = await StaticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return customerAttributeValues;
        }

        /// <summary>
        /// Gets a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValueId">Customer attribute value identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attribute value
        /// </returns>
        public virtual async Task<CustomerAttributeValue> GetCustomerAttributeValueByIdAsync(int customerAttributeValueId)
        {
            return await CustomerAttributeValueRepository.GetByIdAsync(customerAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue)
        {
            await CustomerAttributeValueRepository.InsertAsync(customerAttributeValue);
        }

        /// <summary>
        /// Updates the customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue)
        {
            await CustomerAttributeValueRepository.UpdateAsync(customerAttributeValue);
        }

        #endregion
    }
}