using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.PayPalDirect.Controllers;
using Nop.Plugin.Payments.PayPalDirect.PayPalSvc;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;

namespace Nop.Plugin.Payments.PayPalDirect
{
    /// <summary>
    /// PayPalDirect payment processor
    /// </summary>
    public class PayPalDirectPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly PayPalDirectPaymentSettings _paypalDirectPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;
        private readonly StoreInformationSettings _storeInformationSettings;
        #endregion

        #region Ctor

        public PayPalDirectPaymentProcessor(PayPalDirectPaymentSettings paypalDirectPaymentSettings,
            ISettingService settingService, ICurrencyService currencyService,
            CurrencySettings currencySettings, IWebHelper webHelper,
            StoreInformationSettings storeInformationSettings)
        {
            this._paypalDirectPaymentSettings = paypalDirectPaymentSettings;
            this._settingService = settingService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._webHelper = webHelper;
            this._storeInformationSettings = storeInformationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetPaypalUrl()
        {
            return _paypalDirectPaymentSettings.UseSandbox ? "https://www.sandbox.paypal.com/us/cgi-bin/webscr" :
                "https://www.paypal.com/us/cgi-bin/webscr";
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

        protected string GetApiVersion()
        {
            return "63";
        }

        protected ProcessPaymentResult AuthorizeOrSale(ProcessPaymentRequest processPaymentRequest, bool authorizeOnly)
        {
            var result = new ProcessPaymentResult();

            var req = new DoDirectPaymentReq();
            req.DoDirectPaymentRequest = new DoDirectPaymentRequestType();
            req.DoDirectPaymentRequest.Version = GetApiVersion();
            var details = new DoDirectPaymentRequestDetailsType();
            req.DoDirectPaymentRequest.DoDirectPaymentRequestDetails = details;
            details.IPAddress = _webHelper.GetCurrentIpAddress();
            if (authorizeOnly)
                details.PaymentAction = PaymentActionCodeType.Authorization;
            else
                details.PaymentAction = PaymentActionCodeType.Sale;
            details.CreditCard = new CreditCardDetailsType();
            details.CreditCard.CreditCardNumber = processPaymentRequest.CreditCardNumber;
            details.CreditCard.CreditCardType = GetPaypalCreditCardType(processPaymentRequest.CreditCardType);
            details.CreditCard.ExpMonthSpecified = true;
            details.CreditCard.ExpMonth = processPaymentRequest.CreditCardExpireMonth;
            details.CreditCard.ExpYearSpecified = true;
            details.CreditCard.ExpYear = processPaymentRequest.CreditCardExpireYear;
            details.CreditCard.CVV2 = processPaymentRequest.CreditCardCvv2;
            details.CreditCard.CardOwner = new PayerInfoType();
            details.CreditCard.CardOwner.PayerCountry = GetPaypalCountryCodeType(processPaymentRequest.Customer.BillingAddress.Country);
            details.CreditCard.CreditCardTypeSpecified = true;

            details.CreditCard.CardOwner.Address = new AddressType();
            details.CreditCard.CardOwner.Address.CountrySpecified = true;
            details.CreditCard.CardOwner.Address.Street1 = processPaymentRequest.Customer.BillingAddress.Address1;
            details.CreditCard.CardOwner.Address.Street2 = processPaymentRequest.Customer.BillingAddress.Address2;
            details.CreditCard.CardOwner.Address.CityName = processPaymentRequest.Customer.BillingAddress.City;
            if (processPaymentRequest.Customer.BillingAddress.StateProvince != null)
                details.CreditCard.CardOwner.Address.StateOrProvince = processPaymentRequest.Customer.BillingAddress.StateProvince.Abbreviation;
            else
                details.CreditCard.CardOwner.Address.StateOrProvince = "CA";
            details.CreditCard.CardOwner.Address.Country = GetPaypalCountryCodeType(processPaymentRequest.Customer.BillingAddress.Country);
            details.CreditCard.CardOwner.Address.PostalCode = processPaymentRequest.Customer.BillingAddress.ZipPostalCode;
            details.CreditCard.CardOwner.Payer = processPaymentRequest.Customer.BillingAddress.Email;
            details.CreditCard.CardOwner.PayerName = new PersonNameType();
            details.CreditCard.CardOwner.PayerName.FirstName = processPaymentRequest.Customer.BillingAddress.FirstName;
            details.CreditCard.CardOwner.PayerName.LastName = processPaymentRequest.Customer.BillingAddress.LastName;
            details.PaymentDetails = new PaymentDetailsType();
            details.PaymentDetails.OrderTotal = new BasicAmountType();
            details.PaymentDetails.OrderTotal.Value = Math.Round(processPaymentRequest.OrderTotal, 2).ToString("N", new CultureInfo("en-us"));
            details.PaymentDetails.OrderTotal.currencyID = PaypalHelper.GetPaypalCurrency(_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId));
            details.PaymentDetails.Custom = processPaymentRequest.OrderGuid.ToString();
            details.PaymentDetails.ButtonSource = "nopCommerceCart";


            //ShoppingCart cart = IoC.Resolve<IShoppingCartService>().GetShoppingCartByCustomerSessionGUID(ShoppingCartTypeEnum.ShoppingCart, NopContext.Current.Session.CustomerSessionGUID);
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
            //            currencyID = PaypalHelper.GetPaypalCurrency(IoC.Resolve<ICurrencyService>().PrimaryStoreCurrency),
            //            Value = (item.Quantity * item.ProductVariant.Price).ToString("N", new CultureInfo("en-us"))
            //        }
            //    };
            //};
            //details.PaymentDetails.PaymentDetailsItem = cartItems;

            //shipping
            if (processPaymentRequest.Customer.ShippingAddress != null)
            {
                if (processPaymentRequest.Customer.ShippingAddress.StateProvince != null && processPaymentRequest.Customer.ShippingAddress.Country != null)
                {
                    AddressType shippingAddress = new AddressType();
                    shippingAddress.Name = processPaymentRequest.Customer.ShippingAddress.FirstName + " " + processPaymentRequest.Customer.ShippingAddress.LastName;
                    shippingAddress.Street1 = processPaymentRequest.Customer.ShippingAddress.Address1;
                    shippingAddress.CityName = processPaymentRequest.Customer.ShippingAddress.City;
                    shippingAddress.StateOrProvince = processPaymentRequest.Customer.ShippingAddress.StateProvince.Abbreviation;
                    shippingAddress.PostalCode = processPaymentRequest.Customer.ShippingAddress.ZipPostalCode;
                    shippingAddress.Country = (CountryCodeType)Enum.Parse(typeof(CountryCodeType), processPaymentRequest.Customer.ShippingAddress.Country.TwoLetterIsoCode, true);
                    shippingAddress.CountrySpecified = true;
                    details.PaymentDetails.ShipToAddress = shippingAddress;
                }
            }

            using (var service2 = new PayPalAPIAASoapBinding())
            {
                if (!_paypalDirectPaymentSettings.UseSandbox)
                    service2.Url = "https://api-3t.paypal.com/2.0/";
                else
                    service2.Url = "https://api-3t.sandbox.paypal.com/2.0/";

                service2.RequesterCredentials = new CustomSecurityHeaderType();
                service2.RequesterCredentials.Credentials = new UserIdPasswordType();
                service2.RequesterCredentials.Credentials.Username = _paypalDirectPaymentSettings.ApiAccountName;
                service2.RequesterCredentials.Credentials.Password = _paypalDirectPaymentSettings.ApiAccountPassword;
                service2.RequesterCredentials.Credentials.Signature = _paypalDirectPaymentSettings.Signature;
                service2.RequesterCredentials.Credentials.Subject = "";

                DoDirectPaymentResponseType response = service2.DoDirectPayment(req);

                string error = "";
                bool success = PaypalHelper.CheckSuccess(response, out error);
                if (success)
                {
                    result.AvsResult = response.AVSCode;
                    result.AuthorizationTransactionCode = response.CVV2Code;
                    if (authorizeOnly)
                    {
                        result.AuthorizationTransactionId = response.TransactionID;
                        result.AuthorizationTransactionResult = response.Ack.ToString();

                        result.NewPaymentStatus = PaymentStatus.Authorized;
                    }
                    else
                    {
                        result.CaptureTransactionId = response.TransactionID;
                        result.CaptureTransactionResult = response.Ack.ToString();

                        result.NewPaymentStatus = PaymentStatus.Paid;
                    }
                }
                else
                {
                    result.AddError(error);
                }
            }
            return result;
        }

        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <param name="values">Values</param>
        /// <returns>Result</returns>
        public bool VerifyIPN(string formString, out Dictionary<string, string> values)
        {
            var req = (HttpWebRequest)WebRequest.Create(GetPaypalUrl());
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            string formContent = string.Format("{0}&cmd=_notify-validate", formString);
            req.ContentLength = formContent.Length;

            using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
            {
                sw.Write(formContent);
            }

            string response = null;
            using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                response = HttpUtility.UrlDecode(sr.ReadToEnd());
            }
            bool success = response.Trim().Equals("VERIFIED", StringComparison.OrdinalIgnoreCase);

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string l in formString.Split('&'))
            {
                string line = l.Trim();
                int equalPox = line.IndexOf('=');
                if (equalPox >= 0)
                    values.Add(line.Substring(0, equalPox), line.Substring(equalPox + 1));
            }

