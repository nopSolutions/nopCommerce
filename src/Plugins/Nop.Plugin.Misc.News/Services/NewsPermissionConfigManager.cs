using Nop.Core.Domain.Customers;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.News.Services;

/// <summary>
/// News permission congiguration manager
/// </summary>
public class NewsPermissionConfigManager : IPermissionConfigManager
{
    /// <summary>
    /// Gets all permission configurations
    /// </summary>
    public IList<PermissionConfig> AllConfigs => new List<PermissionConfig>
    {
        new ("Admin area. News. View", NewsDefaults.Permissions.NEWS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. News. Create, edit, delete", NewsDefaults.Permissions.NEWS_MANAGE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. News comments. View", NewsDefaults.Permissions.NEWS_COMMENTS_VIEW, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. News comments. Create, edit, delete", NewsDefaults.Permissions.NEWS_COMMENTS_MANAGE, nameof(StandardPermission.ContentManagement), NopCustomerDefaults.AdministratorsRoleName),
    };
}