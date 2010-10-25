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
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Payment.Methods.PayPal.PayPalSvc;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.Payment.Methods.PayPal
{
    /// <summary>
    /// Paypal Express payment processor
    /// </summary>
    public class PayPalExpressPaymentProcessor : IPaymentMethod
    {
        #region Fields
        private bool useSandBox = true;
        private string APIAccountName;
        private string APIAccountPassword;
        private string Signature;
        private PayPalAPISoapBinding service1;
        private PayPalAPIAASoapBinding service2;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the PayPalExpressPaymentProcessor class
        /// </summary>
        public PayPalExpressPaymentProcessor()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets transaction mode configured by store owner
        /// </summary>
        /// <returns></returns>
        private TransactMode GetCurrentTransactionMode()
        {
            TransactMode transactionModeEnum = TransactMode.Authorize;
            string transactionMode = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PaypalExpress.TransactionMode");
            if (!String.IsNullOrEmpty(transactionMode))
                transactionModeEnum = (TransactMode)Enum.Parse(typeof(TransactMode), transactionMode);
            return transactionModeEnum;
        }

        /// <summary>
        /// Initializes the PayPalExpressPaymentProcessor
        /// </summary>
        private void InitSettings()
        {
            useSandBox = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.PaypalExpress.UseSandbox");
            APIAccountName = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PaypalExpress.APIAccountName");
            APIAccountPassword = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PaypalExpress.APIAccountPassword");
            Signature = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PaypalExpress.Signature");

            if (string.IsNullOrEmpty(APIAccountName))
                throw new NopException("Paypal Express API Account Name is empty");

            if (string.IsNullOrEmpty(Signature))
                throw new NopException("Paypal Express API Account Password is empty");

            if (string.IsNullOrEmpty(APIAccountPassword))
                throw new NopException("Paypal Express Signature is empty");

            service1 = new PayPalAPISoapBinding();
            service2 = new PayPalAPIAASoapBinding();


            if (!useSandBox)
            {
                service2.Url = service1.Url = "https://api-3t.paypal.com/2.0/";
            }
            else
            {
                service2.Url = service1.Url = "https://api-3t.sandbox.paypal.com/2.0/";
            }

            service1.RequesterCredentials = new CustomSecurityHeaderType();
            service1.RequesterCredentials.Credentials = new UserIdPasswordType();
            service1.RequesterCredentials.Credentials.Username = APIAccountName;
            service1.RequesterCredentials.Credentials.Password = APIAccountPassword;
            service1.RequesterCredentials.Credentials.Signature = Signature;
            service1.RequesterCredentials.Credentials.Subject = "";

            service2.RequesterCredentials = new CustomSecurityHeaderType();
            service2.RequesterCredentials.Credentials = new UserIdPasswordType();
            service2.RequesterCredentials.Credentials.Username = APIAccountName;
            service2.RequesterCredentials.Credentials.Password = APIAccountPassword;
            service2.RequesterCredentials.Credentials.Signature = Signature;
            service2.RequesterCredentials.Credentials.Subject = "";
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
            DoExpressCheckout(paymentInfo, orderGuid, processPaymentResult);
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
            return decimal.Zero;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
        {
            InitSettings();

            string authorizationID = processPaymentResult.AuthorizationTransactionId;
            DoCaptureReq req = new DoCaptureReq();
            req.DoCaptureRequest = new DoCaptureRequestType();
            req.DoCaptureRequest.Version = this.APIVersion;
            req.DoCaptureRequest.AuthorizationID = authorizationID;
            req.DoCaptureRequest.Amount = new BasicAmountType();
            req.DoCaptureRequest.Amount.Value = order.OrderTotal.ToString("N", new CultureInfo("en-us"));
            req.DoCaptureRequest.Amount.currencyID = PaypalHelper.GetPaypalCurrency(IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency);
            req.DoCaptureRequest.CompleteType = CompleteCodeType.Complete;
            DoCaptureResponseType response = service2.DoCapture(req);

            string error = string.Empty;
            bool Success = PaypalHelper.CheckSuccess(response, out error);
            if (Success)
            {
                processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                processPaymentResult.CaptureTransactionId = response.DoCaptureResponseDetails.PaymentInfo.TransactionID;
                processPaymentResult.CaptureTransactionResult = response.Ack.ToString();
            }
            else
            {
                processPaymentResult.Error = error;
            }
        }

        /// <summary>
        /// Sets paypal express checkout
        /// </summary>
        /// <param name="OrderTotal">Order total</param>
        /// <param name="ReturnURL">Return URL</param>
        /// <param name="CancelURL">Cancel URL</param>
        /// <returns>Express checkout URL</returns>
        public string SetExpressCheckout(decimal OrderTotal, 
            string ReturnURL, string CancelURL)
        {
            InitSettings();
            TransactMode transactionMode = GetCurrentTransactionMode();

            SetExpressCheckoutReq req = new SetExpressCheckoutReq();
            req.SetExpressCheckoutRequest = new SetExpressCheckoutRequestType();
            req.SetExpressCheckoutRequest.Version = this.APIVersion;
            SetExpressCheckoutRequestDetailsType details = new SetExpressCheckoutRequestDetailsType();
            req.SetExpressCheckoutRequest.SetExpressCheckoutRequestDetails = details;
            if (transactionMode == TransactMode.Authorize)
                details.PaymentAction = PaymentActionCodeType.Authorization;
            else
                details.PaymentAction = PaymentActionCodeType.Sale;
            details.PaymentActionSpecified = true;
            details.OrderTotal = new BasicAmountType();
            details.OrderTotal.Value = OrderTotal.ToString("N", new CultureInfo("en-us"));
            details.OrderTotal.currencyID = PaypalHelper.GetPaypalCurrency(IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency);
            details.ReturnURL = ReturnURL;
            details.CancelURL = CancelURL;
            SetExpressCheckoutResponseType response = service2.SetExpressCheckout(req);
            string error;
            if (PaypalHelper.CheckSuccess(response, out error))
                return GetPaypalUrl(response.Token);
            throw new NopException(error);
        }

        /// <summary>
        /// Gets a paypal express checkout result
        /// </summary>
        /// <param name="token">paypal express checkout token</param>
        /// <returns>Paypal payer</returns>
        public PaypalPayer GetExpressCheckout(string token)
        {
            InitSettings();
            GetExpressCheckoutDetailsReq req = new GetExpressCheckoutDetailsReq();
            GetExpressCheckoutDetailsRequestType request = new GetExpressCheckoutDetailsRequestType();
            req.GetExpressCheckoutDetailsRequest = request;

            request.Token = token;
            request.Version = this.APIVersion;

            GetExpressCheckoutDetailsResponseType response = service2.GetExpressCheckoutDetails(req);

            string error;
            if (!PaypalHelper.CheckSuccess(response, out error))
                throw new NopException(error);

            PaypalPayer payer = new PaypalPayer();
            payer.PayerEmail = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.Payer;
            payer.FirstName = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.PayerName.FirstName;
            payer.LastName = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.PayerName.LastName;
            payer.CompanyName = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.PayerBusiness;
            payer.Address1 = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.Address.Street1;
            payer.Address2 = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.Address.Street2;
            payer.City = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.Address.CityName;
            payer.State = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.Address.StateOrProvince;
            payer.Zipcode = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.Address.PostalCode;
            payer.Phone = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.ContactPhone;
            payer.PaypalCountryName = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.Address.CountryName;
            payer.CountryCode = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.Address.Country.ToString();
            payer.PayerID = response.GetExpressCheckoutDetailsResponseDetails.PayerInfo.PayerID;
            payer.Token = response.GetExpressCheckoutDetailsResponseDetails.Token;
            return payer;
        }

        /// <summary>
        /// Do paypal express checkout
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void DoExpressCheckout(PaymentInfo paymentInfo, 
            Guid orderGuid,  ProcessPaymentResult processPaymentResult)
        {
            InitSettings();
            TransactMode transactionMode = GetCurrentTransactionMode();

            DoExpressCheckoutPaymentReq req = new DoExpressCheckoutPaymentReq();
            DoExpressCheckoutPaymentRequestType request = new DoExpressCheckoutPaymentRequestType();
            req.DoExpressCheckoutPaymentRequest = request;
            request.Version = this.APIVersion;
            DoExpressCheckoutPaymentRequestDetailsType details = new DoExpressCheckoutPaymentRequestDetailsType();
            request.DoExpressCheckoutPaymentRequestDetails = details;
            if (transactionMode == TransactMode.Authorize)
                details.PaymentAction = PaymentActionCodeType.Authorization;
            else
                details.PaymentAction = PaymentActionCodeType.Sale;
            details.PaymentActionSpecified = true;
            details.Token = paymentInfo.PaypalToken;
            details.PayerID = paymentInfo.PaypalPayerId;
            
            details.PaymentDetails = new PaymentDetailsType[1];
            PaymentDetailsType paymentDetails1 = new PaymentDetailsType();
            details.PaymentDetails[0] = paymentDetails1;
            paymentDetails1.OrderTotal = new BasicAmountType();
            paymentDetails1.OrderTotal.Value = paymentInfo.OrderTotal.ToString("N", new CultureInfo("en-us"));
            paymentDetails1.OrderTotal.currencyID = PaypalHelper.GetPaypalCurrency(IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency);
            paymentDetails1.Custom = orderGuid.ToString();
            paymentDetails1.ButtonSource = "nopCommerceCart";
            
            DoExpressCheckoutPaymentResponseType response = service2.DoExpressCheckoutPayment(req);
            string error;
            if (!PaypalHelper.CheckSuccess(response, out error))
                throw new NopException(error);

            if (response.DoExpressCheckoutPaymentResponseDetails.PaymentInfo != null &&
                response.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0] != null)
            {
                processPaymentResult.AuthorizationTransactionId = response.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID;
                processPaymentResult.AuthorizationTransactionResult = response.Ack.ToString();

                if (transactionMode == TransactMode.Authorize)
                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Authorized;
                else
                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
            }
            else
            {
                throw new NopException("response.DoExpressCheckoutPaymentResponseDetails.PaymentInfo is null");
            }
        }

        /// <summary>
        /// Gets Paypal URL
        /// </summary>
        /// <param name="token">Paypal express token</param>
        /// <returns>URL</returns>
        private string GetPaypalUrl(string token)
        {
            return useSandBox ? "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" + token :
                "https://www.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" + token;
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
        /// Gets Paypal API version
        /// </summary>
        public string APIVersion
        {
            get
            {
                return "63";
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
                return PaymentMethodTypeEnum.Button;
            }
        }
        
        #endregion
    }
}
