using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class StoreThemeSelectorViewComponent : NopViewComponent
{
    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly StoreInformationSettings _storeInformationSettings;

    public StoreThemeSelectorViewComponent(ICommonModelFactory commonModelFactory,
        StoreInformationSettings storeInformationSettings)
    {
        _commonModelFactory = commonModelFactory;
        _storeInformationSettings = storeInformationSettings;
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
        if (!_storeInformationSettings.AllowCustomerToSelectTheme)
            return Content("");

        var model = await _commonModelFactory.PrepareStoreThemeSelectorModelAsync();
        return await ViewAsync(model);
    }
}