using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Checkout attribute parser interface
    /// </summary>
    public partial interface ICheckoutAttributeParser
    {
        /// <summary>
        /// Gets selected checkout attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the selected checkout attributes
        /// </returns>
        Task<IList<CheckoutAttribute>> ParseCheckoutAttributesAsync(string attributesXml);

        /// <summary>
        /// Get checkout attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Checkout attribute values</returns>
        IAsyncEnumerable<(CheckoutAttribute attribute, IAsyncEnumerable<CheckoutAttributeValue> values)> ParseCheckoutAttributeValues(string attributesXml);

        /// <summary>
        /// Gets selected checkout attribute value
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute value</returns>
        IList<string> ParseValues(string attributesXml, int checkoutAttributeId);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        string AddCheckoutAttribute(string attributesXml, CheckoutAttribute ca, string value);

        /// <summary>
        /// Removes checkout attributes which cannot be applied to the current cart and returns an update attributes in XML format
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="cart">Shopping cart items</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the updated attributes in XML format
        /// </returns>
        Task<string> EnsureOnlyActiveAttributesAsync(string attributesXml, IList<ShoppingCartItem> cart);

        /// <summary>
        /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
        /// </summary>
        /// <param name="attribute">Checkout attribute</param>
        /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool?> IsConditionMetAsync(CheckoutAttribute attribute, string selectedAttributesXml);

        /// <summary>
        /// Remove an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="attribute">Checkout attribute</param>
        /// <returns>Updated result (XML format)</returns>
        string RemoveCheckoutAttribute(string attributesXml, CheckoutAttribute attribute);
    }
}
