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
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<AddressAttribute> _addressAttributeRepository;
        private readonly IRepository<AddressAttributeValue> _addressAttributeValueRepository;

        #endregion

        #region Ctor

        public AddressAttributeService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<AddressAttribute> addressAttributeRepository,
            IRepository<AddressAttributeValue> addressAttributeValueRepository)
        {
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
            this._addressAttributeRepository = addressAttributeRepository;
            this._addressAttributeValueRepository = addressAttributeValueRepository;
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
                throw new ArgumentNullException(nameof(addressAttribute));

            _addressAttributeRepository.Delete(addressAttribute);

            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributeValuesPatternCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(addressAttribute);
        }

        /// <summary>
        /// Gets all address attributes
        /// </summary>
        /// <returns>Address attributes</returns>
        public virtual IList<AddressAttribute> GetAllAddressAttributes()
        {
            return _cacheManager.Get(NopCommonDefaults.AddressAttributesAllCacheKey, () =>
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

            var key = string.Format(NopCommonDefaults.AddressAttributesByIdCacheKey, addressAttributeId);
            return _cacheManager.Get(key, () => _addressAttributeRepository.GetById(addressAttributeId));
        }

        /// <summary>
        /// Inserts an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        public virtual void InsertAddressAttribute(AddressAttribute addressAttribute)
        {
            if (addressAttribute == null)
                throw new ArgumentNullException(nameof(addressAttribute));

            _addressAttributeRepository.Insert(addressAttribute);

            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributeValuesPatternCacheKey);

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
                throw new ArgumentNullException(nameof(addressAttribute));

            _addressAttributeRepository.Update(addressAttribute);

            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributeValuesPatternCacheKey);

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
                throw new ArgumentNullException(nameof(addressAttributeValue));

            _addressAttributeValueRepository.Delete(addressAttributeValue);

            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributeValuesPatternCacheKey);

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
            var key = string.Format(NopCommonDefaults.AddressAttributeValuesAllCacheKey, addressAttributeId);
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

            var key = string.Format(NopCommonDefaults.AddressAttributeValuesByIdCacheKey, addressAttributeValueId);
            return _cacheManager.Get(key, () => _addressAttributeValueRepository.GetById(addressAttributeValueId));
        }

        /// <summary>
        /// Inserts an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        public virtual void InsertAddressAttributeValue(AddressAttributeValue addressAttributeValue)
        {
            if (addressAttributeValue == null)
                throw new ArgumentNullException(nameof(addressAttributeValue));

            _addressAttributeValueRepository.Insert(addressAttributeValue);

            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributeValuesPatternCacheKey);

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
                throw new ArgumentNullException(nameof(addressAttributeValue));

            _addressAttributeValueRepository.Update(addressAttributeValue);

            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributesPatternCacheKey);
            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressAttributeValuesPatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(addressAttributeValue);
        }

        #endregion
    }
}