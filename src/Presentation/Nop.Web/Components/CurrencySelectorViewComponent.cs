using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class CurrencySelectorViewComponent : NopViewComponent
{
    protected readonly ICommonModelFactory _commonModelFactory;

    public CurrencySelectorViewComponent(ICommonModelFactory commonModelFactory)
    {
        _commonModelFactory = commonModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = await _commonModelFactory.PrepareCurrencySelectorModelAsync();
        if (model.AvailableCurrencies.Count == 1)
            return Content("");

        return await ViewAsync(model);
    }
}