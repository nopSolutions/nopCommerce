using System.Collections.Generic;
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
        IList<VendorAttribute> GetAllVendorAttributes();

        /// <summary>
        /// Gets a vendor attribute 
        /// </summary>
        /// <param name="vendorAttributeId">Vendor attribute identifier</param>
        /// <returns>Vendor attribute</returns>
        VendorAttribute GetVendorAttributeById(int vendorAttributeId);

        /// <summary>
        /// Inserts a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        void InsertVendorAttribute(VendorAttribute vendorAttribute);

        /// <summary>
        /// Updates a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        void UpdateVendorAttribute(VendorAttribute vendorAttribute);

        /// <summary>
        /// Deletes a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        void DeleteVendorAttribute(VendorAttribute vendorAttribute);

        #endregion

        #region Vendor attribute values

        /// <summary>
        /// Gets vendor attribute values by vendor attribute identifier
        /// </summary>
        /// <param name="vendorAttributeId">The vendor attribute identifier</param>
        /// <returns>Vendor attribute values</returns>
        IList<VendorAttributeValue> GetVendorAttributeValues(int vendorAttributeId);

        /// <summary>
        /// Gets a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValueId">Vendor attribute value identifier</param>
        /// <returns>Vendor attribute value</returns>
        VendorAttributeValue GetVendorAttributeValueById(int vendorAttributeValueId);

        /// <summary>
        /// Inserts a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        void InsertVendorAttributeValue(VendorAttributeValue vendorAttributeValue);

        /// <summary>
        /// Updates a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        void UpdateVendorAttributeValue(VendorAttributeValue vendorAttributeValue);

        /// <summary>
        /// Deletes a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        void DeleteVendorAttributeValue(VendorAttributeValue vendorAttributeValue);

        #endregion
    }
}