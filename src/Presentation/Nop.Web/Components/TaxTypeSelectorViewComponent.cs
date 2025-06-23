using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class TaxTypeSelectorViewComponent : NopViewComponent
{
    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly TaxSettings _taxSettings;

    public TaxTypeSelectorViewComponent(ICommonModelFactory commonModelFactory,
        TaxSettings taxSettings)
    {
        _commonModelFactory = commonModelFactory;
        _taxSettings = taxSettings;
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
        if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
            return Content("");

        var model = await _commonModelFactory.PrepareTaxTypeSelectorModelAsync();
        return await ViewAsync(model);
    }
}