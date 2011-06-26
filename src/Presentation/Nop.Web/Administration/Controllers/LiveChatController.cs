using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models.Common;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class LiveChatController : BaseNopController
	{
		#region Fields

        private readonly ILiveChatService _liveChatService;
        private readonly LiveChatSettings _liveChatSettings;
        private readonly ISettingService _settingService;

	    #endregion

		#region Constructors

        public LiveChatController(ILiveChatService liveChatService,
            LiveChatSettings liveChatSettings, ISettingService settingService)
		{
            this._liveChatService = liveChatService;
            this._liveChatSettings = liveChatSettings;
            this._settingService = settingService;
		}

		#endregion 

        #region Methods

        public ActionResult Providers()
        {
            var liveChatProvidersModel = new List<LiveChatProviderModel>();
            var liveChatProviders = _liveChatService.LoadAllLiveChatProviders();
            foreach (var liveChatProvider in liveChatProviders)
            {
                var tmp1 = liveChatProvider.ToModel();
                tmp1.IsActive = liveChatProvider.IsLiveChatProviderActive(_liveChatSettings);
                liveChatProvidersModel.Add(tmp1);
            }
            var gridModel = new GridModel<LiveChatProviderModel>
            {
                Data = liveChatProvidersModel,
                Total = liveChatProvidersModel.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Providers(GridCommand command)
        {
            var liveChatProvidersModel = new List<LiveChatProviderModel>();
            var liveChatProviders = _liveChatService.LoadAllLiveChatProviders();
            foreach (var liveChatProvider in liveChatProviders)
            {
                var tmp1 = liveChatProvider.ToModel();
                tmp1.IsActive = liveChatProvider.IsLiveChatProviderActive(_liveChatSettings);
                liveChatProvidersModel.Add(tmp1);
            }
            liveChatProvidersModel = liveChatProvidersModel.ForCommand(command).ToList();
            var gridModel = new GridModel<LiveChatProviderModel>
            {
                Data = liveChatProvidersModel,
                Total = liveChatProvidersModel.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ProviderUpdate(LiveChatProviderModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Providers");
            }
            
            var lcp = _liveChatService.LoadLiveChatProviderBySystemName(model.SystemName);
            if (lcp.IsLiveChatProviderActive(_liveChatSettings))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    _liveChatSettings.ActiveLiveChatProviderSystemName.Remove(lcp.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_liveChatSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    _liveChatSettings.ActiveLiveChatProviderSystemName.Add(lcp.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_liveChatSettings);
                }
            }

            return Providers(command);
        }

        public ActionResult ConfigureProvider(string systemName)
        {
            var lcs = _liveChatService.LoadLiveChatProviderBySystemName(systemName);
            if (lcs == null) 
                throw new ArgumentException("No live char provider found with the specified system name", "systemName");

            var model = lcs.ToModel();
            string actionName, controllerName;
            RouteValueDictionary routeValues;
            lcs.GetConfigurationRoute(out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;
            return View(model);
        }

        #endregion
    }
}
