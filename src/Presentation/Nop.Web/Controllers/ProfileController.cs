﻿using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Http;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework;

namespace Nop.Web.Controllers;

public partial class ProfileController : BasePublicController
{
    protected readonly CustomerSettings _customerSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProfileModelFactory _profileModelFactory;

    public ProfileController(CustomerSettings customerSettings,
        ICustomerService customerService,
        IPermissionService permissionService,
        IProfileModelFactory profileModelFactory)
    {
        _customerSettings = customerSettings;
        _customerService = customerService;
        _permissionService = permissionService;
        _profileModelFactory = profileModelFactory;
    }

    public virtual async Task<IActionResult> Index(int? id, int? pageNumber)
    {
        if (!_customerSettings.AllowViewingProfiles)
        {
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        }

        var customerId = 0;
        if (id.HasValue)
        {
            customerId = id.Value;
        }

        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer == null || await _customerService.IsGuestAsync(customer))
        {
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        }

        //display "edit" (manage) link
        if (await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.Customers.CUSTOMERS_VIEW))
            DisplayEditLink(Url.Action("Edit", "Customer", new { id = customer.Id, area = AreaNames.ADMIN }));

        var model = await _profileModelFactory.PrepareProfileIndexModelAsync(customer, pageNumber);
        return View(model);
    }
}