using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core.Domain.Cms;
using Nop.Services.Cms;
using Nop.Web.Models.Cms;

namespace Nop.Web.Controllers
{
    public class WidgetController : BaseNopController
    {
		#region Fields

        private readonly IWidgetService _widgetService;

        #endregion

		#region Constructors

        public WidgetController(IWidgetService widgetService)
        {
            this._widgetService = widgetService;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult WidgetsByZone(string widgetZone)
        {
            //model
            var model = new List<WidgetModel>();

            var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone);
            foreach (var widget in widgets)
            {
                var widgetModel = new WidgetModel();

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                widget.GetDisplayWidgetRoute(out actionName, out controllerName, out routeValues);
                widgetModel.ActionName = actionName;
                widgetModel.ControllerName = controllerName;
                widgetModel.RouteValues = routeValues;

                model.Add(widgetModel);
            }

            return PartialView(model);
        }

        #endregion
    }
}
