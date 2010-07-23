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
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;


namespace NopSolutions.NopCommerce.Payment.Methods.Worldpay
{
    /// <summary>
    /// Worldpay payment processor
    /// </summary>
    public class WorldpayPaymentProcessor : IPaymentMethod
    {
        #region Fields
        private bool useSandBox = true;
        private string instanceID;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the WorldpayPaymentProcessor class
        /// </summary>
        public WorldpayPaymentProcessor()
        {
            useSandBox = SettingManager.GetSettingValueBoolean(WorldpayConstants.SETTING_USE_SANDBOX);
            instanceID = SettingManager.GetSettingValue(WorldpayConstants.SETTING_INSTANCEID);
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
            string returnURL = CommonHelper.GetStoreLocation(false) + "WorldpayReturn.aspx";

            RemotePost remotePostHelper = new RemotePost();
            remotePostHelper.FormName = "WorldpayForm";
            remotePostHelper.Url = GetWorldpayUrl();

            remotePostHelper.Add("instId", instanceID);
            remotePostHelper.Add("cartId", order.OrderId.ToString());

            if (!string.IsNullOrEmpty(SettingManager.GetSettingValue(WorldpayConstants.SETTING_CREDITCARD_CODE_PROPERTY)))
            {
                remotePostHelper.Add("paymentType", SettingManager.GetSettingValue(WorldpayConstants.SETTING_CREDITCARD_CODE_PROPERTY));
            }

            if (!string.IsNullOrEmpty(SettingManager.GetSettingValue(WorldpayConstants.SETTING_WorldPayCSSName)))
            {
                remotePostHelper.Add("MC_WorldPayCSSName", SettingManager.GetSettingValue(WorldpayConstants.SETTING_WorldPayCSSName));
            }

            remotePostHelper.Add("currency", CurrencyManager.PrimaryStoreCurrency.CurrencyCode);
            remotePostHelper.Add("email", order.BillingEmail);
            remotePostHelper.Add("hideContact", "true");
            remotePostHelper.Add("noLanguageMenu", "true");
            remotePostHelper.Add("withDelivery", "true");
            remotePostHelper.Add("fixContact", "false");
            remotePostHelper.Add("amount", order.OrderTotal.ToString(new CultureInfo("en-US", false).NumberFormat));
            remotePostHelper.Add("desc", SettingManager.StoreName);
            remotePostHelper.Add("M_UserID", order.CustomerId.ToString());
            remotePostHelper.Add("M_FirstName", order.BillingFirstName);
            remotePostHelper.Add("M_LastName", order.BillingLastName);
            remotePostHelper.Add("M_Addr1", order.BillingAddress1);
            remotePostHelper.Add("tel", order.BillingPhoneNumber);
            remotePostHelper.Add("M_Addr2", order.BillingAddress2);
            remotePostHelper.Add("M_Business", order.BillingCompany);

            CultureInfo cultureInfo = new CultureInfo(NopContext.Current.WorkingLanguage.LanguageCulture);
            remotePostHelper.Add("lang", cultureInfo.TwoLetterISOLanguageName);

            StateProvince billingStateProvince = StateProvinceManager.GetStateProvinceById(order.BillingStateProvinceId);
            if (billingStateProvince != null)
                remotePostHelper.Add("M_StateCounty", billingStateProvince.Abbreviation);
            else
                remotePostHelper.Add("M_StateCounty", order.BillingStateProvince);
            if (!useSandBox)
                remotePostHelper.Add("testMode", "0");
            else
                remotePostHelper.Add("testMode", "100");
            remotePostHelper.Add("postcode", order.BillingZipPostalCode);
            Country billingCountry = CountryManager.GetCountryById(order.BillingCountryId);
            if (billingCountry != null)
                remotePostHelper.Add("country", billingCountry.TwoLetterIsoCode);
            else
                remotePostHelper.Add("country", order.BillingCountry);

            remotePostHelper.Add("address", order.BillingAddress1 + "," + order.BillingCountry);
            remotePostHelper.Add("MC_callback", returnURL);
            remotePostHelper.Add("name", order.BillingFirstName + " " + order.BillingLastName);

            if (order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
            {
                remotePostHelper.Add("delvName", order.ShippingFullName);
                string delvAddress = order.ShippingAddress1;
                delvAddress += (!string.IsNullOrEmpty(order.ShippingAddress2)) ? " " + order.ShippingAddress2 : string.Empty;
                remotePostHelper.Add("delvAddress", delvAddress);
                remotePostHelper.Add("delvPostcode", order.ShippingZipPostalCode);
                Country shippingCountry = CountryManager.GetCountryById(order.ShippingCountryId);
                remotePostHelper.Add("delvCountry", shippingCountry.TwoLetterIsoCode);
            }

            remotePostHelper.Post();
            return string.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return SettingManager.GetSettingValueDecimalNative("PaymentMethod.Worldpay.AdditionalFee");
        }

        /// <summary>
        /// Gets Worldpay URL
        /// </summary>
        /// <returns></returns>
        private string GetWorldpayUrl()
        {
            return useSandBox ? "https://secure-test.wp3.rbsworldpay.com/wcc/purchase" :
               "https://secure.wp3.rbsworldpay.com/wcc/purchase";
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
            throw new NopException("Recurring payments not supported");
        }

        /// <summary>
        /// Cancels recurring payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
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