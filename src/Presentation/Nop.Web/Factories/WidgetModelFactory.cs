using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Cms;
using Nop.Services.Customers;
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

        protected ICustomerService CustomerService { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IStoreContext StoreContext { get; }
        protected IThemeContext ThemeContext { get; }
        protected IWidgetPluginManager WidgetPluginManager { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public WidgetModelFactory(ICustomerService customerService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext)
        {
            CustomerService = customerService;
            StaticCacheManager = staticCacheManager;
            StoreContext = storeContext;
            ThemeContext = themeContext;
            WidgetPluginManager = widgetPluginManager;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the render widget models
        /// </summary>
        /// <param name="widgetZone">Name of widget zone</param>
        /// <param name="additionalData">Additional data object</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the render widget models
        /// </returns>
        public virtual async Task<List<RenderWidgetModel>> PrepareRenderWidgetModelAsync(string widgetZone, object additionalData = null)
        {
            var theme = await ThemeContext.GetWorkingThemeNameAsync();
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var customerRoleIds = await CustomerService.GetCustomerRoleIdsAsync(customer);
            var store = await StoreContext.GetCurrentStoreAsync();

            var cacheKey = StaticCacheManager.PrepareKeyForShortTermCache(NopModelCacheDefaults.WidgetModelKey,
                customerRoleIds, store, widgetZone, theme);

            var cachedModels = await StaticCacheManager.GetAsync(cacheKey, async () =>
                (await WidgetPluginManager.LoadActivePluginsAsync(customer, store.Id, widgetZone))
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