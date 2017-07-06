using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;

namespace Nop.Web.Components
{
    public class WidgetViewComponent : ViewComponent
    {
        private readonly IWidgetModelFactory _widgetModelFactory;

        public WidgetViewComponent(IWidgetModelFactory widgetModelFactory)
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