using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Services.Caching;
using Nop.Services.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Cms
{
    /// <summary>
    /// Represents a widget plugin manager implementation
    /// </summary>
    public partial class WidgetPluginManager : PluginManager<IWidgetPlugin>, IWidgetPluginManager
    {
        #region Fields

        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public WidgetPluginManager(ICacheKeyService cacheKeyService,
            ICustomerService customerService,
            IPluginService pluginService,
            WidgetSettings widgetSettings) : base(cacheKeyService, customerService, pluginService)
        {
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="widgetZone">Widget zone; pass null to load all plugins</param>
        /// <returns>List of active widget</returns>
        public virtual IList<IWidgetPlugin> LoadActivePlugins(Customer customer = null, int storeId = 0, string widgetZone = null)
        {
            var widgets = LoadActivePlugins(_widgetSettings.ActiveWidgetSystemNames, customer, storeId);

            //filter by widget zone
            if (!string.IsNullOrEmpty(widgetZone))
                widgets = widgets.Where(widget =>
                    widget.GetWidgetZones().Contains(widgetZone, StringComparer.InvariantCultureIgnoreCase)).ToList();

            return widgets;
        }

        /// <summary>
        /// Check whether the passed widget is active
        /// </summary>
        /// <param name="widget">Widget to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(IWidgetPlugin widget)
        {
            return IsPluginActive(widget, _widgetSettings.ActiveWidgetSystemNames);
        }

        /// <summary>
        /// Check whether the widget with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of widget to check</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(string systemName, Customer customer = null, int storeId = 0)
        {
            var widget = LoadPluginBySystemName(systemName, customer, storeId);

            return IsPluginActive(widget);
        }

        #endregion
    }
}