using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PaymentController : BaseAdminController
    {
        #region Fields

        private readonly IPaymentService _paymentService;
        private readonly PaymentSettings _paymentSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly ICountryService _countryService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PaymentController(IPaymentService paymentService,
            PaymentSettings paymentSettings,
            ISettingService settingService,
            IPermissionService permissionService,
            ICountryService countryService,
            IPluginFinder pluginFinder,
            IWebHelper webHelper,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            this._paymentService = paymentService;
            this._paymentSettings = paymentSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._countryService = countryService;
            this._pluginFinder = pluginFinder;
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual IActionResult Methods()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult Methods(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedKendoGridJson();

            var paymentMethodsModel = new List<PaymentMethodModel>();
            var paymentMethods = _paymentService.LoadAllPaymentMethods();
            foreach (var paymentMethod in paymentMethods)
            {
                var tmp1 = paymentMethod.ToModel();
                tmp1.IsActive = paymentMethod.IsPaymentMethodActive(_paymentSettings);
                tmp1.LogoUrl = paymentMethod.PluginDescriptor.GetLogoUrl(_webHelper);
                tmp1.ConfigurationUrl = paymentMethod.GetConfigurationPageUrl();
                tmp1.RecurringPaymentType = paymentMethod.RecurringPaymentType.GetLocalizedEnum(_localizationService, _workContext);
                paymentMethodsModel.Add(tmp1);
            }
            paymentMethodsModel = paymentMethodsModel.ToList();
            var gridModel = new DataSourceResult
            {
                Data = paymentMethodsModel,
                Total = paymentMethodsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult MethodUpdate(PaymentMethodModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var pm = _paymentService.LoadPaymentMethodBySystemName(model.SystemName);
            if (pm.IsPaymentMethodActive(_paymentSettings))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    _paymentSettings.ActivePaymentMethodSystemNames.Remove(pm.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_paymentSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    _paymentSettings.ActivePaymentMethodSystemNames.Add(pm.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_paymentSettings);
                }
            }
            var pluginDescriptor = pm.PluginDescriptor;
            pluginDescriptor.FriendlyName = model.FriendlyName;
            pluginDescriptor.DisplayOrder = model.DisplayOrder;

            //update the description file
            PluginManager.SavePluginDescriptor(pluginDescriptor);

            //reset plugin cache
            _pluginFinder.ReloadPlugins();

            return new NullJsonResult();
        }

        public virtual IActionResult MethodRestrictions()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var model = new PaymentMethodRestrictionModel();

            var paymentMethods = _paymentService.LoadAllPaymentMethods();
            var countries = _countryService.GetAllCountries(showHidden: true);
            foreach (var pm in paymentMethods)
            {
                model.AvailablePaymentMethods.Add(pm.ToModel());
            }
            foreach (var c in countries)
            {
                model.AvailableCountries.Add(c.ToModel());
            }
            foreach (var pm in paymentMethods)
            {
                var restictedCountries = _paymentService.GetRestictedCountryIds(pm);
                foreach (var c in countries)
                {
                    var resticted = restictedCountries.Contains(c.Id);
                    if (!model.Resticted.ContainsKey(pm.PluginDescriptor.SystemName))
                        model.Resticted[pm.PluginDescriptor.SystemName] = new Dictionary<int, bool>();
                    model.Resticted[pm.PluginDescriptor.SystemName][c.Id] = resticted;
                }
            }

            return View(model);
        }

        [HttpPost, ActionName("MethodRestrictions")]
        public virtual IActionResult MethodRestrictionsSave(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var paymentMethods = _paymentService.LoadAllPaymentMethods();
            var countries = _countryService.GetAllCountries(showHidden: true);

            foreach (var pm in paymentMethods)
            {
                var formKey = "restrict_" + pm.PluginDescriptor.SystemName;
                var countryIdsToRestrict = (!StringValues.IsNullOrEmpty(form[formKey])
                        ? form[formKey].ToString().Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList()
                        : new List<string>())
                    .Select(x => Convert.ToInt32(x)).ToList();

                var newCountryIds = new List<int>();
                foreach (var c in countries)
                {
                    if (countryIdsToRestrict.Contains(c.Id))
                    {
                        newCountryIds.Add(c.Id);
                    }
                }
                _paymentService.SaveRestictedCountryIds(pm, newCountryIds);
            }

            SuccessNotification(
                _localizationService.GetResource("Admin.Configuration.Payment.MethodRestrictions.Updated"));
            return RedirectToAction("MethodRestrictions");
        }

        #endregion
    }
}