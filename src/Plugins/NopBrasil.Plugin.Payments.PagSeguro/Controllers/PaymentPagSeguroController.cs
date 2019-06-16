using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using NopBrasil.Plugin.Payments.PagSeguro.Models;
using Nop.Web.Framework;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Security;
using System;

namespace NopBrasil.Plugin.Payments.PagSeguro.Controllers
{
    [Area(AreaNames.Admin)]
    public class PaymentPagSeguroController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly PagSeguroPaymentSetting _pagSeguroPaymentSetting;
        private readonly IPermissionService _permissionService;

        public PaymentPagSeguroController(ISettingService settingService,
            PagSeguroPaymentSetting pagSeguroPaymentSetting,
            IPermissionService permissionService)
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _pagSeguroPaymentSetting = pagSeguroPaymentSetting ?? throw new ArgumentNullException(nameof(pagSeguroPaymentSetting));
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        }

        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            var model = new ConfigurationModel()
            {
                PagSeguroToken = _pagSeguroPaymentSetting.PagSeguroToken,
                PagSeguroEmail = _pagSeguroPaymentSetting.PagSeguroEmail,
                PaymentMethodDescription = _pagSeguroPaymentSetting.PaymentMethodDescription,
                IsSandbox = _pagSeguroPaymentSetting.IsSandbox
            };
            return View(@"~/Plugins/Payments.PagSeguro/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            _pagSeguroPaymentSetting.PagSeguroToken = model.PagSeguroToken;
            _pagSeguroPaymentSetting.PagSeguroEmail = model.PagSeguroEmail;
            _pagSeguroPaymentSetting.PaymentMethodDescription = model.PaymentMethodDescription;
            _pagSeguroPaymentSetting.IsSandbox = model.IsSandbox;

            _settingService.SaveSetting(_pagSeguroPaymentSetting);

            return View(@"~/Plugins/Payments.PagSeguro/Views/Configure.cshtml", model);
        }
    }
}
