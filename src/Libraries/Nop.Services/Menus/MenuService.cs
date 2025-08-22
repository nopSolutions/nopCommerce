using Nop.Core;
using Nop.Core.Domain.Menus;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Menus;

/// <summary>
/// Menu service
/// </summary>
public partial class MenuService : IMenuService
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly IRepository<Menu> _menuRepository;
    protected readonly IRepository<MenuItem> _menuItemRepository;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public MenuService(IAclService aclService,
        IRepository<Menu> menuRepository,
        IRepository<MenuItem> menuItemRepository,
        IStoreMappingService storeMappingService,
        IWorkContext workContext)
    {
        _aclService = aclService;
        _menuRepository = menuRepository;
        _menuItemRepository = menuItemRepository;
        _storeMappingService = storeMappingService;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Sort menu items for tree representation
    /// </summary>
    /// <param name="menuItemsByParentId">Menu items for sort</param>
    /// <param name="parentId">Parent menu item identifier</param>
    /// <param name="ignoreMenuItemWithoutExistingParent">A value indicating whether menu items without parent menu item in provided list (source) should be ignored</param>
    /// <returns>
    /// An enumerable containing the sorted menu items
    /// </returns>
    protected virtual IEnumerable<MenuItem> SortMenuItemsForTree(
        ILookup<int, MenuItem> menuItemsByParentId,
        int parentId = 0,
        bool ignoreMenuItemWithoutExistingParent = false)
    {
        ArgumentNullException.ThrowIfNull(menuItemsByParentId);

        var remaining = parentId > 0
            ? new HashSet<int>(0)
            : menuItemsByParentId.Select(g => g.Key).ToHashSet();
        remaining.Remove(parentId);

        foreach (var item in menuItemsByParentId[parentId].OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id))
        {
            yield return item;

            remaining.Remove(item.Id);

            foreach (var subItem in SortMenuItemsForTree(menuItemsByParentId, item.Id, true))
            {
                yield return subItem;
                remaining.Remove(subItem.Id);
            }
        }

        if (ignoreMenuItemWithoutExistingParent)
            yield break;

        //find menu items without parent in provided menu item source and return them
        var orphans = remaining
            .SelectMany(id => menuItemsByParentId[id])
            .OrderBy(c => c.ParentId)
            .ThenBy(c => c.DisplayOrder)
            .ThenBy(c => c.Id);

        foreach (var orphan in orphans)
            yield return orphan;
    }

    #endregion

    #region Methods

    #region Menus

    /// <summary>
    /// Deletes a menu
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task DeleteMenuAsync(Menu menu)
    {
        return _menuRepository.DeleteAsync(menu);
    }

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
    public virtual async Task<IPagedList<Menu>> GetAllMenusAsync(
        MenuType? menuType = null,
        int storeId = 0,
        bool showHidden = false,
        int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
        return await _menuRepository.GetAllPagedAsync(async query =>
        {
            if (menuType is not null)
                query = query.Where(m => m.MenuTypeId == (int)menuType);

            if (!showHidden)
            {
                query = query.Where(m => m.Published);

                //apply ACL constraints
                var customer = await _workContext.GetCurrentCustomerAsync();
                query = await _aclService.ApplyAcl(query, customer);
            }

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            }

            return query
                .OrderBy(m => m.MenuTypeId)
                .ThenBy(m => m.DisplayOrder)
                .ThenBy(m => m.Id);

        }, pageIndex, pageSize, includeDeleted: false);
    }

    /// <summary>
    /// Gets a menu by identifier
    /// </summary>
    /// <param name="menuId">The menu identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a menu
    /// </returns>
    public virtual Task<Menu> GetMenuByIdAsync(int menuId)
    {
        return _menuRepository.GetByIdAsync(menuId, cache => default, includeDeleted: false);
    }

    /// <summary>
    /// Insert a menu
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task InsertMenuAsync(Menu menu)
    {
        return _menuRepository.InsertAsync(menu);
    }

    /// <summary>
    /// Update a menu
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task UpdateMenuAsync(Menu menu)
    {
        return _menuRepository.UpdateAsync(menu);
    }

    #endregion

    #region Menu items

    /// <summary>
    /// Deletes a menu item
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteMenuItemAsync(MenuItem menuItem)
    {
        if (menuItem is null)
            return;

        await deleteChildrenRecursive(menuItem.Id);
        await _menuItemRepository.DeleteAsync(menuItem);

        async Task deleteChildrenRecursive(int parentId)
        {
            var children = await GetAllMenuItemsAsync(parentMenuItemId: parentId, showHidden: true);

            foreach (var child in children)
            {
                await deleteChildrenRecursive(child.Id);
                await _menuItemRepository.DeleteAsync(child);
            }
        }
    }

    /// <summary>
    /// Gets a menu item by identifier
    /// </summary>
    /// <param name="menuItemId">The menu item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a menu item
    /// </returns>
    public virtual Task<MenuItem> GetMenuItemByIdAsync(int menuItemId)
    {
        return _menuItemRepository.GetByIdAsync(menuItemId, cache => default);
    }

    /// <summary>
    /// Insert a menu item
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task InsertMenuItemAsync(MenuItem menuItem)
    {
        return _menuItemRepository.InsertAsync(menuItem);
    }

    /// <summary>
    /// Update a menu item
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateMenuItemAsync(MenuItem menuItem)
    {
        ArgumentNullException.ThrowIfNull(menuItem);

        //validate hierarchy
        var parentMenuItem = await GetMenuItemByIdAsync(menuItem.ParentId);
        while (parentMenuItem != null)
        {
            if (menuItem.Id == parentMenuItem.Id)
            {
                menuItem.ParentId = 0;
                break;
            }

            parentMenuItem = await GetMenuItemByIdAsync(parentMenuItem.ParentId);
        }

        await _menuItemRepository.UpdateAsync(menuItem);
    }

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
    public virtual async Task<IPagedList<MenuItem>> GetAllMenuItemsAsync(
        int menuId = 0,
        int parentMenuItemId = 0,
        int storeId = 0,
        int depth = 0,
        bool treeSorting = false,
        bool showHidden = false,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var menuItems = await _menuItemRepository.GetAllAsync(async query =>
        {
            if (menuId > 0)
                query = query.Where(menuItem => menuItem.MenuId == menuId);

            if (parentMenuItemId > 0)
                query = query.Where(mi => mi.ParentId == parentMenuItemId);

            if (!showHidden)
            {
                query = query.Where(m => m.Published);

                //apply ACL constraints
                var customer = await _workContext.GetCurrentCustomerAsync();
                query = await _aclService.ApplyAcl(query, customer);
            }

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            }

            return query
                .OrderBy(i => i.DisplayOrder)
                .ThenBy(i => i.Id);
        }, includeDeleted: false);

        if (depth > 0)
            menuItems = FilterMenuItemsByDepth(menuItems, parentMenuItemId, depth).ToList();

        if (treeSorting)
            menuItems = SortMenuItemsForTree(menuItems.ToLookup(c => c.ParentId)).ToList();

        //paging
        return new PagedList<MenuItem>(menuItems, pageIndex, pageSize);
    }

    /// <summary>
    /// Ensure that menu items don't exceed a certain depth 
    /// </summary>
    /// <param name="menuItems">Menu items for filtering</param>
    /// <param name="parentId">Parent menu item identifier</param>
    /// <param name="depthLimit">Depth to limit items</param>
    /// <returns>
    /// Menu items limited in depth
    /// </returns>
    public virtual IEnumerable<MenuItem> FilterMenuItemsByDepth(IEnumerable<MenuItem> menuItems, int parentId = 0, int depthLimit = 1)
    {
        ArgumentNullException.ThrowIfNull(menuItems);

        if (depthLimit == 0)
            yield break;

        depthLimit--;

        foreach (var item in menuItems.Where(item => item.ParentId == parentId))
        {
            yield return item;

            foreach (var child in FilterMenuItemsByDepth(menuItems, item.Id, depthLimit).OrderBy(item => item.DisplayOrder))
                yield return child;
        }
    }

    #endregion

    #endregion
}