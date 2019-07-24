using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
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
        /// <returns>Selected product attribute mappings</returns>
        IList<ProductAttributeMapping> ParseProductAttributeMappings(string attributesXml);

        /// <summary>
        /// Get product attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="productAttributeMappingId">Product attribute mapping identifier; pass 0 to load all values</param>
        /// <returns>Product attribute values</returns>
        IList<ProductAttributeValue> ParseProductAttributeValues(string attributesXml, int productAttributeMappingId = 0);

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
        /// <returns>Result</returns>
        bool AreProductAttributesEqual(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes, bool ignoreQuantity = true);

        /// <summary>
        /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
        /// </summary>
        /// <param name="pam">Product attribute</param>
        /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
        /// <returns>Result</returns>
        bool? IsConditionMet(ProductAttributeMapping pam, string selectedAttributesXml);

        /// <summary>
        /// Finds a product attribute combination by attributes stored in XML 
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <returns>Found product attribute combination</returns>
        ProductAttributeCombination FindProductAttributeCombination(Product product,
            string attributesXml, bool ignoreNonCombinableAttributes = true);

        /// <summary>
        /// Generate all combinations
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <param name="allowedAttributeIds">List of allowed attribute identifiers. If null or empty then all attributes would be used.</param>
        /// <returns>Attribute combinations in XML format</returns>
        IList<string> GenerateAllCombinations(Product product, bool ignoreNonCombinableAttributes = false, IList<int> allowedAttributeIds = null);

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
}
