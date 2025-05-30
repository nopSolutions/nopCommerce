using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Components;

public partial class MultistorePreviewViewComponent : NopViewComponent
{
    #region Fields

    protected readonly ICommonModelFactory _commonModelFactory;

    #endregion

    #region Ctor

    public MultistorePreviewViewComponent(ICommonModelFactory commonModelFactory)
    {
        _commonModelFactory = commonModelFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="model">Entity model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(object model)
    {
        if (model is not BaseNopEntityModel entityModel)
            return Content(string.Empty);

        var multistorePreviewModels = await _commonModelFactory.PrepareMultistorePreviewModelsAsync(entityModel);

        return View(multistorePreviewModels);
    }

    #endregion
}