using Nop.Web.Framework.Menu;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents an event that occurs after third party plugins menu item created
/// </summary>
public partial class ThirdPartyPluginsMenuItemCreatedEvent: BaseMenuItemCreatedEvent
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="adminMenu">Admin menu</param>
    /// <param name="rootMenuItem">Third party plugins menu item</param>
    public ThirdPartyPluginsMenuItemCreatedEvent(IAdminMenu adminMenu, AdminMenuItem rootMenuItem) : base(adminMenu)
    {
        MenuItem = rootMenuItem;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a third party plugins menu item
    /// </summary>
    public AdminMenuItem MenuItem { get; protected set; }

    #endregion
}