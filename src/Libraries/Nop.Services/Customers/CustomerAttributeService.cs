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

        private readonly IRepository<CustomerAttribute> _customerAttributeRepository;
        private readonly IRepository<CustomerAttributeValue> _customerAttributeValueRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public CustomerAttributeService(IRepository<CustomerAttribute> customerAttributeRepository,
            IRepository<CustomerAttributeValue> customerAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        {
            _customerAttributeRepository = customerAttributeRepository;
            _customerAttributeValueRepository = customerAttributeValueRepository;
            _staticCacheManager = staticCacheManager;
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
            await _customerAttributeRepository.DeleteAsync(customerAttribute);
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
            return await _customerAttributeRepository.GetAllAsync(query =>
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
            return await _customerAttributeRepository.GetByIdAsync(customerAttributeId, cache => default);
        }

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCustomerAttributeAsync(CustomerAttribute customerAttribute)
        {
            await _customerAttributeRepository.InsertAsync(customerAttribute);
        }

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCustomerAttributeAsync(CustomerAttribute customerAttribute)
        {
            await _customerAttributeRepository.UpdateAsync(customerAttribute);
        }

        /// <summary>
        /// Deletes a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue)
        {
            await _customerAttributeValueRepository.DeleteAsync(customerAttributeValue);
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
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCustomerServicesDefaults.CustomerAttributeValuesByAttributeCacheKey, customerAttributeId);

            var query = from cav in _customerAttributeValueRepository.Table
                orderby cav.DisplayOrder, cav.Id
                where cav.CustomerAttributeId == customerAttributeId
                select cav;

            var customerAttributeValues = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

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
            return await _customerAttributeValueRepository.GetByIdAsync(customerAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue)
        {
            await _customerAttributeValueRepository.InsertAsync(customerAttributeValue);
        }

        /// <summary>
        /// Updates the customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue)
        {
            await _customerAttributeValueRepository.UpdateAsync(customerAttributeValue);
        }

        #endregion
    }
}