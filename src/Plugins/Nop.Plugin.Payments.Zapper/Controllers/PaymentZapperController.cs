using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Zapper.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Logging;
using System.Linq;


namespace Nop.Plugin.Payments.Zapper.Controllers
{
    public class PaymentZapperController : BasePaymentController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPermissionService _permissionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly PaymentSettings _paymentSettings;
        private readonly ZapperPaymentSettings _zapperPaymentSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PaymentZapperController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IPermissionService permissionService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ILogger logger,
            IWebHelper webHelper,
            PaymentSettings paymentSettings,
            ZapperPaymentSettings zapperPaymentSettings,
            ShoppingCartSettings shoppingCartSettings)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._permissionService = permissionService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._logger = logger;
            this._webHelper = webHelper;
            this._paymentSettings = paymentSettings;
            this._zapperPaymentSettings = zapperPaymentSettings;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var zapperPaymentSettings = _settingService.LoadSetting<ZapperPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                MerchantId = zapperPaymentSettings.MerchantId,
                SiteId = zapperPaymentSettings.SiteId,
                PosKey = zapperPaymentSettings.PosKey,
                PosToken = zapperPaymentSettings.PosToken                
            };
            if (storeScope > 0)
            {
                model.MerchantId_OverrideForStore = _settingService.SettingExists(zapperPaymentSettings, x => x.MerchantId, storeScope);
                model.SiteId_OverrideForStore = _settingService.SettingExists(zapperPaymentSettings, x => x.SiteId, storeScope);
                model.PosKey_OverrideForStore = _settingService.SettingExists(zapperPaymentSettings, x => x.PosKey, storeScope);
                model.PosToken_OverrideForStore = _settingService.SettingExists(zapperPaymentSettings, x => x.PosToken, storeScope);
            }

            return View("~/Plugins/Payments.Zapper/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var zapperPaymentSettings = _settingService.LoadSetting<ZapperPaymentSettings>(storeScope);

            //save settings
            zapperPaymentSettings.MerchantId = model.MerchantId;
            zapperPaymentSettings.SiteId = model.SiteId;
            zapperPaymentSettings.PosKey = model.PosKey;
            zapperPaymentSettings.PosToken = model.PosToken;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(zapperPaymentSettings, x => x.MerchantId, model.MerchantId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(zapperPaymentSettings, x => x.SiteId, model.SiteId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(zapperPaymentSettings, x => x.PosKey, model.PosKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(zapperPaymentSettings, x => x.PosToken, model.PosToken_OverrideForStore, storeScope, false);


            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }


        public IActionResult CancelOrder()
        {
            var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();
            if (order != null)
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });

            return RedirectToRoute("HomePage");
        }

        #endregion
    }
}
