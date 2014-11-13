using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer attribute parser
    /// </summary>
    public partial class CustomerAttributeParser : ICustomerAttributeParser
    {
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ILocalizationService _localizationService;

        public CustomerAttributeParser(ICustomerAttributeService customerAttributeService,
            ILocalizationService localizationService)
        {
            this._customerAttributeService = customerAttributeService;
            this._localizationService = localizationService;
        }

        /// <summary>
        /// Gets selected customer attribute identifiers
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected customer attribute identifiers</returns>
        protected virtual IList<int> ParseCustomerAttributeIds(string attributesXml)
        {
            var ids = new List<int>();
            if (String.IsNullOrEmpty(attributesXml))
                return ids;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                foreach (XmlNode node in xmlDoc.SelectNodes(@"//Attributes/CustomerAttribute"))
                {
                    if (node.Attributes != null && node.Attributes["ID"] != null)
                    {
                        string str1 = node.Attributes["ID"].InnerText.Trim();
                        int id;
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
        /// Gets selected customer attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Selected customer attributes</returns>
        public virtual IList<CustomerAttribute> ParseCustomerAttributes(string attributesXml)
        {
            var result = new List<CustomerAttribute>();
            var ids = ParseCustomerAttributeIds(attributesXml);
            foreach (int id in ids)
            {
                var attribute = _customerAttributeService.GetCustomerAttributeById(id);
                if (attribute != null)
                {
                    result.Add(attribute);
                }
            }
            return result;
        }

        /// <summary>
        /// Get customer attribute values
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Customer attribute values</returns>
        public virtual IList<CustomerAttributeValue> ParseCustomerAttributeValues(string attributesXml)
        {
            var values = new List<CustomerAttributeValue>();
            var attributes = ParseCustomerAttributes(attributesXml);
            foreach (var attribute in attributes)
            {
                if (!attribute.ShouldHaveValues())
                    continue;

                var valuesStr = ParseValues(attributesXml, attribute.Id);
                foreach (string valueStr in valuesStr)
                {
                    if (!String.IsNullOrEmpty(valueStr))
                    {
                        int id;
                        if (int.TryParse(valueStr, out id))
                        {
                            var value = _customerAttributeService.GetCustomerAttributeValueById(id);
                            if (value != null)
                                values.Add(value);
                        }
                    }
                }
            }
            return values;
        }

        /// <summary>
        /// Gets selected customer attribute value
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>Customer attribute value</returns>
        public virtual IList<string> ParseValues(string attributesXml, int customerAttributeId)
        {
            var selectedCustomerAttributeValues = new List<string>();
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributesXml);

                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/CustomerAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id;
                        if (int.TryParse(str1, out id))
                        {
                            if (id == customerAttributeId)
                            {
                                var nodeList2 = node1.SelectNodes(@"CustomerAttributeValue/Value");
                                foreach (XmlNode node2 in nodeList2)
                                {
                                    string value = node2.InnerText.Trim();
                                    selectedCustomerAttributeValues.Add(value);
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
            return selectedCustomerAttributeValues;
        }

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ca">Customer attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        public virtual string AddCustomerAttribute(string attributesXml, CustomerAttribute ca, string value)
        {
            string result = string.Empty;
            try
            {
                var xmlDoc = new XmlDocument();
                if (String.IsNullOrEmpty(attributesXml))
                {
                    var element1 = xmlDoc.CreateElement("Attributes");
                    xmlDoc.AppendChild(element1);
                }
                else
                {
                    xmlDoc.LoadXml(attributesXml);
                }
                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

                XmlElement attributeElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/CustomerAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id;
                        if (int.TryParse(str1, out id))
                        {
                            if (id == ca.Id)
                            {
                                attributeElement = (XmlElement)node1;
                                break;
                            }
                        }
                    }
                }

                //create new one if not found
                if (attributeElement == null)
                {
                    attributeElement = xmlDoc.CreateElement("CustomerAttribute");
                    attributeElement.SetAttribute("ID", ca.Id.ToString());
                    rootElement.AppendChild(attributeElement);
                }

                var attributeValueElement = xmlDoc.CreateElement("CustomerAttributeValue");
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
        /// Validates customer attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetAttributeWarnings(string attributesXml)
        {
            var warnings = new List<string>();

            //ensure it's our attributes
            var attributes1 = ParseCustomerAttributes(attributesXml);

            //validate required customer attributes (whether they're chosen/selected/entered)
            var attributes2 = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var a2 in attributes2)
            {
                if (a2.IsRequired)
                {
                    bool found = false;
                    //selected customer attributes
                    foreach (var a1 in attributes1)
                    {
                        if (a1.Id == a2.Id)
                        {
                            var valuesStr = ParseValues(attributesXml, a1.Id);
                            foreach (string str1 in valuesStr)
                            {
                                if (!String.IsNullOrEmpty(str1.Trim()))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                    //if not found
                    if (!found)
                    {
                        var notFoundWarning = string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), a2.GetLocalized(a => a.Name));

                        warnings.Add(notFoundWarning);
                    }
                }
            }

            return warnings;
        }

    }
}
