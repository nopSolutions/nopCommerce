using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Cms;
using Nop.Services.Customers;
using Nop.Web.Framework.Models.Cms;
using Nop.Web.Framework.Themes;

namespace Nop.Web.Framework.Factories
{
    /// <summary>
    /// Represents the widget model factory
    /// </summary>
    public partial class WidgetModelFactory : IWidgetModelFactory
    {
        #region Fields

        protected readonly ICustomerService _customerService;
        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly IStoreContext _storeContext;
        protected readonly IThemeContext _themeContext;
        protected readonly IWidgetPluginManager _widgetPluginManager;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetModelFactory(ICustomerService customerService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext)
        {
            _customerService = customerService;
            _staticCacheManager = staticCacheManager;
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
        /// <param name="useCache">Value indicating whether to get widget models from cache</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the render widget models
        /// </returns>
        public virtual async Task<List<RenderWidgetModel>> PrepareRenderWidgetModelAsync(string widgetZone, object additionalData = null, bool useCache = true)
        {
            var theme = await _themeContext.GetWorkingThemeNameAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
            var store = await _storeContext.GetCurrentStoreAsync();

            var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(WidgetModelDefaults.WidgetModelKey,
                customerRoleIds, store, widgetZone, theme);

            if (!useCache)
            {
                return (await _widgetPluginManager.LoadActivePluginsAsync(customer, store.Id, widgetZone))
                .Select(widget => new RenderWidgetModel
                {
                    WidgetViewComponent = widget.GetWidgetViewComponent(widgetZone),
                    WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone, ["additionalData"] = additionalData }
                }).ToList();
            }

            var widgetModels = await _staticCacheManager.GetAsync(cacheKey, async () =>
                (await _widgetPluginManager.LoadActivePluginsAsync(customer, store.Id, widgetZone))
                .Select(widget => new RenderWidgetModel
                {
                    WidgetViewComponent = widget.GetWidgetViewComponent(widgetZone),
                    WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone }
                }));

            //"WidgetViewComponentArguments" property of widget models depends on "additionalData".
            //We need to clone the cached model before modifications (the updated one should not be cached)
            var models = widgetModels.Select(renderModel => new RenderWidgetModel
            {
                WidgetViewComponent = renderModel.WidgetViewComponent,
                WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone, ["additionalData"] = additionalData }
            }).ToList();

            return models;
        }

        #endregion
    }
}