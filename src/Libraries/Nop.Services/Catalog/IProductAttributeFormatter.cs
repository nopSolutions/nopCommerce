using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;

namespace Nop.Services.Catalog;

/// <summary>
/// Product attribute formatter interface
/// </summary>
public partial interface IProductAttributeFormatter
{
    /// <summary>
    /// Formats attributes
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes
    /// </returns>
    Task<string> FormatAttributesAsync(Product product, string attributesXml);

    /// <summary>
    /// Formats attributes
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="customer">Customer</param>
    /// <param name="store">Store</param>
    /// <param name="separator">Separator</param>
    /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
    /// <param name="renderPrices">A value indicating whether to render prices</param>
    /// <param name="renderProductAttributes">A value indicating whether to render product attributes</param>
    /// <param name="renderGiftCardAttributes">A value indicating whether to render gift card attributes</param>
    /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes
    /// </returns>
    Task<string> FormatAttributesAsync(Product product, string attributesXml,
        Customer customer, Store store, string separator = "<br />", bool htmlEncode = true, bool renderPrices = true,
        bool renderProductAttributes = true, bool renderGiftCardAttributes = true,
        bool allowHyperlinks = true);
}