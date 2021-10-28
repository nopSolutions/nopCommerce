using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Cms;
using Nop.Core.Events;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Cms;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class WidgetController : BaseAdminController
    {
        #region Fields

        protected IEventPublisher EventPublisher { get; }
        protected IPermissionService PermissionService { get; }
        protected ISettingService SettingService { get; }
        protected IWidgetModelFactory WidgetModelFactory { get; }
        protected IWidgetPluginManager WidgetPluginManager { get; }
        protected WidgetSettings WidgetSettings { get; }

        #endregion

        #region Ctor

        public WidgetController(IEventPublisher eventPublisher,
            IPermissionService permissionService,
            ISettingService settingService,
            IWidgetModelFactory widgetModelFactory,
            IWidgetPluginManager widgetPluginManager,
            WidgetSettings widgetSettings)
        {
            EventPublisher = eventPublisher;
            PermissionService = permissionService;
            SettingService = settingService;
            WidgetModelFactory = widgetModelFactory;
            WidgetPluginManager = widgetPluginManager;
            WidgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //prepare model
            var model = await WidgetModelFactory.PrepareWidgetSearchModelAsync(new WidgetSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(WidgetSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await WidgetModelFactory.PrepareWidgetListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> WidgetUpdate(WidgetModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var widget = await WidgetPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
            if (WidgetPluginManager.IsPluginActive(widget, WidgetSettings.ActiveWidgetSystemNames))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    WidgetSettings.ActiveWidgetSystemNames.Remove(widget.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(WidgetSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    WidgetSettings.ActiveWidgetSystemNames.Add(widget.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(WidgetSettings);
                }
            }

            var pluginDescriptor = widget.PluginDescriptor;

            //display order
            pluginDescriptor.DisplayOrder = model.DisplayOrder;

            //update the description file
            pluginDescriptor.Save();

            //raise event
            await EventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

            return new NullJsonResult();
        }

        #endregion
    }
}