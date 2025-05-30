namespace Nop.Web.Framework.Menu;

/// <summary>
/// Admin menu interface
/// </summary>
public partial interface IAdminMenu
{
    /// <summary>
    /// Gets the root node
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records (Visible == false)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the root menu item
    /// </returns>
    Task<AdminMenuItem> GetRootNodeAsync(bool showHidden = false);

    /// <summary>
    /// Generates an admin menu item URL 
    /// </summary>
    /// <param name="controllerName">The name of the controller</param>
    /// <param name="actionName">The name of the action method</param>
    /// <returns>Menu item URL</returns>
    string GetMenuItemUrl(string controllerName, string actionName);
}