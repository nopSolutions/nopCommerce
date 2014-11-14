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
        /// Gets selected product attribute maping identifiers
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected product attribute mapping identifiers</returns>
        IList<int> ParseProductAttributeMappingIds(string attributesXml);

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
        /// <returns>Product attribute values</returns>
        IList<ProductAttributeValue> ParseProductAttributeValues(string attributesXml);

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
        /// <returns>Attributes</returns>
        string AddProductAttribute(string attributesXml, ProductAttributeMapping productAttributeMapping, string value);

        /// <summary>
        /// Are attributes equal
        /// </summary>
        /// <param name="attributesXml1">The attributes of the first product</param>
        /// <param name="attributesXml2">The attributes of the second product</param>
        /// <returns>Result</returns>
        bool AreProductAttributesEqual(string attributesXml1, string attributesXml2);

        /// <summary>
        /// Finds a product attribute combination by attributes stored in XML 
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Found product attribute combination</returns>
        ProductAttributeCombination FindProductAttributeCombination(Product product,
            string attributesXml);

        /// <summary>
        /// Generate all combinations
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Attribute combinations in XML format</returns>
        IList<string> GenerateAllCombinations(Product product);

        #endregion

        #region Gift card attributes

        /// <summary>
        /// Add gift card attrbibutes
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
        /// Get gift card attrbibutes
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
