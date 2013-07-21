using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product attribute parser
    /// </summary>
    public partial class ProductAttributeParser : IProductAttributeParser
    {
        #region Fields

        private readonly IProductAttributeService _productAttributeService;

        #endregion

        #region Ctor

        public ProductAttributeParser(IProductAttributeService productAttributeService)
        {
            this._productAttributeService = productAttributeService;
        }

        #endregion

        #region Product attributes

        /// <summary>
        /// Gets selected product variant attribute identifiers
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected product variant attribute identifiers</returns>
        public virtual IList<int> ParseProductVariantAttributeIds(string attributes)
        {
            var ids = new List<int>();
            if (String.IsNullOrEmpty(attributes))
                return ids;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributes);

                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/ProductVariantAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id = 0;
                        if (int.TryParse(str1, out id))
                        {
                            ids.Add(id);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return ids;
        }

        /// <summary>
        /// Gets selected product variant attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected product variant attributes</returns>
        public virtual IList<ProductVariantAttribute> ParseProductVariantAttributes(string attributes)
        {
            var pvaCollection = new List<ProductVariantAttribute>();
            var ids = ParseProductVariantAttributeIds(attributes);
            foreach (int id in ids)
            {
                var pva = _productAttributeService.GetProductVariantAttributeById(id);
                if (pva != null)
                {
                    pvaCollection.Add(pva);
                }
            }
            return pvaCollection;
        }

        /// <summary>
        /// Get product variant attribute values
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Product variant attribute values</returns>
        public virtual IList<ProductVariantAttributeValue> ParseProductVariantAttributeValues(string attributes)
        {
            var pvaValues = new List<ProductVariantAttributeValue>();
            var pvaCollection = ParseProductVariantAttributes(attributes);
            foreach (var pva in pvaCollection)
            {
                if (!pva.ShouldHaveValues())
                    continue;

                var pvaValuesStr = ParseValues(attributes, pva.Id);
                foreach (string pvaValueStr in pvaValuesStr)
                {
                    if (!String.IsNullOrEmpty(pvaValueStr))
                    {
                        int pvaValueId = 0;
                        if (int.TryParse(pvaValueStr, out pvaValueId))
                        {
                            var pvaValue = _productAttributeService.GetProductVariantAttributeValueById(pvaValueId);
                            if (pvaValue != null)
                                pvaValues.Add(pvaValue);
                        }
                    }
                }
            }
            return pvaValues;
        }

        /// <summary>
        /// Gets selected product variant attribute value
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="productVariantAttributeId">Product variant attribute identifier</param>
        /// <returns>Product variant attribute value</returns>
        public virtual IList<string> ParseValues(string attributes, int productVariantAttributeId)
        {
            var selectedProductVariantAttributeValues = new List<string>();
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributes);

                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/ProductVariantAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 =node1.Attributes["ID"].InnerText.Trim();
                        int id = 0;
                        if (int.TryParse(str1, out id))
                        {
                            if (id == productVariantAttributeId)
                            {
                                var nodeList2 = node1.SelectNodes(@"ProductVariantAttributeValue/Value");
                                foreach (XmlNode node2 in nodeList2)
                                {
                                    string value = node2.InnerText.Trim();
                                    selectedProductVariantAttributeValues.Add(value);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return selectedProductVariantAttributeValues;
        }

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="pva">Product variant attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        public virtual string AddProductAttribute(string attributes, ProductVariantAttribute pva, string value)
        {
            string result = string.Empty;
            try
            {
                var xmlDoc = new XmlDocument();
                if (String.IsNullOrEmpty(attributes))
                {
                    var element1 = xmlDoc.CreateElement("Attributes");
                    xmlDoc.AppendChild(element1);
                }
                else
                {
                    xmlDoc.LoadXml(attributes);
                }
                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

                XmlElement pvaElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/ProductVariantAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 =node1.Attributes["ID"].InnerText.Trim();
                        int id = 0;
                        if (int.TryParse(str1, out id))
                        {
                            if (id == pva.Id)
                            {
                                pvaElement = (XmlElement)node1;
                                break;
                            }
                        }
                    }
                }

                //create new one if not found
                if (pvaElement == null)
                {
                    pvaElement = xmlDoc.CreateElement("ProductVariantAttribute");
                    pvaElement.SetAttribute("ID", pva.Id.ToString());
                    rootElement.AppendChild(pvaElement);
                }

                var pvavElement = xmlDoc.CreateElement("ProductVariantAttributeValue");
                pvaElement.AppendChild(pvavElement);

                var pvavVElement = xmlDoc.CreateElement("Value");
                pvavVElement.InnerText = value;
                pvavElement.AppendChild(pvavVElement);

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return result;
        }

        /// <summary>
        /// Are attributes equal
        /// </summary>
        /// <param name="attributes1">The attributes of the first product</param>
        /// <param name="attributes2">The attributes of the second product</param>
        /// <returns>Result</returns>
        public virtual bool AreProductAttributesEqual(string attributes1, string attributes2)
        {
            bool attributesEqual = true;
            if (ParseProductVariantAttributeIds(attributes1).Count == ParseProductVariantAttributeIds(attributes2).Count)
            {
                var pva1Collection = ParseProductVariantAttributes(attributes1);
                var pva2Collection = ParseProductVariantAttributes(attributes2);
                foreach (var pva1 in pva1Collection)
                {
                    bool hasAttribute = false;
                    foreach (var pva2 in pva2Collection)
                    {
                        if (pva1.Id == pva2.Id)
                        {
                            hasAttribute = true;
                            var pvaValues1Str = ParseValues(attributes1, pva1.Id);
                            var pvaValues2Str = ParseValues(attributes2, pva2.Id);
                            if (pvaValues1Str.Count == pvaValues2Str.Count)
                            {
                                foreach (string str1 in pvaValues1Str)
                                {
                                    bool hasValue = false;
                                    foreach (string str2 in pvaValues2Str)
                                    {
                                        if (str1.Trim().ToLower() == str2.Trim().ToLower())
                                        {
                                            hasValue = true;
                                            break;
                                        }
                                    }

                                    if (!hasValue)
                                    {
                                        attributesEqual = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                attributesEqual = false;
                                break;
                            }
                        }
                    }

                    if (hasAttribute == false)
                    {
                        attributesEqual = false;
                        break;
                    }
                }
            }
            else
            {
                attributesEqual = false;
            }

            return attributesEqual;
        }

        /// <summary>
        /// Finds a product variant attribute combination by attributes stored in XML 
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Found product variant attribute combination</returns>
        public virtual ProductVariantAttributeCombination FindProductVariantAttributeCombination(Product product, 
            string attributesXml)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //existing combinations
            var combinations = _productAttributeService.GetAllProductVariantAttributeCombinations(product.Id);
            if (combinations.Count == 0)
                return null;

            foreach (var combination in combinations)
            {
                bool attributesEqual = AreProductAttributesEqual(combination.AttributesXml, attributesXml);
                if (attributesEqual)
                    return combination;
            }

            return null;
        }

        /// <summary>
        /// Generate all combinations
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Attribute combinations in XML format</returns>
        public virtual IList<string> GenerateAllCombinations(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //let's get all possible
            var allProductVariantAttributes = _productAttributeService.GetProductVariantAttributesByProductId(product.Id);
            var allPossibleAttributeCombinations = new List<List<ProductVariantAttribute>>();
            for (int counter = 0; counter < (1 << allProductVariantAttributes.Count); ++counter)
            {
                var combination = new List<ProductVariantAttribute>();
                for (int i = 0; i < allProductVariantAttributes.Count; ++i)
                {
                    if ((counter & (1 << i)) == 0)
                    {
                        combination.Add(allProductVariantAttributes[i]);
                    }
                }

                allPossibleAttributeCombinations.Add(combination);
            }

            var allAttributesXml = new List<string>();
            foreach (var combination in allPossibleAttributeCombinations)
            {
                var attributesXml = new List<string>();
                foreach (var pva in combination)
                {
                    if (!pva.ShouldHaveValues())
                        continue;

                    var pvaValues = _productAttributeService.GetProductVariantAttributeValues(pva.Id);
                    if (pvaValues.Count == 0)
                        continue;

                    //checkboxes could have several values ticked
                    var allPossibleCheckboxCombinations = new List<List<ProductVariantAttributeValue>>();
                    if (pva.AttributeControlType == AttributeControlType.Checkboxes)
                    {
                        for (int counter = 0; counter < (1 << pvaValues.Count); ++counter)
                        {
                            var checkboxCombination = new List<ProductVariantAttributeValue>();
                            for (int i = 0; i < pvaValues.Count; ++i)
                            {
                                if ((counter & (1 << i)) == 0)
                                {
                                    checkboxCombination.Add(pvaValues[i]);
                                }
                            }

                            allPossibleCheckboxCombinations.Add(checkboxCombination);
                        }
                    }

                    if (attributesXml.Count == 0)
                    {
                        //first set of values
                        if (pva.AttributeControlType == AttributeControlType.Checkboxes)
                        {
                            //checkboxes could have several values ticked
                            foreach (var checkboxCombination in allPossibleCheckboxCombinations)
                            {
                                var tmp1 = "";
                                foreach (var checkboxValue in checkboxCombination)
                                {
                                    tmp1 = AddProductAttribute(tmp1, pva, checkboxValue.Id.ToString());
                                }
                                if (!String.IsNullOrEmpty(tmp1))
                                {
                                    attributesXml.Add(tmp1);
                                }
                            }
                        }
                        else
                        {
                            //other attribute types (dropdownlist, radiobutton, color squares)
                            foreach (var pvaValue in pvaValues)
                            {
                                var tmp1 = AddProductAttribute("", pva, pvaValue.Id.ToString());
                                attributesXml.Add(tmp1);
                            }
                        }
                    }
                    else
                    {
                        //next values. let's "append" them to already generated attribute combinations in XML format
                        var attributesXmlTmp = new List<string>();
                        if (pva.AttributeControlType == AttributeControlType.Checkboxes)
                        {
                            //checkboxes could have several values ticked
                            foreach (var str1 in attributesXml)
                            {
                                foreach (var checkboxCombination in allPossibleCheckboxCombinations)
                                {
                                    var tmp1 = str1;
                                    foreach (var checkboxValue in checkboxCombination)
                                    {
                                        tmp1 = AddProductAttribute(tmp1, pva, checkboxValue.Id.ToString());
                                    }
                                    if (!String.IsNullOrEmpty(tmp1))
                                    {
                                        attributesXmlTmp.Add(tmp1);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //other attribute types (dropdownlist, radiobutton, color squares)
                            foreach (var pvaValue in pvaValues)
                            {
                                foreach (var str1 in attributesXml)
                                {
                                    var tmp1 = AddProductAttribute(str1, pva, pvaValue.Id.ToString());
                                    attributesXmlTmp.Add(tmp1);
                                }
                            }
                        }
                        attributesXml.Clear();
                        attributesXml.AddRange(attributesXmlTmp);
                    }
                }
                allAttributesXml.AddRange(attributesXml);
            }

            return allAttributesXml;
        }

        #endregion

        #region Gift card attributes

        /// <summary>
        /// Add gift card attrbibutes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="recipientName">Recipient name</param>
        /// <param name="recipientEmail">Recipient email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="giftCardMessage">Message</param>
        /// <returns>Attributes</returns>
        public string AddGiftCardAttribute(string attributes, string recipientName,
            string recipientEmail, string senderName, string senderEmail, string giftCardMessage)
        {
            string result = string.Empty;
            try
            {
                recipientName = recipientName.Trim();
                recipientEmail = recipientEmail.Trim();
                senderName = senderName.Trim();
                senderEmail = senderEmail.Trim();

                var xmlDoc = new XmlDocument();
                if (String.IsNullOrEmpty(attributes))
                {
                    var element1 = xmlDoc.CreateElement("Attributes");
                    xmlDoc.AppendChild(element1);
                }
                else
                {
                    xmlDoc.LoadXml(attributes);
                }

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

                var giftCardElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo");
                if (giftCardElement == null)
                {
                    giftCardElement = xmlDoc.CreateElement("GiftCardInfo");
                    rootElement.AppendChild(giftCardElement);
                }

                var recipientNameElement = xmlDoc.CreateElement("RecipientName");
                recipientNameElement.InnerText = recipientName;
                giftCardElement.AppendChild(recipientNameElement);

                var recipientEmailElement = xmlDoc.CreateElement("RecipientEmail");
                recipientEmailElement.InnerText = recipientEmail;
                giftCardElement.AppendChild(recipientEmailElement);

                var senderNameElement = xmlDoc.CreateElement("SenderName");
                senderNameElement.InnerText = senderName;
                giftCardElement.AppendChild(senderNameElement);

                var senderEmailElement = xmlDoc.CreateElement("SenderEmail");
                senderEmailElement.InnerText = senderEmail;
                giftCardElement.AppendChild(senderEmailElement);

                var messageElement = xmlDoc.CreateElement("Message");
                messageElement.InnerText = giftCardMessage;
                giftCardElement.AppendChild(messageElement);

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return result;
        }

        /// <summary>
        /// Get gift card attrbibutes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="recipientName">Recipient name</param>
        /// <param name="recipientEmail">Recipient email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="giftCardMessage">Message</param>
        public void GetGiftCardAttribute(string attributes, out string recipientName,
            out string recipientEmail, out string senderName,
            out string senderEmail, out string giftCardMessage)
        {
            recipientName = string.Empty;
            recipientEmail = string.Empty;
            senderName = string.Empty;
            senderEmail = string.Empty;
            giftCardMessage = string.Empty;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributes);

                var recipientNameElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/RecipientName");
                var recipientEmailElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/RecipientEmail");
                var senderNameElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/SenderName");
                var senderEmailElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/SenderEmail");
                var messageElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/Message");

                if (recipientNameElement != null)
                    recipientName = recipientNameElement.InnerText;
                if (recipientEmailElement != null)
                    recipientEmail = recipientEmailElement.InnerText;
                if (senderNameElement != null)
                    senderName = senderNameElement.InnerText;
                if (senderEmailElement != null)
                    senderEmail = senderEmailElement.InnerText;
                if (messageElement != null)
                    giftCardMessage = messageElement.InnerText;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
        }

        #endregion
    }
}
