using Nop.Core.Domain.Customers;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.Polls.Services;

/// <summary>
/// News permission congiguration manager
/// </summary>
public class PollPermissionConfigManager : IPermissionConfigManager
{
    /// <summary>
    /// Gets all permission configurations
    /// </summary>
    public IList<PermissionConfig> AllConfigs => new List<PermissionConfig>
    {
        new ("Admin area. Polls. View", PollsDefaults.Permissions.POLLS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Polls. Create, edit, delete", PollsDefaults.Permissions.POLLS_MANAGE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
    };
}
