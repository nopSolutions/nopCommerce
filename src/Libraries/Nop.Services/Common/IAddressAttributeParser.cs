using System.Collections.Generic;
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
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected address attributes</returns>
        IList<AddressAttribute> ParseAddressAttributes(string attributes);

        /// <summary>
        /// Get address attribute values
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Address attribute values</returns>
        IList<AddressAttributeValue> ParseAddressAttributeValues(string attributes);

        /// <summary>
        /// Gets selected address attribute value
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="addressAttributeId">Address attribute identifier</param>
        /// <returns>Address attribute value</returns>
        IList<string> ParseValues(string attributes, int addressAttributeId);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="attribute">Address attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        string AddAddressAttribute(string attributes, AddressAttribute attribute, string value);

        /// <summary>
        /// Validates address attributes
        /// </summary>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        IList<string> GetAttributeWarnings(string selectedAttributes);
    }
}
