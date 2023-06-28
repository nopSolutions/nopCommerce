using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Attributes;

namespace Nop.Services.Attributes
{
    /// <summary>
    /// Represents an attribute parser
    /// </summary>
    /// <typeparam name="TAttribute">Type of the attribute (see <see cref="BaseAttribute"/>)</typeparam>
    /// <typeparam name="TAttributeValue">Type of the attribute value (see <see cref="BaseAttributeValue"/>)</typeparam>
    public partial interface IAttributeParser<TAttribute, TAttributeValue>
        where TAttribute : BaseAttribute
        where TAttributeValue : BaseAttributeValue
    {
        /// <summary>
        /// Gets selected attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the selected attributes
        /// </returns>
        Task<IList<TAttribute>> ParseAttributesAsync(string attributesXml);

        /// <summary>
        /// Gets selected attribute identifiers
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected attribute identifiers</returns>
        IEnumerable<int> ParseAttributeIds(string attributesXml);

        /// <summary>
        /// Remove an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="attributeId">Attribute identifier</param>
        /// <returns>Updated result (XML format)</returns>
        string RemoveAttribute(string attributesXml, int attributeId);

        /// <summary>
        /// Get custom attributes from the passed form
        /// </summary>
        /// <param name="form">Form values</param>
        /// <param name="attributeControlName">Name of the attribute control</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the attributes in XML format
        /// </returns>
        Task<string> ParseCustomAttributesAsync(IFormCollection form, string attributeControlName);

        /// <summary>
        /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
        /// </summary>
        /// <param name="conditionAttributeXml">Condition attributes (XML format)</param>
        /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool?> IsConditionMetAsync(string conditionAttributeXml, string selectedAttributesXml);

        /// <summary>
        /// Gets selected attribute value
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="attributeId">Attribute identifier</param>
        /// <returns>Attribute value</returns>
        IList<string> ParseValues(string attributesXml, int attributeId);

        /// <summary>
        /// Get attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the attribute values
        /// </returns>
        Task<IList<TAttributeValue>> ParseAttributeValuesAsync(string attributesXml);

        /// <summary>
        /// Get attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Attribute values</returns>
        IAsyncEnumerable<(TAttribute attribute, IAsyncEnumerable<TAttributeValue> values)> ParseAttributeValues(string attributesXml);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="attribute">Attribute</param>
        /// <param name="value">Attribute value</param>
        /// <returns>Attributes</returns>
        string AddAttribute(string attributesXml, TAttribute attribute, string value);

        /// <summary>
        /// Validates attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warnings
        /// </returns>
        Task<IList<string>> GetAttributeWarningsAsync(string attributesXml);
    }
}
