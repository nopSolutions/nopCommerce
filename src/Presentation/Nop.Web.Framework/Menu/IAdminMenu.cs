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
    Task<AdminMenuItem> GetRootNodeAsync(bool showHidden = false);

    /// <summary>
    /// Generates an admin menu item URL 
    /// </summary>
    /// <param name="controllerName">he name of the controller</param>
    /// <param name="actionName">The name of the action method</param>
    string GetMenuItemUrl(string controllerName, string actionName);
}