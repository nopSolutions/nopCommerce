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
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Payment.Methods.AuthorizeNet.net.authorize.api;

namespace NopSolutions.NopCommerce.Payment.Methods.AuthorizeNET
{
    /// <summary>
    /// Authorize.Net payment processor
    /// </summary>
    public class AuthorizeNetPaymentProcessor : IPaymentMethod
    {
        #region Fields

        private bool useSandBox = true;
        private string loginID;
        private string transactionKey;
        private Service webService = null;

        #endregion

        #region Utilities

        /// <summary>
        /// Initializes the Authorize.NET payment processor
        /// </summary>
        private void InitSettings()
        {
            useSandBox = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.AuthorizeNET.UseSandbox");
            transactionKey = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.AuthorizeNET.TransactionKey");
            loginID = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.AuthorizeNET.LoginID");

            if (string.IsNullOrEmpty(transactionKey))
                throw new NopException("Authorize.NET API transaction key is not set");

            if (string.IsNullOrEmpty(loginID))
                throw new NopException("Authorize.NET API login ID is not set");

            webService = new NopSolutions.NopCommerce.Payment.Methods.AuthorizeNet.net.authorize.api.Service();
            if (useSandBox)
            {
                webService.Url = "https://apitest.authorize.net/soap/v1/Service.asmx";
            }
            else
            {
                webService.Url = "https://api.authorize.net/soap/v1/Service.asmx";
            }
        }

        /// <summary>
        /// Gets Authorize.NET URL
        /// </summary>
        /// <returns></returns>
        private string GetAuthorizeNETUrl()
        {
            return useSandBox ? "https://test.authorize.net/gateway/transact.dll" :
                "https://secure.authorize.net/gateway/transact.dll";
        }

        // Populate merchant authentication (ARB Support)
        private MerchantAuthenticationType PopulateMerchantAuthentication()
        {
            MerchantAuthenticationType authentication = new MerchantAuthenticationType();
            authentication.name = loginID;
            authentication.transactionKey = transactionKey;
            return authentication;
        }
        /// <summary>
        ///  Get errors (ARB Support)
        /// </summary>
        /// <param name="response"></param>
        private static string GetErrors(ANetApiResponseType response)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("The API request failed with the following errors:");
            for (int i = 0; i < response.messages.Length; i++)
            {
                sb.AppendLine("[" + response.messages[i].code + "] " + response.messages[i].text);
            }
            return sb.ToString();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets current transaction mode
        /// </summary>
        /// <returns>Current transaction mode</returns>
        public static TransactMode GetCurrentTransactionMode()
        {
            TransactMode transactionModeEnum = TransactMode.Authorize;
            string transactionMode = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.AuthorizeNET.TransactionMode");
            if (!String.IsNullOrEmpty(transactionMode))
                transactionModeEnum = (TransactMode)Enum.Parse(typeof(TransactMode), transactionMode);
            return transactionModeEnum;
        }

        /// <summary>
        /// Process payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessPayment(PaymentInfo paymentInfo, Customer customer, Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            InitSettings();
            TransactMode transactionMode = GetCurrentTransactionMode();

            WebClient webClient = new WebClient();
            NameValueCollection form = new NameValueCollection();
            form.Add("x_login", loginID);
            form.Add("x_tran_key", transactionKey);
            if (useSandBox)
                form.Add("x_test_request", "TRUE");
            else
                form.Add("x_test_request", "FALSE");

            form.Add("x_delim_data", "TRUE");
            form.Add("x_delim_char", "|");
            form.Add("x_encap_char", "");
            form.Add("x_version", APIVersion);
            form.Add("x_relay_response", "FALSE");
            form.Add("x_method", "CC");
            form.Add("x_currency_code", IoC.Resolve<ICurrencyService>().PrimaryStoreCurrency.CurrencyCode);
            if (transactionMode == TransactMode.Authorize)
                form.Add("x_type", "AUTH_ONLY");
            else if (transactionMode == TransactMode.AuthorizeAndCapture)
                form.Add("x_type", "AUTH_CAPTURE");
            else
                throw new NopException("Not supported transaction mode");

