using Nop.Core;
using Nop.Core.Domain.Menus;

namespace Nop.Services.Menus;

/// <summary>
/// Menu service interface
/// </summary>
public partial interface IMenuService
{
    #region Menus

    /// <summary>
    /// Deletes a menu
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteMenuAsync(Menu menu);

    /// <summary>
    /// Gets all menus
    /// </summary>
    /// <returns>
    /// <param name="menuType">Menu type</param>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <param name="showHidden">A value indicating whether to load hidden records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// A task that represents the asynchronous operation
    /// The task result contains the menus
    /// </returns>
    Task<IPagedList<Menu>> GetAllMenusAsync(
        MenuType? menuType = null,
        int storeId = 0,
        bool showHidden = false,
        int pageIndex = 0,
        int pageSize = int.MaxValue);

    /// <summary>
    /// Gets a menu by identifier
    /// </summary>
    /// <param name="menuId">The menu identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a menu
    /// </returns>
    Task<Menu> GetMenuByIdAsync(int menuId);

    /// <summary>
    /// Insert a menu
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertMenuAsync(Menu menu);

    /// <summary>
    /// Update a menu
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateMenuAsync(Menu menu);

    #endregion

    #region Menu items

    /// <summary>
    /// Deletes a menu item
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteMenuItemAsync(MenuItem menuItem);

    /// <summary>
    /// Ensure that menu items don't exceed a certain depth 
    /// </summary>
    /// <param name="menuItems">Menu items for filtering</param>
    /// <param name="parentId">Parent menu item identifier</param>
    /// <param name="depthLimit">Depth to limit items</param>
    /// <returns>
    /// Menu items limited in depth
    /// </returns>
    IEnumerable<MenuItem> FilterMenuItemsByDepth(IEnumerable<MenuItem> menuItems, int parentId = 0, int depthLimit = 1);

    /// <summary>
    /// Gets a menu item by identifier
    /// </summary>
    /// <param name="menuItemId">Menu item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a menu item
    /// </returns>
    Task<MenuItem> GetMenuItemByIdAsync(int menuItemId);

    /// <summary>
    /// Insert a menu item
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertMenuItemAsync(MenuItem menuItem);

    /// <summary>
    /// Update a menu item
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateMenuItemAsync(MenuItem menuItem);

    /// <summary>
    /// Get all menu items
    /// </summary>
    /// <param name="menuId">Menu identifier; 0 or null to load all records</param>
    /// <param name="parentMenuItemId">Parent menu item identifier</param>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <param name="depth">Depth to limit items</param>
    /// <param name="treeSorting">A value indicating whether to sort menu items for tree representation</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains menu items
    /// </returns>
    Task<IPagedList<MenuItem>> GetAllMenuItemsAsync(
        int menuId = 0,
        int parentMenuItemId = 0,
        int storeId = 0,
        int depth = 0,
        bool treeSorting = false,
        bool showHidden = false,
        int pageIndex = 0,
        int pageSize = int.MaxValue);

    #endregion
}