using Nop.Core.Domain.Customers;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.RFQ.Services;

/// <summary>
/// RFQ permission provider
/// </summary>
public class RfqPermissionConfigManager : IPermissionConfigManager
{
    public const string ADMIN_ACCESS_RFQ = "Misc.RFQ.AccessRFQ.Admin.AccessRFQ";
    public const string ACCESS_RFQ = "Misc.RFQ.AccessRFQ.PublicStore.AccessRFQ";

    /// <summary>
    /// Gets all permission configurations
    /// </summary>
    public IList<PermissionConfig> AllConfigs =>
        new List<PermissionConfig>
        {
            new("Admin area. Access to the customer’s Request and Price Offer functionality", ADMIN_ACCESS_RFQ , nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName),
            new("Public store. Access to the customer’s Request and Price Offer functionality", ACCESS_RFQ , nameof(StandardPermission.Orders), NopCustomerDefaults.AdministratorsRoleName, NopCustomerDefaults.RegisteredRoleName)
        };
}