using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Cms;

/// <summary>
/// Represents a widget plugin manager implementation
/// </summary>
public partial class WidgetPluginManager : PluginManager<IWidgetPlugin>, IWidgetPluginManager
{
    #region Fields

    protected readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public WidgetPluginManager(ICustomerService customerService,
        IPluginService pluginService,
        WidgetSettings widgetSettings) : base(customerService, pluginService)
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of active widget
    /// </returns>
    public virtual async Task<IList<IWidgetPlugin>> LoadActivePluginsAsync(Customer customer = null, int storeId = 0, string widgetZone = null)
    {
        var widgets = await LoadActivePluginsAsync(_widgetSettings.ActiveWidgetSystemNames, customer, storeId);

        //filter by widget zone
        if (!string.IsNullOrEmpty(widgetZone))
            widgets = await widgets.WhereAwait(async widget =>
                (await widget.GetWidgetZonesAsync()).Contains(widgetZone, StringComparer.InvariantCultureIgnoreCase)).ToListAsync();

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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
    {
        var widget = await LoadPluginBySystemNameAsync(systemName, customer, storeId);

        return IsPluginActive(widget);
    }

    #endregion
}