using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Vendors;
using Nop.Services.Events;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Represents a vendor attribute service implementation
    /// </summary>
    public partial class VendorAttributeService : IVendorAttributeService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<VendorAttribute> _vendorAttributeRepository;
        private readonly IRepository<VendorAttributeValue> _vendorAttributeValueRepository;

        #endregion

        #region Ctor

        public VendorAttributeService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<VendorAttribute> vendorAttributeRepository,
            IRepository<VendorAttributeValue> vendorAttributeValueRepository)
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _vendorAttributeRepository = vendorAttributeRepository;
            _vendorAttributeValueRepository = vendorAttributeValueRepository;
        }

        #endregion

        #region Methods

        #region Vendor attributes

        /// <summary>
        /// Gets all vendor attributes
        /// </summary>
        /// <returns>Vendor attributes</returns>
        public virtual IList<VendorAttribute> GetAllVendorAttributes()
        {
            return _cacheManager.Get(NopVendorsServiceDefaults.VendorAttributesAllCacheKey, () =>
            {
                return _vendorAttributeRepository.Table
                    .OrderBy(vendorAttribute => vendorAttribute.DisplayOrder).ThenBy(vendorAttribute => vendorAttribute.Id)
                    .ToList();
            });
        }

        /// <summary>
        /// Gets a vendor attribute 
        /// </summary>
        /// <param name="vendorAttributeId">Vendor attribute identifier</param>
        /// <returns>Vendor attribute</returns>
        public virtual VendorAttribute GetVendorAttributeById(int vendorAttributeId)
        {
            if (vendorAttributeId == 0)
                return null;

            var key = string.Format(NopVendorsServiceDefaults.VendorAttributesByIdCacheKey, vendorAttributeId);
            return _cacheManager.Get(key, () => _vendorAttributeRepository.GetById(vendorAttributeId));
        }

        /// <summary>
        /// Inserts a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        public virtual void InsertVendorAttribute(VendorAttribute vendorAttribute)
        {
            if (vendorAttribute == null)
                throw new ArgumentNullException(nameof(vendorAttribute));

            _vendorAttributeRepository.Insert(vendorAttribute);

            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(vendorAttribute);
        }

        /// <summary>
        /// Updates a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        public virtual void UpdateVendorAttribute(VendorAttribute vendorAttribute)
        {
            if (vendorAttribute == null)
                throw new ArgumentNullException(nameof(vendorAttribute));

            _vendorAttributeRepository.Update(vendorAttribute);

            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(vendorAttribute);
        }

        /// <summary>
        /// Deletes a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        public virtual void DeleteVendorAttribute(VendorAttribute vendorAttribute)
        {
            if (vendorAttribute == null)
                throw new ArgumentNullException(nameof(vendorAttribute));

            _vendorAttributeRepository.Delete(vendorAttribute);

            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(vendorAttribute);
        }

        #endregion

        #region Vendor attribute values

        /// <summary>
        /// Gets vendor attribute values by vendor attribute identifier
        /// </summary>
        /// <param name="vendorAttributeId">The vendor attribute identifier</param>
        /// <returns>Vendor attribute values</returns>
        public virtual IList<VendorAttributeValue> GetVendorAttributeValues(int vendorAttributeId)
        {
            var key = string.Format(NopVendorsServiceDefaults.VendorAttributeValuesAllCacheKey, vendorAttributeId);
            return _cacheManager.Get(key, () =>
            {
                return _vendorAttributeValueRepository.Table
                    .OrderBy(vendorAttributeValue => vendorAttributeValue.DisplayOrder).ThenBy(vendorAttributeValue => vendorAttributeValue.Id)
                    .Where(vendorAttributeValue => vendorAttributeValue.VendorAttributeId == vendorAttributeId)
                    .ToList();
            });
        }

        /// <summary>
        /// Gets a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValueId">Vendor attribute value identifier</param>
        /// <returns>Vendor attribute value</returns>
        public virtual VendorAttributeValue GetVendorAttributeValueById(int vendorAttributeValueId)
        {
            if (vendorAttributeValueId == 0)
                return null;

            var key = string.Format(NopVendorsServiceDefaults.VendorAttributeValuesByIdCacheKey, vendorAttributeValueId);
            return _cacheManager.Get(key, () => _vendorAttributeValueRepository.GetById(vendorAttributeValueId));
        }

        /// <summary>
        /// Inserts a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        public virtual void InsertVendorAttributeValue(VendorAttributeValue vendorAttributeValue)
        {
            if (vendorAttributeValue == null)
                throw new ArgumentNullException(nameof(vendorAttributeValue));

            _vendorAttributeValueRepository.Insert(vendorAttributeValue);

            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(vendorAttributeValue);
        }

        /// <summary>
        /// Updates the vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        public virtual void UpdateVendorAttributeValue(VendorAttributeValue vendorAttributeValue)
        {
            if (vendorAttributeValue == null)
                throw new ArgumentNullException(nameof(vendorAttributeValue));

            _vendorAttributeValueRepository.Update(vendorAttributeValue);

            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(vendorAttributeValue);
        }

        /// <summary>
        /// Deletes a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        public virtual void DeleteVendorAttributeValue(VendorAttributeValue vendorAttributeValue)
        {
            if (vendorAttributeValue == null)
                throw new ArgumentNullException(nameof(vendorAttributeValue));

            _vendorAttributeValueRepository.Delete(vendorAttributeValue);

            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributesPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopVendorsServiceDefaults.VendorAttributeValuesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(vendorAttributeValue);
        }

        #endregion

        #endregion
    }
}