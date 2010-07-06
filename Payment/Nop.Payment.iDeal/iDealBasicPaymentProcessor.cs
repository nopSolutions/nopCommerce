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
// Contributor(s): Philip van Heijningen, _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Payment.Methods.iDeal
{
    /// <summary>
    /// iDeal payment processor
    /// </summary>
    public class iDealBasicPaymentProcessor : IPaymentMethod
    {
        #region Methods
        /// <summary>
        /// Process payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            processPaymentResult.PaymentStatus = PaymentStatusEnum.Pending;
        }

        /// <summary>
        /// Post process payment (payment gateways that require redirecting)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        public string PostProcessPayment(Order order)
        {
            string validUntil = DateTime.UtcNow.ToString("o", new CultureInfo("en-us"));
            RemotePost remotePostHelper = new RemotePost
            {
                FormName = "IdealCheckoutForm",
                Url = iDealBasicPaymentSettings.Url
            };

            remotePostHelper.Add("urlCancel", CommonHelper.GetStoreLocation(false) + "idealreturn.aspx?s=1&o=" + order.OrderId);
            remotePostHelper.Add("urlSuccess", CommonHelper.GetStoreLocation(false) + "checkoutcompleted.aspx");
            remotePostHelper.Add("urlError", CommonHelper.GetStoreLocation(false) + "idealreturn.aspx?s=2&o=" + order.OrderId);

            remotePostHelper.Add("merchantID", iDealBasicPaymentSettings.MerchantId);
            remotePostHelper.Add("subID", iDealBasicPaymentSettings.SubId);
            remotePostHelper.Add("description", "Order " + order.OrderId);

            remotePostHelper.Add("language", "nl");
            remotePostHelper.Add("currency", CurrencyManager.PrimaryStoreCurrency.CurrencyCode);
            remotePostHelper.Add("paymentType", "ideal");

            remotePostHelper.Add("purchaseID", order.OrderId.ToString());
            remotePostHelper.Add("amount", ((int)(order.OrderTotal * 100)).ToString());
            remotePostHelper.Add("validUntil", validUntil);

            StringBuilder hasString = new StringBuilder(iDealBasicPaymentSettings.HashKey);
            hasString.Append(iDealBasicPaymentSettings.MerchantId);
            hasString.Append(iDealBasicPaymentSettings.SubId);
            hasString.Append(((int)(order.OrderTotal * 100)).ToString());
            hasString.Append(order.OrderId.ToString());
            hasString.Append("ideal");
            hasString.Append(validUntil);
            ulong aantal = 0;
            int price = 0;
            int i = 1;
            foreach (OrderProductVariant product in order.OrderProductVariants)
            {
                hasString.Append(product.ProductVariantId.ToString());
                remotePostHelper.Add("itemNumber" + i, product.ProductVariantId.ToString());

                if (!string.IsNullOrEmpty(product.ProductVariant.Product.LocalizedShortDescription))
                {
                    string shortDescription = HttpUtility.UrlEncode(product.ProductVariant.Product.LocalizedShortDescription);
                    remotePostHelper.Add("itemDescription" + i, shortDescription);
                    hasString.Append(shortDescription);
                }
                else
                {
                    string fullProductName = HttpUtility.UrlEncode(product.ProductVariant.LocalizedFullProductName);
                    remotePostHelper.Add("itemDescription" + i, fullProductName);
                    hasString.Append(fullProductName);
                }
                int itemPrice = ((int)(product.PriceExclTax * 100)) / product.Quantity;
                remotePostHelper.Add("itemQuantity" + i, product.Quantity.ToString());
                remotePostHelper.Add("itemPrice" + i, itemPrice.ToString());

                hasString.Append(product.Quantity.ToString());
                hasString.Append(itemPrice.ToString());

                aantal += (ulong)product.Quantity;
                price += (int)(product.PriceExclTax * 100);
                i++;
            }

            decimal orderShippingPrice = order.OrderShippingExclTax;
            if (orderShippingPrice > 0)
            {
                price += (int)(orderShippingPrice * 100);

                remotePostHelper.Add("itemNumber" + i, "zending");
                remotePostHelper.Add("itemDescription" + i, "Verzend kosten");
                remotePostHelper.Add("itemQuantity" + i, "1");
                remotePostHelper.Add("itemPrice" + i, ((int)(orderShippingPrice * 100)).ToString());

                hasString.Append("zending");
                hasString.Append("Verzend kosten");
                hasString.Append(1);
                hasString.Append(((int)(orderShippingPrice * 100)).ToString());
                aantal++;
                i++;
            }

            // Add additional handling fee
            if (iDealBasicPaymentSettings.AdditionalFee > 0)
            {
                price += (int)(iDealBasicPaymentSettings.AdditionalFee * 100);

                remotePostHelper.Add("itemNumber" + i, "administratiekosten");
                remotePostHelper.Add("itemDescription" + i, "Administratiekosten");
                remotePostHelper.Add("itemQuantity" + i, "1");
                remotePostHelper.Add("itemPrice" + i, ((int)(iDealBasicPaymentSettings.AdditionalFee * 100)).ToString());

                hasString.Append("administratiekosten");
                hasString.Append("Administratiekosten");
                hasString.Append(1);
                hasString.Append(((int)(iDealBasicPaymentSettings.AdditionalFee * 100)).ToString());
                aantal++;
                i++;
            }

            // Add Taxes
            if (order.OrderTax > 0)
            {
                price += (int)(order.OrderTax * 100);

                remotePostHelper.Add("itemNumber" + i, "belasting");
                remotePostHelper.Add("itemDescription" + i, "Belasting");
                remotePostHelper.Add("itemQuantity" + i, "1");
                remotePostHelper.Add("itemPrice" + i, ((int)(order.OrderTax * 100)).ToString());

                hasString.Append("belasting");
                hasString.Append("Belasting");
                hasString.Append(1);
                hasString.Append(((int)(order.OrderTax * 100)).ToString());
                aantal++;
                i++;
            }

            if ((int)(order.OrderTotal * 100) != price)
            {
                int currencyconversion = (int)(order.OrderTotal * 100) - price;

                remotePostHelper.Add("itemNumber" + i, "currencyconversion");
                remotePostHelper.Add("itemDescription" + i, "currencyConversion");
                remotePostHelper.Add("itemQuantity" + i, "1");
                remotePostHelper.Add("itemPrice" + i, currencyconversion.ToString());

                hasString.Append("currencyconversion");
                hasString.Append("currencyConversion");
                hasString.Append(1);
                hasString.Append(currencyconversion.ToString());
                aantal++;
            }

            // Generate Sha1
            hasString = hasString.Replace(" ", string.Empty).Replace("\t", string.Empty).Replace("\n", string.Empty);
            string hashString = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(hasString.ToString())));
            hashString = hashString.Replace("-", string.Empty).ToLower();
            remotePostHelper.Add("hash", hashString);

            // Do post if needed
            if (aantal != 0 && price != 0)
            {
                remotePostHelper.Post();
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return iDealBasicPaymentSettings.AdditionalFee;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
        {
            throw new NopException("Capture method not supported");
        }

        /// <summary>
        /// Refunds payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void Refund(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NopException("Refund method not supported");
        }

        /// <summary>
        /// Voids payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void Void(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NopException("Void method not supported");
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessRecurringPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cancels recurring payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool CanCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool CanRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool CanVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        /// <returns>A recurring payment type of payment method</returns>
        public RecurringPaymentTypeEnum SupportRecurringPayments
        {
            get
            {
                return RecurringPaymentTypeEnum.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        /// <returns>A payment method type</returns>
        public PaymentMethodTypeEnum PaymentMethodType
        {
            get
            {
                return PaymentMethodTypeEnum.Standard;
            }
        }
        #endregion
    }
}
