using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ProfilePostsViewComponent : NopViewComponent
{
    protected readonly ICustomerService _customerService;
    protected readonly IProfileModelFactory _profileModelFactory;

    public ProfilePostsViewComponent(ICustomerService customerService, IProfileModelFactory profileModelFactory)
    {
        _customerService = customerService;
        _profileModelFactory = profileModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="customerProfileId">The customer profile identifier</param>
    /// <param name="pageNumber">The page number</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(int customerProfileId, int pageNumber)
    {
        var customer = await _customerService.GetCustomerByIdAsync(customerProfileId);
        ArgumentNullException.ThrowIfNull(customer);

        var model = await _profileModelFactory.PrepareProfilePostsModelAsync(customer, pageNumber);
        return await ViewAsync(model);
    }
}