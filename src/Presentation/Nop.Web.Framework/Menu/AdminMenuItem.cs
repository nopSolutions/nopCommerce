namespace Nop.Web.Framework.Menu;

/// <summary>
/// Admin menu item
/// </summary>
public partial class AdminMenuItem
{
    #region Fields

    protected IList<string> _permissionNames;
    protected string _url;

    #endregion

    #region Ctor

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminMenuItem"/> class.
    /// </summary>
    public AdminMenuItem()
    {
        ChildNodes = new List<AdminMenuItem>();
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Inserts new menu item
    /// </summary>
    /// <param name="itemSystemName">Menu item to get place for insert</param>
    /// <param name="newMenuItem">New menu item</param>
    /// <param name="before">The flag which indicates where to place new menu item, before (when true) or after (when false) the exist one</param>
    /// <returns>True if a new menu item has been inserted</returns>
    protected virtual bool Insert(string itemSystemName, AdminMenuItem newMenuItem, bool before)
    {
        var position = 0;
        var inserted = false;

        foreach (var adminMenuItem in ChildNodes.ToList())
            if (!adminMenuItem.SystemName.Equals(itemSystemName))
                position += 1;
            else
            {
                ChildNodes.Insert(position + (before ? 0 : 1), newMenuItem);
                inserted = true;
                break;
            }

        if (inserted)
            return true;

        foreach (var adminMenuItem in ChildNodes)
        {
            inserted = adminMenuItem.Insert(itemSystemName, newMenuItem, before);

            if (inserted)
                break;
        }

        return inserted;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Checks whether this node or child ones has a specified system name
    /// </summary>
    /// <param name="systemName">System name</param>
    /// <returns>Result</returns>
    public bool ContainsSystemName(string systemName)
    {
        return GetItemBySystem(systemName) != null;
    }

    /// <summary>
    /// Gets the menu item by the system name
    /// </summary>
    /// <param name="systemName">Menu item system name</param>
    /// <returns>Menu item if found, otherwise null</returns>
    public AdminMenuItem GetItemBySystem(string systemName)
    {
        if (string.IsNullOrEmpty(systemName))
            return null;

        return SystemName.Equals(systemName) ? this : ChildNodes.Select(n => n.GetItemBySystem(systemName)).FirstOrDefault(n => n != null);
    }

    /// <summary>
    /// Inserts new menu item before the existing one
    /// </summary>
    /// <param name="itemSystemName">Menu item to get place for insert</param>
    /// <param name="newMenuItem">New menu item</param>
    /// <returns>True if a new menu item has been inserted</returns>
    public virtual bool InsertBefore(string itemSystemName, AdminMenuItem newMenuItem)
    {
        return Insert(itemSystemName, newMenuItem, true);
    }

    /// <summary>
    /// Inserts new menu item after the existing one
    /// </summary>
    /// <param name="itemSystemName">Menu item to get place for insert</param>
    /// <param name="newMenuItem">New menu item</param>
    /// <returns>True if a new menu item has been inserted</returns>
    public virtual bool InsertAfter(string itemSystemName, AdminMenuItem newMenuItem)
    {
        return Insert(itemSystemName, newMenuItem, false);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets permission names
    /// </summary>
    public IList<string> PermissionNames
    {
        get
        {
            if (_permissionNames != null)
                return _permissionNames;

            if (ChildNodes.Any())
                return ChildNodes.SelectMany(p => p.PermissionNames).ToList();

            return [];
        }
        set => _permissionNames = value;
    }

    /// <summary>
    /// Gets or sets the system name.
    /// </summary>
    public string SystemName { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string Url
    {
        get => _url;
        set
        {
            _url = value;
            var parts = value?.TrimEnd('/').Split('/') ?? [];
            var len = parts.Length;
            if (len < 3)
            {
                ControllerName = string.Empty;
                ActionName = string.Empty;
            }
            else
            {
                ControllerName = parts[len - 2];
                ActionName = parts[len - 1];
            }
        }
    }

    /// <summary>
    /// Gets or sets the child nodes.
    /// </summary>
    public IList<AdminMenuItem> ChildNodes { get; set; }

    /// <summary>
    /// Gets or sets the icon class (Font Awesome: http://fontawesome.io/)
    /// </summary>
    public string IconClass { get; set; }

    /// <summary>
    /// Gets or sets the item is visible
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to open url in new tab (window) or not
    /// </summary>
    public bool OpenUrlInNewTab { get; set; }

    /// <summary>
    /// Gets the controller name from URL
    /// </summary>
    public string ControllerName { get; protected set; }

    /// <summary>
    /// Gets the action name from URL
    /// </summary>
    public string ActionName { get; protected set; }

    #endregion
}