using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.AuthorizeNet.Controllers;
using Nop.Plugin.Payments.AuthorizeNet.net.authorize.api;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Payments;
using Nop.Core.Domain;

namespace Nop.Plugin.Payments.AuthorizeNet
{
    /// <summary>
    /// AuthorizeNet payment processor
    /// </summary>
    public class AuthorizeNetPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly AuthorizeNetPaymentSettings _authorizeNetPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;
        private readonly StoreInformationSettings _storeInformationSettings;

        #endregion

        #region Ctor

        public AuthorizeNetPaymentProcessor(AuthorizeNetPaymentSettings authorizeNetPaymentSettings,
            ISettingService settingService, ICurrencyService currencyService,
            CurrencySettings currencySettings, IWebHelper webHelper,
            StoreInformationSettings storeInformationSettings)
        {
            this._authorizeNetPaymentSettings = authorizeNetPaymentSettings;
            this._settingService = settingService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._webHelper = webHelper;
            this._storeInformationSettings = storeInformationSettings;
        }

        #endregion

        #region Utilities


        /// <summary>
        /// Gets Authorize.NET URL
        /// </summary>
        /// <returns></returns>
        private string GetAuthorizeNETUrl()
        {
            return _authorizeNetPaymentSettings.UseSandbox ? "https://test.authorize.net/gateway/transact.dll" :
                "https://secure.authorize.net/gateway/transact.dll";
        }

        /// <summary>
        /// Gets Authorize.NET API version
        /// </summary>
        private string GetApiVersion()
        {
            return "3.1";
        }

