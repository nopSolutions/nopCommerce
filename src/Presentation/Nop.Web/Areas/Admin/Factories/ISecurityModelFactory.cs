using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Web.Areas.Admin.Models.Security;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the security model factory
/// </summary>
public partial interface ISecurityModelFactory
{
    /// <summary>
    /// Prepare permission configuration model
    /// </summary>
    /// <param name="model">Permission configuration model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the permission configuration model
    /// </returns>
    Task<PermissionConfigurationModel> PreparePermissionConfigurationModelAsync(PermissionConfigurationModel model);

    /// <summary>
    /// Prepare permission item model
    /// </summary>
    /// <param name="permissionRecord">Permission record</param>
    /// <param name="availableRoles">All available customer roles</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the permission item model
    /// </returns>
    Task<PermissionItemModel> PreparePermissionItemModelAsync(PermissionRecord permissionRecord, IList<CustomerRole> availableRoles = null);

    /// <summary>
    /// Prepare permission category list model
    /// </summary>
    /// <param name="searchModel">permission category search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the permission category list model
    /// </returns>
    Task<PermissionCategoryListModel> PreparePermissionCategoryListModelAsync(PermissionCategorySearchModel searchModel);

    /// <summary>
    /// Prepare paged permission item list model
    /// </summary>
    /// <param name="searchModel">Permission item search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the permission item list model
    /// </returns>
    Task<PermissionItemListModel> PreparePermissionItemListModelAsync(PermissionItemSearchModel searchModel);
}