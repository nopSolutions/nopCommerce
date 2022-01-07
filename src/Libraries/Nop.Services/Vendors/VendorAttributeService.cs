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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor attributes
        /// </returns>
        public virtual async Task<IList<VendorAttribute>> GetAllVendorAttributesAsync()
        {
            return await _vendorAttributeRepository.GetAllAsync(
                query => query.OrderBy(vendorAttribute => vendorAttribute.DisplayOrder).ThenBy(vendorAttribute => vendorAttribute.Id),
                cache => default);
        }

        /// <summary>
        /// Gets a vendor attribute 
        /// </summary>
        /// <param name="vendorAttributeId">Vendor attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor attribute
        /// </returns>
        public virtual async Task<VendorAttribute> GetVendorAttributeByIdAsync(int vendorAttributeId)
        {
            return await _vendorAttributeRepository.GetByIdAsync(vendorAttributeId, cache => default);
        }

        /// <summary>
        /// Inserts a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertVendorAttributeAsync(VendorAttribute vendorAttribute)
        {
            await _vendorAttributeRepository.InsertAsync(vendorAttribute);
        }

        /// <summary>
        /// Updates a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateVendorAttributeAsync(VendorAttribute vendorAttribute)
        {
            await _vendorAttributeRepository.UpdateAsync(vendorAttribute);
        }

        /// <summary>
        /// Deletes a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteVendorAttributeAsync(VendorAttribute vendorAttribute)
        {
            await _vendorAttributeRepository.DeleteAsync(vendorAttribute);
        }

        #endregion

        #region Vendor attribute values

        /// <summary>
        /// Gets vendor attribute values by vendor attribute identifier
        /// </summary>
        /// <param name="vendorAttributeId">The vendor attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor attribute values
        /// </returns>
        public virtual async Task<IList<VendorAttributeValue>> GetVendorAttributeValuesAsync(int vendorAttributeId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopVendorDefaults.VendorAttributeValuesByAttributeCacheKey, vendorAttributeId);

            var query = _vendorAttributeValueRepository.Table
                .Where(vendorAttributeValue => vendorAttributeValue.VendorAttributeId == vendorAttributeId)
                .OrderBy(vendorAttributeValue => vendorAttributeValue.DisplayOrder)
                .ThenBy(vendorAttributeValue => vendorAttributeValue.Id);

            return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
        }

        /// <summary>
        /// Gets a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValueId">Vendor attribute value identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor attribute value
        /// </returns>
        public virtual async Task<VendorAttributeValue> GetVendorAttributeValueByIdAsync(int vendorAttributeValueId)
        {
            return await _vendorAttributeValueRepository.GetByIdAsync(vendorAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue)
        {
            await _vendorAttributeValueRepository.InsertAsync(vendorAttributeValue);
        }

        /// <summary>
        /// Updates the vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue)
        {
            await _vendorAttributeValueRepository.UpdateAsync(vendorAttributeValue);
        }

        /// <summary>
        /// Deletes a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue)
        {
            await _vendorAttributeValueRepository.DeleteAsync(vendorAttributeValue);
        }

        #endregion

        #endregion
    }
}