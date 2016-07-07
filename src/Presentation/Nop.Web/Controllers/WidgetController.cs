using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Cms;
using Nop.Web.Framework.Themes;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Cms;

namespace Nop.Web.Controllers
{
    public partial class WidgetController : BasePublicController
    {
		#region Fields

        private readonly IWidgetService _widgetService;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly ICacheManager _cacheManager;

        #endregion

		#region Constructors

        public WidgetController(IWidgetService widgetService, 
            IStoreContext storeContext,
            IThemeContext themeContext,
            ICacheManager cacheManager)
        {
            this._widgetService = widgetService;
            this._storeContext = storeContext;
            this._themeContext = themeContext;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult WidgetsByZone(string widgetZone, object additionalData = null)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.WIDGET_MODEL_KEY, _storeContext.CurrentStore.Id, widgetZone, _themeContext.WorkingThemeName);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
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
                return model;
            });

            //no data?
            if (!cacheModel.Any())
                return Content("");

            //"RouteValues" property of widget models depends on "additionalData".
            //We need to clone the cached model before modifications (the updated one should not be cached)
            var clonedModel = new List<RenderWidgetModel>();
            foreach (var widgetModel in cacheModel)
            {
                var clonedWidgetModel = new RenderWidgetModel();
                clonedWidgetModel.ActionName = widgetModel.ActionName;
                clonedWidgetModel.ControllerName = widgetModel.ControllerName;
                if (widgetModel.RouteValues != null)
                    clonedWidgetModel.RouteValues = new RouteValueDictionary(widgetModel.RouteValues);

                if (additionalData != null)
                {
                    if (clonedWidgetModel.RouteValues == null)
                        clonedWidgetModel.RouteValues = new RouteValueDictionary();
                    clonedWidgetModel.RouteValues.Add("additionalData", additionalData);
                }

                clonedModel.Add(clonedWidgetModel);
            }

            return PartialView(clonedModel);
        }

        #endregion
    }
}
