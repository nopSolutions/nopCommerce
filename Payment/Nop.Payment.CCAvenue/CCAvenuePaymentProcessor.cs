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
// Contributor(s): praneeth kumar.p_______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using System.Globalization;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Payment.Methods.CCAvenue
{
    /// <summary>
    /// CCAvenue payment processor
    /// </summary>
    public class CCAvenuePaymentProcessor : IPaymentMethod
    {
        #region Fields

        CCAvenueHelper _myUtility = new CCAvenueHelper();
        private string _payURI;
        private string _merchantID;
        private string _key;
        private string _merchantParam;

        #endregion

        #region Methods

        private void InitSettings()
        {
            _merchantID = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.CCAvenue.MerchantID");
            _key = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.CCAvenue.Key");
            _merchantParam = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.CCAvenue.MerchantParam");
            _payURI = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.CCAvenue.PayURI");

            if (String.IsNullOrWhiteSpace(_merchantID))
                throw new NopException("CCAvenue merchant ID is not set");

            if (String.IsNullOrWhiteSpace(_key))
                throw new NopException("CCAvenue key is not set");
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
            InitSettings();

            RemotePost remotePostHelper = new RemotePost();
            remotePostHelper.FormName = "CCAvenueForm";
            remotePostHelper.Url = _payURI;
            remotePostHelper.Add("Merchant_Id", _merchantID.ToString());
            remotePostHelper.Add("Amount", order.OrderTotal.ToString(new CultureInfo("en-US", false).NumberFormat));
            remotePostHelper.Add("Currency", IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency.CurrencyCode);
            remotePostHelper.Add("Order_Id", order.OrderId.ToString());
            remotePostHelper.Add("Redirect_Url", CommonHelper.GetStoreLocation(false) + "CCAvenueReturn.aspx");
            remotePostHelper.Add("Checksum", _myUtility.getchecksum(_merchantID.ToString(), order.OrderId.ToString(), order.OrderTotal.ToString(), CommonHelper.GetStoreLocation(false) + "CCAvenueReturn.aspx", _key));


            //Billing details
            remotePostHelper.Add("billing_cust_name", order.BillingFirstName);
            remotePostHelper.Add("billing_cust_address", order.BillingAddress1);
            remotePostHelper.Add("billing_cust_tel", order.BillingPhoneNumber);
            remotePostHelper.Add("billing_cust_email", order.BillingEmail);
            remotePostHelper.Add("billing_cust_city", order.BillingCity);
            StateProvince billingStateProvince = IoCFactory.Resolve<IStateProvinceManager>().GetStateProvinceById(order.BillingStateProvinceId);
            if (billingStateProvince != null)
                remotePostHelper.Add("billing_cust_state", billingStateProvince.Abbreviation);
            else
                remotePostHelper.Add("billing_cust_state", order.BillingStateProvince);
            remotePostHelper.Add("billing_zip_code", order.BillingZipPostalCode);
            Country billingCountry = IoCFactory.Resolve<ICountryManager>().GetCountryById(order.BillingCountryId);
            if (billingCountry != null)
                remotePostHelper.Add("billing_cust_country", billingCountry.ThreeLetterIsoCode);
            else
                remotePostHelper.Add("billing_cust_country", order.BillingCountry);

            //Delivery details

            if (order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
            {
                remotePostHelper.Add("delivery_cust_name", order.ShippingFirstName);
                remotePostHelper.Add("delivery_cust_address", order.ShippingAddress1);
                remotePostHelper.Add("delivery_cust_notes", string.Empty);
                remotePostHelper.Add("delivery_cust_tel", order.ShippingPhoneNumber);
                remotePostHelper.Add("delivery_cust_city", order.ShippingCity);
                StateProvince deliveryStateProvince = IoCFactory.Resolve<IStateProvinceManager>().GetStateProvinceById(order.ShippingStateProvinceId);
                if (deliveryStateProvince != null)
                    remotePostHelper.Add("delivery_cust_state", deliveryStateProvince.Abbreviation);
                else
                    remotePostHelper.Add("delivery_cust_state", order.ShippingStateProvince);
                remotePostHelper.Add("delivery_zip_code", order.ShippingZipPostalCode);
                Country deliveryCountry = IoCFactory.Resolve<ICountryManager>().GetCountryById(order.ShippingCountryId);
                if (deliveryCountry != null)
                    remotePostHelper.Add("delivery_cust_country", deliveryCountry.ThreeLetterIsoCode);
                else
                    remotePostHelper.Add("delivery_cust_country", order.ShippingCountry);
            }

            remotePostHelper.Add("Merchant_Param", _merchantParam);
            remotePostHelper.Post();
            return string.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.CCAvenue.AdditionalFee");
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

        #region Properties
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
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool CanPartiallyRefund
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
