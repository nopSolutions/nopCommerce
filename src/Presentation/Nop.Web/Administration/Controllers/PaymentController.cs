using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models.Payments;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Nop.Services.Security;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class PaymentController : BaseNopController
	{
		#region Fields

        private readonly IPaymentService _paymentService;
        private readonly PaymentSettings _paymentSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

		#endregion

		#region Constructors

        public PaymentController(IPaymentService paymentService, PaymentSettings paymentSettings,
            ISettingService settingService, IPermissionService permissionService)
		{
            this._paymentService = paymentService;
            this._paymentSettings = paymentSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
		}

		#endregion 

        #region Methods

        public ActionResult Methods()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var paymentMethodsModel = new List<PaymentMethodModel>();
            var paymentMethods = _paymentService.LoadAllPaymentMethods();
            foreach (var paymentMethod in paymentMethods)
            {
                var tmp1 = paymentMethod.ToModel();
                tmp1.IsActive = paymentMethod.IsPaymentMethodActive(_paymentSettings);
                paymentMethodsModel.Add(tmp1);
            }
            var gridModel = new GridModel<PaymentMethodModel>
            {
                Data = paymentMethodsModel,
                Total = paymentMethodsModel.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Methods(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var paymentMethodsModel = new List<PaymentMethodModel>();
            var paymentMethods = _paymentService.LoadAllPaymentMethods();
            foreach (var paymentMethod in paymentMethods)
            {
                var tmp1 = paymentMethod.ToModel();
                tmp1.IsActive = paymentMethod.IsPaymentMethodActive(_paymentSettings);
                paymentMethodsModel.Add(tmp1);
            }
            paymentMethodsModel = paymentMethodsModel.ForCommand(command).ToList();
            var gridModel = new GridModel<PaymentMethodModel>
            {
                Data = paymentMethodsModel,
                Total = paymentMethodsModel.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult MethodUpdate(PaymentMethodModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Methods");
            }
            
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
            
            return Methods(command);
        }

        public ActionResult ConfigureMethod(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var pm = _paymentService.LoadPaymentMethodBySystemName(systemName);
            if (pm == null) throw new ArgumentException("No payment method found with the specified system name", "systemName");

            var model = pm.ToModel();
            string actionName, controllerName;
            RouteValueDictionary routeValues;
            pm.GetConfigurationRoute(out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;
            return View(model);
        }

        #endregion
    }
}
