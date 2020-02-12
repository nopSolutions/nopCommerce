using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Caching.CachingDefaults;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer attribute service
    /// </summary>
    public partial class CustomerAttributeService : ICustomerAttributeService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<CustomerAttribute> _customerAttributeRepository;
        private readonly IRepository<CustomerAttributeValue> _customerAttributeValueRepository;

        #endregion

        #region Ctor

        public CustomerAttributeService(IEventPublisher eventPublisher,
            IRepository<CustomerAttribute> customerAttributeRepository,
            IRepository<CustomerAttributeValue> customerAttributeValueRepository)
        {
            _eventPublisher = eventPublisher;
            _customerAttributeRepository = customerAttributeRepository;
            _customerAttributeValueRepository = customerAttributeValueRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public virtual void DeleteCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException(nameof(customerAttribute));

            _customerAttributeRepository.Delete(customerAttribute);

            //event notification
            _eventPublisher.EntityDeleted(customerAttribute);
        }

        /// <summary>
        /// Gets all customer attributes
        /// </summary>
        /// <returns>Customer attributes</returns>
        public virtual IList<CustomerAttribute> GetAllCustomerAttributes()
        {
            var query = from ca in _customerAttributeRepository.Table
                orderby ca.DisplayOrder, ca.Id
                select ca;

            return query.ToCachedList(NopCustomerServiceCachingDefaults.CustomerAttributesAllCacheKey);
        }

        /// <summary>
        /// Gets a customer attribute 
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>Customer attribute</returns>
        public virtual CustomerAttribute GetCustomerAttributeById(int customerAttributeId)
        {
            if (customerAttributeId == 0)
                return null;

            var key = string.Format(NopCustomerServiceCachingDefaults.CustomerAttributesByIdCacheKey, customerAttributeId);

            return _customerAttributeRepository.ToCachedGetById(customerAttributeId, key);
        }

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public virtual void InsertCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException(nameof(customerAttribute));

            _customerAttributeRepository.Insert(customerAttribute);
            
            //event notification
            _eventPublisher.EntityInserted(customerAttribute);
        }

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public virtual void UpdateCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException(nameof(customerAttribute));

            _customerAttributeRepository.Update(customerAttribute);

            //event notification
            _eventPublisher.EntityUpdated(customerAttribute);
        }

        /// <summary>
        /// Deletes a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public virtual void DeleteCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            if (customerAttributeValue == null)
                throw new ArgumentNullException(nameof(customerAttributeValue));

            _customerAttributeValueRepository.Delete(customerAttributeValue);

            //event notification
            _eventPublisher.EntityDeleted(customerAttributeValue);
        }

        /// <summary>
        /// Gets customer attribute values by customer attribute identifier
        /// </summary>
        /// <param name="customerAttributeId">The customer attribute identifier</param>
        /// <returns>Customer attribute values</returns>
        public virtual IList<CustomerAttributeValue> GetCustomerAttributeValues(int customerAttributeId)
        {
            var key = string.Format(NopCustomerServiceCachingDefaults.CustomerAttributeValuesAllCacheKey, customerAttributeId);

            var query = from cav in _customerAttributeValueRepository.Table
                orderby cav.DisplayOrder, cav.Id
                where cav.CustomerAttributeId == customerAttributeId
                select cav;
            var customerAttributeValues = query.ToCachedList(key);

            return customerAttributeValues;
        }

        /// <summary>
        /// Gets a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValueId">Customer attribute value identifier</param>
        /// <returns>Customer attribute value</returns>
        public virtual CustomerAttributeValue GetCustomerAttributeValueById(int customerAttributeValueId)
        {
            if (customerAttributeValueId == 0)
                return null;

            var key = string.Format(NopCustomerServiceCachingDefaults.CustomerAttributeValuesByIdCacheKey, customerAttributeValueId);

            return _customerAttributeValueRepository.ToCachedGetById(customerAttributeValueId, key);
        }

        /// <summary>
        /// Inserts a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public virtual void InsertCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            if (customerAttributeValue == null)
                throw new ArgumentNullException(nameof(customerAttributeValue));

            _customerAttributeValueRepository.Insert(customerAttributeValue);

            //event notification
            _eventPublisher.EntityInserted(customerAttributeValue);
        }

        /// <summary>
        /// Updates the customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public virtual void UpdateCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            if (customerAttributeValue == null)
                throw new ArgumentNullException(nameof(customerAttributeValue));

            _customerAttributeValueRepository.Update(customerAttributeValue);

            //event notification
            _eventPublisher.EntityUpdated(customerAttributeValue);
        }

        #endregion
    }
}