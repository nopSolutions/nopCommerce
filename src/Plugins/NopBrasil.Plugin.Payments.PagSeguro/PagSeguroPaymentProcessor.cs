using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using NopBrasil.Plugin.Payments.PagSeguro.Controllers;
using NopBrasil.Plugin.Payments.PagSeguro.Services;
using NopBrasil.Plugin.Payments.PagSeguro.Task;

namespace NopBrasil.Plugin.Payments.PagSeguro
{
    public class PagSeguroPaymentProcessor : BasePlugin, IPaymentMethod
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPagSeguroService _pagSeguroService;
        private readonly ISettingService _settingService;
        private readonly PagSeguroPaymentSetting _pagSeguroSetting;
        private readonly CheckPaymentTask _checkPaymentTask;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public PagSeguroPaymentProcessor(ILogger logger,
            IHttpContextAccessor httpContextAccessor,
            IPagSeguroService pagSeguroService,
            ISettingService settingService,
            PagSeguroPaymentSetting pagSeguroSetting,
            CheckPaymentTask checkPaymentTask,
            IWebHelper webHelper,
            ILocalizationService localizationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _pagSeguroService = pagSeguroService ?? throw new ArgumentNullException(nameof(pagSeguroService));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _pagSeguroSetting = pagSeguroSetting ?? throw new ArgumentNullException(nameof(pagSeguroSetting));
            _checkPaymentTask = checkPaymentTask ?? throw new ArgumentNullException(nameof(checkPaymentTask));
            _webHelper = webHelper ?? throw new ArgumentNullException(nameof(webHelper));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        }

        public override void Install()
        {
            _localizationService.AddOrUpdatePluginLocaleResource("NopBrasil.Plugins.Payments.PagSeguro.Fields.Redirection", "Você será redirecionado para a pagina do Uol PagSeguro");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.EmailAdmin.PagSeguro", "Email - PagSeguro");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.Token.PagSeguro", "Token - PagSeguro");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.MethodDescription.PagSeguro", "Descrição que será exibida no checkout");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.MethodDescription.PagSeguro.IsSandbox", "Ambiente Sandbox");
            _checkPaymentTask.InstallTask();
            base.Install();
        }

        public override void Uninstall()
        {
            _checkPaymentTask.UninstallTask();
            _settingService.DeleteSetting<PagSeguroPaymentSetting>();
            _localizationService.DeletePluginLocaleResource("NopBrasil.Plugins.Payments.PagSeguro.Fields.Redirection");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.EmailAdmin.PagSeguro");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.Token.PagSeguro");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.MethodDescription.PagSeguro");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.MethodDescription.PagSeguro.IsSandbox");
            base.Uninstall();
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var processPaymentResult = new ProcessPaymentResult()
            {
                NewPaymentStatus = PaymentStatus.Pending
            };
            return processPaymentResult;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            try
            {
                var uri = _pagSeguroService.CreatePayment(postProcessPaymentRequest);
                _httpContextAccessor.HttpContext.Response.Redirect(uri.AbsoluteUri);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart) => 0;

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest) => new CapturePaymentResult();

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest) => new RefundPaymentResult();

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest) => new VoidPaymentResult();

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest) => new ProcessPaymentResult();

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest) => new CancelRecurringPaymentResult();

        public bool CanRePostProcessPayment(Order order) => false;

        public Type GetControllerType() => typeof(PaymentPagSeguroController);

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool HidePaymentMethod(IList<Nop.Core.Domain.Orders.ShoppingCartItem> cart) => false;

        public IList<string> ValidatePaymentForm(IFormCollection form) => new List<string>();

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form) => new ProcessPaymentRequest();

        public bool SkipPaymentInfo => false;

        public string PaymentMethodDescription => _pagSeguroSetting.PaymentMethodDescription;

        public override string GetConfigurationPageUrl() => $"{_webHelper.GetStoreLocation()}Admin/PaymentPagSeguro/Configure";

        public string GetPublicViewComponentName() => "PaymentPagSeguro";
    }
}
