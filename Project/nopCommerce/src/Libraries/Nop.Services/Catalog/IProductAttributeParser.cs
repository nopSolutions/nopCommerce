using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog;

/// <summary>
/// Product attribute parser interface
/// </summary>
public partial interface IProductAttributeParser
{
    #region Product attributes

    /// <summary>
    /// Gets selected product attribute mappings
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the selected product attribute mappings
    /// </returns>
    Task<IList<ProductAttributeMapping>> ParseProductAttributeMappingsAsync(string attributesXml);

    /// <summary>
    /// Get product attribute values
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="productAttributeMappingId">Product attribute mapping identifier; pass 0 to load all values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute values
    /// </returns>
    Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(string attributesXml, int productAttributeMappingId = 0);

    /// <summary>
    /// Gets selected product attribute values
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
    /// <returns>Product attribute values</returns>
    IList<string> ParseValues(string attributesXml, int productAttributeMappingId);

    /// <summary>
    /// Adds an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="productAttributeMapping">Product attribute mapping</param>
    /// <param name="value">Value</param>
    /// <param name="quantity">Quantity (used with AttributeValueType.AssociatedToProduct to specify the quantity entered by the customer)</param>
    /// <returns>Updated result (XML format)</returns>
    string AddProductAttribute(string attributesXml, ProductAttributeMapping productAttributeMapping, string value, int? quantity = null);

    /// <summary>
    /// Remove an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="productAttributeMapping">Product attribute mapping</param>
    /// <returns>Updated result (XML format)</returns>
    string RemoveProductAttribute(string attributesXml, ProductAttributeMapping productAttributeMapping);

    /// <summary>
    /// Are attributes equal
    /// </summary>
    /// <param name="attributesXml1">The attributes of the first product</param>
    /// <param name="attributesXml2">The attributes of the second product</param>
    /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
    /// <param name="ignoreQuantity">A value indicating whether we should ignore the quantity of attribute value entered by the customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<bool> AreProductAttributesEqualAsync(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes, bool ignoreQuantity = true);

    /// <summary>
    /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
    /// </summary>
    /// <param name="pam">Product attribute</param>
    /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<bool?> IsConditionMetAsync(ProductAttributeMapping pam, string selectedAttributesXml);

    /// <summary>
    /// Finds a product attribute combination by attributes stored in XML 
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found product attribute combination
    /// </returns>
    Task<ProductAttributeCombination> FindProductAttributeCombinationAsync(Product product,
        string attributesXml, bool ignoreNonCombinableAttributes = true);

    /// <summary>
    /// Generate all combinations
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
    /// <param name="allowedAttributeIds">List of allowed attribute identifiers. If null or empty then all attributes would be used.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute combinations in XML format
    /// </returns>
    Task<IList<string>> GenerateAllCombinationsAsync(Product product, bool ignoreNonCombinableAttributes = false, IList<int> allowedAttributeIds = null);

    /// <summary>
    /// Parse a customer entered price of the product
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="form">Form</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer entered price of the product
    /// </returns>
    Task<decimal> ParseCustomerEnteredPriceAsync(Product product, IFormCollection form);

    /// <summary>
    /// Parse a entered quantity of the product
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="form">Form</param>
    /// <returns>Customer entered price of the product</returns>
    int ParseEnteredQuantity(Product product, IFormCollection form);

    /// <summary>
    /// Parse product rental dates on the product details page
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="form">Form</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    void ParseRentalDates(Product product, IFormCollection form, out DateTime? startDate, out DateTime? endDate);

    /// <summary>
    /// Get product attributes from the passed form
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="form">Form values</param>
    /// <param name="errors">Errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes in XML format
    /// </returns>
    Task<string> ParseProductAttributesAsync(Product product, IFormCollection form, List<string> errors);

    #endregion

    #region Gift card attributes

    /// <summary>
    /// Add gift card attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="recipientName">Recipient name</param>
    /// <param name="recipientEmail">Recipient email</param>
    /// <param name="senderName">Sender name</param>
    /// <param name="senderEmail">Sender email</param>
    /// <param name="giftCardMessage">Message</param>
    /// <returns>Attributes</returns>
    string AddGiftCardAttribute(string attributesXml, string recipientName,
        string recipientEmail, string senderName, string senderEmail, string giftCardMessage);

    /// <summary>
    /// Get gift card attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="recipientName">Recipient name</param>
    /// <param name="recipientEmail">Recipient email</param>
    /// <param name="senderName">Sender name</param>
    /// <param name="senderEmail">Sender email</param>
    /// <param name="giftCardMessage">Message</param>
    void GetGiftCardAttribute(string attributesXml, out string recipientName,
        out string recipientEmail, out string senderName,
        out string senderEmail, out string giftCardMessage);

    #endregion
}