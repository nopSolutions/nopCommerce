using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.WorldpayHpp.Constants;
using Nop.Plugin.Payments.WorldpayHpp.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.WorldpayHpp.Services
{
    public class WorldpayHppPaymentProcessor : BasePlugin, IPaymentMethod
    {
        private readonly ISettingService _settingService;
        private readonly WorldpayHppService _hppService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;

        public WorldpayHppPaymentProcessor(
            ISettingService settingService,
            WorldpayHppService hppService,
            ILogger logger,
            IWebHelper webHelper,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
             IGenericAttributeService genericAttributeService
            )
        {
            _settingService = settingService;
            _hppService = hppService;
            _logger = logger;
            _webHelper = webHelper;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _genericAttributeService = genericAttributeService;
        }

        public bool SupportCapture => false;
        public bool SupportRefund => false;
        public bool SupportPartiallyRefund => false;
        public bool SupportVoid => true;
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;
        public bool SkipPaymentInfo => true;

        public string PaymentMethodDescription => "Pay securely via Worldpay Hosted Payment Pages";

        public async Task<ProcessPaymentResult> ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            // nopCommerce will call PostProcessPayment to redirect
            var result = new ProcessPaymentResult
            {
                NewPaymentStatus = PaymentStatus.Pending
            };
            return await Task.FromResult(result);
        }

        public async Task PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var order = postProcessPaymentRequest.Order;

            // Load settings here
            var settings = await _settingService.LoadSettingAsync<WorldpayHppSettings>();

            var session = await _hppService.CreateSessionAsync(order);

            var httpContext = _httpContextAccessor.HttpContext;
            if (string.IsNullOrEmpty(session.Url))
                throw new Exception("Worldpay HPP redirectUrl not provided");

            httpContext.Response.Redirect(session.Url);
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
            => Task.FromResult(false);

        public string GetPublicViewComponentName() => "PaymentWorldpayHpp";

        public override string GetConfigurationPageUrl()
            => $"{_webHelper.GetStoreLocation()}Admin/WorldpayHpp/Configure";

        public override async Task InstallAsync()
        {
            var defaults = new WorldpayHppSettings
            {
                Environment = "TEST",
                //UseIframe = false
            };

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {

                ["Plugins.Payments.WorldPayHpp.PaymentMethodDescription"] = "WorldPay Checkout with using methods like debit and credit card payments and other supported methods",
            });

             await _settingService.SaveSettingAsync(defaults);
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<WorldpayHppSettings>();
            await base.UninstallAsync();
        }

        public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            // nopCommerce will call PostProcessPayment to redirect
            var result = new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Pending }; 
            return await Task.FromResult(result);
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var order = postProcessPaymentRequest.Order; 
            // Load settings here
            var settings = await _settingService.LoadSettingAsync<WorldpayHppSettings>(); 
            var paymentResponse = await _hppService.CreateSessionAsync(order);
            var httpContext = _httpContextAccessor.HttpContext;

            if (!string.IsNullOrWhiteSpace(paymentResponse?.Links?.Self?.Href))
            {
                await _genericAttributeService.SaveAttributeAsync(order, KeyName.WORLDPAY_PAYMENT_QUERY_HREF, paymentResponse.Links.Self.Href);
            }


            
            if (string.IsNullOrEmpty(paymentResponse.Url)) 
                throw new Exception("Worldpay HPP redirectUrl not provided"); 
            
            httpContext.Response.Redirect(paymentResponse.Url);
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            decimal handlingFee = 0;

            //throw new NotImplementedException();
            return Task.FromResult(handlingFee);
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            //it's not a redirection payment method. So we always return false
            return Task.FromResult(false);
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            throw new NotImplementedException();
        }

        public Type GetPublicViewComponent()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            //return await _localizationService.GetResourceAsync("Plugins.Payments.WorldpayHpp.PaymentMethodDescription");
            return await Task.FromResult("Pay securely via Worldpay Hosted Payment Pages");
        }
    }
}
