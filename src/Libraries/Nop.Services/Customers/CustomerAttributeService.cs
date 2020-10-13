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
        public virtual async Task DeleteCustomerAttribute(CustomerAttribute customerAttribute)
        {
            await _customerAttributeRepository.Delete(customerAttribute);
        }

        /// <summary>
        /// Gets all customer attributes
        /// </summary>
        /// <returns>Customer attributes</returns>
        public virtual async Task<IList<CustomerAttribute>> GetAllCustomerAttributes()
        {
            return await _customerAttributeRepository.GetAll(query =>
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
        /// <returns>Customer attribute</returns>
        public virtual async Task<CustomerAttribute> GetCustomerAttributeById(int customerAttributeId)
        {
            return await _customerAttributeRepository.GetById(customerAttributeId, cache => default);
        }

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public virtual async Task InsertCustomerAttribute(CustomerAttribute customerAttribute)
        {
            await _customerAttributeRepository.Insert(customerAttribute);
        }

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public virtual async Task UpdateCustomerAttribute(CustomerAttribute customerAttribute)
        {
            await _customerAttributeRepository.Update(customerAttribute);
        }

        /// <summary>
        /// Deletes a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public virtual async Task DeleteCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            await _customerAttributeValueRepository.Delete(customerAttributeValue);
        }

        /// <summary>
        /// Gets customer attribute values by customer attribute identifier
        /// </summary>
        /// <param name="customerAttributeId">The customer attribute identifier</param>
        /// <returns>Customer attribute values</returns>
        public virtual async Task<IList<CustomerAttributeValue>> GetCustomerAttributeValues(int customerAttributeId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCustomerServicesDefaults.CustomerAttributeValuesByAttributeCacheKey, customerAttributeId);

            var query = from cav in _customerAttributeValueRepository.Table
                orderby cav.DisplayOrder, cav.Id
                where cav.CustomerAttributeId == customerAttributeId
                select cav;

            var customerAttributeValues = await _staticCacheManager.Get(key, async () => await query.ToAsyncEnumerable().ToListAsync());

            return customerAttributeValues;
        }

        /// <summary>
        /// Gets a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValueId">Customer attribute value identifier</param>
        /// <returns>Customer attribute value</returns>
        public virtual async Task<CustomerAttributeValue> GetCustomerAttributeValueById(int customerAttributeValueId)
        {
            return await _customerAttributeValueRepository.GetById(customerAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public virtual async Task InsertCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            await _customerAttributeValueRepository.Insert(customerAttributeValue);
        }

        /// <summary>
        /// Updates the customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public virtual async Task UpdateCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            await _customerAttributeValueRepository.Update(customerAttributeValue);
        }

        #endregion
    }
}