            form.Add("x_amount", paymentInfo.OrderTotal.ToString("0.00", CultureInfo.InvariantCulture));
            form.Add("x_card_num", paymentInfo.CreditCardNumber);
            form.Add("x_exp_date", paymentInfo.CreditCardExpireMonth.ToString("D2") + paymentInfo.CreditCardExpireYear.ToString());
            form.Add("x_card_code", paymentInfo.CreditCardCvv2);
            form.Add("x_first_name", paymentInfo.BillingAddress.FirstName);
            form.Add("x_last_name", paymentInfo.BillingAddress.LastName);
            if (string.IsNullOrEmpty(paymentInfo.BillingAddress.Company))
                form.Add("x_company", paymentInfo.BillingAddress.Company);
            form.Add("x_address", paymentInfo.BillingAddress.Address1);
            form.Add("x_city", paymentInfo.BillingAddress.City);
            if (paymentInfo.BillingAddress.StateProvince != null)
                form.Add("x_state", paymentInfo.BillingAddress.StateProvince.Abbreviation);
            form.Add("x_zip", paymentInfo.BillingAddress.ZipPostalCode);
            if (paymentInfo.BillingAddress.Country != null)
                form.Add("x_country", paymentInfo.BillingAddress.Country.TwoLetterIsoCode);
            //20 chars maximum
            form.Add("x_invoice_num", orderGuid.ToString().Substring(0,20));
            form.Add("x_customer_ip",NopContext.Current.UserHostAddress);

            string reply = null;
            Byte[] responseData = webClient.UploadValues(GetAuthorizeNETUrl(), form);
            reply = Encoding.ASCII.GetString(responseData);

            if (!String.IsNullOrEmpty(reply))
            {
                string[] responseFields = reply.Split('|');
                switch (responseFields[0])
                {
                    case "1":
                        processPaymentResult.AuthorizationTransactionCode = string.Format("{0},{1}", responseFields[6], responseFields[4]);
                        processPaymentResult.AuthorizationTransactionResult = string.Format("Approved ({0}: {1})", responseFields[2], responseFields[3]);
                        processPaymentResult.AVSResult = responseFields[5];
                        //responseFields[38];
                        if (transactionMode == TransactMode.Authorize)
                        {
                            processPaymentResult.PaymentStatus = PaymentStatusEnum.Authorized;
                        }
                        else
                        {
                            processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                        }
                        break;
                    case "2":
                        processPaymentResult.Error = string.Format("Declined ({0}: {1})", responseFields[2], responseFields[3]);
                        processPaymentResult.FullError = string.Format("Declined ({0}: {1})", responseFields[2], responseFields[3]);
                        break;
                    case "3":
                        processPaymentResult.Error = string.Format("Error: {0}", reply);
                        processPaymentResult.FullError = string.Format("Error: {0}", reply);
                        break;

                }
            }
            else
            {
                processPaymentResult.Error = "Authorize.NET unknown error";
                processPaymentResult.FullError = "Authorize.NET unknown error";
            }
        }

