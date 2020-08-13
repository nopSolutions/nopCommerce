using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Data;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address attribute service
    /// </summary>
    public partial class AddressAttributeService : IAddressAttributeService
    {
        #region Fields

        private readonly IRepository<AddressAttribute> _addressAttributeRepository;
        private readonly IRepository<AddressAttributeValue> _addressAttributeValueRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public AddressAttributeService(IRepository<AddressAttribute> addressAttributeRepository,
            IRepository<AddressAttributeValue> addressAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        {
            _addressAttributeRepository = addressAttributeRepository;
            _addressAttributeValueRepository = addressAttributeValueRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        public virtual void DeleteAddressAttribute(AddressAttribute addressAttribute)
        {
            _addressAttributeRepository.Delete(addressAttribute);
        }

        /// <summary>
        /// Gets all address attributes
        /// </summary>
        /// <returns>Address attributes</returns>
        public virtual IList<AddressAttribute> GetAllAddressAttributes()
        {
            return _addressAttributeRepository.GetAll(query=>
            {
                return from aa in query
                    orderby aa.DisplayOrder, aa.Id
                    select aa;
            }, cache => default);
        }

        /// <summary>
        /// Gets an address attribute 
        /// </summary>
        /// <param name="addressAttributeId">Address attribute identifier</param>
        /// <returns>Address attribute</returns>
        public virtual AddressAttribute GetAddressAttributeById(int addressAttributeId)
        {
            return _addressAttributeRepository.GetById(addressAttributeId, cache => default);
        }

        /// <summary>
        /// Inserts an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        public virtual void InsertAddressAttribute(AddressAttribute addressAttribute)
        {
            _addressAttributeRepository.Insert(addressAttribute);
        }

        /// <summary>
        /// Updates the address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        public virtual void UpdateAddressAttribute(AddressAttribute addressAttribute)
        {
            _addressAttributeRepository.Update(addressAttribute);
        }

        /// <summary>
        /// Deletes an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        public virtual void DeleteAddressAttributeValue(AddressAttributeValue addressAttributeValue)
        {
            _addressAttributeValueRepository.Delete(addressAttributeValue);
        }

        /// <summary>
        /// Gets address attribute values by address attribute identifier
        /// </summary>
        /// <param name="addressAttributeId">The address attribute identifier</param>
        /// <returns>Address attribute values</returns>
        public virtual IList<AddressAttributeValue> GetAddressAttributeValues(int addressAttributeId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCommonDefaults.AddressAttributeValuesByAttributeCacheKey, addressAttributeId);

            var query = from aav in _addressAttributeValueRepository.Table
                orderby aav.DisplayOrder, aav.Id
                where aav.AddressAttributeId == addressAttributeId
                select aav;
            var addressAttributeValues = _staticCacheManager.Get(key, query.ToList);

            return addressAttributeValues;
        }

        /// <summary>
        /// Gets an address attribute value
        /// </summary>
        /// <param name="addressAttributeValueId">Address attribute value identifier</param>
        /// <returns>Address attribute value</returns>
        public virtual AddressAttributeValue GetAddressAttributeValueById(int addressAttributeValueId)
        {
            return _addressAttributeValueRepository.GetById(addressAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        public virtual void InsertAddressAttributeValue(AddressAttributeValue addressAttributeValue)
        {
            _addressAttributeValueRepository.Insert(addressAttributeValue);
        }

        /// <summary>
        /// Updates the address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        public virtual void UpdateAddressAttributeValue(AddressAttributeValue addressAttributeValue)
        {
            _addressAttributeValueRepository.Update(addressAttributeValue);
        }

        #endregion
    }
}