using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.PayPalDirect.Controllers;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;

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
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        #endregion

        #region Ctor

        public PayPalDirectPaymentProcessor(PayPalDirectPaymentSettings paypalDirectPaymentSettings,
            ISettingService settingService, 
            IGenericAttributeService genericAttributeService,
            ICurrencyService currencyService, ICustomerService customerService,
            CurrencySettings currencySettings, IWebHelper webHelper, 
            IOrderTotalCalculationService orderTotalCalculationService)
        {
            this._paypalDirectPaymentSettings = paypalDirectPaymentSettings;
            this._settingService = settingService;
            this._genericAttributeService = genericAttributeService;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._currencySettings = currencySettings;
            this._webHelper = webHelper;
            this._orderTotalCalculationService = orderTotalCalculationService;
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

        protected PayPalAPIInterfaceServiceService GetService()
        {
            var config = new Dictionary<string, string>();
            var url = _paypalDirectPaymentSettings.UseSandbox ? "https://api-3t.sandbox.paypal.com/2.0" : "https://api-3t.paypal.com/2.0";
            var mode = _paypalDirectPaymentSettings.UseSandbox ? "sandbox" : "live";

            config.Add("PayPalAPI", url);
            config.Add("mode", mode);
            config.Add("account0.apiUsername", _paypalDirectPaymentSettings.ApiAccountName);
            config.Add("account0.apiPassword", _paypalDirectPaymentSettings.ApiAccountPassword);
            config.Add("account0.apiSignature", _paypalDirectPaymentSettings.Signature);

            var service = new PayPalAPIInterfaceServiceService(config);
            return service;
        }

        /// <summary>
        /// Get Paypal country code
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>Paypal country code</returns>
        protected CountryCodeType GetPaypalCountryCodeType(Country country)
        {
            var payerCountry = CountryCodeType.US;
            try
            {
                payerCountry = (CountryCodeType)Enum.Parse(typeof(CountryCodeType), country.TwoLetterIsoCode.ToUpperInvariant());
            }
            catch
            {
            }
            return payerCountry;
        }

        /// <summary>
        /// Get Paypal credit card type
        /// </summary>
        /// <param name="creditCardType">Credit card type</param>
        /// <returns>Paypal credit card type</returns>
        protected CreditCardTypeType GetPaypalCreditCardType(string creditCardType)
        {
            if (String.IsNullOrEmpty(creditCardType))
                return CreditCardTypeType.VISA;

            if (creditCardType.Equals("VISA", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.VISA;
            if (creditCardType.Equals("MASTERCARD", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.MASTERCARD;
            if (creditCardType.Equals("DISCOVER", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.DISCOVER;
            if (creditCardType.Equals("AMEX", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.AMEX;
            if (creditCardType.Equals("MAESTRO", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.MAESTRO;
            if (creditCardType.Equals("SOLO", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.SOLO;
            if (creditCardType.Equals("SWITCH", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.SWITCH;

            return (CreditCardTypeType)Enum.Parse(typeof(CreditCardTypeType), creditCardType);
        }

        protected string GetApiVersion()
        {
            return "117";
        }

        protected ProcessPaymentResult AuthorizeOrSale(ProcessPaymentRequest processPaymentRequest, bool authorizeOnly)
        {
            var result = new ProcessPaymentResult();

            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (customer == null)
                throw new Exception("Customer cannot be loaded");

            var req = new DoDirectPaymentReq();
            req.DoDirectPaymentRequest = new DoDirectPaymentRequestType();
            req.DoDirectPaymentRequest.Version = GetApiVersion();
            var details = new DoDirectPaymentRequestDetailsType();
            req.DoDirectPaymentRequest.DoDirectPaymentRequestDetails = details;
            details.IPAddress = _webHelper.GetCurrentIpAddress() ?? "";
            if (authorizeOnly)
                details.PaymentAction = PaymentActionCodeType.AUTHORIZATION;
            else
                details.PaymentAction = PaymentActionCodeType.SALE;
            //credit card
            details.CreditCard = new CreditCardDetailsType();
            details.CreditCard.CreditCardNumber = processPaymentRequest.CreditCardNumber;
            details.CreditCard.CreditCardType = GetPaypalCreditCardType(processPaymentRequest.CreditCardType);
            details.CreditCard.ExpMonth = processPaymentRequest.CreditCardExpireMonth;
            details.CreditCard.ExpYear = processPaymentRequest.CreditCardExpireYear;
            details.CreditCard.CVV2 = processPaymentRequest.CreditCardCvv2;
            details.CreditCard.CardOwner = new PayerInfoType();
            details.CreditCard.CardOwner.PayerCountry = GetPaypalCountryCodeType(customer.BillingAddress.Country);
            //billing address
            details.CreditCard.CardOwner.Address = new AddressType();
            details.CreditCard.CardOwner.Address.Street1 = customer.BillingAddress.Address1;
            details.CreditCard.CardOwner.Address.Street2 = customer.BillingAddress.Address2;
            details.CreditCard.CardOwner.Address.CityName = customer.BillingAddress.City;
            if (customer.BillingAddress.StateProvince != null)
                details.CreditCard.CardOwner.Address.StateOrProvince = customer.BillingAddress.StateProvince.Abbreviation;
            else
                details.CreditCard.CardOwner.Address.StateOrProvince = "CA";
            details.CreditCard.CardOwner.Address.Country = GetPaypalCountryCodeType(customer.BillingAddress.Country);
            details.CreditCard.CardOwner.Address.PostalCode = customer.BillingAddress.ZipPostalCode;
            details.CreditCard.CardOwner.Payer = customer.BillingAddress.Email;
            details.CreditCard.CardOwner.PayerName = new PersonNameType();
            details.CreditCard.CardOwner.PayerName.FirstName = customer.BillingAddress.FirstName;
            details.CreditCard.CardOwner.PayerName.LastName = customer.BillingAddress.LastName;
            //order totals
            var payPalCurrency = PaypalHelper.GetPaypalCurrency(_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId));
            details.PaymentDetails = new PaymentDetailsType();
            details.PaymentDetails.OrderTotal = new BasicAmountType();
            details.PaymentDetails.OrderTotal.value = Math.Round(processPaymentRequest.OrderTotal, 2).ToString("N", new CultureInfo("en-us"));
            details.PaymentDetails.OrderTotal.currencyID = payPalCurrency;
            details.PaymentDetails.Custom = processPaymentRequest.OrderGuid.ToString();
            details.PaymentDetails.ButtonSource = "nopCommerceCart";
            //shipping
            if (customer.ShippingAddress != null)
            {
                if (customer.ShippingAddress.StateProvince != null && customer.ShippingAddress.Country != null)
                {
                    var shippingAddress = new AddressType();
                    shippingAddress.Name = customer.ShippingAddress.FirstName + " " + customer.ShippingAddress.LastName;
                    shippingAddress.Street1 = customer.ShippingAddress.Address1;
                    shippingAddress.Street2 = customer.ShippingAddress.Address2;
                    shippingAddress.CityName = customer.ShippingAddress.City;
                    shippingAddress.StateOrProvince = customer.ShippingAddress.StateProvince.Abbreviation;
                    shippingAddress.PostalCode = customer.ShippingAddress.ZipPostalCode;
                    shippingAddress.Country = (CountryCodeType)Enum.Parse(typeof(CountryCodeType), customer.ShippingAddress.Country.TwoLetterIsoCode, true);
                    details.PaymentDetails.ShipToAddress = shippingAddress;
                }
            }

            //send request
            var service = GetService();
            DoDirectPaymentResponseType response = service.DoDirectPayment(req);

            string error;
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
            return result;
        }

        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <param name="values">Values</param>
        /// <returns>Result</returns>
        public bool VerifyIpn(string formString, out Dictionary<string, string> values)
        {
            var req = (HttpWebRequest)WebRequest.Create(GetPaypalUrl());
            req.Method = WebRequestMethods.Http.Post;
            req.ContentType = MimeTypes.ApplicationXWwwFormUrlencoded;
            //now PayPal requires user-agent. otherwise, we can get 403 error
            req.UserAgent = HttpContext.Current.Request.UserAgent;

            string formContent = string.Format("{0}&cmd=_notify-validate", formString);
            req.ContentLength = formContent.Length;

            //PayPal requires TLS 1.2 since January 2016
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
            {
                sw.Write(formContent);
            }

            string response;
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
            
            return AuthorizeOrSale(processPaymentRequest, false);
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                _paypalDirectPaymentSettings.AdditionalFee, _paypalDirectPaymentSettings.AdditionalFeePercentage);
            return result;
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
            req.DoCaptureRequest.Amount.value = Math.Round(capturePaymentRequest.Order.OrderTotal, 2).ToString("N", new CultureInfo("en-us"));
            req.DoCaptureRequest.Amount.currencyID = PaypalHelper.GetPaypalCurrency(_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId));
            req.DoCaptureRequest.CompleteType = CompleteCodeType.COMPLETE;

            var service = GetService();
            DoCaptureResponseType response = service.DoCapture(req);

            string error;
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
            req.RefundTransactionRequest = new RefundTransactionRequestType
            {
                Version = GetApiVersion(),
                TransactionID = transactionId
            };

            if (refundPaymentRequest.IsPartialRefund)
            {
                req.RefundTransactionRequest.RefundType = RefundType.PARTIAL;
                req.RefundTransactionRequest.Amount = new BasicAmountType
                {
                    currencyID = PaypalHelper.GetPaypalCurrency(_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)),
                    value = refundPaymentRequest.AmountToRefund.ToString()
                };
            }
            else
                req.RefundTransactionRequest.RefundType = RefundType.FULL;

            var service = GetService();
            RefundTransactionResponseType response = service.RefundTransaction(req);

            string error;
            bool success = PaypalHelper.CheckSuccess(response, out error);
            if (success)
            {
                result.NewPaymentStatus = (refundPaymentRequest.IsPartialRefund &&
                    refundPaymentRequest.Order.RefundedAmount + refundPaymentRequest.AmountToRefund < refundPaymentRequest.Order.OrderTotal) ?
                    PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded;

                //set refund transaction id for preventing refund twice
                _genericAttributeService.SaveAttribute(refundPaymentRequest.Order, "RefundTransactionId", response.RefundTransactionID);
            }
            else
                result.AddError(error);

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

            var service = GetService();
            DoVoidResponseType response = service.DoVoid(req);

            string error;
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

            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);

            var req = new CreateRecurringPaymentsProfileReq();
            req.CreateRecurringPaymentsProfileRequest = new CreateRecurringPaymentsProfileRequestType();
            req.CreateRecurringPaymentsProfileRequest.Version = GetApiVersion();
            var details = new CreateRecurringPaymentsProfileRequestDetailsType();
            req.CreateRecurringPaymentsProfileRequest.CreateRecurringPaymentsProfileRequestDetails = details;

            details.CreditCard = new CreditCardDetailsType();
            details.CreditCard.CreditCardNumber = processPaymentRequest.CreditCardNumber;
            details.CreditCard.CreditCardType = GetPaypalCreditCardType(processPaymentRequest.CreditCardType);
            details.CreditCard.ExpMonth = processPaymentRequest.CreditCardExpireMonth;
            details.CreditCard.ExpYear = processPaymentRequest.CreditCardExpireYear;
            details.CreditCard.CVV2 = processPaymentRequest.CreditCardCvv2;
            details.CreditCard.CardOwner = new PayerInfoType();
            details.CreditCard.CardOwner.PayerCountry = GetPaypalCountryCodeType(customer.BillingAddress.Country);

            details.CreditCard.CardOwner.Address = new AddressType();
            details.CreditCard.CardOwner.Address.Street1 = customer.BillingAddress.Address1;
            details.CreditCard.CardOwner.Address.Street2 = customer.BillingAddress.Address2;
            details.CreditCard.CardOwner.Address.CityName = customer.BillingAddress.City;
            if (customer.BillingAddress.StateProvince != null)
                details.CreditCard.CardOwner.Address.StateOrProvince = customer.BillingAddress.StateProvince.Abbreviation;
            else
                details.CreditCard.CardOwner.Address.StateOrProvince = "CA";
            details.CreditCard.CardOwner.Address.Country = GetPaypalCountryCodeType(customer.BillingAddress.Country);
            details.CreditCard.CardOwner.Address.PostalCode = customer.BillingAddress.ZipPostalCode;
            details.CreditCard.CardOwner.Payer = customer.BillingAddress.Email;
            details.CreditCard.CardOwner.PayerName = new PersonNameType();
            details.CreditCard.CardOwner.PayerName.FirstName = customer.BillingAddress.FirstName;
            details.CreditCard.CardOwner.PayerName.LastName = customer.BillingAddress.LastName;

            //start date
            details.RecurringPaymentsProfileDetails = new RecurringPaymentsProfileDetailsType();
            details.RecurringPaymentsProfileDetails.BillingStartDate = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);
            details.RecurringPaymentsProfileDetails.ProfileReference = processPaymentRequest.OrderGuid.ToString();

            //schedule
            details.ScheduleDetails = new ScheduleDetailsType();
            details.ScheduleDetails.Description = "Recurring payment";
            details.ScheduleDetails.PaymentPeriod = new BillingPeriodDetailsType();
            details.ScheduleDetails.PaymentPeriod.Amount = new BasicAmountType();
            details.ScheduleDetails.PaymentPeriod.Amount.value = Math.Round(processPaymentRequest.OrderTotal, 2).ToString("N", new CultureInfo("en-us"));
            details.ScheduleDetails.PaymentPeriod.Amount.currencyID = PaypalHelper.GetPaypalCurrency(_currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId));
            details.ScheduleDetails.PaymentPeriod.BillingFrequency = processPaymentRequest.RecurringCycleLength;
            switch (processPaymentRequest.RecurringCyclePeriod)
            {
                case RecurringProductCyclePeriod.Days:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.DAY;
                    break;
                case RecurringProductCyclePeriod.Weeks:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.WEEK;
                    break;
                case RecurringProductCyclePeriod.Months:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.MONTH;
                    break;
                case RecurringProductCyclePeriod.Years:
                    details.ScheduleDetails.PaymentPeriod.BillingPeriod = BillingPeriodType.YEAR;
                    break;
                default:
                    throw new NopException("Not supported cycle period");
            }
            details.ScheduleDetails.PaymentPeriod.TotalBillingCycles = processPaymentRequest.RecurringTotalCycles;



            var service = GetService();
            CreateRecurringPaymentsProfileResponseType response = service.CreateRecurringPaymentsProfile(req);

            string error;
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
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            var order = cancelPaymentRequest.Order;

            var req = new ManageRecurringPaymentsProfileStatusReq();
            req.ManageRecurringPaymentsProfileStatusRequest = new ManageRecurringPaymentsProfileStatusRequestType();
            req.ManageRecurringPaymentsProfileStatusRequest.Version = GetApiVersion();
            var details = new ManageRecurringPaymentsProfileStatusRequestDetailsType();
            req.ManageRecurringPaymentsProfileStatusRequest.ManageRecurringPaymentsProfileStatusRequestDetails = details;

            details.Action = StatusChangeActionType.CANCEL;
            //Recurring payments profile ID returned in the CreateRecurringPaymentsProfile response
            details.ProfileID = order.SubscriptionTransactionId;

            var service = GetService();
            var response = service.ManageRecurringPaymentsProfileStatus(req);
            string error;
            if (!PaypalHelper.CheckSuccess(response, out error))
            {
                result.AddError(error);
            }

            return result;
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
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.PayPalDirect.Controllers" }, { "area", null } };
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
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.PayPalDirect.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentPayPalDirectController);
        }

        public override void Install()
        {
            //settings
            var settings = new PayPalDirectPaymentSettings
            {
                TransactMode = TransactMode.Authorize,
                UseSandbox = true,
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode", "Transaction mode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode.Hint", "Specify transaction mode.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ApiAccountName", "API Account Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ApiAccountName.Hint", "Specify API account name.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ApiAccountPassword", "API Account Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ApiAccountPassword.Hint", "Specify API account password.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.Signature", "Signature");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.Signature.Hint", "Specify signature.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
           
            base.Install();
        }
        
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<PayPalDirectPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.TransactMode.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ApiAccountName");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ApiAccountName.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ApiAccountPassword");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.ApiAccountPassword.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.Signature");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.Signature.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage.Hint");
           
            base.Uninstall();
        }

        #endregion

        #region Properties

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
                return true;
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

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}