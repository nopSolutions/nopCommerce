using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays the nopCommerce news
/// </summary>
public partial class NopCommerceNewsViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IHomeModelFactory _homeModelFactory;

    #endregion

    #region Ctor

    public NopCommerceNewsViewComponent(IHomeModelFactory homeModelFactory)
    {
        _homeModelFactory = homeModelFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            //prepare model
            var model = await _homeModelFactory.PrepareNopCommerceNewsModelAsync();

            return View(model);
        }
        catch
        {
            return Content(string.Empty);
        }
    }

    #endregion
}