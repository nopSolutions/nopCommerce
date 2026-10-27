using Nop.Core.Domain.Customers;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.Forums.Services;

/// <summary>
/// Forums permission configuration manager
/// </summary>
public class ForumPermissionConfigManager : IPermissionConfigManager
{
    /// <summary>
    /// Gets all permission configurations
    /// </summary>
    public IList<PermissionConfig> AllConfigs => new List<PermissionConfig>
    {
        new ("Admin area. Forums. View", ForumDefaults.Permissions.FORUMS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Forums. Create, edit, delete", ForumDefaults.Permissions.FORUMS_MANAGE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
    };
}