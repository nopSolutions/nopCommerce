using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models.Plugins;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.PromotionFeed;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class PluginController : BaseNopController
	{
		#region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;

	    #endregion

		#region Constructors

        public PluginController(IPluginFinder pluginFinder,
            ILocalizationService localizationService, IWebHelper webHelper,
            IPermissionService permissionService)
		{
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
		}

		#endregion 

        #region Utilities
        [NonAction]
        private PluginModel PreparePluginModel(PluginDescriptor pluginDescriptor)
        {
            var pluginModel = pluginDescriptor.ToModel();
            //set configuration URL
            //plugins do not provide a genral URL for configuration
            //because some of them have some custom URLs for configuration
            //for example, discount requirement plugins require additional parameters and attached to a certain discount
            var pluginInstance = pluginDescriptor.Instance();
            if (pluginInstance is IPaymentMethod)
            {
                //payment plugin
                pluginModel.ConfigurationUrl = Url.Action("ConfigureMethod", "Payment", new {systemName = pluginDescriptor.SystemName}, "http");
            }
            else if (pluginInstance is IShippingRateComputationMethod)
            {
                //shipping rate computation method
                pluginModel.ConfigurationUrl = Url.Action("ConfigureProvider", "Shipping", new { systemName = pluginDescriptor.SystemName }, "http");
            }
            else if (pluginInstance is ITaxProvider)
            {
                //tax provider
                pluginModel.ConfigurationUrl = Url.Action("ConfigureProvider", "Tax", new { systemName = pluginDescriptor.SystemName }, "http");
            }
            else if (pluginInstance is IExternalAuthenticationMethod)
            {
                //external auth method
                pluginModel.ConfigurationUrl = Url.Action("ConfigureMethod", "ExternalAuthentication", new { systemName = pluginDescriptor.SystemName }, "http");
            }
            else if (pluginInstance is ISmsProvider)
            {
                //SMS provider
                pluginModel.ConfigurationUrl = Url.Action("ConfigureProvider", "Sms", new { systemName = pluginDescriptor.SystemName }, "http");
            }
            else if (pluginInstance is IPromotionFeed)
            {
                //promotion feed
                pluginModel.ConfigurationUrl = Url.Action("ConfigureMethod", "PromotionFeed", new { systemName = pluginDescriptor.SystemName }, "http");
            }
            else if (pluginInstance is IMiscPlugin)
            {
                //Misc plugins
                pluginModel.ConfigurationUrl = Url.Action("ConfigureMiscPlugin", "Plugin", new { systemName = pluginDescriptor.SystemName }, "http");
            }
            return pluginModel;
        }
        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //TODO allow store owner to edit display order of plugins
            var pluginDescriptors = _pluginFinder.GetPluginDescriptors(false);
            var model = new GridModel<PluginModel>
            {
                Data = pluginDescriptors.Select(x => PreparePluginModel(x))
                .OrderBy(x => x.Group)
                .ThenBy(x => x.DisplayOrder).ToList(),
                Total = pluginDescriptors.Count()
            };
            return View(model);
        }

        public ActionResult Install(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptors(false)
                    .Where(x => x.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //install plugin
                pluginDescriptor.Instance().Install();
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.Installed"));

                //restart application
                _webHelper.RestartAppDomain("~/Admin/Plugin/List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
             
            return RedirectToAction("List");
        }

        public ActionResult Uninstall(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptors(false)
                    .Where(x => x.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is installed
                if (!pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //uninstall plugin
                pluginDescriptor.Instance().Uninstall();
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.Uninstalled"));

                //restart application
                _webHelper.RestartAppDomain("~/Admin/Plugin/List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }

        public ActionResult ReloadList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //restart application
            _webHelper.RestartAppDomain("~/Admin/Plugin/List");
            return RedirectToAction("List");
        }
        
        public ActionResult ConfigureMiscPlugin(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();


            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IMiscPlugin>(systemName);
            if (descriptor == null || !descriptor.Installed)
                return Redirect("List");

            var plugin  = descriptor.Instance<IMiscPlugin>();

            string actionName, controllerName;
            RouteValueDictionary routeValues;
            plugin.GetConfigurationRoute(out actionName, out controllerName, out routeValues);
            var model = new MiscPluginModel();
            model.FriendlyName = descriptor.FriendlyName;
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;
            return View(model);
        }
        #endregion
    }
}