            return success;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            if (_paypalDirectPaymentSettings.TransactMode == TransactMode.Authorize)
            {
                return AuthorizeOrSale(processPaymentRequest, true);
            }
            else
            {
                return AuthorizeOrSale(processPaymentRequest, false);
            }
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return _paypalDirectPaymentSettings.AdditionalFee;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();

            string authorizationId = capturePaymentRequest.Order.AuthorizationTransactionId;
            var req = new DoCaptureReq();
            req.DoCaptureRequest = new DoCaptureRequestType();
            req.DoCaptureRequest.Version = GetApiVersion();
            req.DoCaptureRequest.AuthorizationID = authorizationId;
            req.DoCaptureRequest.Amount = new BasicAmountType();
            req.DoCaptureRequest.Amount.Value = Math.Round(capturePaymentRequest.Order.OrderTotal, 2).ToString("N", new CultureInfo("en-us"));
            req.DoCaptureRequest.Amount.currencyID = PaypalHelper.GetPaypalCurrency(_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId));
            req.DoCaptureRequest.CompleteType = CompleteCodeType.Complete;

            using (var service2 = new PayPalAPIAASoapBinding())
            {
                if (!_paypalDirectPaymentSettings.UseSandbox)
                    service2.Url = "https://api-3t.paypal.com/2.0/";
                else
                    service2.Url = "https://api-3t.sandbox.paypal.com/2.0/";

                service2.RequesterCredentials = new CustomSecurityHeaderType();
                service2.RequesterCredentials.Credentials = new UserIdPasswordType();
                service2.RequesterCredentials.Credentials.Username = _paypalDirectPaymentSettings.ApiAccountName;
                service2.RequesterCredentials.Credentials.Password = _paypalDirectPaymentSettings.ApiAccountPassword;
                service2.RequesterCredentials.Credentials.Signature = _paypalDirectPaymentSettings.Signature;
                service2.RequesterCredentials.Credentials.Subject = "";

                DoCaptureResponseType response = service2.DoCapture(req);

                string error = "";
                bool success = PaypalHelper.CheckSuccess(response, out error);
                if (success)
                {
                    result.NewPaymentStatus = PaymentStatus.Paid;
                    result.CaptureTransactionId = response.DoCaptureResponseDetails.PaymentInfo.TransactionID;
                    result.CaptureTransactionResult = response.Ack.ToString();
                }
                else
                {
                    result.AddError(error);
                }
            }
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();

            string transactionId = refundPaymentRequest.Order.CaptureTransactionId;

            var req = new RefundTransactionReq();
            req.RefundTransactionRequest = new RefundTransactionRequestType();
            //NOTE: Specify amount in partial refund
            req.RefundTransactionRequest.RefundType = RefundType.Full;
            req.RefundTransactionRequest.RefundTypeSpecified = true;
            req.RefundTransactionRequest.Version = GetApiVersion();
            req.RefundTransactionRequest.TransactionID = transactionId;

            using (var service1 = new PayPalAPISoapBinding())
            {
                if (!_paypalDirectPaymentSettings.UseSandbox)
                    service1.Url = "https://api-3t.paypal.com/2.0/";
                else
                    service1.Url = "https://api-3t.sandbox.paypal.com/2.0/";

                service1.RequesterCredentials = new CustomSecurityHeaderType();
                service1.RequesterCredentials.Credentials = new UserIdPasswordType();
                service1.RequesterCredentials.Credentials.Username = _paypalDirectPaymentSettings.ApiAccountName;
                service1.RequesterCredentials.Credentials.Password = _paypalDirectPaymentSettings.ApiAccountPassword;
                service1.RequesterCredentials.Credentials.Signature = _paypalDirectPaymentSettings.Signature;
                service1.RequesterCredentials.Credentials.Subject = "";

                RefundTransactionResponseType response = service1.RefundTransaction(req);

                string error = string.Empty;
                bool Success = PaypalHelper.CheckSuccess(response, out error);
                if (Success)
                {
                    result.NewPaymentStatus = PaymentStatus.Refunded;
                    //cancelPaymentResult.RefundTransactionID = response.RefundTransactionID;
                }
                else
                {
                    result.AddError(error);
                }
            }

            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();

            string transactionId = voidPaymentRequest.Order.AuthorizationTransactionId;
            if (String.IsNullOrEmpty(transactionId))
                transactionId = voidPaymentRequest.Order.CaptureTransactionId;

            var req = new DoVoidReq();
            req.DoVoidRequest = new DoVoidRequestType();
            req.DoVoidRequest.Version = GetApiVersion();
            req.DoVoidRequest.AuthorizationID = transactionId;


            using (var service2 = new PayPalAPIAASoapBinding())
            {
                if (!_paypalDirectPaymentSettings.UseSandbox)
                    service2.Url = "https://api-3t.paypal.com/2.0/";
                else
                    service2.Url = "https://api-3t.sandbox.paypal.com/2.0/";

                service2.RequesterCredentials = new CustomSecurityHeaderType();
                service2.RequesterCredentials.Credentials = new UserIdPasswordType();
                service2.RequesterCredentials.Credentials.Username = _paypalDirectPaymentSettings.ApiAccountName;
                service2.RequesterCredentials.Credentials.Password = _paypalDirectPaymentSettings.ApiAccountPassword;
                service2.RequesterCredentials.Credentials.Signature = _paypalDirectPaymentSettings.Signature;
                service2.RequesterCredentials.Credentials.Subject = "";

                DoVoidResponseType response = service2.DoVoid(req);

                string error = "";
                bool success = PaypalHelper.CheckSuccess(response, out error);
                if (success)
                {
                    result.NewPaymentStatus = PaymentStatus.Voided;
                    //result.VoidTransactionID = response.RefundTransactionID;
                }
                else
                {
                    result.AddError(error);
                }
            }
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();

            var req = new CreateRecurringPaymentsProfileReq();
            req.CreateRecurringPaymentsProfileRequest = new CreateRecurringPaymentsProfileRequestType();
            req.CreateRecurringPaymentsProfileRequest.Version = GetApiVersion();
            var details = new CreateRecurringPaymentsProfileRequestDetailsType();
            req.CreateRecurringPaymentsProfileRequest.CreateRecurringPaymentsProfileRequestDetails = details;

            details.CreditCard = new CreditCardDetailsType();
            details.CreditCard.CreditCardNumber = processPaymentRequest.CreditCardNumber;
            details.CreditCard.CreditCardType = GetPaypalCreditCardType(processPaymentRequest.CreditCardType);
            details.CreditCard.ExpMonthSpecified = true;
            details.CreditCard.ExpMonth = processPaymentRequest.CreditCardExpireMonth;
            details.CreditCard.ExpYearSpecified = true;
            details.CreditCard.ExpYear = processPaymentRequest.CreditCardExpireYear;
            details.CreditCard.CVV2 = processPaymentRequest.CreditCardCvv2;
            details.CreditCard.CardOwner = new PayerInfoType();
            details.CreditCard.CardOwner.PayerCountry = GetPaypalCountryCodeType(processPaymentRequest.Customer.BillingAddress.Country);
            details.CreditCard.CreditCardTypeSpecified = true;

            details.CreditCard.CardOwner.Address = new AddressType();
            details.CreditCard.CardOwner.Address.CountrySpecified = true;
            details.CreditCard.CardOwner.Address.Street1 = processPaymentRequest.Customer.BillingAddress.Address1;
            details.CreditCard.CardOwner.Address.Street2 = processPaymentRequest.Customer.BillingAddress.Address2;
            details.CreditCard.CardOwner.Address.CityName = processPaymentRequest.Customer.BillingAddress.City;
            if (processPaymentRequest.Customer.BillingAddress.StateProvince != null)
                details.CreditCard.CardOwner.Address.StateOrProvince = processPaymentRequest.Customer.BillingAddress.StateProvince.Abbreviation;
            else
                details.CreditCard.CardOwner.Address.StateOrProvince = "CA";
            details.CreditCard.CardOwner.Address.Country = GetPaypalCountryCodeType(processPaymentRequest.Customer.BillingAddress.Country);
            details.CreditCard.CardOwner.Address.PostalCode = processPaymentRequest.Customer.BillingAddress.ZipPostalCode;
            details.CreditCard.CardOwner.Payer = processPaymentRequest.Customer.BillingAddress.Email;
            details.CreditCard.CardOwner.PayerName = new PersonNameType();
            details.CreditCard.CardOwner.PayerName.FirstName = processPaymentRequest.Customer.BillingAddress.FirstName;
            details.CreditCard.CardOwner.PayerName.LastName = processPaymentRequest.Customer.BillingAddress.LastName;

            //start date
            details.RecurringPaymentsProfileDetails = new RecurringPaymentsProfileDetailsType();
            details.RecurringPaymentsProfileDetails.BillingStartDate = DateTime.UtcNow;
            details.RecurringPaymentsProfileDetails.ProfileReference = processPaymentRequest.OrderGuid.ToString();

            //schedule
            details.ScheduleDetails = new ScheduleDetailsType();
            details.ScheduleDetails.Description = string.Format("{0} - {1}", _storeInformationSettings.StoreName, "recurring payment");
            details.ScheduleDetails.PaymentPeriod = new BillingPeriodDetailsType();
            details.ScheduleDetails.PaymentPeriod.Amount = new BasicAmountType();
            details.ScheduleDetails.PaymentPeriod.Amount.Value = Math.Round(processPaymentRequest.OrderTotal, 2).ToString("N", new CultureInfo("en-us"));
            details.ScheduleDetails.PaymentPeriod.Amount.currencyID = PaypalHelper.GetPaypalCurrency(_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId));
            details.ScheduleDetails.PaymentPeriod.BillingFrequency = processPaymentRequest.RecurringCycleLength;
            switch (processPaymentRequest.RecurringCyclePeriod)
            {
                case RecurringProductCyclePeriod.Days:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.Day;
                    break;
                case RecurringProductCyclePeriod.Weeks:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.Week;
                    break;
                case RecurringProductCyclePeriod.Months:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.Month;
                    break;
                case RecurringProductCyclePeriod.Years:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.Year;
                    break;
                default:
                    throw new NopException("Not supported cycle period");
            }
            details.ScheduleDetails.PaymentPeriod.TotalBillingCycles = processPaymentRequest.RecurringTotalCycles;
            details.ScheduleDetails.PaymentPeriod.TotalBillingCyclesSpecified = true;



            using (var service2 = new PayPalAPIAASoapBinding())
            {
                if (!_paypalDirectPaymentSettings.UseSandbox)
                    service2.Url = "https://api-3t.paypal.com/2.0/";
                else
                    service2.Url = "https://api-3t.sandbox.paypal.com/2.0/";

                service2.RequesterCredentials = new CustomSecurityHeaderType();
                service2.RequesterCredentials.Credentials = new UserIdPasswordType();
                service2.RequesterCredentials.Credentials.Username = _paypalDirectPaymentSettings.ApiAccountName;
                service2.RequesterCredentials.Credentials.Password = _paypalDirectPaymentSettings.ApiAccountPassword;
                service2.RequesterCredentials.Credentials.Signature = _paypalDirectPaymentSettings.Signature;
                service2.RequesterCredentials.Credentials.Subject = "";

                CreateRecurringPaymentsProfileResponseType response = service2.CreateRecurringPaymentsProfile(req);

                string error = "";
                bool success = PaypalHelper.CheckSuccess(response, out error);
                if (success)
                {
                    result.NewPaymentStatus = PaymentStatus.Pending;
                    if (response.CreateRecurringPaymentsProfileResponseDetails != null)
                    {
                        result.SubscriptionTransactionId = response.CreateRecurringPaymentsProfileResponseDetails.ProfileID;
                    }
                }
                else
                {
                    result.AddError(error);
                }
            }

            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            //always success (cancel only on PayPal site)
            return new CancelRecurringPaymentResult();
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //it's not a redirection payment method. So we always return false
            return false;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentPayPalDirect";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.PayPalDirect.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentPayPalDirect";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.PayPalDirect.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentPayPalDirectController);
        }

        public override void Install()
        {
            var settings = new PayPalDirectPaymentSettings()
            {
                TransactMode = TransactMode.Authorize,
                UseSandbox = true,
            };
            _settingService.SaveSetting(settings);

            base.Install();
        }

        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.Automatic;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Standard;
            }
        }

        #endregion
    }
}