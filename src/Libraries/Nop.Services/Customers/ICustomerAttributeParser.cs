using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected customer attributes</returns>
        Task<IList<CustomerAttribute>> ParseCustomerAttributesAsync(string attributesXml);

        /// <summary>
        /// Get customer attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Customer attribute values</returns>
        Task<IList<CustomerAttributeValue>> ParseCustomerAttributeValuesAsync(string attributesXml);

        //TODO: migrate to an extension method
        /// <summary>
        /// Gets selected customer attribute value
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>Customer attribute value</returns>
        IList<string> ParseValues(string attributesXml, int customerAttributeId);

        //TODO: migrate to an extension method
        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ca">Customer attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        string AddCustomerAttribute(string attributesXml, CustomerAttribute ca, string value);

        /// <summary>
        /// Validates customer attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        Task<IList<string>> GetAttributeWarningsAsync(string attributesXml);
    }
}