        // Populate merchant authentication (ARB Support)
        private MerchantAuthenticationType PopulateMerchantAuthentication()
        {
            var authentication = new MerchantAuthenticationType();
            authentication.name = _authorizeNetPaymentSettings.LoginId;
            authentication.transactionKey = _authorizeNetPaymentSettings.TransactionKey;
            return authentication;
        }
        /// <summary>
        ///  Get errors (ARB Support)
        /// </summary>
        /// <param name="response"></param>
        private static string GetErrors(ANetApiResponseType response)
        {
            var sb = new StringBuilder();
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
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();

            var webClient = new WebClient();
            var form = new NameValueCollection();
            form.Add("x_login", _authorizeNetPaymentSettings.LoginId);
            form.Add("x_tran_key", _authorizeNetPaymentSettings.TransactionKey);
            if (_authorizeNetPaymentSettings.UseSandbox)
                form.Add("x_test_request", "TRUE");
            else
                form.Add("x_test_request", "FALSE");

            form.Add("x_delim_data", "TRUE");
            form.Add("x_delim_char", "|");
            form.Add("x_encap_char", "");
            form.Add("x_version", GetApiVersion());
            form.Add("x_relay_response", "FALSE");
            form.Add("x_method", "CC");
            form.Add("x_currency_code", _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
            if (_authorizeNetPaymentSettings.TransactMode == TransactMode.Authorize)
                form.Add("x_type", "AUTH_ONLY");
            else if (_authorizeNetPaymentSettings.TransactMode == TransactMode.AuthorizeAndCapture)
                form.Add("x_type", "AUTH_CAPTURE");
            else
                throw new NopException("Not supported transaction mode");

            var orderTotal = Math.Round(processPaymentRequest.OrderTotal, 2);
            form.Add("x_amount", orderTotal.ToString("0.00", CultureInfo.InvariantCulture));
            form.Add("x_card_num", processPaymentRequest.CreditCardNumber);
            form.Add("x_exp_date", processPaymentRequest.CreditCardExpireMonth.ToString("D2") + processPaymentRequest.CreditCardExpireYear.ToString());
            form.Add("x_card_code", processPaymentRequest.CreditCardCvv2);
            form.Add("x_first_name", processPaymentRequest.Customer.BillingAddress.FirstName);
            form.Add("x_last_name", processPaymentRequest.Customer.BillingAddress.LastName);
            if (!string.IsNullOrEmpty(processPaymentRequest.Customer.BillingAddress.Company))
                form.Add("x_company", processPaymentRequest.Customer.BillingAddress.Company);
            form.Add("x_address", processPaymentRequest.Customer.BillingAddress.Address1);
            form.Add("x_city", processPaymentRequest.Customer.BillingAddress.City);
            if (processPaymentRequest.Customer.BillingAddress.StateProvince != null)
                form.Add("x_state", processPaymentRequest.Customer.BillingAddress.StateProvince.Abbreviation);
            form.Add("x_zip", processPaymentRequest.Customer.BillingAddress.ZipPostalCode);
            if (processPaymentRequest.Customer.BillingAddress.Country != null)
                form.Add("x_country", processPaymentRequest.Customer.BillingAddress.Country.TwoLetterIsoCode);
            //20 chars maximum
            form.Add("x_invoice_num", processPaymentRequest.OrderGuid.ToString().Substring(0, 20));
            form.Add("x_customer_ip", _webHelper.GetCurrentIpAddress());

            string reply = null;
            Byte[] responseData = webClient.UploadValues(GetAuthorizeNETUrl(), form);
            reply = Encoding.ASCII.GetString(responseData);

            if (!String.IsNullOrEmpty(reply))
            {
                string[] responseFields = reply.Split('|');
                switch (responseFields[0])
                {
                    case "1":
                        result.AuthorizationTransactionCode = string.Format("{0},{1}", responseFields[6], responseFields[4]);
                        result.AuthorizationTransactionResult = string.Format("Approved ({0}: {1})", responseFields[2], responseFields[3]);
                        result.AvsResult = responseFields[5];
                        //responseFields[38];
                        if (_authorizeNetPaymentSettings.TransactMode == TransactMode.Authorize)
                        {
                            result.NewPaymentStatus = PaymentStatus.Authorized;
                        }
                        else
                        {
                            result.NewPaymentStatus = PaymentStatus.Paid;
                        }
                        break;
                    case "2":
                        result.AddError(string.Format("Declined ({0}: {1})", responseFields[2], responseFields[3]));
                        break;
                    case "3":
                        result.AddError(string.Format("Error: {0}", reply));
                        break;

                }
            }
            else
            {
                result.AddError("Authorize.NET unknown error");
            }

            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //nothing
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return _authorizeNetPaymentSettings.AdditionalFee;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();

            WebClient webClient = new WebClient();
            NameValueCollection form = new NameValueCollection();
            form.Add("x_login", _authorizeNetPaymentSettings.LoginId);
            form.Add("x_tran_key", _authorizeNetPaymentSettings.TransactionKey);
            if (_authorizeNetPaymentSettings.UseSandbox)
                form.Add("x_test_request", "TRUE");
            else
                form.Add("x_test_request", "FALSE");

            form.Add("x_delim_data", "TRUE");
            form.Add("x_delim_char", "|");
            form.Add("x_encap_char", "");
            form.Add("x_version", GetApiVersion());
            form.Add("x_relay_response", "FALSE");
            form.Add("x_method", "CC");
            form.Add("x_currency_code", _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
            form.Add("x_type", "PRIOR_AUTH_CAPTURE");

            var orderTotal = Math.Round(capturePaymentRequest.Order.OrderTotal, 2);
            form.Add("x_amount", orderTotal.ToString("0.00", CultureInfo.InvariantCulture));
            string[] codes = capturePaymentRequest.Order.AuthorizationTransactionCode.Split(',');
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
                        result.CaptureTransactionId = string.Format("{0},{1}", responseFields[6], responseFields[4]);
                        result.CaptureTransactionResult = string.Format("Approved ({0}: {1})", responseFields[2], responseFields[3]);
                        //result.AVSResult = responseFields[5];
                        //responseFields[38];
                        result.NewPaymentStatus = PaymentStatus.Paid;
                        break;
                    case "2":
                        result.AddError(string.Format("Declined ({0}: {1})", responseFields[2], responseFields[3]));
                        break;
                    case "3":
                        result.AddError(string.Format("Error: {0}", reply));
                        break;
                }
            }
            else
            {
                result.AddError("Authorize.NET unknown error");
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
            result.AddError("Refund method not supported");
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
            result.AddError("Void method not supported");
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

            var authentication = PopulateMerchantAuthentication();
            if (!processPaymentRequest.IsRecurringPayment)
            {
                var subscription = new ARBSubscriptionType();
                var creditCard = new net.authorize.api.CreditCardType();

                subscription.name = processPaymentRequest.OrderGuid.ToString();

                creditCard.cardNumber = processPaymentRequest.CreditCardNumber;
                creditCard.expirationDate = processPaymentRequest.CreditCardExpireYear + "-" + processPaymentRequest.CreditCardExpireMonth; // required format for API is YYYY-MM
                creditCard.cardCode = processPaymentRequest.CreditCardCvv2;

                subscription.payment = new PaymentType();
                subscription.payment.Item = creditCard;

                subscription.billTo = new NameAndAddressType();
                subscription.billTo.firstName = processPaymentRequest.Customer.BillingAddress.FirstName;
                subscription.billTo.lastName = processPaymentRequest.Customer.BillingAddress.LastName;
                subscription.billTo.address = processPaymentRequest.Customer.BillingAddress.Address1 + " " + processPaymentRequest.Customer.BillingAddress.Address2;
                subscription.billTo.city = processPaymentRequest.Customer.BillingAddress.City;
                if (processPaymentRequest.Customer.BillingAddress.StateProvince != null)
                {
                    subscription.billTo.state = processPaymentRequest.Customer.BillingAddress.StateProvince.Abbreviation;
                }
                subscription.billTo.zip = processPaymentRequest.Customer.BillingAddress.ZipPostalCode;

                if (processPaymentRequest.Customer.ShippingAddress != null)
                {
                    subscription.shipTo = new NameAndAddressType();
                    subscription.shipTo.firstName = processPaymentRequest.Customer.ShippingAddress.FirstName;
                    subscription.shipTo.lastName = processPaymentRequest.Customer.ShippingAddress.LastName;
                    subscription.shipTo.address = processPaymentRequest.Customer.ShippingAddress.Address1 + " " + processPaymentRequest.Customer.ShippingAddress.Address2;
                    subscription.shipTo.city = processPaymentRequest.Customer.ShippingAddress.City;
                    if (processPaymentRequest.Customer.ShippingAddress.StateProvince != null)
                    {
                        subscription.shipTo.state = processPaymentRequest.Customer.ShippingAddress.StateProvince.Abbreviation;
                    }
                    subscription.shipTo.zip = processPaymentRequest.Customer.ShippingAddress.ZipPostalCode;

                }

                subscription.customer = new CustomerType();
                subscription.customer.email = processPaymentRequest.Customer.BillingAddress.Email;
                subscription.customer.phoneNumber = processPaymentRequest.Customer.BillingAddress.PhoneNumber;

                subscription.order = new OrderType();
                subscription.order.description = string.Format("{0} {1}", _storeInformationSettings.StoreName, "Recurring payment");

                // Create a subscription that is leng of specified occurrences and interval is amount of days ad runs

                subscription.paymentSchedule = new PaymentScheduleType();
                DateTime dtNow = DateTime.UtcNow;
                subscription.paymentSchedule.startDate = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day);
                subscription.paymentSchedule.startDateSpecified = true;

                subscription.paymentSchedule.totalOccurrences = Convert.ToInt16(processPaymentRequest.RecurringTotalCycles);
                subscription.paymentSchedule.totalOccurrencesSpecified = true;

                var orderTotal = Math.Round(processPaymentRequest.OrderTotal, 2);
                subscription.amount = orderTotal;
                subscription.amountSpecified = true;

                // Interval can't be updated once a subscription is created.
                subscription.paymentSchedule.interval = new PaymentScheduleTypeInterval();
                switch (processPaymentRequest.RecurringCyclePeriod)
                {
                    case RecurringProductCyclePeriod.Days:
                        subscription.paymentSchedule.interval.length = Convert.ToInt16(processPaymentRequest.RecurringCycleLength);
                        subscription.paymentSchedule.interval.unit = ARBSubscriptionUnitEnum.days;
                        break;
                    case RecurringProductCyclePeriod.Weeks:
                        subscription.paymentSchedule.interval.length = Convert.ToInt16(processPaymentRequest.RecurringCycleLength * 7);
                        subscription.paymentSchedule.interval.unit = ARBSubscriptionUnitEnum.days;
                        break;
                    case RecurringProductCyclePeriod.Months:
                        subscription.paymentSchedule.interval.length = Convert.ToInt16(processPaymentRequest.RecurringCycleLength);
                        subscription.paymentSchedule.interval.unit = ARBSubscriptionUnitEnum.months;
                        break;
                    case RecurringProductCyclePeriod.Years:
                        subscription.paymentSchedule.interval.length = Convert.ToInt16(processPaymentRequest.RecurringCycleLength * 12);
                        subscription.paymentSchedule.interval.unit = ARBSubscriptionUnitEnum.months;
                        break;
                    default:
                        throw new NopException("Not supported cycle period");
                }

                using (var webService = new net.authorize.api.Service())
                {
                    if (_authorizeNetPaymentSettings.UseSandbox)
                        webService.Url = "https://apitest.authorize.net/soap/v1/Service.asmx";
                    else
                        webService.Url = "https://api.authorize.net/soap/v1/Service.asmx";

                    var response = webService.ARBCreateSubscription(authentication, subscription);

                    if (response.resultCode == MessageTypeEnum.Ok)
                    {
                        result.SubscriptionTransactionId = response.subscriptionId.ToString();
                        result.AuthorizationTransactionCode = response.resultCode.ToString();
                        result.AuthorizationTransactionResult = string.Format("Approved ({0}: {1})", response.resultCode.ToString(), response.subscriptionId.ToString());

                        if (_authorizeNetPaymentSettings.TransactMode == TransactMode.Authorize)
                        {
                            result.NewPaymentStatus = PaymentStatus.Authorized;
                        }
                        else
                        {
                            result.NewPaymentStatus = PaymentStatus.Paid;
                        }
                    }
                    else
                    {
                        result.AddError(string.Format("Error processing recurring payment. {0}", GetErrors(response)));
                    }
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
            //always success
            var result = new CancelRecurringPaymentResult();
            var authentication = PopulateMerchantAuthentication();
            long subscriptionId = 0;
            long.TryParse(cancelPaymentRequest.Order.SubscriptionTransactionId, out subscriptionId);


            using (var webService = new net.authorize.api.Service())
            {
                if (_authorizeNetPaymentSettings.UseSandbox)
                    webService.Url = "https://apitest.authorize.net/soap/v1/Service.asmx";
                else
                    webService.Url = "https://api.authorize.net/soap/v1/Service.asmx";

                var response = webService.ARBCancelSubscription(authentication, subscriptionId);

                if (response.resultCode == MessageTypeEnum.Ok)
                {
                    //ok
                }
                else
                {
                    result.AddError("Error cancelling subscription, please contact customer support. " + GetErrors(response));
                }
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
            controllerName = "PaymentAuthorizeNet";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.AuthorizeNet.Controllers" }, { "area", null } };
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
            controllerName = "PaymentAuthorizeNet";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.AuthorizeNet.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentAuthorizeNetController);
        }

        public override void Install()
        {
            var settings = new AuthorizeNetPaymentSettings()
            {
                UseSandbox = true,
                TransactMode = TransactMode.Authorize,
                TransactionKey = "123",
                LoginId = "456"
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
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.Manual;
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
