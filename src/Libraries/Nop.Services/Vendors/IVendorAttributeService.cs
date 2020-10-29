using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Vendors;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Represents a vendor attribute service
    /// </summary>
    public partial interface IVendorAttributeService
    {
        #region Vendor attributes

        /// <summary>
        /// Gets all vendor attributes
        /// </summary>
        /// <returns>Vendor attributes</returns>
        Task<IList<VendorAttribute>> GetAllVendorAttributesAsync();

        /// <summary>
        /// Gets a vendor attribute 
        /// </summary>
        /// <param name="vendorAttributeId">Vendor attribute identifier</param>
        /// <returns>Vendor attribute</returns>
        Task<VendorAttribute> GetVendorAttributeByIdAsync(int vendorAttributeId);

        /// <summary>
        /// Inserts a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        Task InsertVendorAttributeAsync(VendorAttribute vendorAttribute);

        /// <summary>
        /// Updates a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        Task UpdateVendorAttributeAsync(VendorAttribute vendorAttribute);

        /// <summary>
        /// Deletes a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        Task DeleteVendorAttributeAsync(VendorAttribute vendorAttribute);

        #endregion

        #region Vendor attribute values

        /// <summary>
        /// Gets vendor attribute values by vendor attribute identifier
        /// </summary>
        /// <param name="vendorAttributeId">The vendor attribute identifier</param>
        /// <returns>Vendor attribute values</returns>
        Task<IList<VendorAttributeValue>> GetVendorAttributeValuesAsync(int vendorAttributeId);

        /// <summary>
        /// Gets a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValueId">Vendor attribute value identifier</param>
        /// <returns>Vendor attribute value</returns>
        Task<VendorAttributeValue> GetVendorAttributeValueByIdAsync(int vendorAttributeValueId);

        /// <summary>
        /// Inserts a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        Task InsertVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue);

        /// <summary>
        /// Updates a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        Task UpdateVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue);

        /// <summary>
        /// Deletes a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        Task DeleteVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue);

        #endregion
    }
}