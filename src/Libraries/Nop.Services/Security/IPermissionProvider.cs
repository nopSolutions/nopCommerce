using Nop.Core.Domain.Security;

namespace Nop.Services.Security;

/// <summary>
/// Permission provider
/// </summary>
public partial interface IPermissionProvider
{
    /// <summary>
    /// Get permissions
    /// </summary>
    /// <returns>Permissions</returns>
    IEnumerable<PermissionRecord> GetPermissions();

    /// <summary>
    /// Get default permissions
    /// </summary>
    /// <returns>Default permissions</returns>
    HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions();
}