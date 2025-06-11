using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.PeachPayments.Domains;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Services.Customers;
using Nop.Core.Domain.Customers;
using System.Security.Cryptography;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Nop.Plugin.Payments.PeachPayments.Components;

namespace Nop.Plugin.Payments.PeachPayments
{
    public class PeachPaymentsPaymentMethod : BasePlugin, IPaymentMethod
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly PeachPaymentsSettings _settings;
        private readonly PaymentSettings _paymentSettings;
        private readonly WidgetSettings _widgetSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerService _customerService;
        private readonly IHttpClientFactory _httpClientFactory;

        public PeachPaymentsPaymentMethod(IUrlHelperFactory urlHelperFactory,
                IActionContextAccessor actionContextAccessor,
                ICurrencyService currencyService,
                IAddressService addressService,
                ICountryService countryService,
                IHttpContextAccessor httpContextAccessor,
                ILocalizationService localizationService,
                ISettingService settingService,
                PeachPaymentsSettings settings,
                PaymentSettings paymentSettings,
                WidgetSettings widgetSettings,
                IWebHelper webHelper, ICustomerService customerService,
                IGenericAttributeService genericAttributeService,
                IHttpClientFactory httpClientFactory
        )
        {
            _genericAttributeService = genericAttributeService;
            _customerService = customerService;
            _currencyService = currencyService;
            _addressService = addressService;
            _countryService = countryService;
            _httpContextAccessor = httpContextAccessor;
            _webHelper = webHelper;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
            _settingService = settingService;
            _settings = settings;
            _paymentSettings = paymentSettings;
            _widgetSettings = widgetSettings;
            _httpClientFactory = httpClientFactory;
        }
        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => false;

        public bool HideInWidgetList => true;

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            return Task.FromResult(false);
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            //already set
            var paymentInfo = new ProcessPaymentRequest();

