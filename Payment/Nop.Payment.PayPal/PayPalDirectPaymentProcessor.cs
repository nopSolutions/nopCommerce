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
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Payment.Methods.PayPal.PayPalSvc;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.Payment.Methods.PayPal
{
    /// <summary>
    /// Paypal Direct payment processor
    /// </summary>
    public class PayPalDirectPaymentProcessor : IPaymentMethod
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
        /// Creates a new instance of the PayPalDirectPaymentProcessor class
        /// </summary>
        public PayPalDirectPaymentProcessor()
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
            string transactionMode = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PaypalDirect.TransactionMode");
            if (!String.IsNullOrEmpty(transactionMode))
                transactionModeEnum = (TransactMode)Enum.Parse(typeof(TransactMode), transactionMode);
            return transactionModeEnum;
        }

        /// <summary>
        /// Initializes the PayPalDirectPaymentProcessor
        /// </summary>
        private void InitSettings()
        {
            useSandBox = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.PaypalDirect.UseSandbox");
            APIAccountName = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PaypalDirect.APIAccountName");
            APIAccountPassword = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PaypalDirect.APIAccountPassword");
            Signature = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PaypalDirect.Signature");

            if (string.IsNullOrEmpty(APIAccountName))
                throw new NopException("Paypal Direct API Account Name is empty");

            if (string.IsNullOrEmpty(Signature))
                throw new NopException("Paypal Direct API Account Password is empty");

            if (string.IsNullOrEmpty(APIAccountPassword))
                throw new NopException("Paypal Direct Signature is empty");

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
            TransactMode transactionMode = GetCurrentTransactionMode();

            if (transactionMode == TransactMode.Authorize)
            {
                AuthorizeOrSale(paymentInfo, customer, orderGuid, processPaymentResult, true);
                if (!String.IsNullOrEmpty(processPaymentResult.Error))
                    return;
            }
            else
            {
                AuthorizeOrSale(paymentInfo, customer, orderGuid, processPaymentResult, false);
                if (!String.IsNullOrEmpty(processPaymentResult.Error))
                    return;
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
            return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.PaypalDirect.AdditionalFee");
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
            req.DoCaptureRequest.Amount.currencyID = PaypalHelper.GetPaypalCurrency(IoCFactory.Resolve<ICurrencyService>().PrimaryStoreCurrency);
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
        /// Authorizes the payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        /// <param name="authorizeOnly">A value indicating whether to authorize only; true - authorize; false - sale</param>
        protected void AuthorizeOrSale(PaymentInfo paymentInfo, Customer customer,
            Guid orderGuid, ProcessPaymentResult processPaymentResult, bool authorizeOnly)
        {
            InitSettings();

            DoDirectPaymentReq req = new DoDirectPaymentReq();
            req.DoDirectPaymentRequest = new DoDirectPaymentRequestType();
            req.DoDirectPaymentRequest.Version = this.APIVersion;
            DoDirectPaymentRequestDetailsType details = new DoDirectPaymentRequestDetailsType();
            req.DoDirectPaymentRequest.DoDirectPaymentRequestDetails = details;
            details.IPAddress = NopContext.Current.UserHostAddress;
            if (authorizeOnly)
                details.PaymentAction = PaymentActionCodeType.Authorization;
            else
                details.PaymentAction = PaymentActionCodeType.Sale;
            details.CreditCard = new CreditCardDetailsType();
            details.CreditCard.CreditCardNumber = paymentInfo.CreditCardNumber;
            details.CreditCard.CreditCardType = GetPaypalCreditCardType(paymentInfo.CreditCardType);
            details.CreditCard.ExpMonthSpecified = true;
            details.CreditCard.ExpMonth = paymentInfo.CreditCardExpireMonth;
            details.CreditCard.ExpYearSpecified = true;
            details.CreditCard.ExpYear = paymentInfo.CreditCardExpireYear;
            details.CreditCard.CVV2 = paymentInfo.CreditCardCvv2;
            details.CreditCard.CardOwner = new PayerInfoType();
            details.CreditCard.CardOwner.PayerCountry = GetPaypalCountryCodeType(paymentInfo.BillingAddress.Country);
            details.CreditCard.CreditCardTypeSpecified = true;

            details.CreditCard.CardOwner.Address = new AddressType();
            details.CreditCard.CardOwner.Address.CountrySpecified = true;
            details.CreditCard.CardOwner.Address.Street1 = paymentInfo.BillingAddress.Address1;
            details.CreditCard.CardOwner.Address.Street2 = paymentInfo.BillingAddress.Address2;
            details.CreditCard.CardOwner.Address.CityName = paymentInfo.BillingAddress.City;
            if (paymentInfo.BillingAddress.StateProvince != null)
                details.CreditCard.CardOwner.Address.StateOrProvince = paymentInfo.BillingAddress.StateProvince.Abbreviation;
            else
                details.CreditCard.CardOwner.Address.StateOrProvince = "CA";
            details.CreditCard.CardOwner.Address.Country = GetPaypalCountryCodeType(paymentInfo.BillingAddress.Country);
            details.CreditCard.CardOwner.Address.PostalCode = paymentInfo.BillingAddress.ZipPostalCode;
            details.CreditCard.CardOwner.Payer = paymentInfo.BillingAddress.Email;
            details.CreditCard.CardOwner.PayerName = new PersonNameType();
            details.CreditCard.CardOwner.PayerName.FirstName = paymentInfo.BillingAddress.FirstName;
            details.CreditCard.CardOwner.PayerName.LastName = paymentInfo.BillingAddress.LastName;
            details.PaymentDetails = new PaymentDetailsType();
            details.PaymentDetails.OrderTotal = new BasicAmountType();
            details.PaymentDetails.OrderTotal.Value = paymentInfo.OrderTotal.ToString("N", new CultureInfo("en-us"));
            details.PaymentDetails.OrderTotal.currencyID = PaypalHelper.GetPaypalCurrency(IoCFactory.Resolve<ICurrencyService>().PrimaryStoreCurrency);
            details.PaymentDetails.Custom = orderGuid.ToString();
            details.PaymentDetails.ButtonSource = "nopCommerceCart";


            //ShoppingCart cart = IoCFactory.Resolve<IShoppingCartService>().GetShoppingCartByCustomerSessionGUID(ShoppingCartTypeEnum.ShoppingCart, NopContext.Current.Session.CustomerSessionGUID);
            //PaymentDetailsItemType[] cartItems = new PaymentDetailsItemType[cart.Count];
            //for (int i = 0; i < cart.Count; i++)
            //{
            //    ShoppingCartItem item = cart[i];
            //    cartItems[i] = new PaymentDetailsItemType()
            //    {
            //        Name = item.ProductVariant.FullProductName,
            //        Number = item.ProductVariant.ProductVariantID.ToString(),
            //        Quantity = item.Quantity.ToString(),
            //        Amount = new BasicAmountType()
            //        {
            //            currencyID = PaypalHelper.GetPaypalCurrency(IoCFactory.Resolve<ICurrencyService>().PrimaryStoreCurrency),
            //            Value = (item.Quantity * item.ProductVariant.Price).ToString("N", new CultureInfo("en-us"))
            //        }
            //    };
            //};
            //details.PaymentDetails.PaymentDetailsItem = cartItems;

            //shipping
            if (paymentInfo.ShippingAddress != null)
            {
                if (paymentInfo.ShippingAddress.StateProvince != null && paymentInfo.ShippingAddress.Country != null)
                {
                    AddressType shippingAddress = new AddressType();
                    shippingAddress.Name = paymentInfo.ShippingAddress.FirstName + " " + paymentInfo.ShippingAddress.LastName;
                    shippingAddress.Street1 = paymentInfo.ShippingAddress.Address1;
                    shippingAddress.CityName = paymentInfo.ShippingAddress.City;
                    shippingAddress.StateOrProvince = paymentInfo.ShippingAddress.StateProvince.Abbreviation;
                    shippingAddress.PostalCode = paymentInfo.ShippingAddress.ZipPostalCode;
                    shippingAddress.Country = (CountryCodeType)Enum.Parse(typeof(CountryCodeType), paymentInfo.ShippingAddress.Country.TwoLetterIsoCode, true);
                    shippingAddress.CountrySpecified = true;
                    details.PaymentDetails.ShipToAddress = shippingAddress;
                }
            }

            DoDirectPaymentResponseType response = service2.DoDirectPayment(req);

            string error = string.Empty;
            bool Success = PaypalHelper.CheckSuccess(response, out error);
            if (Success)
            {
                processPaymentResult.AVSResult = response.AVSCode;
                processPaymentResult.AuthorizationTransactionCode = response.CVV2Code;
                if (authorizeOnly)
                {
                    processPaymentResult.AuthorizationTransactionId = response.TransactionID;
                    processPaymentResult.AuthorizationTransactionResult = response.Ack.ToString();

                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Authorized;
                }
                else
                {
                    processPaymentResult.CaptureTransactionId = response.TransactionID;
                    processPaymentResult.CaptureTransactionResult = response.Ack.ToString();

                    processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
                }
            }
            else
            {
                processPaymentResult.Error = error;
                processPaymentResult.FullError = error;
            }
        }

        /// <summary>
        /// Get Paypal country code
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>Paypal country code</returns>
        protected CountryCodeType GetPaypalCountryCodeType(Country country)
        {
            CountryCodeType payerCountry = CountryCodeType.US;
            try
            {
                payerCountry = (CountryCodeType)Enum.Parse(typeof(CountryCodeType), country.TwoLetterIsoCode);
            }
            catch
            {
            }
            return payerCountry;
        }

        /// <summary>
        /// Get Paypal credit card type
        /// </summary>
        /// <param name="CreditCardType">Credit card type</param>
        /// <returns>Paypal credit card type</returns>
        protected CreditCardTypeType GetPaypalCreditCardType(string CreditCardType)
        {
            CreditCardTypeType creditCardTypeType = (CreditCardTypeType)Enum.Parse(typeof(CreditCardTypeType), CreditCardType);
            return creditCardTypeType;
            //if (CreditCardType.ToLower() == "visa")
            //    return CreditCardTypeType.Visa;

            //if (CreditCardType.ToLower() == "mastercard")
            //    return CreditCardTypeType.MasterCard;

            //if (CreditCardType.ToLower() == "americanexpress")
            //    return CreditCardTypeType.Amex;

            //if (CreditCardType.ToLower() == "discover")
            //    return CreditCardTypeType.Discover;

            //throw new NopException("Unknown credit card type");
        }

        /// <summary>
        /// Refunds payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void Refund(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            InitSettings();

            string transactionID = cancelPaymentResult.CaptureTransactionId;

            RefundTransactionReq req = new RefundTransactionReq();
            req.RefundTransactionRequest = new RefundTransactionRequestType();
            //NOTE: Specify amount in partial refund
            req.RefundTransactionRequest.RefundType = RefundType.Full;
            req.RefundTransactionRequest.RefundTypeSpecified = true;
            req.RefundTransactionRequest.Version = this.APIVersion;
            req.RefundTransactionRequest.TransactionID = transactionID;
            RefundTransactionResponseType response = service1.RefundTransaction(req);

            string error = string.Empty;
            bool Success = PaypalHelper.CheckSuccess(response, out error);
            if (Success)
            {
                cancelPaymentResult.PaymentStatus = PaymentStatusEnum.Refunded;
                //cancelPaymentResult.RefundTransactionID = response.RefundTransactionID;
            }
            else
            {
                cancelPaymentResult.Error = error;
            }
        }

        /// <summary>
        /// Voids payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>        
        public void Void(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            InitSettings();

            string transactionID = cancelPaymentResult.AuthorizationTransactionId;
            if (String.IsNullOrEmpty(transactionID))
                transactionID = cancelPaymentResult.CaptureTransactionId;

            DoVoidReq req = new DoVoidReq();
            req.DoVoidRequest = new DoVoidRequestType();
            req.DoVoidRequest.Version = this.APIVersion;
            req.DoVoidRequest.AuthorizationID = transactionID;
            DoVoidResponseType response = service2.DoVoid(req);

            string error = string.Empty;
            bool Success = PaypalHelper.CheckSuccess(response, out error);
            if (Success)
            {
                cancelPaymentResult.PaymentStatus = PaymentStatusEnum.Voided;
                //cancelPaymentResult.VoidTransactionID = response.RefundTransactionID;
            }
            else
            {
                cancelPaymentResult.Error = error;
            }
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

            CreateRecurringPaymentsProfileReq req = new CreateRecurringPaymentsProfileReq();
            req.CreateRecurringPaymentsProfileRequest = new CreateRecurringPaymentsProfileRequestType();
            req.CreateRecurringPaymentsProfileRequest.Version = this.APIVersion;
            CreateRecurringPaymentsProfileRequestDetailsType details = new CreateRecurringPaymentsProfileRequestDetailsType();
            req.CreateRecurringPaymentsProfileRequest.CreateRecurringPaymentsProfileRequestDetails = details;
            
            details.CreditCard = new CreditCardDetailsType();
            details.CreditCard.CreditCardNumber = paymentInfo.CreditCardNumber;
            details.CreditCard.CreditCardType = GetPaypalCreditCardType(paymentInfo.CreditCardType);
            details.CreditCard.ExpMonthSpecified = true;
            details.CreditCard.ExpMonth = paymentInfo.CreditCardExpireMonth;
            details.CreditCard.ExpYearSpecified = true;
            details.CreditCard.ExpYear = paymentInfo.CreditCardExpireYear;
            details.CreditCard.CVV2 = paymentInfo.CreditCardCvv2;
            details.CreditCard.CardOwner = new PayerInfoType();
            details.CreditCard.CardOwner.PayerCountry = GetPaypalCountryCodeType(paymentInfo.BillingAddress.Country);
            details.CreditCard.CreditCardTypeSpecified = true;

            details.CreditCard.CardOwner.Address = new AddressType();
            details.CreditCard.CardOwner.Address.CountrySpecified = true;
            details.CreditCard.CardOwner.Address.Street1 = paymentInfo.BillingAddress.Address1;
            details.CreditCard.CardOwner.Address.Street2 = paymentInfo.BillingAddress.Address2;
            details.CreditCard.CardOwner.Address.CityName = paymentInfo.BillingAddress.City;
            if (paymentInfo.BillingAddress.StateProvince != null)
                details.CreditCard.CardOwner.Address.StateOrProvince = paymentInfo.BillingAddress.StateProvince.Abbreviation;
            else
                details.CreditCard.CardOwner.Address.StateOrProvince = "CA";
            details.CreditCard.CardOwner.Address.Country = GetPaypalCountryCodeType(paymentInfo.BillingAddress.Country);
            details.CreditCard.CardOwner.Address.PostalCode = paymentInfo.BillingAddress.ZipPostalCode;
            details.CreditCard.CardOwner.Payer = paymentInfo.BillingAddress.Email;
            details.CreditCard.CardOwner.PayerName = new PersonNameType();
            details.CreditCard.CardOwner.PayerName.FirstName = paymentInfo.BillingAddress.FirstName;
            details.CreditCard.CardOwner.PayerName.LastName = paymentInfo.BillingAddress.LastName;

            //start date
            details.RecurringPaymentsProfileDetails = new RecurringPaymentsProfileDetailsType();
            details.RecurringPaymentsProfileDetails.BillingStartDate = DateTime.UtcNow;
            details.RecurringPaymentsProfileDetails.ProfileReference = orderGuid.ToString();

            //schedule
            details.ScheduleDetails = new ScheduleDetailsType();
            details.ScheduleDetails.Description = string.Format("{0} - {1}", IoCFactory.Resolve<ISettingManager>().StoreName, "recurring payment");
            details.ScheduleDetails.PaymentPeriod = new BillingPeriodDetailsType();
            details.ScheduleDetails.PaymentPeriod.Amount = new BasicAmountType();
            details.ScheduleDetails.PaymentPeriod.Amount.Value = paymentInfo.OrderTotal.ToString("N", new CultureInfo("en-us"));
            details.ScheduleDetails.PaymentPeriod.Amount.currencyID = PaypalHelper.GetPaypalCurrency(IoCFactory.Resolve<ICurrencyService>().PrimaryStoreCurrency);
            details.ScheduleDetails.PaymentPeriod.BillingFrequency = paymentInfo.RecurringCycleLength;
            switch (paymentInfo.RecurringCyclePeriod)
            {
                case (int)RecurringProductCyclePeriodEnum.Days:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.Day;
                    break;
                case (int)RecurringProductCyclePeriodEnum.Weeks:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.Week;
                    break;
                case (int)RecurringProductCyclePeriodEnum.Months:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.Month;
                    break;
                case (int)RecurringProductCyclePeriodEnum.Years:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.Year;
                    break;
                default:
                    throw new NopException("Not supported cycle period");
            }
            details.ScheduleDetails.PaymentPeriod.TotalBillingCycles = paymentInfo.RecurringTotalCycles;
            details.ScheduleDetails.PaymentPeriod.TotalBillingCyclesSpecified = true;

            CreateRecurringPaymentsProfileResponseType response = service2.CreateRecurringPaymentsProfile(req);

            string error = string.Empty;
            bool Success = PaypalHelper.CheckSuccess(response, out error);
            if (Success)
            {
                processPaymentResult.PaymentStatus = PaymentStatusEnum.Pending;
                if (response.CreateRecurringPaymentsProfileResponseDetails != null)
                {
                    processPaymentResult.SubscriptionTransactionId = response.CreateRecurringPaymentsProfileResponseDetails.ProfileID;
                }
            }
            else
            {
                processPaymentResult.Error = error;
                processPaymentResult.FullError = error;
            }
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
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool CanVoid
        {
            get
            {
                return true;
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
                return RecurringPaymentTypeEnum.Automatic;
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
