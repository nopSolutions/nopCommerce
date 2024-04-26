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

    public async Task<IViewComponentResult> InvokeAsync(int selectedTabId = 0)
    {
        var model = await _customerModelFactory.PrepareCustomerNavigationModelAsync(selectedTabId);
        return View(model);
    }
}