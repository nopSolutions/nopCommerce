using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Core.Domain.Directory;
using Nop.Core;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Synchrony.Controllers;
using Nop.Core.Domain.Localization;
using Nop.Services.Plugins;
using Microsoft.AspNetCore.Http;
using Nop.Data;
using Nop.Plugin.Payments.Synchrony.Models;
using Nop.Core.Http.Extensions;
using Nop.Services.Localization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Synchrony
{
    /// <summary>
    /// Synchrony payment processor
    /// </summary>
    public class SynchronyPaymentProcessor : BasePlugin, IPaymentMethod
    {
        private readonly ISettingService _settingService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ITaxService _taxService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IRepository<Language> _languageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;

        public SynchronyPaymentProcessor(
            ISettingService settingService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IWebHelper webHelper,
            ICheckoutAttributeParser checkoutAttributeParser,
            ITaxService taxService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IRepository<Language> languageRepository,
            IHttpContextAccessor httpContextAccessor,
            IWorkContext workContext,
            IStoreContext storeContext,
            ILocalizationService localizationService
        )
        {
            _settingService = settingService;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            _webHelper = webHelper;
            _checkoutAttributeParser = checkoutAttributeParser;
            _taxService = taxService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _languageRepository = languageRepository;
            _httpContextAccessor = httpContextAccessor;
            _workContext = workContext;
            _storeContext = storeContext;
            _localizationService = localizationService;
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

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return Task.FromResult(result);
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        public Type GetControllerType()
        {
            return typeof(SynchronyPaymentController);
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return Task.FromResult(false);
        }

        public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            // nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();

            var statusmsg = processPaymentRequest.CustomValues.Where(x => x.Key == "StatusMessage").FirstOrDefault();
            var authCode = processPaymentRequest.CustomValues.Where(x => x.Key == "AuthCode").FirstOrDefault();
            if (authCode.Value != null && !string.IsNullOrEmpty(authCode.Value.ToString()))
            {
                result.AuthorizationTransactionCode = authCode.Value.ToString();
            }
            result.AllowStoringCreditCardNumber = true;
            if (statusmsg.Value != null && statusmsg.Value.Equals("Auth Approved"))
                result.NewPaymentStatus = PaymentStatus.Paid;
            else
                result.NewPaymentStatus = PaymentStatus.Pending;

            return Task.FromResult(result);
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
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

        public override async Task InstallAsync()
        {
            // settings
            await _settingService.SaveSettingAsync(SynchronyPaymentSettings.Default());

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            // settings
            await _settingService.DeleteSettingAsync<SynchronyPaymentSettings>();

            await base.UninstallAsync();
        }

        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Standard;
            }
        }

        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

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

        public Task<string> GetPaymentMethodDescriptionAsync() => Task.FromResult("Allows for payment via ABC Warehouse Card.");

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            var warnings = new List<string>();
            return Task.FromResult<IList<string>>(warnings);
        }

        public async Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            var syfPaymentInfoJson = _httpContextAccessor.HttpContext.Session.GetString("syfPaymentInfo");
            var syfPaymentInfo = JsonSerializer.Deserialize<AuthenticationTokenResponse>(syfPaymentInfoJson);
            if (syfPaymentInfo?.StatusMessage != "Auth Approved" &&
                syfPaymentInfo?.StatusMessage != "Account Authentication Success")
            {
                throw new Exception("Failure when getting Synchrony Payment Info.");
            }

            var paymentInfo = new ProcessPaymentRequest();
            paymentInfo.CreditCardNumber = syfPaymentInfo.AccountNumber;
            paymentInfo.CreditCardCvv2 =
                syfPaymentInfo.PromoCode != null && syfPaymentInfo.PromoCode.Length > 3 ?
                syfPaymentInfo.PromoCode.Substring(syfPaymentInfo.PromoCode.Length - 4, 3) :
                syfPaymentInfo.PromoCode;

            var authCode = syfPaymentInfo.AuthCode;
            if (authCode != null)
            {
                paymentInfo.CustomValues.Add("AuthCode", syfPaymentInfo.AuthCode);
            }

            var CreditCardName = syfPaymentInfo.FirstName + " " + syfPaymentInfo.LastName;
            paymentInfo.CreditCardName = CreditCardName.Length > 50 ? CreditCardName.Substring(0, 50) : CreditCardName;
            paymentInfo.CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;
            paymentInfo.StoreId = (await _storeContext.GetCurrentStoreAsync()).Id;
            paymentInfo.CustomValues.Add("StatusMessage", syfPaymentInfo.StatusMessage);
            return paymentInfo;
        }

        public string GetPublicViewComponentName()
        {
            return "Synchrony";
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/SynchronyPayment/Configure";
        }
    }
}
