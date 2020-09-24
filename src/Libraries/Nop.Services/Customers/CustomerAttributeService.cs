using System.Collections.Generic;
using System.Linq;
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
        public virtual void DeleteCustomerAttribute(CustomerAttribute customerAttribute)
        {
            _customerAttributeRepository.Delete(customerAttribute);
        }

        /// <summary>
        /// Gets all customer attributes
        /// </summary>
        /// <returns>Customer attributes</returns>
        public virtual IList<CustomerAttribute> GetAllCustomerAttributes()
        {
            return _customerAttributeRepository.GetAll(query =>
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
        public virtual CustomerAttribute GetCustomerAttributeById(int customerAttributeId)
        {
            return _customerAttributeRepository.GetById(customerAttributeId, cache => default);
        }

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public virtual void InsertCustomerAttribute(CustomerAttribute customerAttribute)
        {
            _customerAttributeRepository.Insert(customerAttribute);
        }

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public virtual void UpdateCustomerAttribute(CustomerAttribute customerAttribute)
        {
            _customerAttributeRepository.Update(customerAttribute);
        }

        /// <summary>
        /// Deletes a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public virtual void DeleteCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            _customerAttributeValueRepository.Delete(customerAttributeValue);
        }

        /// <summary>
        /// Gets customer attribute values by customer attribute identifier
        /// </summary>
        /// <param name="customerAttributeId">The customer attribute identifier</param>
        /// <returns>Customer attribute values</returns>
        public virtual IList<CustomerAttributeValue> GetCustomerAttributeValues(int customerAttributeId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCustomerServicesDefaults.CustomerAttributeValuesByAttributeCacheKey, customerAttributeId);

            var query = from cav in _customerAttributeValueRepository.Table
                orderby cav.DisplayOrder, cav.Id
                where cav.CustomerAttributeId == customerAttributeId
                select cav;
            var customerAttributeValues = _staticCacheManager.Get(key, query.ToList);

            return customerAttributeValues;
        }

        /// <summary>
        /// Gets a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValueId">Customer attribute value identifier</param>
        /// <returns>Customer attribute value</returns>
        public virtual CustomerAttributeValue GetCustomerAttributeValueById(int customerAttributeValueId)
        {
            return _customerAttributeValueRepository.GetById(customerAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public virtual void InsertCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            _customerAttributeValueRepository.Insert(customerAttributeValue);
        }

        /// <summary>
        /// Updates the customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public virtual void UpdateCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            _customerAttributeValueRepository.Update(customerAttributeValue);
        }

        #endregion
    }
}