            return Task.FromResult(paymentInfo);
        }

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            string method_description = _settings.PeachPaymentsCheckoutDisplayText;
            if (string.IsNullOrEmpty(method_description))
                method_description = "Pay now using Peach Payments";
            return method_description;
        }

        public string GetPublicViewComponentName()
        {
            return PeachPaymentsDefaults.PAYMENT_INFO_VIEW_COMPONENT_NAME;
        }

        public async Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var remotePostHelperData = new Dictionary<string, string>();
            int sandBoxModeInt = (int)SandboxMode.Enabled;
            bool isSandboxMode = _settings.SandBoxModeId == sandBoxModeInt;
            string checkoutUrl = isSandboxMode ? "https://testsecure.peachpayments.com/checkout/initiate" : "https://secure.peachpayments.com/checkout/initiate";
            string ourBaseUrl = _webHelper.GetStoreLocation();
            // Checkout doesn't allow `localhost` as a valid URL but does allow `127.0.0.1`
            if (ourBaseUrl.StartsWith("http://localhost") || ourBaseUrl.StartsWith("https://localhost"))
            {
                ourBaseUrl = ourBaseUrl.Replace("localhost", "127.0.0.1");
            }

            var billingAddress = await _addressService.GetAddressByIdAsync(postProcessPaymentRequest.Order.BillingAddressId);
            var peachcurrencycode = await _currencyService.GetCurrencyByIdAsync(_settings.CurrencyId);
            if (postProcessPaymentRequest.Order.CustomerCurrencyCode == peachcurrencycode.CurrencyCode)
            {
                remotePostHelperData.Add("amount", Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2).ToString());
            }
            else
            {
                var primary_currency = await _currencyService.GetCurrencyByCodeAsync(postProcessPaymentRequest.Order.CustomerCurrencyCode);
                var amount_to_be_converted = postProcessPaymentRequest.Order.OrderTotal;
                var amount_converted = await _currencyService.ConvertCurrencyAsync(amount_to_be_converted, primary_currency, peachcurrencycode);
                remotePostHelperData.Add("amount", Math.Round(amount_converted, 2).ToString());
            }

            if (isSandboxMode)
            {
                remotePostHelperData.Add("authentication.entityId", _settings.CheckoutChannelSandbox);
            }
            else
            {
                remotePostHelperData.Add("authentication.entityId", _settings.CheckoutChannel);
            }

            if (!string.IsNullOrEmpty(billingAddress.City))
                remotePostHelperData.Add("billing.city", billingAddress.City);
            if (!string.IsNullOrEmpty(billingAddress.Company))
                remotePostHelperData.Add("billing.company", billingAddress.Company);
            var billingCountry = await _countryService.GetCountryByAddressAsync(billingAddress);
            if (billingCountry != null)
                remotePostHelperData.Add("billing.country", billingCountry.TwoLetterIsoCode);
            if (!string.IsNullOrEmpty(billingAddress.FirstName))
                remotePostHelperData.Add("billing.customer.givenName", billingAddress.FirstName);
            if (!string.IsNullOrEmpty(billingAddress.LastName))
                remotePostHelperData.Add("billing.customer.surname", billingAddress.LastName);
            if (!string.IsNullOrEmpty(billingAddress.ZipPostalCode))
                remotePostHelperData.Add("billing.postcode", billingAddress.ZipPostalCode);

            if (!string.IsNullOrEmpty(billingAddress.Address1))
                remotePostHelperData.Add("billing.street1", billingAddress.Address1);
            if (!string.IsNullOrEmpty(billingAddress.Address2))
                remotePostHelperData.Add("billing.street2", billingAddress.Address2);
            remotePostHelperData.Add("currency", peachcurrencycode.CurrencyCode);
            remotePostHelperData.Add("customParameters[plugin_version]", NopVersion.CURRENT_VERSION);

            var customer = await _customerService.GetCustomerByIdAsync(postProcessPaymentRequest.Order.CustomerId);
            if (customer != null)
            {
                //remotePostHelperData.Add("customer.email", customer.Email);
                bool isGuest = await _customerService.IsGuestAsync(customer);
                if (!isGuest)
                {

                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        remotePostHelperData.Add("customer.email", customer.Email);
                    }
                    if (!string.IsNullOrEmpty(customer.FirstName))
                    {
                        remotePostHelperData.Add("customer.firstname", customer.FirstName);
                    }
                    if (!string.IsNullOrEmpty(billingAddress.PhoneNumber))
                    {
                        remotePostHelperData.Add("customer.mobile", billingAddress.PhoneNumber);
                    }
                    if (!string.IsNullOrEmpty(customer.LastName))
                    {
                        remotePostHelperData.Add("customer.surname",customer.LastName);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(billingAddress.Email))
                    {
                        remotePostHelperData.Add("customer.email", billingAddress.Email);
                    }
                    if (!string.IsNullOrEmpty(billingAddress.FirstName))
                    {
                        remotePostHelperData.Add("customer.firstname", billingAddress.FirstName);
                    }
                    if (!string.IsNullOrEmpty(billingAddress.PhoneNumber))
                    {
                        remotePostHelperData.Add("customer.mobile", billingAddress.PhoneNumber);
                    }
                    if (!string.IsNullOrEmpty(billingAddress.LastName))
                    {
                        remotePostHelperData.Add("customer.surname", billingAddress.LastName);
                    }
                }
            }

            remotePostHelperData.Add("merchantTransactionId", _settings.MerchantIdPrefix + postProcessPaymentRequest.Order.Id.ToString());
            remotePostHelperData.Add("nonce", Guid.NewGuid().ToString().Replace('-'.ToString(), string.Empty));
            remotePostHelperData.Add("originator", "NopCommerce");
            remotePostHelperData.Add("paymentType", "DB");
            

            //Delivery details
            var shippingAddress = await _addressService.GetAddressByIdAsync(postProcessPaymentRequest.Order.ShippingAddressId ?? 0);

            if (postProcessPaymentRequest.Order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                if (!string.IsNullOrEmpty(shippingAddress.City))
                    remotePostHelperData.Add("shipping.city", shippingAddress.City);
                if (!string.IsNullOrEmpty(shippingAddress.Company))
                    remotePostHelperData.Add("shipping.company", shippingAddress.Company);
                var shippingCountry = await _countryService.GetCountryByAddressAsync(shippingAddress);
                if (shippingCountry != null)
                    remotePostHelperData.Add("shipping.country", shippingCountry.TwoLetterIsoCode);
                if (!string.IsNullOrEmpty(shippingAddress.FirstName))
                    remotePostHelperData.Add("shipping.customer.givenName", shippingAddress.FirstName);
                if (!string.IsNullOrEmpty(shippingAddress.LastName))
                    remotePostHelperData.Add("shipping.customer.surname", shippingAddress.LastName);
                if (!string.IsNullOrEmpty(shippingAddress.ZipPostalCode))
                    remotePostHelperData.Add("shipping.postcode", shippingAddress.ZipPostalCode);
                if (!string.IsNullOrEmpty(shippingAddress.Address1))
                    remotePostHelperData.Add("shipping.street1", shippingAddress.Address1);
                if (!string.IsNullOrEmpty(shippingAddress.Address2))
                    remotePostHelperData.Add("shipping.street2", shippingAddress.Address2);
            }

            remotePostHelperData.Add("shopperResultUrl", ourBaseUrl + "Plugins/PeachPayments/Result");
            string signature = GenerateSignature(remotePostHelperData, isSandboxMode);

            remotePostHelperData.Add("signature", signature);

            try
            {
                JsonContent content = JsonContent.Create(remotePostHelperData);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Referrer = new Uri(ourBaseUrl);
                httpClient.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var response = await httpClient.PostAsync(checkoutUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    string value = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine(value);
                    var responseContent = JsonConvert.DeserializeObject<CheckoutRedirectResponse>(value);

                    _httpContextAccessor.HttpContext.Response.Redirect(responseContent.RedirectUrl);
                    return;
                }
                else
                {
                    string value = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(value);
                }
                // we need to handle error case here
                // Not sure how to show error to user.
                throw new NopException("Payment method couldn't be loaded");
            }
            catch (Exception ep)
            {
                throw new Exception(ep.Message);
            }
        }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Pending };

            return Task.FromResult(result);
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public string GenerateSignature(Dictionary<string, string> remotePostHelperData, bool isSandbox)
        {
            var signaturedata = string.Empty;
            foreach (var item in remotePostHelperData)
                signaturedata = signaturedata + item.Key + item.Value;
            string keyhmac = string.Empty;
            var enc = Encoding.Default;
            if (isSandbox)
                keyhmac = _settings.SecretTokenSandbox;
            else
                keyhmac = _settings.SecretToken;
            var hash = new HMACSHA256(enc.GetBytes(keyhmac));
            byte[] baHashedText = hash.ComputeHash(enc.GetBytes(signaturedata));
            string signature = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return signature;
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of validating errors
        /// </returns>
        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            throw new NotImplementedException();
        }
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(PeachPaymentsDefaults.ConfigurationRouteName);
        }
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new PeachPaymentsSettings
            {
                PaymentType = PaymentType.Capture,
                LogoInHeaderLinks = @"<!-- Peachpayments Logo --><li><a href=""https://www.peachpayments.com/"" title=""How PeachPayments Works"" onclick=""javascript:window.open('https://www.peachpayments.com/','WIPeachPayment','toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, width=1060, height=700'); return false;""><img style=""padding-top:10px;"" src=""https://www.peachpayments.com/hubfs/peach-logo-EC5228.svg"" border=""0"" alt=""Now accepting Peachpayments""></a></li><!-- PeachPayments Logo -->",
                LogoInFooter = @"<!-- Peachpayments Logo --><div><a href=""https://www.peachpayments.com/"" title=""How PeachPayments Works"" onclick=""javascript:window.open('https://www.peachpayments.com/','WIPeachPayment','toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, width=1060, height=700'); return false;""><img src=""https://www.peachpayments.com/hubfs/peach-logo-EC5228.svg"" border=""0"" alt=""PeachPayments Acceptance Mark""></a></div><!-- PeachPayments Logo -->",
                StyleLayout = "vertical",
                StyleColor = "blue",
                StyleShape = "rect",
                StyleLabel = "peach",
                DisplayButtonsOnProductDetails = true,
                DisplayButtonsOnShoppingCart = true,
                DisplayPayLaterMessages = false,
                RequestTimeout = PeachPaymentsDefaults.RequestTimeout,
                MinDiscountAmount = 0.5M,
            });

            if (!_paymentSettings.ActivePaymentMethodSystemNames.Contains(PeachPaymentsDefaults.SystemName))
            {
                _paymentSettings.ActivePaymentMethodSystemNames.Add(PeachPaymentsDefaults.SystemName);
                await _settingService.SaveSettingAsync(_paymentSettings);
            }

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(PeachPaymentsDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(PeachPaymentsDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Payments.PeachPayments.Fields.CheckoutChannel"] = "Checkout Channel",
                ["Plugins.Payments.PeachPayments.Fields.CheckoutChannel.Required"] = "Checkout channel is required",
                ["Plugins.Payments.PeachPayments.Fields.SecretToken"] = "Secret Token",
                ["Plugins.Payments.PeachPayments.Fields.CheckoutChannelSandbox"] = "Checkout Channel (Sandbox)",
                ["Plugins.Payments.PeachPayments.Fields.CheckoutChannelSandbox.Required"] = "Checkout channel(sandbox) is required",
                ["Plugins.Payments.PeachPayments.Fields.SecretTokenSandbox"] = "Secret Token (Sandbox)",
                ["Plugins.Payments.PeachPayments.Fields.Callbackurl"] = "Callback url",
                ["Plugins.Payments.PeachPayments.Fields.MerchantIdPrefix"] = "Merchant Id Prefix",
                ["Plugins.Payments.PeachPayments.Fields.MerchantIdPrefix.Required"] = "Merchant Id is required and size between 8 to 10 characters",
                ["Plugins.Payments.PeachPayments.Fields.PeachPaymentsCheckoutDisplayText"] = "Peach Payments Checkout Display Text",
                ["Plugins.Payments.PeachPayments.Fields.SortOrder"] = "Sort Order",
                ["Plugins.Payments.PeachPayments.Fields.SandBoxMode"] = "Sandbox Mode",
                ["Plugins.Payments.PeachPayments.Fields.Currency"] = "Peach Payments Currency",
                ["Plugins.Payments.PeachPayments.PaymentMethodDescription"] = "Pay now using Peach Payments",
                ["Plugins.Payments.PeachPayments.Fields.CheckoutChannel.Hint"] = "The channel that you received from Peach Payments.",
                ["Plugins.Payments.PeachPayments.Fields.SecretToken.Hint"] = "This is the key generated with the Peach Payments Console. You can find this under Checkout>Live configuration.",
                ["Plugins.Payments.PeachPayments.Fields.CheckoutChannelSandbox.Hint"] = "The channel that you received from Peach Payments for sandbox mode.",
                ["Plugins.Payments.PeachPayments.Fields.SecretTokenSandbox.Hint"] = "This is the key generated with the Peach Payments Console for sandbox mode. You can find this under  Checkout>Live configuration.",
                ["Plugins.Payments.PeachPayments.Fields.Callbackurl.Hint"] = "Callback url to be configured in Peach Payments Console webhooks",
                ["Plugins.Payments.PeachPayments.Fields.MerchantIdPrefix.Hint"] = "Merchant Prefix is alphanumberic field of length between 8 to 10 characters. Any non alphanumric character and spaces will be removed automatically.",
                ["Plugins.Payments.PeachPayments.Fields.PeachPaymentsCheckoutDisplayText.Hint"] = "This text will be shown on the checkout page for Peach Payments option. If left blank default text will be shown.",

                ["Plugins.Payments.PeachPayments.Fields.Currency.Hint"] = "Select your payment Currency. Please make sure you are activated for this currency with Peach Payments. Currency conversion is done according to nopCommerce's configuration. "
            });

            await base.InstallAsync();
        }

        public Type GetPublicViewComponent()
        {
            return typeof(PeachPaymentsViewComponent);
        }
    }

    record CheckoutRedirectResponse(string RedirectUrl);
}