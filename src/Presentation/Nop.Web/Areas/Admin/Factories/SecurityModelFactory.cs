using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Security;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the security model factory implementation
/// </summary>
public partial class SecurityModelFactory : ISecurityModelFactory
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public SecurityModelFactory(ICustomerService customerService,
        ILocalizationService localizationService,
        IPermissionService permissionService)
    {
        _customerService = customerService;
        _localizationService = localizationService;
        _permissionService = permissionService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare permission configuration model
    /// </summary>
    /// <param name="model">Permission configuration model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the permission configuration model
    /// </returns>
    public virtual async Task<PermissionConfigurationModel> PreparePermissionConfigurationModelAsync(PermissionConfigurationModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var customerRoles = await _customerService.GetAllCustomerRolesAsync(true);
        model.AreCustomerRolesAvailable = customerRoles.Any();
        var permissionRecords = await _permissionService.GetAllPermissionRecordsAsync();
        model.IsPermissionsAvailable = permissionRecords.Any();

        return model;
    }

    /// <summary>
    /// Prepare permission item model
    /// </summary>
    /// <param name="permissionRecord">Permission record</param>
    /// <param name="availableRoles">All available customer roles</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the permission item model
    /// </returns>
    public virtual async Task<PermissionItemModel> PreparePermissionItemModelAsync(PermissionRecord permissionRecord, IList<CustomerRole> availableRoles = null)
    {
        availableRoles ??= await _customerService.GetAllCustomerRolesAsync(showHidden: true);

        var mapping = await _permissionService.GetMappingByPermissionRecordIdAsync(permissionRecord.Id);

        var names = await mapping
            .Select(m => availableRoles.FirstOrDefault(p => p.Id == m.CustomerRoleId))
            .Where(r => r != null).Select(r => r.Name).ToListAsync();

        var (ids, appliedFor) = (mapping.Select(m => m.CustomerRoleId).ToList(), string.Join(", ", names));

        //fill in model values from the entity
        var permissionItemModel = new PermissionItemModel
        {
            Id = permissionRecord.Id,
            PermissionName = await _localizationService.GetLocalizedPermissionNameAsync(permissionRecord),
            PermissionAppliedFor = appliedFor,
            SelectedCustomerRoleIds = ids.ToList(),
            AvailableCustomerRoles = availableRoles.Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Id.ToString(),
                Selected = ids.Contains(role.Id)
            }).ToList()
        };

        return permissionItemModel;
    }

    /// <summary>
    /// Prepare permission category list model
    /// </summary>
    /// <param name="searchModel">permission category search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the permission category list model
    /// </returns>
    public virtual async Task<PermissionCategoryListModel> PreparePermissionCategoryListModelAsync(PermissionCategorySearchModel searchModel)
    {
        var permissions = await _permissionService.GetAllPermissionRecordsAsync();

        var types = permissions
            .GroupBy(p => p.Category, p => p)
            .Select(p => p.Key).ToList();

        var pagedTypes = types.ToPagedList(searchModel);

        //prepare list model
        var model = new PermissionCategoryListModel().PrepareToGrid(searchModel, pagedTypes, () =>
        {
            //fill in model values from the entity
            return pagedTypes.Select(t => new PermissionCategoryModel
            {
                Name = t,
                Text = CommonHelper.SplitCamelCaseWord(t)
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged permission item list model
    /// </summary>
    /// <param name="searchModel">Permission item search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the permission item list model
    /// </returns>
    public virtual async Task<PermissionItemListModel> PreparePermissionItemListModelAsync(PermissionItemSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get permissions
        var permissionItems = (await _permissionService.GetAllPermissionRecordsAsync())
            .Where(p => p.Category == searchModel.PermissionCategoryName)
            .ToList()
            .ToPagedList(searchModel);

        var availableRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);

        //prepare list model
        var model = await new PermissionItemListModel().PrepareToGridAsync(searchModel, permissionItems, () =>
        {
            //fill in model values from the entity
            return permissionItems.SelectAwait(async item => await PreparePermissionItemModelAsync(item, availableRoles));
        });

        return model;
    }

    #endregion
}