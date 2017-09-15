using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.ExternalAuthentication;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ExternalAuthenticationController : BaseAdminController
	{
		#region Fields

        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IPluginFinder _pluginFinder;

        #endregion

        #region Ctor

        public ExternalAuthenticationController(IExternalAuthenticationService externalAuthenticationService, 
            ExternalAuthenticationSettings externalAuthenticationSettings,
            ISettingService settingService,
            IPermissionService permissionService,
            IPluginFinder pluginFinder)
		{
            this._externalAuthenticationService = externalAuthenticationService;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._pluginFinder = pluginFinder;
		}

		#endregion 

        #region Methods

        public virtual IActionResult Methods()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult Methods(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedKendoGridJson();

            var methodsModel = new List<AuthenticationMethodModel>();
            var methods = _externalAuthenticationService.LoadAllExternalAuthenticationMethods();
            foreach (var method in methods)
            {
                var tmp1 = method.ToModel();
                tmp1.IsActive = method.IsMethodActive(_externalAuthenticationSettings);
                tmp1.ConfigurationUrl = method.GetConfigurationPageUrl();
                methodsModel.Add(tmp1);
            }
            methodsModel = methodsModel.ToList();
            var gridModel = new DataSourceResult
            {
                Data = methodsModel,
                Total = methodsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult MethodUpdate(AuthenticationMethodModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var eam = _externalAuthenticationService.LoadExternalAuthenticationMethodBySystemName(model.SystemName);
            if (eam.IsMethodActive(_externalAuthenticationSettings))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(eam.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_externalAuthenticationSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(eam.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_externalAuthenticationSettings);
                }
            }
            var pluginDescriptor = eam.PluginDescriptor;
            pluginDescriptor.DisplayOrder = model.DisplayOrder;

            //update the description file
            PluginManager.SavePluginDescriptor(pluginDescriptor);

            //reset plugin cache
            _pluginFinder.ReloadPlugins();

            return new NullJsonResult();
        }
        
        #endregion
    }
}