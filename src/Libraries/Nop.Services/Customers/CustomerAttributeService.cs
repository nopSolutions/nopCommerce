using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Services.Events;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer attribute service
    /// </summary>
    public partial class CustomerAttributeService : ICustomerAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string CUSTOMERATTRIBUTES_ALL_KEY = "Nop.customerattribute.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer attribute ID
        /// </remarks>
        private const string CUSTOMERATTRIBUTES_BY_ID_KEY = "Nop.customerattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer attribute ID
        /// </remarks>
        private const string CUSTOMERATTRIBUTEVALUES_ALL_KEY = "Nop.customerattributevalue.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer attribute value ID
        /// </remarks>
        private const string CUSTOMERATTRIBUTEVALUES_BY_ID_KEY = "Nop.customerattributevalue.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CUSTOMERATTRIBUTES_PATTERN_KEY = "Nop.customerattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CUSTOMERATTRIBUTEVALUES_PATTERN_KEY = "Nop.customerattributevalue.";
        #endregion
        
        #region Fields

        private readonly IRepository<CustomerAttribute> _customerAttributeRepository;
        private readonly IRepository<CustomerAttributeValue> _customerAttributeValueRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="customerAttributeRepository">Customer attribute repository</param>
        /// <param name="customerAttributeValueRepository">Customer attribute value repository</param>
        /// <param name="eventPublisher">Event published</param>
        public CustomerAttributeService(ICacheManager cacheManager,
            IRepository<CustomerAttribute> customerAttributeRepository,
            IRepository<CustomerAttributeValue> customerAttributeValueRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._customerAttributeRepository = customerAttributeRepository;
            this._customerAttributeValueRepository = customerAttributeValueRepository;
            this._eventPublisher = eventPublisher;
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
                throw new ArgumentNullException("customerAttribute");

            _customerAttributeRepository.Delete(customerAttribute);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(customerAttribute);
        }

        /// <summary>
        /// Gets all customer attributes
        /// </summary>
        /// <returns>Customer attributes</returns>
        public virtual IList<CustomerAttribute> GetAllCustomerAttributes()
        {
            string key = CUSTOMERATTRIBUTES_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from ca in _customerAttributeRepository.Table
                            orderby ca.DisplayOrder
                            select ca;
                return query.ToList();
            });
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

            string key = string.Format(CUSTOMERATTRIBUTES_BY_ID_KEY, customerAttributeId);
            return _cacheManager.Get(key, () => _customerAttributeRepository.GetById(customerAttributeId));
        }

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        public virtual void InsertCustomerAttribute(CustomerAttribute customerAttribute)
        {
            if (customerAttribute == null)
                throw new ArgumentNullException("customerAttribute");

            _customerAttributeRepository.Insert(customerAttribute);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTEVALUES_PATTERN_KEY);

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
                throw new ArgumentNullException("customerAttribute");

            _customerAttributeRepository.Update(customerAttribute);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTEVALUES_PATTERN_KEY);

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
                throw new ArgumentNullException("customerAttributeValue");

            _customerAttributeValueRepository.Delete(customerAttributeValue);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTEVALUES_PATTERN_KEY);

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
            string key = string.Format(CUSTOMERATTRIBUTEVALUES_ALL_KEY, customerAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from cav in _customerAttributeValueRepository.Table
                            orderby cav.DisplayOrder
                            where cav.CustomerAttributeId == customerAttributeId
                            select cav;
                var customerAttributeValues = query.ToList();
                return customerAttributeValues;
            });
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

            string key = string.Format(CUSTOMERATTRIBUTEVALUES_BY_ID_KEY, customerAttributeValueId);
            return _cacheManager.Get(key, () => _customerAttributeValueRepository.GetById(customerAttributeValueId));
        }

        /// <summary>
        /// Inserts a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        public virtual void InsertCustomerAttributeValue(CustomerAttributeValue customerAttributeValue)
        {
            if (customerAttributeValue == null)
                throw new ArgumentNullException("customerAttributeValue");

            _customerAttributeValueRepository.Insert(customerAttributeValue);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTEVALUES_PATTERN_KEY);

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
                throw new ArgumentNullException("customerAttributeValue");

            _customerAttributeValueRepository.Update(customerAttributeValue);

            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CUSTOMERATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(customerAttributeValue);
        }
        
        #endregion
    }
}
