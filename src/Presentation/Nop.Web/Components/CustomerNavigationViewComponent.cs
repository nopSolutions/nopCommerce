using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class CustomerNavigationViewComponent : NopViewComponent
{
    protected readonly ICustomerModelFactory _customerModelFactory;

    public CustomerNavigationViewComponent(ICustomerModelFactory customerModelFactory)
    {
        _customerModelFactory = customerModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="selectedTabId">The selected tab identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(int selectedTabId = 0)
    {
        var model = await _customerModelFactory.PrepareCustomerNavigationModelAsync(selectedTabId);
        return await ViewAsync(model);
    }
}