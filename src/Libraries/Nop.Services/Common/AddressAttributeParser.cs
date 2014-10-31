using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address attribute parser
    /// </summary>
    public partial class AddressAttributeParser : IAddressAttributeParser
    {
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ILocalizationService _localizationService;

        public AddressAttributeParser(IAddressAttributeService addressAttributeService,
            ILocalizationService localizationService)
        {
            this._addressAttributeService = addressAttributeService;
            this._localizationService = localizationService;
        }

        /// <summary>
        /// Gets selected address attribute identifiers
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected address attribute identifiers</returns>
        protected virtual IList<int> ParseAddressAttributeIds(string attributes)
        {
            var ids = new List<int>();
            if (String.IsNullOrEmpty(attributes))
                return ids;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributes);

                foreach (XmlNode node in xmlDoc.SelectNodes(@"//Attributes/AddressAttribute"))
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
        /// Gets selected address attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected address attributes</returns>
        public virtual IList<AddressAttribute> ParseAddressAttributes(string attributes)
        {
            var caCollection = new List<AddressAttribute>();
            var ids = ParseAddressAttributeIds(attributes);
            foreach (int id in ids)
            {
                var ca = _addressAttributeService.GetAddressAttributeById(id);
                if (ca != null)
                {
                    caCollection.Add(ca);
                }
            }
            return caCollection;
        }

        /// <summary>
        /// Get address attribute values
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Address attribute values</returns>
        public virtual IList<AddressAttributeValue> ParseAddressAttributeValues(string attributes)
        {
            var caValues = new List<AddressAttributeValue>();
            var caCollection = ParseAddressAttributes(attributes);
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
                            var caValue = _addressAttributeService.GetAddressAttributeValueById(caValueId);
                            if (caValue != null)
                                caValues.Add(caValue);
                        }
                    }
                }
            }
            return caValues;
        }

        /// <summary>
        /// Gets selected address attribute value
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="addressAttributeId">Address attribute identifier</param>
        /// <returns>Address attribute value</returns>
        public virtual IList<string> ParseValues(string attributes, int addressAttributeId)
        {
            var selectedAddressAttributeValues = new List<string>();
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributes);

                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/AddressAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id;
                        if (int.TryParse(str1, out id))
                        {
                            if (id == addressAttributeId)
                            {
                                var nodeList2 = node1.SelectNodes(@"AddressAttributeValue/Value");
                                foreach (XmlNode node2 in nodeList2)
                                {
                                    string value = node2.InnerText.Trim();
                                    selectedAddressAttributeValues.Add(value);
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
            return selectedAddressAttributeValues;
        }

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="attribute">Address attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        public virtual string AddAddressAttribute(string attributes, AddressAttribute attribute, string value)
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
                var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/AddressAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id;
                        if (int.TryParse(str1, out id))
                        {
                            if (id == attribute.Id)
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
                    caElement = xmlDoc.CreateElement("AddressAttribute");
                    caElement.SetAttribute("ID", attribute.Id.ToString());
                    rootElement.AppendChild(caElement);
                }

                var cavElement = xmlDoc.CreateElement("AddressAttributeValue");
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
        /// Validates address attributes
        /// </summary>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetAttributeWarnings(string selectedAttributes)
        {
            var warnings = new List<string>();

            //ensure it's our attributes
            var cva1Collection = ParseAddressAttributes(selectedAttributes);

            //validate required address attributes (whether they're chosen/selected/entered)
            var cva2Collection = _addressAttributeService.GetAllAddressAttributes();
            foreach (var cva2 in cva2Collection)
            {
                if (cva2.IsRequired)
                {
                    bool found = false;
                    //selected address attributes
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
