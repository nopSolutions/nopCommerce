using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models.Cms;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class WidgetController : BaseNopController
	{
		#region Fields

        private readonly IWidgetService _widgetService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly WidgetSettings _widgetSettings;

	    #endregion

		#region Constructors

        public WidgetController(IWidgetService widgetService,
            IPermissionService permissionService, ISettingService settingService,
            WidgetSettings widgetSettings)
		{
            this._widgetService = widgetService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._widgetSettings = widgetSettings;
		}

		#endregion 
        
        #region Methods
        
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var widgetsModel = new List<WidgetModel>();
            var widgets = _widgetService.LoadAllWidgets();
            foreach (var widget in widgets)
            {
                var tmp1 = widget.ToModel();
                tmp1.IsActive = widget.IsWidgetActive(_widgetSettings);
                widgetsModel.Add(tmp1);
            }
            var gridModel = new GridModel<WidgetModel>
            {
                Data = widgetsModel,
                Total = widgetsModel.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var widgetsModel = new List<WidgetModel>();
            var widgets = _widgetService.LoadAllWidgets();
            foreach (var widget in widgets)
            {
                var tmp1 = widget.ToModel();
                tmp1.IsActive = widget.IsWidgetActive(_widgetSettings);
                widgetsModel.Add(tmp1);
            }
            widgetsModel = widgetsModel.ForCommand(command).ToList();
            var gridModel = new GridModel<WidgetModel>
            {
                Data = widgetsModel,
                Total = widgetsModel.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult WidgetUpdate(WidgetModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var widget = _widgetService.LoadWidgetBySystemName(model.SystemName);
            if (widget.IsWidgetActive(_widgetSettings))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    _widgetSettings.ActiveWidgetSystemNames.Remove(widget.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_widgetSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    _widgetSettings.ActiveWidgetSystemNames.Add(widget.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_widgetSettings);
                }
            }

            return List(command);
        }
        
        public ActionResult ConfigureWidget(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var widget = _widgetService.LoadWidgetBySystemName(systemName);
            if (widget == null)
                //No widget found with the specified id
                return RedirectToAction("List");

            var model = widget.ToModel();
            string actionName, controllerName;
            RouteValueDictionary routeValues;
            widget.GetConfigurationRoute(out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;
            return View(model);
        }
        
        #endregion
    }
}
