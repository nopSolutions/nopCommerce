using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Core;
using Nop.Services.Payments;
using Nop.Core.Domain.Orders;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text.Json;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Payments;

namespace AbcWarehouse.Plugin.Payments.UniFi
{
    public class UniFiPaymentsProcessor : BasePlugin, IPaymentMethod
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly UniFiPaymentsSettings _settings;

        public UniFiPaymentsProcessor(
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IWorkContext workContext,
            UniFiPaymentsSettings settings)
        {
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _workContext = workContext;
            _settings = settings;
        }

        public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var httpClient = new HttpClient();
            var bearerToken = await _genericAttributeService.GetAttributeAsync<string>(
                await _workContext.GetCurrentCustomerAsync(),
                "UniFiBearerToken");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);

            var transactEndpoint = _settings.UseIntegration ?
                "https://api-stg.syf.com/v1/credit/authorizations/digital-buy/transactions" :
                "https://api.syf.com/v1/credit/authorizations/digital-buy/transactions";
            var transactionToken = _httpContextAccessor.HttpContext.Session.GetString("TransactionToken");
            var address1 = _httpContextAccessor.HttpContext.Session.GetString("Address1");
            var city = _httpContextAccessor.HttpContext.Session.GetString("City");
            var state = _httpContextAccessor.HttpContext.Session.GetString("State");
            var zip = _httpContextAccessor.HttpContext.Session.GetString("Zip");
            var amount = processPaymentRequest.OrderTotal;

            // https://stackoverflow.com/a/23121386
            // allows dot notation in simple json object
            var payload = new Dictionary<string, object>
            {
                { "transactionToken", transactionToken },
                { "addressInfo", new Dictionary<string, object> {
                    { "cipher.addressLine1", address1 },
                    { "cipher.city", city },
                    { "cipher.state", state },
                    { "cipher.zipCode", zip },
                } },
                { "transactionInfo", new [] {
                    new {
                        amount = amount.ToString("0.00"),
                    }
                } }
            };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(
                transactEndpoint, content
            );

            var result = new ProcessPaymentResult();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                result.AddError("Failure to process payment.");
                return result;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);
            var statusCode = responseJson.RootElement
                                               .GetProperty("transactionInfo")
                                               .GetProperty("statusCode").GetString();
            
            if (statusCode != "000")
            {
                result.AddError("Payment not approved.");
            }
            else
            {
                result.NewPaymentStatus = PaymentStatus.Paid;
            }

            return result;
        }

        public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            // nothing
            return Task.CompletedTask;
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return Task.FromResult(false);
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return Task.FromResult(result);
        }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return Task.FromResult(result);
        }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return Task.FromResult(result);
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return Task.FromResult(result);
        }

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return Task.FromResult(result);
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            // let's ensure that at least 5 seconds passed after order is placed
            // P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            var warnings = new List<string>();
            return Task.FromResult<IList<string>>(warnings);
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();

            _httpContextAccessor.HttpContext.Session.SetString("TransactionToken", form["transactionToken"]);
            _httpContextAccessor.HttpContext.Session.SetString("Address1", form["custAddress1"]);
            _httpContextAccessor.HttpContext.Session.SetString("City", form["custCity"]);
            _httpContextAccessor.HttpContext.Session.SetString("State", form["custState"]);
            _httpContextAccessor.HttpContext.Session.SetString("Zip", form["custZipCode"]);
            paymentInfo.OrderTotal = Convert.ToDecimal(form["transAmount1"]);

            return Task.FromResult(paymentInfo);
        }

        public string GetPublicViewComponentName()
        {
            return "UniFiPaymentsProcessor";
        }

        public Task<string> GetPaymentMethodDescriptionAsync() => Task.FromResult("Allows for payment via ABC Warehouse Card.");

        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Standard;
            }
        }

        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

        public override string GetConfigurationPageUrl()
        {
            return
                $"{_webHelper.GetStoreLocation()}Admin/UniFiPayments/Configure";
        }

        public override async Task InstallAsync()
        {
            await UpdateLocales();

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourcesAsync(UniFiPaymentsLocales.Base);

            await base.UninstallAsync();
        }

        public override async Task UpdateAsync(string oldVersion, string currentVersion)
        {
            await UpdateLocales();
        }

        private async Task UpdateLocales()
        {
            await _localizationService.AddLocaleResourceAsync(
                new Dictionary<string, string>
                {
                    [UniFiPaymentsLocales.ClientId] = "Client ID",
                    [UniFiPaymentsLocales.ClientIdHint] = "Client ID provided by Synchrony.",
                    [UniFiPaymentsLocales.ClientSecret] = "Client Secret",
                    [UniFiPaymentsLocales.ClientSecretHint] = "Client Secret provided by Synchrony.",
                    [UniFiPaymentsLocales.UseIntegration] = "Use Integration",
                    [UniFiPaymentsLocales.UseIntegrationHint] = "Use the integration region (for testing).",
                });
        }
    }
}
