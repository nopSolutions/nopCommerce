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
        #region Constants

        /// <summary>
        /// Key for caching all vendor attributes
        /// </summary>
        private const string VENDORATTRIBUTES_ALL_KEY = "Nop.vendorattribute.all";

        /// <summary>
        /// Key for caching a vendor attribute
        /// </summary>
        /// <remarks>
        /// {0} : vendor attribute ID
        /// </remarks>
        private const string VENDORATTRIBUTES_BY_ID_KEY = "Nop.vendorattribute.id-{0}";

        /// <summary>
        /// Key for caching vendor attribute values of the vendor attribute
        /// </summary>
        /// <remarks>
        /// {0} : vendor attribute ID
        /// </remarks>
        private const string VENDORATTRIBUTEVALUES_ALL_KEY = "Nop.vendorattributevalue.all-{0}";

        /// <summary>
        /// Key for caching a vendor attribute value
        /// </summary>
        /// <remarks>
        /// {0} : vendor attribute value ID
        /// </remarks>
        private const string VENDORATTRIBUTEVALUES_BY_ID_KEY = "Nop.vendorattributevalue.id-{0}";

        /// <summary>
        /// Key pattern to clear cached vendor attributes
        /// </summary>
        private const string VENDORATTRIBUTES_PATTERN_KEY = "Nop.vendorattribute.";

        /// <summary>
        /// Key pattern to clear cached vendor attribute values
        /// </summary>
        private const string VENDORATTRIBUTEVALUES_PATTERN_KEY = "Nop.vendorattributevalue.";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<VendorAttribute> _vendorAttributeRepository;
        private readonly IRepository<VendorAttributeValue> _vendorAttributeValueRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="vendorAttributeRepository">Vendor attribute repository</param>
        /// <param name="vendorAttributeValueRepository">Vendor attribute value repository</param>
        public VendorAttributeService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<VendorAttribute> vendorAttributeRepository,
            IRepository<VendorAttributeValue> vendorAttributeValueRepository)
        {
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
            this._vendorAttributeRepository = vendorAttributeRepository;
            this._vendorAttributeValueRepository = vendorAttributeValueRepository;
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
            var key = VENDORATTRIBUTES_ALL_KEY;
            return _cacheManager.Get(key, () =>
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

            var key = string.Format(VENDORATTRIBUTES_BY_ID_KEY, vendorAttributeId);
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

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

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

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

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

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(vendorAttribute);
        }

        #endregion

        #region Vendor attribute vallues

        /// <summary>
        /// Gets vendor attribute values by vendor attribute identifier
        /// </summary>
        /// <param name="vendorAttributeId">The vendor attribute identifier</param>
        /// <returns>Vendor attribute values</returns>
        public virtual IList<VendorAttributeValue> GetVendorAttributeValues(int vendorAttributeId)
        {
            var key = string.Format(VENDORATTRIBUTEVALUES_ALL_KEY, vendorAttributeId);
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

            var key = string.Format(VENDORATTRIBUTEVALUES_BY_ID_KEY, vendorAttributeValueId);
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

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

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

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

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

            _cacheManager.RemoveByPattern(VENDORATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(VENDORATTRIBUTEVALUES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(vendorAttributeValue);
        }

        #endregion

        #endregion
    }
}