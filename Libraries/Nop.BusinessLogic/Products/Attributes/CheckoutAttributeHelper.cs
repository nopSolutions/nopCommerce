//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils.Html;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Products.Attributes
{
    /// <summary>
    /// Checkout attribute helper
    /// </summary>
    public class CheckoutAttributeHelper
    {
        #region Product attributes

        /// <summary>
        /// Gets selected checkout attribute identifiers
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected checkout attribute identifiers</returns>
        public static List<int> ParseCheckoutAttributeIds(string attributes)
        {
            var Ids = new List<int>();
            if (String.IsNullOrEmpty(attributes))
                return Ids;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributes);

                XmlNodeList nodeList1 = xmlDoc.SelectNodes(@"//Attributes/CheckoutAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id = 0;
                        if (int.TryParse(str1, out id))
                        {
                            Ids.Add(id);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return Ids;
        }

        /// <summary>
        /// Gets selected checkout attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Selected checkout attributes</returns>
        public static List<CheckoutAttribute> ParseCheckoutAttributes(string attributes)
        {
            var caCollection = new List<CheckoutAttribute>();
            var Ids = ParseCheckoutAttributeIds(attributes);
            foreach (int id in Ids)
            {
                var ca = IoC.Resolve<ICheckoutAttributeService>().GetCheckoutAttributeById(id);
                if (ca != null)
                {
                    caCollection.Add(ca);
                }
            }
            return caCollection;
        }

        /// <summary>
        /// Get checkout attribute values
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Checkout attribute values</returns>
        public static List<CheckoutAttributeValue> ParseCheckoutAttributeValues(string attributes)
        {
            var caValues = new List<CheckoutAttributeValue>();
            var caCollection = ParseCheckoutAttributes(attributes);
            foreach (var ca in caCollection)
            {
                if (!ca.ShouldHaveValues)
                    continue;

                var caValuesStr = ParseValues(attributes, ca.CheckoutAttributeId);
                foreach (string caValueStr in caValuesStr)
                {
                    if (!String.IsNullOrEmpty(caValueStr))
                    {
                        int caValueId = 0;
                        if (int.TryParse(caValueStr, out caValueId))
                        {
                            var caValue = IoC.Resolve<ICheckoutAttributeService>().GetCheckoutAttributeValueById(caValueId);
                            if (caValue != null)
                                caValues.Add(caValue);
                        }
                    }
                }
            }
            return caValues;
        }

        /// <summary>
        /// Gets selected checkout attribute value
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute value</returns>
        public static List<string> ParseValues(string attributes, int checkoutAttributeId)
        {
            var selectedCheckoutAttributeValues = new List<string>();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(attributes);

                XmlNodeList nodeList1 = xmlDoc.SelectNodes(@"//Attributes/CheckoutAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id = 0;
                        if (int.TryParse(str1, out id))
                        {
                            if (id == checkoutAttributeId)
                            {
                                XmlNodeList nodeList2 = node1.SelectNodes(@"CheckoutAttributeValue/Value");
                                foreach (XmlNode node2 in nodeList2)
                                {
                                    string value = node2.InnerText.Trim();
                                    selectedCheckoutAttributeValues.Add(value);
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
            return selectedCheckoutAttributeValues;
        }

        /// <summary>
        /// Adds an attribute
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="value">Value</param>
        /// <returns>Attributes</returns>
        public static string AddCheckoutAttribute(string attributes, CheckoutAttribute ca, string value)
        {
            string result = string.Empty;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                if (String.IsNullOrEmpty(attributes))
                {
                    XmlElement _element1 = xmlDoc.CreateElement("Attributes");
                    xmlDoc.AppendChild(_element1);
                }
                else
                {
                    xmlDoc.LoadXml(attributes);
                }
                XmlElement rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");

                XmlElement caElement = null;
                //find existing
                XmlNodeList nodeList1 = xmlDoc.SelectNodes(@"//Attributes/CheckoutAttribute");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id = 0;
                        if (int.TryParse(str1, out id))
                        {
                            if (id == ca.CheckoutAttributeId)
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
                    caElement = xmlDoc.CreateElement("CheckoutAttribute");
                    caElement.SetAttribute("ID", ca.CheckoutAttributeId.ToString());
                    rootElement.AppendChild(caElement);
                }

                XmlElement cavElement = xmlDoc.CreateElement("CheckoutAttributeValue");
                caElement.AppendChild(cavElement);

                XmlElement cavVElement = xmlDoc.CreateElement("Value");
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

        #endregion

        #region Formatting

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Attributes</returns>
        public static string FormatAttributes(string attributes)
        {
            var customer = NopContext.Current.User;
            return FormatAttributes(attributes, customer, "<br />");
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="customer">Customer</param>
        /// <param name="serapator">Serapator</param>
        /// <returns>Attributes</returns>
        public static string FormatAttributes(string attributes, Customer customer, string serapator)
        {
            return FormatAttributes(attributes, customer, serapator, true);
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="customer">Customer</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>Attributes</returns>
        public static string FormatAttributes(string attributes,
            Customer customer, string serapator, bool htmlEncode)
        {
            return FormatAttributes(attributes, customer, serapator, htmlEncode, true);
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="customer">Customer</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <returns>Attributes</returns>
        public static string FormatAttributes(string attributes,
            Customer customer, string serapator, bool htmlEncode, bool renderPrices)
        {
            var result = new StringBuilder();

            var caCollection = ParseCheckoutAttributes(attributes);
            for (int i = 0; i < caCollection.Count; i++)
            {
                var ca = caCollection[i];
                var valuesStr = ParseValues(attributes, ca.CheckoutAttributeId);
                for (int j = 0; j < valuesStr.Count; j++)
                {
                    string valueStr = valuesStr[j];
                    string caAttribute = string.Empty;
                    if (!ca.ShouldHaveValues)
                    {
                        if (ca.AttributeControlType == AttributeControlTypeEnum.MultilineTextbox)
                        {
                            caAttribute = string.Format("{0}: {1}", ca.LocalizedName, HtmlHelper.FormatText(valueStr, false, true, true, false, false, false));
                        }
                        else
                        {
                            caAttribute = string.Format("{0}: {1}", ca.LocalizedName, valueStr);
                        }
                    }
                    else
                    {
                        int caId = 0;
                        if (int.TryParse(valueStr, out caId))
                        {
                            var caValue = IoC.Resolve<ICheckoutAttributeService>().GetCheckoutAttributeValueById(caId);
                            if (caValue != null)
                            {
                                caAttribute = string.Format("{0}: {1}", ca.LocalizedName, caValue.LocalizedName);
                                if (renderPrices)
                                {
                                    decimal priceAdjustmentBase = IoC.Resolve<ITaxService>().GetCheckoutAttributePrice(caValue, customer);
                                    decimal priceAdjustment = IoC.Resolve<ICurrencyService>().ConvertCurrency(priceAdjustmentBase, IoC.Resolve<ICurrencyService>().PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                                    if (priceAdjustmentBase > 0)
                                    {
                                        string priceAdjustmentStr = PriceHelper.FormatPrice(priceAdjustment);
                                        caAttribute += string.Format(" [+{0}]", priceAdjustmentStr);
                                    }
                                }
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(caAttribute))
                    {
                        if (i != 0 || j != 0)
                        {
                            result.Append(serapator);
                        }
                        
                        //we don't encode multiline textbox input
                        if (htmlEncode &&
                            ca.AttributeControlType != AttributeControlTypeEnum.MultilineTextbox)
                        {
                            result.Append(HttpUtility.HtmlEncode(caAttribute));
                        }
                        else
                        {
                            result.Append(caAttribute);
                        }
                    }
                }
            }

            return result.ToString();
        }

        #endregion
    }
}
