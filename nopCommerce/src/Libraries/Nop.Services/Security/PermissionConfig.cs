namespace Nop.Services.Security;

/// <summary>
/// Permission config
/// </summary>
public partial class PermissionConfig
{
    #region Ctor

    public PermissionConfig(string name, string systemName, string category, params string[] defaultCustomerRoles)
    {
        Name = name;
        SystemName = systemName;
        Category = category;

        DefaultCustomerRoles.AddRange(defaultCustomerRoles);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the permission name
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets or sets the permission system name
    /// </summary>
    public string SystemName { get; protected set; }

    /// <summary>
    /// Gets or sets the permission category
    /// </summary>
    public string Category { get; protected set; }

    /// <summary>
    /// Gets default list of customer roles system name
    /// </summary>
    public List<string> DefaultCustomerRoles { get; } = new();

    #endregion
}