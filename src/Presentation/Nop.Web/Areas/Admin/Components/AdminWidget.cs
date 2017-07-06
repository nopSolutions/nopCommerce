using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Models.Cms;
using Nop.Services.Cms;

namespace Nop.Web.Areas.Admin.Components
{
    public class AdminWidgetViewComponent : ViewComponent
    {
        private readonly IWidgetService _widgetService;

        public AdminWidgetViewComponent(IWidgetService widgetService)
        {
            this._widgetService = widgetService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            var model = new List<RenderWidgetModel>();

            var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone);
            foreach (var widget in widgets)
            {
                widget.GetPublicViewComponent(out string viewComponentName);

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