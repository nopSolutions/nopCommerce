using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Services.Events;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address attribute service
    /// </summary>
    public partial class AddressAttributeService : IAddressAttributeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string ADDRESSATTRIBUTES_ALL_KEY = "Nop.addressattribute.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address attribute ID
        /// </remarks>
        private const string ADDRESSATTRIBUTES_BY_ID_KEY = "Nop.addressattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address attribute ID
        /// </remarks>
        private const string ADDRESSATTRIBUTEVALUES_ALL_KEY = "Nop.addressattributevalue.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : address attribute value ID
        /// </remarks>
        private const string ADDRESSATTRIBUTEVALUES_BY_ID_KEY = "Nop.addressattributevalue.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ADDRESSATTRIBUTES_PATTERN_KEY = "Nop.addressattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ADDRESSATTRIBUTEVALUES_PATTERN_KEY = "Nop.addressattributevalue.";
        #endregion
        
        #region Fields

        private readonly IRepository<AddressAttribute> _addressAttributeRepository;
        private readonly IRepository<AddressAttributeValue> _addressAttributeValueRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="addressAttributeRepository">Address attribute repository</param>
        /// <param name="addressAttributeValueRepository">Address attribute value repository</param>
        /// <param name="eventPublisher">Event published</param>
        public AddressAttributeService(ICacheManager cacheManager,
            IRepository<AddressAttribute> addressAttributeRepository,
            IRepository<AddressAttributeValue> addressAttributeValueRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._addressAttributeRepository = addressAttributeRepository;
            this._addressAttributeValueRepository = addressAttributeValueRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        public virtual void DeleteAddressAttribute(AddressAttribute addressAttribute)
        {
            if (addressAttribute == null)
                throw new ArgumentNullException("addressAttribute");

            _addressAttributeRepository.Delete(addressAttribute);

            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(addressAttribute);
        }

        /// <summary>
        /// Gets all address attributes
        /// </summary>
        /// <returns>Address attributes</returns>
        public virtual IList<AddressAttribute> GetAllAddressAttributes()
        {
            string key = ADDRESSATTRIBUTES_ALL_KEY;
            return _cacheManager.Get(key, () =>
            {
                var query = from aa in _addressAttributeRepository.Table
                            orderby aa.DisplayOrder, aa.Id
                            select aa;
                return query.ToList();
            });
        }

        /// <summary>
        /// Gets an address attribute 
        /// </summary>
        /// <param name="addressAttributeId">Address attribute identifier</param>
        /// <returns>Address attribute</returns>
        public virtual AddressAttribute GetAddressAttributeById(int addressAttributeId)
        {
            if (addressAttributeId == 0)
                return null;

            string key = string.Format(ADDRESSATTRIBUTES_BY_ID_KEY, addressAttributeId);
            return _cacheManager.Get(key, () => _addressAttributeRepository.GetById(addressAttributeId));
        }

        /// <summary>
        /// Inserts an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        public virtual void InsertAddressAttribute(AddressAttribute addressAttribute)
        {
            if (addressAttribute == null)
                throw new ArgumentNullException("addressAttribute");

            _addressAttributeRepository.Insert(addressAttribute);

            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(addressAttribute);
        }

        /// <summary>
        /// Updates the address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        public virtual void UpdateAddressAttribute(AddressAttribute addressAttribute)
        {
            if (addressAttribute == null)
                throw new ArgumentNullException("addressAttribute");

            _addressAttributeRepository.Update(addressAttribute);

            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(addressAttribute);
        }

        /// <summary>
        /// Deletes an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        public virtual void DeleteAddressAttributeValue(AddressAttributeValue addressAttributeValue)
        {
            if (addressAttributeValue == null)
                throw new ArgumentNullException("addressAttributeValue");

            _addressAttributeValueRepository.Delete(addressAttributeValue);

            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(addressAttributeValue);
        }

        /// <summary>
        /// Gets address attribute values by address attribute identifier
        /// </summary>
        /// <param name="addressAttributeId">The address attribute identifier</param>
        /// <returns>Address attribute values</returns>
        public virtual IList<AddressAttributeValue> GetAddressAttributeValues(int addressAttributeId)
        {
            string key = string.Format(ADDRESSATTRIBUTEVALUES_ALL_KEY, addressAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from aav in _addressAttributeValueRepository.Table
                            orderby aav.DisplayOrder, aav.Id
                            where aav.AddressAttributeId == addressAttributeId
                            select aav;
                var addressAttributeValues = query.ToList();
                return addressAttributeValues;
            });
        }
        
        /// <summary>
        /// Gets an address attribute value
        /// </summary>
        /// <param name="addressAttributeValueId">Address attribute value identifier</param>
        /// <returns>Address attribute value</returns>
        public virtual AddressAttributeValue GetAddressAttributeValueById(int addressAttributeValueId)
        {
            if (addressAttributeValueId == 0)
                return null;

            string key = string.Format(ADDRESSATTRIBUTEVALUES_BY_ID_KEY, addressAttributeValueId);
            return _cacheManager.Get(key, () => _addressAttributeValueRepository.GetById(addressAttributeValueId));
        }

        /// <summary>
        /// Inserts an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        public virtual void InsertAddressAttributeValue(AddressAttributeValue addressAttributeValue)
        {
            if (addressAttributeValue == null)
                throw new ArgumentNullException("addressAttributeValue");

            _addressAttributeValueRepository.Insert(addressAttributeValue);

            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(addressAttributeValue);
        }

        /// <summary>
        /// Updates the address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        public virtual void UpdateAddressAttributeValue(AddressAttributeValue addressAttributeValue)
        {
            if (addressAttributeValue == null)
                throw new ArgumentNullException("addressAttributeValue");

            _addressAttributeValueRepository.Update(addressAttributeValue);

            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(ADDRESSATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(addressAttributeValue);
        }
        
        #endregion
    }
}
