using System.Xml;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Attributes;

namespace Nop.Services.Orders;

/// <summary>
/// Checkout attribute parser extensions
/// </summary>
public static partial class CheckoutAttributeParserExtensions
{
    /// <summary>
    /// Removes checkout attributes which cannot be applied to the current cart and returns an update attributes in XML format
    /// </summary>
    /// <param name="parser">Checkout attribute parser</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="cart">Shopping cart items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the updated attributes in XML format
    /// </returns>
    public static async Task<string> EnsureOnlyActiveAttributesAsync(this IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> parser, string attributesXml, IList<ShoppingCartItem> cart)
    {
        if (string.IsNullOrEmpty(attributesXml))
            return attributesXml;

        var result = attributesXml;

        //removing "shippable" checkout attributes if there's no any shippable products in the cart
        //do not inject IShoppingCartService via constructor because it'll cause circular references
        var shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
        if (await shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            return result;

        //find attribute IDs to remove
        var checkoutAttributeIdsToRemove = new List<int>();
        var attributes = await parser.ParseAttributesAsync(attributesXml);

        foreach (var ca in attributes)
            if (ca.ShippableProductRequired)
                checkoutAttributeIdsToRemove.Add(ca.Id);

        //remove them from XML
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(attributesXml);

            var nodesToRemove = new List<XmlNode>();
            var nodes = xmlDoc.SelectNodes(@$"//Attributes/{nameof(CheckoutAttribute)}");
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes?["ID"] == null)
                        continue;

                    var str1 = node.Attributes["ID"].InnerText.Trim();

                    if (!int.TryParse(str1, out var id))
                        continue;

                    if (checkoutAttributeIdsToRemove.Contains(id))
                        nodesToRemove.Add(node);
                }

                foreach (var node in nodesToRemove)
                    node.ParentNode?.RemoveChild(node);
            }

            result = xmlDoc.OuterXml;
        }
        catch
        {
            //ignore
        }

        return result;
    }
}