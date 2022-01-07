using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Vendors;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Represents a vendor attribute parser
    /// </summary>
    public partial interface IVendorAttributeParser
    {
        /// <summary>
        /// Gets vendor attributes from XML
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of vendor attributes
        /// </returns>
        Task<IList<VendorAttribute>> ParseVendorAttributesAsync(string attributesXml);

        /// <summary>
        /// Get vendor attribute values from XML
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of vendor attribute values
        /// </returns>
        Task<IList<VendorAttributeValue>> ParseVendorAttributeValuesAsync(string attributesXml);

        /// <summary>
        /// Gets values of the selected vendor attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="vendorAttributeId">Vendor attribute identifier</param>
        /// <returns>Values of the vendor attribute</returns>
        IList<string> ParseValues(string attributesXml, int vendorAttributeId);

        /// <summary>
        /// Adds a vendor attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes in XML format</returns>
        string AddVendorAttribute(string attributesXml, VendorAttribute vendorAttribute, string value);

        /// <summary>
        /// Validates vendor attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warnings
        /// </returns>
        Task<IList<string>> GetAttributeWarningsAsync(string attributesXml);
    }
}