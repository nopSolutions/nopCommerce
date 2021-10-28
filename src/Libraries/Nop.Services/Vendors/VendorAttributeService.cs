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

        protected IRepository<VendorAttribute> VendorAttributeRepository { get; }
        protected IRepository<VendorAttributeValue> VendorAttributeValueRepository { get; }
        protected IStaticCacheManager StaticCacheManager { get; }

        #endregion

        #region Ctor

        public VendorAttributeService(IRepository<VendorAttribute> vendorAttributeRepository,
            IRepository<VendorAttributeValue> vendorAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        {
            VendorAttributeRepository = vendorAttributeRepository;
            VendorAttributeValueRepository = vendorAttributeValueRepository;
            StaticCacheManager = staticCacheManager;
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
            return await VendorAttributeRepository.GetAllAsync(
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
            return await VendorAttributeRepository.GetByIdAsync(vendorAttributeId, cache => default);
        }

        /// <summary>
        /// Inserts a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertVendorAttributeAsync(VendorAttribute vendorAttribute)
        {
            await VendorAttributeRepository.InsertAsync(vendorAttribute);
        }

        /// <summary>
        /// Updates a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateVendorAttributeAsync(VendorAttribute vendorAttribute)
        {
            await VendorAttributeRepository.UpdateAsync(vendorAttribute);
        }

        /// <summary>
        /// Deletes a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteVendorAttributeAsync(VendorAttribute vendorAttribute)
        {
            await VendorAttributeRepository.DeleteAsync(vendorAttribute);
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
            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopVendorDefaults.VendorAttributeValuesByAttributeCacheKey, vendorAttributeId);

            var query = VendorAttributeValueRepository.Table
                .Where(vendorAttributeValue => vendorAttributeValue.VendorAttributeId == vendorAttributeId)
                .OrderBy(vendorAttributeValue => vendorAttributeValue.DisplayOrder)
                .ThenBy(vendorAttributeValue => vendorAttributeValue.Id);

            return await StaticCacheManager.GetAsync(key, async () => await query.ToListAsync());
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
            return await VendorAttributeValueRepository.GetByIdAsync(vendorAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue)
        {
            await VendorAttributeValueRepository.InsertAsync(vendorAttributeValue);
        }

        /// <summary>
        /// Updates the vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue)
        {
            await VendorAttributeValueRepository.UpdateAsync(vendorAttributeValue);
        }

        /// <summary>
        /// Deletes a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue)
        {
            await VendorAttributeValueRepository.DeleteAsync(vendorAttributeValue);
        }

        #endregion

        #endregion
    }
}