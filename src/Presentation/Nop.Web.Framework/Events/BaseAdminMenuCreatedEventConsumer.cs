using Nop.Services.Events;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Base admin menu created event consumer
/// </summary>
public abstract class BaseAdminMenuCreatedEventConsumer : IConsumer<AdminMenuCreatedEvent>
{
    #region Fields

    protected readonly IPluginManager<IPlugin> _pluginManager;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    protected BaseAdminMenuCreatedEventConsumer(IPluginManager<IPlugin> pluginManager)
    {
        _pluginManager = pluginManager;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Checks is the current customer has rights to access this menu item
    /// By default, always return true
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true if access is granted, otherwise false
    /// </returns>
    protected virtual Task<bool> CheckAccessAsync()
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// Gets the menu item
    /// </summary>
    /// <param name="plugin">The instance of <see cref="IPlugin"/> interface</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the instance of <see cref="AdminMenuItem"/>
    /// </returns>
    protected virtual Task<AdminMenuItem> GetAdminMenuItemAsync(IPlugin plugin)
    {
        var menuItem = plugin?.GetAdminMenuItem();

        return Task.FromResult(menuItem);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        if (!await CheckAccessAsync())
            return;

        var plugin = await _pluginManager.LoadPluginBySystemNameAsync(PluginSystemName);

        //the LoadPluginBySystemNameAsync method returns only plugins that are already fully installed,
        //while the IConsumer<AdminMenuCreatedEvent> event can be called before the installation is complete
        if (plugin ==  null)
            return;

        var newItem = await GetAdminMenuItemAsync(plugin);

        if (newItem == null)
            return;

        switch (InsertType)
        {
            case MenuItemInsertType.After:
                eventMessage.RootMenuItem.InsertAfter(AfterMenuSystemName, newItem);
                break;
            case MenuItemInsertType.Before:
                eventMessage.RootMenuItem.InsertBefore(BeforeMenuSystemName, newItem);
                break;
            case MenuItemInsertType.TryAfterThanBefore:
                if (!eventMessage.RootMenuItem.InsertAfter(AfterMenuSystemName, newItem))
                    eventMessage.RootMenuItem.InsertBefore(BeforeMenuSystemName, newItem);
                break;
            case MenuItemInsertType.TryBeforeThanAfter:
                if (!eventMessage.RootMenuItem.InsertBefore(BeforeMenuSystemName, newItem))
                    eventMessage.RootMenuItem.InsertAfter(AfterMenuSystemName, newItem);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
    
    #region Properties

    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    protected abstract string PluginSystemName { get; }

    /// <summary>
    /// Menu item insertion type (by default: <see cref="MenuItemInsertType.Before"/>)
    /// </summary>
    protected virtual MenuItemInsertType InsertType => MenuItemInsertType.Before;

    /// <summary>
    /// The system name of the menu item after with need to insert the current one
    /// </summary>
    protected virtual string AfterMenuSystemName => string.Empty;

    /// <summary>
    /// The system name of the menu item before with need to insert the current one
    /// </summary>
    protected virtual string BeforeMenuSystemName => string.Empty;

    #endregion
}