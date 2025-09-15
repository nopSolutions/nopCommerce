using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers.Api;

/// <summary>
/// Base API controller
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    #region Fields

    protected readonly IWorkContext _workContext;
    protected readonly IPermissionService _permissionService;
    protected readonly ICustomerService _customerService;

    #endregion

    #region Ctor

    public BaseApiController(
        IWorkContext workContext,
        IPermissionService permissionService,
        ICustomerService customerService)
    {
        _workContext = workContext;
        _permissionService = permissionService;
        _customerService = customerService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get current customer
    /// </summary>
    /// <returns>Current customer</returns>
    protected async Task<Nop.Core.Domain.Customers.Customer> GetCurrentCustomerAsync()
    {
        return await _workContext.GetCurrentCustomerAsync();
    }

    /// <summary>
    /// Check if current customer has permission
    /// </summary>
    /// <param name="permission">Permission system name</param>
    /// <returns>True if has permission, false otherwise</returns>
    protected async Task<bool> HasPermissionAsync(string permission)
    {
        var customer = await GetCurrentCustomerAsync();
        return await _permissionService.AuthorizeAsync(permission, customer);
    }

    #endregion
}