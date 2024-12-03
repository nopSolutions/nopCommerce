using Nop.Web.Framework.Menu;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents the base event that occurs after admin menu item create
/// </summary>
public abstract partial class BaseMenuItemCreatedEvent
{
    private readonly IAdminMenu _adminMenu;

    protected BaseMenuItemCreatedEvent(IAdminMenu adminMenu)
    {
        _adminMenu = adminMenu;
    }

    /// <summary>
    /// Generates an admin menu item URL 
    /// </summary>
    /// <param name="controllerName">The name of the controller</param>
    /// <param name="actionName">The name of the action method</param>
    public virtual string GetMenuItemUrl(string controllerName, string actionName)
    {
        return _adminMenu.GetMenuItemUrl(controllerName, actionName);
    }
}
