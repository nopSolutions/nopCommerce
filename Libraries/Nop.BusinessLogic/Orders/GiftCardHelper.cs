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
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Product attribute helper
    /// </summary>
    public class GiftCardHelper
    {
        /// <summary>
        /// Gets a gift card initial value
        /// </summary>
        /// <param name="gc">Order produvt variant</param>
        /// <returns>Gift card remaining amount</returns>
        public static decimal GetGiftCardInitialValue(GiftCard gc)
        {
            if (gc == null)
                return decimal.Zero;

            decimal result = gc.Amount;
            return result;
        }

        /// <summary>
        /// Gets a gift card remaining amount
        /// </summary>
        /// <param name="gc">Order produvt variant</param>
        /// <returns>Gift card remaining amount</returns>
        public static decimal GetGiftCardRemainingAmount(GiftCard gc)
        {
            if (gc == null)
                return decimal.Zero;

            decimal result = GetGiftCardInitialValue(gc);

            var usageHistoryCollection = IoC.Resolve<IOrderService>().GetAllGiftCardUsageHistoryEntries(gc.GiftCardId, null, null);
            foreach (var usageHistory in usageHistoryCollection)
            {
                result -= usageHistory.UsedValue;
            }

            if (result < decimal.Zero)
                result = decimal.Zero;
            result = Math.Round(result, 2);

            return result;
        }

        /// <summary>
        /// Get active gift cards
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Active gift cards</returns>
        public static List<GiftCard> GetActiveGiftCards(Customer customer)
        {
            var result = new List<GiftCard>();
            if (customer == null)
                return result;

            string[] couponCodes = GetCouponCodes(customer.GiftCardCouponCodes);
            foreach (var couponCode in couponCodes)
            {
                var _gcCollection = IoC.Resolve<IOrderService>().GetAllGiftCards(null, null,
                    null, null, null, null, null, true, couponCode);
                foreach (var _gc in _gcCollection)
                {
                    if (IsGiftCardValid(_gc))
                        result.Add(_gc);
                }
            }

            return result;
        }

        /// <summary>
        /// Is gift card valid
        /// </summary>
        /// <param name="gc">Order produvt variant</param>
        /// <returns>Result</returns>
        public static bool IsGiftCardValid(GiftCard gc)
        {
            if (gc == null)
                return false;

            decimal remainingAmount = GetGiftCardRemainingAmount(gc);
            if (remainingAmount > decimal.Zero)
                return true;
                
            return false;
        }

        /// <summary>
        /// Is gift card valid
        /// </summary>
        /// <param name="giftCardCouponCode">Gift card coupon code</param>
        /// <returns>Result</returns>
        public static bool IsGiftCardValid(string giftCardCouponCode)
        {
            if (String.IsNullOrEmpty(giftCardCouponCode))
                return false;

            var _gcCollection = IoC.Resolve<IOrderService>().GetAllGiftCards(null, null,
                    null, null, null, null, null, true, giftCardCouponCode);
            foreach (var _gc in _gcCollection)
            {
                if (IsGiftCardValid(_gc))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="giftCartCouponCodes">Existing gift cart coupon codes</param>
        /// <returns>Coupon codes</returns>
        public static string[] GetCouponCodes(string giftCartCouponCodes)
        {
            var couponCodes = new List<string>();
            if (String.IsNullOrEmpty(giftCartCouponCodes))
                return couponCodes.ToArray();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(giftCartCouponCodes);

                XmlNodeList nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["Code"] != null)
                    {
                        string _code = node1.Attributes["Code"].InnerText.Trim();
                        couponCodes.Add(_code);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return couponCodes.ToArray();
        }

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="giftCartCouponCodes">Existing gift cart coupon codes</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>New coupon codes document</returns>
        public static string AddCouponCode(string giftCartCouponCodes, string couponCode)
        {
            string result = string.Empty;
            try
            {
                couponCode = couponCode.Trim().ToLower();

                XmlDocument xmlDoc = new XmlDocument();
                if (String.IsNullOrEmpty(giftCartCouponCodes))
                {
                    XmlElement _element1 = xmlDoc.CreateElement("GiftCardCouponCodes");
                    xmlDoc.AppendChild(_element1);
                }
                else
                {
                    xmlDoc.LoadXml(giftCartCouponCodes);
                }
                XmlElement rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//GiftCardCouponCodes");

                XmlElement gcElement = null;
                //find existing
                XmlNodeList nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["Code"] != null)
                    {
                        string _couponCode = node1.Attributes["Code"].InnerText.Trim();
                        if (_couponCode.ToLower() == couponCode.ToLower())
                        {
                            gcElement = (XmlElement)node1;
                            break;
                        }
                    }
                }

                //create new one if not found
                if (gcElement == null)
                {
                    gcElement = xmlDoc.CreateElement("CouponCode");
                    gcElement.SetAttribute("Code", couponCode);
                    rootElement.AppendChild(gcElement);
                }

                result = xmlDoc.OuterXml;
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return result;
        }

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="giftCartCouponCodes">Existing gift cart coupon codes</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>New coupon codes document</returns>
        public static string RemoveCouponCode(string giftCartCouponCodes, string couponCode)
        {
            string result = string.Empty;
            var existingCouponCodes = GetCouponCodes(giftCartCouponCodes);
            foreach (string existingCouponCode in existingCouponCodes)
            {
                if (!existingCouponCode.Equals(couponCode))
                {
                    result = AddCouponCode(result, existingCouponCode);
                }
            }
            return result;
        }

        /// <summary>
        /// Generate new gift card code
        /// </summary>
        /// <returns>Result</returns>
        public static string GenerateGiftCardCode()
        {
            int length = 13;
            string result = Guid.NewGuid().ToString();
            if (result.Length > length)
                result = result.Substring(0, length);
            return result;
        }
    }
}
