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
        private readonly IWidgetService _widgetService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetModelFactory(IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IWidgetService widgetService,
            IWorkContext workContext)
        {
            this._cacheManager = cacheManager;
            this._storeContext = storeContext;
            this._themeContext = themeContext;
            this._widgetService = widgetService;
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

            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                //model
                var model = new List<RenderWidgetModel>();

                var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
                foreach (var widget in widgets)
                {
                    model.Add(new RenderWidgetModel
                    {
                        WidgetViewComponentName = widget.GetWidgetViewComponentName(widgetZone),
                        WidgetViewComponentArguments = new RouteValueDictionary
                            {
                                { "widgetZone", widgetZone }
                            }
                    });
                }
                return model;
            });

            //"WidgetViewComponentArguments" property of widget models depends on "additionalData".
            //We need to clone the cached model before modifications (the updated one should not be cached)
            var clonedModel = cachedModel.Select(renderModel => new RenderWidgetModel
                {
                    WidgetViewComponentName = renderModel.WidgetViewComponentName,
                    WidgetViewComponentArguments = new RouteValueDictionary
                        {
                            { "widgetZone", widgetZone },
                            { "additionalData", additionalData }
                        }
                }
            ).ToList();

            return clonedModel;
        }

        #endregion
    }
}