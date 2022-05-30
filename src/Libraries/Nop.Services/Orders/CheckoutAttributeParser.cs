using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Common;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Checkout attribute parser
    /// </summary>
    public partial class CheckoutAttributeParser : BaseAttributeParser, ICheckoutAttributeParser
    {
        #region Fields

        private readonly ICheckoutAttributeService _checkoutAttributeService;

        #endregion

        #region Ctor

        public CheckoutAttributeParser(ICheckoutAttributeService checkoutAttributeService)
        {
            _checkoutAttributeService = checkoutAttributeService;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Gets selected checkout attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the selected checkout attributes
        /// </returns>
        public virtual async Task<IList<CheckoutAttribute>> ParseCheckoutAttributesAsync(string attributesXml)
        {
            var result = new List<CheckoutAttribute>();
            if (string.IsNullOrEmpty(attributesXml))
                return result;

            var ids = ParseAttributeIds(attributesXml);
            foreach (var id in ids)
            {
                var attribute = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(id);
                if (attribute != null) 
                    result.Add(attribute);
            }

            return result;
        }

        /// <summary>
        /// Get checkout attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Checkout attribute values</returns>
        public virtual async IAsyncEnumerable<(CheckoutAttribute attribute, IAsyncEnumerable<CheckoutAttributeValue> values)> ParseCheckoutAttributeValues(string attributesXml)
        {
            if (string.IsNullOrEmpty(attributesXml))
                yield break;

            var attributes = await ParseCheckoutAttributesAsync(attributesXml);

            foreach (var attribute in attributes)
            {
                if (!attribute.ShouldHaveValues())
                    continue;

                var valuesStr = ParseValues(attributesXml, attribute.Id);

                yield return (attribute, getValues(valuesStr));
            }

            async IAsyncEnumerable<CheckoutAttributeValue> getValues(IList<string> valuesStr)
            {
                foreach (var valueStr in valuesStr)
                {
                    if (string.IsNullOrEmpty(valueStr))
                        continue;

                    if (!int.TryParse(valueStr, out var id))
                        continue;

                    var value = await _checkoutAttributeService.GetCheckoutAttributeValueByIdAsync(id);
                    if (value != null)
                        yield return value;
                }
            }
        }

        /// <summary>
        /// Gets selected checkout attribute value
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute value</returns>
        public virtual IList<string> ParseValues(string attributesXml, int checkoutAttributeId)
        {
            var selectedCheckoutAttributeValues = new List<string>();
            if (string.IsNullOrEmpty(attributesXml))
                return selectedCheckoutAttributeValues;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/CheckoutAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["ID"] == null) 
                        continue;

                    var str1 = node1.Attributes["ID"].InnerText.Trim();
                    if (!int.TryParse(str1, out var id)) 
                        continue;

                    if (id != checkoutAttributeId) 
                        continue;

                    var nodeList2 = node1.SelectNodes(@"CheckoutAttributeValue/Value");
                    foreach (XmlNode node2 in nodeList2)
                    {
                        var value = node2.InnerText.Trim();
                        selectedCheckoutAttributeValues.Add(value);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return selectedCheckoutAttributeValues;
        }

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        public virtual string AddCheckoutAttribute(string attributesXml, CheckoutAttribute ca, string value)
        {
            var result = string.Empty;
            try
            {
                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(attributesXml))
                {
                    var element1 = xmlDoc.CreateElement("Attributes");
                    xmlDoc.AppendChild(element1);
                }
                else
                    xmlDoc.LoadXml(attributesXml);

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

                XmlElement attributeElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/CheckoutAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["ID"] == null) 
                        continue;

                    var str1 = node1.Attributes["ID"].InnerText.Trim();

                    if (!int.TryParse(str1, out var id)) 
                        continue;

                    if (id != ca.Id) 
                        continue;

                    attributeElement = (XmlElement)node1;
                    break;
                }

                //create new one if not found
                if (attributeElement == null)
                {
                    attributeElement = xmlDoc.CreateElement("CheckoutAttribute");
                    attributeElement.SetAttribute("ID", ca.Id.ToString());
                    rootElement.AppendChild(attributeElement);
                }

                var attributeValueElement = xmlDoc.CreateElement("CheckoutAttributeValue");
                attributeElement.AppendChild(attributeValueElement);

                var attributeValueValueElement = xmlDoc.CreateElement("Value");
                attributeValueValueElement.InnerText = value;
                attributeValueElement.AppendChild(attributeValueValueElement);

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return result;
        }

        /// <summary>
        /// Removes checkout attributes which cannot be applied to the current cart and returns an update attributes in XML format
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="cart">Shopping cart items</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the updated attributes in XML format
        /// </returns>
        public virtual async Task<string> EnsureOnlyActiveAttributesAsync(string attributesXml, IList<ShoppingCartItem> cart)
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
            var attributes = await ParseCheckoutAttributesAsync(attributesXml);

            foreach (var ca in attributes)
                if (ca.ShippableProductRequired)
                    checkoutAttributeIdsToRemove.Add(ca.Id);
            //remove them from XML
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                var nodesToRemove = new List<XmlNode>();
                foreach (XmlNode node in xmlDoc.SelectNodes(@"//Attributes/CheckoutAttribute"))
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
                    node.ParentNode.RemoveChild(node);

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return result;
        }

        /// <summary>
        /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
        /// </summary>
        /// <param name="attribute">Checkout attribute</param>
        /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool?> IsConditionMetAsync(CheckoutAttribute attribute, string selectedAttributesXml)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            var conditionAttributeXml = attribute.ConditionAttributeXml;
            if (string.IsNullOrEmpty(conditionAttributeXml))
                //no condition
                return null;

            //load an attribute this one depends on
            var dependOnAttribute = (await ParseCheckoutAttributesAsync(conditionAttributeXml)).FirstOrDefault();
            if (dependOnAttribute == null)
                return true;

            var valuesThatShouldBeSelected = ParseValues(conditionAttributeXml, dependOnAttribute.Id)
                //a workaround here:
                //ConditionAttributeXml can contain "empty" values (nothing is selected)
                //but in other cases (like below) we do not store empty values
                //that's why we remove empty values here
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
            var selectedValues = ParseValues(selectedAttributesXml, dependOnAttribute.Id);
            if (valuesThatShouldBeSelected.Count != selectedValues.Count)
                return false;

            //compare values
            var allFound = true;
            foreach (var t1 in valuesThatShouldBeSelected)
            {
                var found = false;
                foreach (var t2 in selectedValues)
                    if (t1 == t2)
                        found = true;
                if (!found)
                    allFound = false;
            }

            return allFound;
        }

        /// <summary>
        /// Remove an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="attribute">Checkout attribute</param>
        /// <returns>Updated result (XML format)</returns>
        public virtual string RemoveCheckoutAttribute(string attributesXml, CheckoutAttribute attribute)
        {
            return RemoveAttribute(attributesXml, attribute.Id);
        }

        #endregion

        #region Properties

        protected override string RootElementName { get; set; } = "Attributes";

        protected override string ChildElementName { get; set; } = "CheckoutAttribute";

        #endregion
    }
}