using System.Collections.Generic;
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
        /// <returns>List of vendor attributes</returns>
        IList<VendorAttribute> ParseVendorAttributes(string attributesXml);

        /// <summary>
        /// Get vendor attribute values from XML
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>List of vendor attribute values</returns>
        IList<VendorAttributeValue> ParseVendorAttributeValues(string attributesXml);

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
        /// <returns>Warnings</returns>
        IList<string> GetAttributeWarnings(string attributesXml);
    }
}