using System.Collections.Generic;
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
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected checkout attributes</returns>
        IList<CheckoutAttribute> ParseCheckoutAttributes(string attributes);

        /// <summary>
        /// Get checkout attribute values
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Checkout attribute values</returns>
        IList<CheckoutAttributeValue> ParseCheckoutAttributeValues(string attributes);

        /// <summary>
        /// Gets selected checkout attribute value
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute value</returns>
        IList<string> ParseValues(string attributes, int checkoutAttributeId);

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        string AddCheckoutAttribute(string attributes, CheckoutAttribute ca, string value);

        /// <summary>
        /// Removes checkout attributes which cannot be applied to the current cart and returns an update attributes in XML format
        /// </summary>
        /// <param name="attributes">Attributes in XML format</param>
        /// <param name="cart">Shopping cart items</param>
        /// <returns>Updated attributes in XML format</returns>
        string EnsureOnlyActiveAttributes(string attributes, IList<ShoppingCartItem> cart);
    }
}
