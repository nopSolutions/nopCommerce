using System.Collections.Generic;
using System.Linq;
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

        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetModelFactory(IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext)
        {
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _themeContext = themeContext;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
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
            var cacheKey = string.Format(NopModelCacheDefaults.WidgetModelKey,
                _workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, widgetZone, _themeContext.WorkingThemeName);

            var cachedModels = _cacheManager.Get(cacheKey, () =>
                _widgetPluginManager.LoadActivePlugins(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id, widgetZone)
                .Select(widget => new RenderWidgetModel
                {
                    WidgetViewComponentName = widget.GetWidgetViewComponentName(widgetZone),
                    WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone }
                }));

            //"WidgetViewComponentArguments" property of widget models depends on "additionalData".
            //We need to clone the cached model before modifications (the updated one should not be cached)
            var models = cachedModels.Select(renderModel => new RenderWidgetModel
            {
                WidgetViewComponentName = renderModel.WidgetViewComponentName,
                WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone, ["additionalData"] = additionalData }
            }).ToList();

            return models;
        }

        #endregion
    }
}