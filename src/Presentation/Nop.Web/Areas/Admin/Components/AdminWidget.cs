using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Cms;
using Nop.Web.Areas.Admin.Models.Cms;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components
{
    public class AdminWidgetViewComponent : NopViewComponent
    {
        private readonly IWidgetService _widgetService;

        public AdminWidgetViewComponent(IWidgetService widgetService)
        {
            this._widgetService = widgetService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData = null)
        {
            var model = new List<RenderWidgetModel>();

            //add widget zone to view component arguments
            additionalData = new RouteValueDictionary()
            {
                { "widgetZone", widgetZone },
                { "additionalData", additionalData }
            };

            var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone);
            foreach (var widget in widgets)
            {
                widget.GetPublicViewComponent(widgetZone, out string viewComponentName);

                var widgetModel = new RenderWidgetModel
                {
                    WidgetViewComponentName = viewComponentName,
                    WidgetViewComponentArguments = additionalData
                };

                model.Add(widgetModel);
            }

            //no data?
            if (!model.Any())
                return Content("");

            return View(model);
        }
    }
}