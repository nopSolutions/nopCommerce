using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Cms;
using Nop.Web.Framework.Themes;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Cms;

namespace Nop.Web.Factories
{
    /// <summary>
    /// 部件模型工厂
    /// Represents the widget model factory
    /// </summary>
    public partial class WidgetModelFactory : IWidgetModelFactory
    {
		#region Fields

        private readonly IWidgetService _widgetService;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetModelFactory(IWidgetService widgetService, 
            IStoreContext storeContext,
            IThemeContext themeContext,
            ICacheManager cacheManager,
            IWorkContext workContext)
        {
            this._widgetService = widgetService;
            this._storeContext = storeContext;
            this._themeContext = themeContext;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获得部件模型
        /// Get the render widget models
        /// </summary>
        /// <param name="widgetZone">部件区域 Name of widget zone</param>
        /// <param name="additionalData">附加数据 Additional data object</param>
        /// <returns>List of the render widget models</returns>
        public virtual List<RenderWidgetModel> GetRenderWidgetModels(string widgetZone, object additionalData = null)
        {
            //缓存key
            var cacheKey = string.Format(ModelCacheEventConsumer.WIDGET_MODEL_KEY,
                _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, widgetZone, _themeContext.WorkingThemeName);

            //获得部件模型（支持缓存）
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                //model
                var model = new List<RenderWidgetModel>();

                var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
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

            //"RouteValues" property of widget models depends on "additionalData".
            //路由信息，依赖于附加数据。
            //We need to clone the cached model before modifications (the updated one should not be cached)
            //原则：修改后的数据不应该被缓存。
            var clonedModel = new List<RenderWidgetModel>();
            foreach (var widgetModel in cachedModel)
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

            return clonedModel;
        }

        #endregion
    }
}
