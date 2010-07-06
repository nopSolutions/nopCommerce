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
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;


namespace NopSolutions.NopCommerce.Payment.Methods.TwoCheckout
{
    /// <summary>
    /// 2Checkout payment processor
    /// </summary>
    public class TwoCheckoutPaymentProcessor : IPaymentMethod
    {
        #region Fields
        private bool useSandBox = true;
        private string vendorID;
        private string serverURL = "https://www.2checkout.com/2co/buyer/purchase";
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the TwoCheckoutPaymentProcessor class
        /// </summary>
        public TwoCheckoutPaymentProcessor()
        {
            useSandBox = SettingManager.GetSettingValueBoolean("PaymentMethod.TwoCheckout.UseSandbox");
            vendorID = SettingManager.GetSettingValue("PaymentMethod.TwoCheckout.VendorID");

            if (string.IsNullOrEmpty(vendorID))
                throw new NopException("2Checkout Vendor Id is empty");
        }
        #endregion

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
            string returnURL = CommonHelper.GetStoreLocation(false) + "TwoCheckoutReturn.aspx";

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}?id_type=1", serverURL);
            
            //products
            var orderProductVariants = order.OrderProductVariants;
            for (int i = 0; i < orderProductVariants.Count; i++)
            {
                int pNum = i + 1;
                OrderProductVariant opv = orderProductVariants[i];
                ProductVariant pv = orderProductVariants[i].ProductVariant;
                Product product = pv.Product;

                string c_prod = string.Format("c_prod_{0}", pNum);
                string c_prod_value = string.Format("{0},{1}", pv.SKU, opv.Quantity);
                builder.AppendFormat("&{0}={1}", c_prod, c_prod_value);
                string c_name = string.Format("c_name_{0}", pNum);
                string c_name_value = pv.LocalizedFullProductName;
                builder.AppendFormat("&{0}={1}", HttpUtility.UrlEncode(c_name), HttpUtility.UrlEncode(c_name_value));

                string c_description = string.Format("c_description_{0}", pNum);
                string c_description_value = pv.LocalizedFullProductName;
                if (!String.IsNullOrEmpty(opv.AttributeDescription))
                {
                    c_description_value = c_description_value + ". " + opv.AttributeDescription;
                    c_description_value = c_description_value.Replace("<br />", ". ");
                }
                builder.AppendFormat("&{0}={1}", HttpUtility.UrlEncode(c_description), HttpUtility.UrlEncode(c_description_value));

                string c_price = string.Format("c_price_{0}", pNum);
                string c_price_value = opv.UnitPriceInclTax.ToString("0.00", CultureInfo.InvariantCulture);
                builder.AppendFormat("&{0}={1}", c_price, c_price_value);

                string c_tangible = string.Format("c_tangible_{0}", pNum);
                string c_tangible_value = "Y";
                if (pv.IsDownload)
                {
                    c_tangible_value = "N";
                }
                builder.AppendFormat("&{0}={1}", c_tangible, c_tangible_value);
            }

            builder.AppendFormat("&x_login={0}", vendorID);
            builder.AppendFormat("&x_amount={0}", order.OrderTotal.ToString("0.00", CultureInfo.InvariantCulture));
            builder.AppendFormat("&x_invoice_num={0}", order.OrderId);
            //("x_receipt_link_url", returnURL);
            //("x_return_url", returnURL);
            //("x_return", returnURL);
            if (useSandBox)
                builder.AppendFormat("&demo=Y");
            builder.AppendFormat("&x_First_Name={0}",HttpUtility.UrlEncode(order.BillingFirstName));
            builder.AppendFormat("&x_Last_Name={0}",  HttpUtility.UrlEncode(order.BillingLastName));
            builder.AppendFormat("&x_Address={0}",  HttpUtility.UrlEncode(order.BillingAddress1));
            builder.AppendFormat("&x_City={0}", HttpUtility.UrlEncode(order.BillingCity));
            StateProvince billingStateProvince = StateProvinceManager.GetStateProvinceById(order.BillingStateProvinceId);
            if (billingStateProvince != null)
                 builder.AppendFormat("&x_State={0}", HttpUtility.UrlEncode(billingStateProvince.Abbreviation));
            else
                builder.AppendFormat("&x_State={0}", HttpUtility.UrlEncode(order.BillingStateProvince));
            builder.AppendFormat("&x_Zip={0}", HttpUtility.UrlEncode(order.BillingZipPostalCode));
            Country billingCountry = CountryManager.GetCountryById(order.BillingCountryId);
            if (billingCountry != null)
                builder.AppendFormat("&x_Country={0}", HttpUtility.UrlEncode(billingCountry.ThreeLetterIsoCode));
            else
                builder.AppendFormat("&x_Country={0}", HttpUtility.UrlEncode(order.BillingCountry));
            builder.AppendFormat("&x_EMail={0}", HttpUtility.UrlEncode(order.BillingEmail));
            builder.AppendFormat("&x_Phone={0}", HttpUtility.UrlEncode(order.BillingPhoneNumber));
            HttpContext.Current.Response.Redirect(builder.ToString());
            return string.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return SettingManager.GetSettingValueDecimalNative("PaymentMethod.TwoCheckout.AdditionalFee");
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
        /// Cancels recurring payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
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
            throw new NopException("Recurring payments not supported");
        }

        /// <summary>
        /// Claculates MD5 hash
        /// </summary>
        /// <param name="input">input</param>
        /// <returns>MD5 hash</returns>
        public static string CalculateMD5hash(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// Gets a payment status
        /// </summary>
        /// <param name="message_type"> Indicates type of message (ORDER_CREATED, FRAUD_STATUS_CHANGED, SHIP_STATUS_CHANGED, INVOICE_STATUS_CHANGED, REFUND_ISSUED, RECURRING_INSTALLMENT_SUCCESS, RECURRING_INSTALLMENT_FAILED, RECURRING_STOPPED, RECURRING_COMPLETE, or RECURRING_RESTARTED)</param>
        /// <param name="invoice_status">Invoice status (approved, pending, deposited, or declined)</param>
        /// <param name="fraud_status">2Checkout fraud review (pass, fail, or wait); This parameter could be empty for some sales</param>
        /// <param name="payment_type">2Checkout payment type</param>
        /// <returns>Payment status</returns>
        public static PaymentStatusEnum GetPaymentStatus(string message_type,
            string invoice_status, string fraud_status, string payment_type)
        {
            PaymentStatusEnum result = PaymentStatusEnum.Pending;

            switch (message_type.ToUpperInvariant())
            {
                case "ORDER_CREATED":
                    {
                    }
                    break;
                case "FRAUD_STATUS_CHANGED":
                    {
                        if (fraud_status == "pass")
                        {
                            if (invoice_status == "approved")
                            {
                                result = PaymentStatusEnum.Paid;
                            }
                            else
                            {
                                if (payment_type == "paypal ec")
                                {
                                    result = PaymentStatusEnum.Paid;
                                }
                            }
                        }
                    }
                    break;
                case "INVOICE_STATUS_CHANGED":
                    {
                    }
                    break;
                case "REFUND_ISSUED":
                    {
                        result = PaymentStatusEnum.Refunded;
                    }
                    break;
                case "SHIP_STATUS_CHANGED":
                case "RECURRING_INSTALLMENT_SUCCESS":
                case "RECURRING_INSTALLMENT_FAILED":
                case "RECURRING_STOPPED":
                case "RECURRING_COMPLETE":
                case "RECURRING_RESTARTED":
                    break;
                default:
                    break;
            }
            return result;
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