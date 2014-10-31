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
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected customer attribute identifiers</returns>
        protected virtual IList<int> ParseCustomerAttributeIds(string attributes)
        {
            var ids = new List<int>();
            if (String.IsNullOrEmpty(attributes))
                return ids;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributes);

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
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected customer attributes</returns>
        public virtual IList<CustomerAttribute> ParseCustomerAttributes(string attributes)
        {
            var caCollection = new List<CustomerAttribute>();
            var ids = ParseCustomerAttributeIds(attributes);
            foreach (int id in ids)
            {
                var ca = _customerAttributeService.GetCustomerAttributeById(id);
                if (ca != null)
                {
                    caCollection.Add(ca);
                }
            }
            return caCollection;
        }

        /// <summary>
        /// Get customer attribute values
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Customer attribute values</returns>
        public virtual IList<CustomerAttributeValue> ParseCustomerAttributeValues(string attributes)
        {
            var caValues = new List<CustomerAttributeValue>();
            var caCollection = ParseCustomerAttributes(attributes);
            foreach (var ca in caCollection)
            {
                if (!ca.ShouldHaveValues())
                    continue;

                var caValuesStr = ParseValues(attributes, ca.Id);
                foreach (string caValueStr in caValuesStr)
                {
                    if (!String.IsNullOrEmpty(caValueStr))
                    {
                        int caValueId;
                        if (int.TryParse(caValueStr, out caValueId))
                        {
                            var caValue = _customerAttributeService.GetCustomerAttributeValueById(caValueId);
                            if (caValue != null)
                                caValues.Add(caValue);
                        }
                    }
                }
            }
            return caValues;
        }

        /// <summary>
        /// Gets selected customer attribute value
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>Customer attribute value</returns>
        public virtual IList<string> ParseValues(string attributes, int customerAttributeId)
        {
            var selectedCustomerAttributeValues = new List<string>();
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributes);

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
        /// <param name="attributes">Attributes</param>
        /// <param name="ca">Customer attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        public virtual string AddCustomerAttribute(string attributes, CustomerAttribute ca, string value)
        {
            string result = string.Empty;
            try
            {
                var xmlDoc = new XmlDocument();
                if (String.IsNullOrEmpty(attributes))
                {
                    var _element1 = xmlDoc.CreateElement("Attributes");
                    xmlDoc.AppendChild(_element1);
                }
                else
                {
                    xmlDoc.LoadXml(attributes);
                }
                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

                XmlElement caElement = null;
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
                                caElement = (XmlElement)node1;
                                break;
                            }
                        }
                    }
                }

                //create new one if not found
                if (caElement == null)
                {
                    caElement = xmlDoc.CreateElement("CustomerAttribute");
                    caElement.SetAttribute("ID", ca.Id.ToString());
                    rootElement.AppendChild(caElement);
                }

                var cavElement = xmlDoc.CreateElement("CustomerAttributeValue");
                caElement.AppendChild(cavElement);

                var cavVElement = xmlDoc.CreateElement("Value");
                cavVElement.InnerText = value;
                cavElement.AppendChild(cavVElement);

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
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetAttributeWarnings(string selectedAttributes)
        {
            var warnings = new List<string>();

            //ensure it's our attributes
            var cva1Collection = ParseCustomerAttributes(selectedAttributes);

            //validate required customer attributes (whether they're chosen/selected/entered)
            var cva2Collection = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var cva2 in cva2Collection)
            {
                if (cva2.IsRequired)
                {
                    bool found = false;
                    //selected customer attributes
                    foreach (var cva1 in cva1Collection)
                    {
                        if (cva1.Id == cva2.Id)
                        {
                            var cvaValuesStr = ParseValues(selectedAttributes, cva1.Id);
                            foreach (string str1 in cvaValuesStr)
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
                        var notFoundWarning = string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), cva2.GetLocalized(a => a.Name));

                        warnings.Add(notFoundWarning);
                    }
                }
            }

            return warnings;
        }

    }
}
