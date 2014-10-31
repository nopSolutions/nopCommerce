using System.Collections.Generic;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer attribute parser interface
    /// </summary>
    public partial interface ICustomerAttributeParser
    {
        /// <summary>
        /// Gets selected customer attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected customer attributes</returns>
        IList<CustomerAttribute> ParseCustomerAttributes(string attributes);

        /// <summary>
        /// Get customer attribute values
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Customer attribute values</returns>
        IList<CustomerAttributeValue> ParseCustomerAttributeValues(string attributes);

        /// <summary>
        /// Gets selected customer attribute value
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>Customer attribute value</returns>
        IList<string> ParseValues(string attributes, int customerAttributeId);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="ca">Customer attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        string AddCustomerAttribute(string attributes, CustomerAttribute ca, string value);

        /// <summary>
        /// Validates customer attributes
        /// </summary>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        IList<string> GetAttributeWarnings(string selectedAttributes);
    }
}
