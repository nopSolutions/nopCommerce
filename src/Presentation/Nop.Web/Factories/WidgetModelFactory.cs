using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Cms;
using Nop.Web.Framework.Themes;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Cms;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the widget model factory
    /// </summary>
    public partial class WidgetModelFactory : IWidgetModelFactory
    {
		#region Fields

        private readonly IWidgetService _widgetService;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetModelFactory(IWidgetService widgetService, 
            IStoreContext storeContext,
            IThemeContext themeContext,
            IStaticCacheManager cacheManager,
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
        /// Get the render widget models
        /// </summary>
        /// <param name="widgetZone">Name of widget zone</param>
        /// <param name="additionalData">Additional data object</param>
        /// <returns>List of the render widget models</returns>
        public virtual List<RenderWidgetModel> PrepareRenderWidgetModel(string widgetZone, object additionalData = null)
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.WIDGET_MODEL_KEY,
                _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, widgetZone, _themeContext.WorkingThemeName);

            //add widget zone to view component arguments
            additionalData = new RouteValueDictionary()
            {
                { "widgetZone", widgetZone },
                { "additionalData", additionalData }
            };

            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                //model
                var model = new List<RenderWidgetModel>();

                var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
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
                return model;
            });

            //"WidgetViewComponentArguments" property of widget models depends on "additionalData".
            //We need to clone the cached model before modifications (the updated one should not be cached)
            var clonedModel = new List<RenderWidgetModel>();

            foreach (var widgetModel in cachedModel)
            {
                var clonedWidgetModel = new RenderWidgetModel
                {
                    WidgetViewComponentName = widgetModel.WidgetViewComponentName
                };

                if (widgetModel.WidgetViewComponentArguments != null)
                    clonedWidgetModel.WidgetViewComponentArguments = new RouteValueDictionary(widgetModel.WidgetViewComponentArguments);

                if (additionalData != null)
                {
                    if (clonedWidgetModel.WidgetViewComponentArguments == null)
                        clonedWidgetModel.WidgetViewComponentArguments = new RouteValueDictionary();

                    clonedWidgetModel.WidgetViewComponentArguments = additionalData;
                }

                clonedModel.Add(clonedWidgetModel);
            }

            return clonedModel;
        }

        #endregion
    }
}