using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using NopBrasil.Plugin.Payments.PagSeguro.Models;
using Nop.Web.Framework;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Security;
using System;
using Nop.Core;
using Nop.Services.Messages;
using Nop.Services.Localization;

namespace NopBrasil.Plugin.Payments.PagSeguro.Controllers
{
    [Area(AreaNames.Admin)]
    public class PaymentPagSeguroController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public PaymentPagSeguroController(ISettingService settingService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _storeContext = storeContext ?? throw new ArgumentNullException(nameof(storeContext));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var pagSeguroPaymentSetting = _settingService.LoadSetting<PagSeguroPaymentSetting>(storeScope);

            var model = new ConfigurationModel()
            {
                PagSeguroToken = pagSeguroPaymentSetting.PagSeguroToken,
                PagSeguroEmail = pagSeguroPaymentSetting.PagSeguroEmail,
                PaymentMethodDescription = pagSeguroPaymentSetting.PaymentMethodDescription,
                IsSandbox = pagSeguroPaymentSetting.IsSandbox,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.PagSeguroToken_OverrideForStore = _settingService.SettingExists(pagSeguroPaymentSetting, x => x.PagSeguroToken, storeScope);
                model.PagSeguroEmail_OverrideForStore = _settingService.SettingExists(pagSeguroPaymentSetting, x => x.PagSeguroEmail, storeScope);
                model.PaymentMethodDescription_OverrideForStore = _settingService.SettingExists(pagSeguroPaymentSetting, x => x.PaymentMethodDescription, storeScope);
                model.IsSandbox_OverrideForStore = _settingService.SettingExists(pagSeguroPaymentSetting, x => x.IsSandbox, storeScope);
            }
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

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var pagSeguroPaymentSetting = _settingService.LoadSetting<PagSeguroPaymentSetting>(storeScope);

            pagSeguroPaymentSetting.PagSeguroToken = model.PagSeguroToken;
            pagSeguroPaymentSetting.PagSeguroEmail = model.PagSeguroEmail;
            pagSeguroPaymentSetting.PaymentMethodDescription = model.PaymentMethodDescription;
            pagSeguroPaymentSetting.IsSandbox = model.IsSandbox;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSetting(pagSeguroPaymentSetting, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}
