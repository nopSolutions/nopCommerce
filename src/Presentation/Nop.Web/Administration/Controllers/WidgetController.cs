using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models.Cms;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class WidgetController : BaseNopController
	{
		#region Fields

        private readonly IWidgetService _widgetService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;

	    #endregion

		#region Constructors

        public WidgetController(IWidgetService widgetService,
            ILocalizationService localizationService, IWorkContext workContext,
            IPermissionService permissionService, ICustomerActivityService customerActivityService)
		{
            this._widgetService = widgetService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
		}

		#endregion 
        
        #region Utilities

        protected WidgetModel PrepareWidgetModel(Widget widget)
        {
            var widgetPlugin = _widgetService.LoadWidgetPluginBySystemName(widget.PluginSystemName);
            if (widgetPlugin == null || !widgetPlugin.PluginDescriptor.Installed)
                return null;    //don't throw an exception. Maybe, plugin widget was deleted.
            var model = new WidgetModel()
            {
                Id = widget.Id,
                WidgetZoneId = widget.WidgetZoneId,
                WidgetZoneName = widget.WidgetZone.GetLocalizedEnum(_localizationService, _workContext),
                PluginFriendlyName = widgetPlugin.PluginDescriptor.FriendlyName,
                DisplayOrder = widget.DisplayOrder,
            };

            return model;
        }

        #endregion

        #region Methods
        
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var model = new WidgetListModel();
            //widget plugins
            model.AvailableWidgetPlugins = _widgetService.LoadAllWidgetPlugins()
                .Select(x => new WidgetPluginModel()
                {
                    SystemName = x.PluginDescriptor.SystemName,
                    FriendlyName = x.PluginDescriptor.FriendlyName
                })
                .ToList();
            //widgets
            foreach (var widget in _widgetService.GetAllWidgets().OrderBy(w => w.WidgetZoneId).ThenBy(w => w.DisplayOrder))
            {
                var widgetModel = PrepareWidgetModel(widget);
                if (widgetModel != null)
                    model.AvailableWidgets.Add(widgetModel);
            }

            return View(model);
        }

        //create
        public ActionResult Create(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var widgetPlugin = _widgetService.LoadWidgetPluginBySystemName(systemName);
            if (widgetPlugin == null)
                throw new ArgumentException("No widget plugin found with the specified system name");
            
            var model = new EditWidgetModel();
            model.DisplayOrder = 1; //default value
            model.PluginFriendlyName = widgetPlugin.PluginDescriptor.FriendlyName;
            var supportedWidgetZones = widgetPlugin.SupportedWidgetZones();
            if (supportedWidgetZones == null || supportedWidgetZones.Count == 0)
            {
                model.SupportedWidgetZones = WidgetZone.HeadHtmlTag.ToSelectList();
            }
            else
            {
                var zoneValues = from supportedWidgetZone in supportedWidgetZones
                             select new { ID = Convert.ToInt32(supportedWidgetZone), Name = supportedWidgetZone.GetLocalizedEnum(_localizationService, _workContext) };
                model.SupportedWidgetZones = new SelectList(zoneValues, "ID", "Name");
            }

            string actionName, controllerName;
            RouteValueDictionary routeValues;
            widgetPlugin.GetConfigurationRoute(0, out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(string systemName, EditWidgetModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var widgetPlugin = _widgetService.LoadWidgetPluginBySystemName(systemName);
            if (widgetPlugin == null)
                throw new ArgumentException("No widget plugin found with the specified system name");


            var widget = new Widget();
            widget.PluginSystemName = systemName;
            widget.DisplayOrder = model.DisplayOrder;
            widget.WidgetZoneId = model.WidgetZoneId;
            _widgetService.InsertWidget(widget);
            
            model.Id = widget.Id;

            var supportedWidgetZones = widgetPlugin.SupportedWidgetZones();
            if (supportedWidgetZones == null || supportedWidgetZones.Count == 0)
            {
                model.SupportedWidgetZones = WidgetZone.HeadHtmlTag.ToSelectList();
            }
            else
            {
                var zoneValues = from supportedWidgetZone in supportedWidgetZones
                                 select new { ID = Convert.ToInt32(supportedWidgetZone), Name = supportedWidgetZone.GetLocalizedEnum(_localizationService, _workContext) };
                model.SupportedWidgetZones = new SelectList(zoneValues, "ID", "Name");
            }

            string actionName, controllerName;
            RouteValueDictionary routeValues;
            widgetPlugin.GetConfigurationRoute(widget.Id, out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;

            model.PluginFriendlyName = widgetPlugin.PluginDescriptor.FriendlyName;


            ViewBag.RedirectedToList = true;

            //activity log
            _customerActivityService.InsertActivity("AddNewWidget", _localizationService.GetResource("ActivityLog.AddNewWidget"), widget.Id);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Widgets.Added"));
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var widget = _widgetService.GetWidgetById(id);
            if (widget == null)
                throw new ArgumentException("No widget found with the specified id", "id");

            var widgetPlugin = _widgetService.LoadWidgetPluginBySystemName(widget.PluginSystemName);
            if (widgetPlugin == null)
                throw new ArgumentException("No widget plugin found with the specified system name");


            var model = new EditWidgetModel();
            model.Id = widget.Id;
            model.DisplayOrder = widget.DisplayOrder;
            model.PluginFriendlyName = widgetPlugin.PluginDescriptor.FriendlyName;
            model.WidgetZoneId = widget.WidgetZoneId;
            var supportedWidgetZones = widgetPlugin.SupportedWidgetZones();
            if (supportedWidgetZones == null || supportedWidgetZones.Count == 0)
            {
                model.SupportedWidgetZones = WidgetZone.HeadHtmlTag.ToSelectList();
            }
            else
            {
                var zoneValues = from supportedWidgetZone in supportedWidgetZones
                                 select new { ID = Convert.ToInt32(supportedWidgetZone), Name = supportedWidgetZone.GetLocalizedEnum(_localizationService, _workContext) };
                model.SupportedWidgetZones = new SelectList(zoneValues, "ID", "Name");
            }

            string actionName, controllerName;
            RouteValueDictionary routeValues;
            widgetPlugin.GetConfigurationRoute(widget.Id, out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EditWidgetModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var widget = _widgetService.GetWidgetById(model.Id);
            if (widget == null)
                throw new ArgumentException("No widget found with the specified id");

            var widgetPlugin = _widgetService.LoadWidgetPluginBySystemName(widget.PluginSystemName);
            if (widgetPlugin == null)
                throw new ArgumentException("No widget plugin found with the specified system name");

            string actionName, controllerName;
            RouteValueDictionary routeValues;
            widgetPlugin.GetConfigurationRoute(widget.Id, out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;

            var supportedWidgetZones = widgetPlugin.SupportedWidgetZones();
            if (supportedWidgetZones == null || supportedWidgetZones.Count == 0)
            {
                model.SupportedWidgetZones = WidgetZone.HeadHtmlTag.ToSelectList();
            }
            else
            {
                var zoneValues = from supportedWidgetZone in supportedWidgetZones
                                 select new { ID = Convert.ToInt32(supportedWidgetZone), Name = supportedWidgetZone.GetLocalizedEnum(_localizationService, _workContext) };
                model.SupportedWidgetZones = new SelectList(zoneValues, "ID", "Name");
            }

            model.PluginFriendlyName = widgetPlugin.PluginDescriptor.FriendlyName;

            widget.DisplayOrder = model.DisplayOrder;
            widget.WidgetZoneId = model.WidgetZoneId;

            _widgetService.UpdateWidget(widget);
            
            //activity log
            _customerActivityService.InsertActivity("EditWidget", _localizationService.GetResource("ActivityLog.EditWidget"), widget.Id);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Widgets.Updated"));
            return View(model);
        }
        
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var widget = _widgetService.GetWidgetById(id);
            if (widget == null)
                throw new ArgumentException("No widget found with the specified id");

            _widgetService.DeleteWidget(widget);

            //activity log
            _customerActivityService.InsertActivity("DeleteWidget", _localizationService.GetResource("ActivityLog.DeleteWidget"), widget.Id);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Widgets.Deleted"));
            return RedirectToAction("List");
        }
        #endregion
    }
}
