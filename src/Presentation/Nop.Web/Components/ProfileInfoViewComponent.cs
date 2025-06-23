using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ProfileInfoViewComponent : NopViewComponent
{
    protected readonly ICustomerService _customerService;
    protected readonly IProfileModelFactory _profileModelFactory;

    public ProfileInfoViewComponent(ICustomerService customerService, IProfileModelFactory profileModelFactory)
    {
        _customerService = customerService;
        _profileModelFactory = profileModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="customerProfileId">The customer profile identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(int customerProfileId)
    {
        var customer = await _customerService.GetCustomerByIdAsync(customerProfileId);
        ArgumentNullException.ThrowIfNull(customer);

        var model = await _profileModelFactory.PrepareProfileInfoModelAsync(customer);
        return await ViewAsync(model);
    }
}