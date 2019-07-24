using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Common;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address attribute parser interface
    /// </summary>
    public partial interface IAddressAttributeParser
    {
        /// <summary>
        /// Gets selected address attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected address attributes</returns>
        IList<AddressAttribute> ParseAddressAttributes(string attributesXml);

        /// <summary>
        /// Get address attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Address attribute values</returns>
        IList<AddressAttributeValue> ParseAddressAttributeValues(string attributesXml);

        /// <summary>
        /// Gets selected address attribute value
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="addressAttributeId">Address attribute identifier</param>
        /// <returns>Address attribute value</returns>
        IList<string> ParseValues(string attributesXml, int addressAttributeId);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="attribute">Address attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        string AddAddressAttribute(string attributesXml, AddressAttribute attribute, string value);

        /// <summary>
        /// Validates address attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        IList<string> GetAttributeWarnings(string attributesXml);

        /// <summary>
        /// Get custom address attributes from the passed form
        /// </summary>
        /// <param name="form">Form values</param>
        /// <returns>Attributes in XML format</returns>
        string ParseCustomAddressAttributes(IFormCollection form);
    }
}