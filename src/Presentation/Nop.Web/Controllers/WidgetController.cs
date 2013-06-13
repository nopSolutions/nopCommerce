using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Web.Models.Cms;

namespace Nop.Web.Controllers
{
    public partial class WidgetController : BaseNopController
    {
		#region Fields

        private readonly IWidgetService _widgetService;
        private readonly IStoreContext _storeContext;

        #endregion

		#region Constructors

        public WidgetController(IWidgetService widgetService, 
            IStoreContext storeContext)
        {
            this._widgetService = widgetService;
            this._storeContext = storeContext;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult WidgetsByZone(string widgetZone)
        {
            //model
            var model = new List<RenderWidgetModel>();

            var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone, _storeContext.CurrentStore.Id);
            foreach (var widget in widgets)
            {
                var widgetModel = new RenderWidgetModel();

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                widget.GetDisplayWidgetRoute(widgetZone, out actionName, out controllerName, out routeValues);
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