        /// <summary>
        /// Post process payment (payment gateways that require redirecting)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        public string PostProcessPayment(Order order)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.AuthorizeNET.AdditionalFee");
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
        {
            InitSettings();

            WebClient webClient = new WebClient();
            NameValueCollection form = new NameValueCollection();
            form.Add("x_login", loginID);
            form.Add("x_tran_key", transactionKey);
            if (useSandBox)
                form.Add("x_test_request", "TRUE");
            else
                form.Add("x_test_request", "FALSE");

            form.Add("x_delim_data", "TRUE");
            form.Add("x_delim_char", "|");
            form.Add("x_encap_char", "");
            form.Add("x_version", APIVersion);
            form.Add("x_relay_response", "FALSE");
            form.Add("x_method", "CC");
            form.Add("x_currency_code", IoC.Resolve<ICurrencyService>().PrimaryStoreCurrency.CurrencyCode);
            form.Add("x_type", "PRIOR_AUTH_CAPTURE");

            form.Add("x_amount", order.OrderTotal.ToString("0.00", CultureInfo.InvariantCulture));
            string[] codes = processPaymentResult.AuthorizationTransactionCode.Split(',');
            //x_trans_id. When x_test_request (sandbox) is set to a positive response, 
            //or when Test mode is enabled on the payment gateway, this value will be "0".
            form.Add("x_trans_id", codes[0]);

            string reply = null;
            Byte[] responseData = webClient.UploadValues(GetAuthorizeNETUrl(), form);
            reply = Encoding.ASCII.GetString(responseData);

            if (!String.IsNullOrEmpty(reply))
            {
                string[] responseFields = reply.Split('|');
                switch (responseFields[0])
                {
                    case "1":
                        processPaymentResult.AuthorizationTransactionCode = string.Format("{0},{1}", responseFields[6], responseFields[4]);
                        processPaymentResult.AuthorizationTransactionResult = string.Format("Approved ({0}: {1})", responseFields[2], responseFields[3]);
                        processPaymentResult.AVSResult = responseFields[5];
                        //responseFields[38];
                        processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                        break;
                    case "2":
                        processPaymentResult.Error = string.Format("Declined ({0}: {1})", responseFields[2], responseFields[3]);
                        processPaymentResult.FullError = string.Format("Declined ({0}: {1})", responseFields[2], responseFields[3]);
                        break;
                    case "3":
                        processPaymentResult.Error = string.Format("Error: {0}", reply);
                        processPaymentResult.FullError = string.Format("Error: {0}", reply);
                        break;
                }
            }
            else
            {
                processPaymentResult.Error = "Authorize.NET unknown error";
                processPaymentResult.FullError = "Authorize.NET unknown error";
            }
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
            InitSettings();
            MerchantAuthenticationType authentication = PopulateMerchantAuthentication();
            if (!paymentInfo.IsRecurringPayment)
            {
                ARBSubscriptionType subscription = new ARBSubscriptionType();
                NopSolutions.NopCommerce.Payment.Methods.AuthorizeNet.net.authorize.api.CreditCardType creditCard = new NopSolutions.NopCommerce.Payment.Methods.AuthorizeNet.net.authorize.api.CreditCardType();

                subscription.name = orderGuid.ToString();

                creditCard.cardNumber = paymentInfo.CreditCardNumber;
                creditCard.expirationDate = paymentInfo.CreditCardExpireYear + "-" + paymentInfo.CreditCardExpireMonth; // required format for API is YYYY-MM
                creditCard.cardCode = paymentInfo.CreditCardCvv2;

                subscription.payment = new PaymentType();
                subscription.payment.Item = creditCard;

                subscription.billTo = new NameAndAddressType();
                subscription.billTo.firstName = paymentInfo.BillingAddress.FirstName;
                subscription.billTo.lastName = paymentInfo.BillingAddress.LastName;
                subscription.billTo.address = paymentInfo.BillingAddress.Address1 + " " + paymentInfo.BillingAddress.Address2;
                subscription.billTo.city = paymentInfo.BillingAddress.City;
                if (paymentInfo.BillingAddress.StateProvince != null)
                {
                    subscription.billTo.state = paymentInfo.BillingAddress.StateProvince.Abbreviation;
                }
                subscription.billTo.zip = paymentInfo.BillingAddress.ZipPostalCode;

                if (paymentInfo.ShippingAddress != null)
                {
                    subscription.shipTo = new NameAndAddressType();
                    subscription.shipTo.firstName = paymentInfo.ShippingAddress.FirstName;
                    subscription.shipTo.lastName = paymentInfo.ShippingAddress.LastName;
                    subscription.shipTo.address = paymentInfo.ShippingAddress.Address1 + " " + paymentInfo.ShippingAddress.Address2;
                    subscription.shipTo.city = paymentInfo.ShippingAddress.City;
                    if (paymentInfo.ShippingAddress.StateProvince != null)
                    {
                        subscription.shipTo.state = paymentInfo.ShippingAddress.StateProvince.Abbreviation;
                    }
                    subscription.shipTo.zip = paymentInfo.ShippingAddress.ZipPostalCode;

                }

                subscription.customer = new CustomerType();
                subscription.customer.email = customer.BillingAddress.Email;
                subscription.customer.phoneNumber = customer.BillingAddress.PhoneNumber;

                subscription.order = new OrderType();
                subscription.order.description = string.Format("{0} {1}", IoC.Resolve<ISettingManager>().StoreName, "Recurring payment");

                // Create a subscription that is leng of specified occurrences and interval is amount of days ad runs

                subscription.paymentSchedule = new PaymentScheduleType();
                DateTime dtNow = DateTime.UtcNow;
                subscription.paymentSchedule.startDate = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day);
                subscription.paymentSchedule.startDateSpecified = true;

                subscription.paymentSchedule.totalOccurrences = Convert.ToInt16(paymentInfo.RecurringTotalCycles);
                subscription.paymentSchedule.totalOccurrencesSpecified = true;

                subscription.amount = paymentInfo.OrderTotal;
                subscription.amountSpecified = true;

                // Interval can't be updated once a subscription is created.
                subscription.paymentSchedule.interval = new PaymentScheduleTypeInterval();
                switch (paymentInfo.RecurringCyclePeriod)
                {
                    case (int)RecurringProductCyclePeriodEnum.Days:
                        subscription.paymentSchedule.interval.length = Convert.ToInt16(paymentInfo.RecurringCycleLength);
                        subscription.paymentSchedule.interval.unit = ARBSubscriptionUnitEnum.days;
                        break;
                    case (int)RecurringProductCyclePeriodEnum.Weeks:
                        subscription.paymentSchedule.interval.length = Convert.ToInt16(paymentInfo.RecurringCycleLength * 7);
                        subscription.paymentSchedule.interval.unit = ARBSubscriptionUnitEnum.days;
                        break;
                    case (int)RecurringProductCyclePeriodEnum.Months:
                        subscription.paymentSchedule.interval.length = Convert.ToInt16(paymentInfo.RecurringCycleLength);
                        subscription.paymentSchedule.interval.unit = ARBSubscriptionUnitEnum.months;
                        break;
                    case (int)RecurringProductCyclePeriodEnum.Years:
                        subscription.paymentSchedule.interval.length = Convert.ToInt16(paymentInfo.RecurringCycleLength * 12);
                        subscription.paymentSchedule.interval.unit = ARBSubscriptionUnitEnum.months;
                        break;
                    default:
                        throw new NopException("Not supported cycle period");
                }


                ARBCreateSubscriptionResponseType response = webService.ARBCreateSubscription(authentication, subscription);

                if (response.resultCode == MessageTypeEnum.Ok)
                {
                    processPaymentResult.SubscriptionTransactionId = response.subscriptionId.ToString();
                    processPaymentResult.AuthorizationTransactionCode = response.resultCode.ToString();
                    processPaymentResult.AuthorizationTransactionResult = string.Format("Approved ({0}: {1})", response.resultCode.ToString(), response.subscriptionId.ToString());
                }
                else
                {
                    processPaymentResult.Error = string.Format("Error processing recurring payment. {0}", GetErrors(response));
                    processPaymentResult.FullError = string.Format("Error processing recurring payment. {0}", GetErrors(response));
                }
            }
        }

        /// <summary>
        /// Cancels recurring payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            InitSettings();
            MerchantAuthenticationType authentication = PopulateMerchantAuthentication();
            long subscriptionID = 0;
            long.TryParse(cancelPaymentResult.SubscriptionTransactionId, out subscriptionID);
            ARBCancelSubscriptionResponseType response = webService.ARBCancelSubscription(authentication, subscriptionID);

            if (response.resultCode == MessageTypeEnum.Ok)
            {
                //ok
            }
            else
            {
                cancelPaymentResult.Error = "Error cancelling subscription, please contact customer support. " + GetErrors(response);
                cancelPaymentResult.FullError = "Error cancelling subscription, please contact customer support. " + GetErrors(response);
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets Authorize.NET API version
        /// </summary>
        public string APIVersion
        {
            get
            {
                return "3.1";
            }
        }

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool CanCapture
        {
            get
            {
                return true;
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
                return RecurringPaymentTypeEnum.Manual;
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
