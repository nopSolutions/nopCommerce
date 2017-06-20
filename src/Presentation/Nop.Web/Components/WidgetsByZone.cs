using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class WidgetsByZoneViewComponent : ViewComponent
    {
        private readonly IWidgetModelFactory _widgetModelFactory;

        public WidgetsByZoneViewComponent(IWidgetModelFactory widgetModelFactory)
        {
            this._widgetModelFactory = widgetModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            var model = _widgetModelFactory.PrepareRenderWidgetModel(widgetZone, additionalData);

            //no data?
            if (!model.Any())
                return Content("");

            return View(model);
        }
    }
}
