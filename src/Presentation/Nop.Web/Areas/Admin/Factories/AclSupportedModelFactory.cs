using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the model factory which supports access control list (ACL)
/// </summary>
public partial class AclSupportedModelFactory : IAclSupportedModelFactory
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ICustomerService _customerService;

    #endregion

    #region Ctor

    public AclSupportedModelFactory(IAclService aclService,
        ICustomerService customerService)
    {
        _aclService = aclService;
        _customerService = customerService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare selected and all available customer roles for the passed model
    /// </summary>
    /// <typeparam name="TModel">ACL supported model type</typeparam>
    /// <param name="model">Model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareModelCustomerRolesAsync<TModel>(TModel model) where TModel : IAclSupportedModel
    {
        ArgumentNullException.ThrowIfNull(model);

        //prepare available customer roles
        var availableRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
        model.AvailableCustomerRoles = availableRoles.Select(role => new SelectListItem
        {
            Text = role.Name,
            Value = role.Id.ToString(),
            Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
        }).ToList();
    }

    /// <summary>
    /// Prepare selected and all available customer roles for the passed model by ACL mappings
    /// </summary>
    /// <typeparam name="TModel">ACL supported model type</typeparam>
    /// <param name="model">Model</param>
    /// <param name="entityName">Entity name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareModelCustomerRolesAsync<TModel>(TModel model, string entityName)
        where TModel : BaseNopEntityModel, IAclSupportedModel
    {
        ArgumentNullException.ThrowIfNull(model);

        //prepare customer roles with granted access
        model.SelectedCustomerRoleIds = (await _aclService.GetCustomerRoleIdsWithAccessAsync(model.Id, entityName)).ToList();

        await PrepareModelCustomerRolesAsync(model);
    }

    #endregion
}