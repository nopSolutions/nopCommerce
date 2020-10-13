using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Vendors;
using Nop.Data;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Represents a vendor attribute service implementation
    /// </summary>
    public partial class VendorAttributeService : IVendorAttributeService
    {
        #region Fields

        private readonly IRepository<VendorAttribute> _vendorAttributeRepository;
        private readonly IRepository<VendorAttributeValue> _vendorAttributeValueRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public VendorAttributeService(IRepository<VendorAttribute> vendorAttributeRepository,
            IRepository<VendorAttributeValue> vendorAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        {
            _vendorAttributeRepository = vendorAttributeRepository;
            _vendorAttributeValueRepository = vendorAttributeValueRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        #region Vendor attributes

        /// <summary>
        /// Gets all vendor attributes
        /// </summary>
        /// <returns>Vendor attributes</returns>
        public virtual async Task<IList<VendorAttribute>> GetAllVendorAttributes()
        {
            return await _vendorAttributeRepository.GetAll(
                query => query.OrderBy(vendorAttribute => vendorAttribute.DisplayOrder).ThenBy(vendorAttribute => vendorAttribute.Id),
                cache => default);
        }

        /// <summary>
        /// Gets a vendor attribute 
        /// </summary>
        /// <param name="vendorAttributeId">Vendor attribute identifier</param>
        /// <returns>Vendor attribute</returns>
        public virtual async Task<VendorAttribute> GetVendorAttributeById(int vendorAttributeId)
        {
            return await _vendorAttributeRepository.GetById(vendorAttributeId, cache => default);
        }

        /// <summary>
        /// Inserts a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        public virtual async Task InsertVendorAttribute(VendorAttribute vendorAttribute)
        {
            await _vendorAttributeRepository.Insert(vendorAttribute);
        }

        /// <summary>
        /// Updates a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        public virtual async Task UpdateVendorAttribute(VendorAttribute vendorAttribute)
        {
            await _vendorAttributeRepository.Update(vendorAttribute);
        }

        /// <summary>
        /// Deletes a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        public virtual async Task DeleteVendorAttribute(VendorAttribute vendorAttribute)
        {
            await _vendorAttributeRepository.Delete(vendorAttribute);
        }

        #endregion

        #region Vendor attribute values

        /// <summary>
        /// Gets vendor attribute values by vendor attribute identifier
        /// </summary>
        /// <param name="vendorAttributeId">The vendor attribute identifier</param>
        /// <returns>Vendor attribute values</returns>
        public virtual async Task<IList<VendorAttributeValue>> GetVendorAttributeValues(int vendorAttributeId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopVendorDefaults.VendorAttributeValuesByAttributeCacheKey, vendorAttributeId);

            var query = _vendorAttributeValueRepository.Table
                .Where(vendorAttributeValue => vendorAttributeValue.VendorAttributeId == vendorAttributeId)
                .OrderBy(vendorAttributeValue => vendorAttributeValue.DisplayOrder)
                .ThenBy(vendorAttributeValue => vendorAttributeValue.Id);

            return await _staticCacheManager.Get(key, async () => await query.ToAsyncEnumerable().ToListAsync());
        }

        /// <summary>
        /// Gets a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValueId">Vendor attribute value identifier</param>
        /// <returns>Vendor attribute value</returns>
        public virtual async Task<VendorAttributeValue> GetVendorAttributeValueById(int vendorAttributeValueId)
        {
            return await _vendorAttributeValueRepository.GetById(vendorAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        public virtual async Task InsertVendorAttributeValue(VendorAttributeValue vendorAttributeValue)
        {
            await _vendorAttributeValueRepository.Insert(vendorAttributeValue);
        }

        /// <summary>
        /// Updates the vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        public virtual async Task UpdateVendorAttributeValue(VendorAttributeValue vendorAttributeValue)
        {
            await _vendorAttributeValueRepository.Update(vendorAttributeValue);
        }

        /// <summary>
        /// Deletes a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        public virtual async Task DeleteVendorAttributeValue(VendorAttributeValue vendorAttributeValue)
        {
            await _vendorAttributeValueRepository.Delete(vendorAttributeValue);
        }

        #endregion

        #endregion
    }
}