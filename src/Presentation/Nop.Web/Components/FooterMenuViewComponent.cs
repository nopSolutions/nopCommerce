using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Menus;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class FooterMenuViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IMenuModelFactory _menuModelFactory;

    #endregion

    #region Ctor

    public FooterMenuViewComponent(IMenuModelFactory menuModelFactory)
    {
        _menuModelFactory = menuModelFactory;
    }

    #endregion

    #region Methods 

    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View(await _menuModelFactory.PrepareMenuModelsAsync(MenuType.Footer));
    }

    #endregion
}
