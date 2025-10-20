using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Menus;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Menus;

namespace Nop.Web.Components;

public partial class MainMenuViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IMenuModelFactory _menuModelFactory;

    #endregion

    #region Ctor

    public MainMenuViewComponent(IMenuModelFactory menuModelFactory)
    {
        _menuModelFactory = menuModelFactory;
    }

    #endregion

    #region Methods 

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var menus = await _menuModelFactory.PrepareMenuModelsAsync(MenuType.Main);
        return View(menus?.FirstOrDefault() ?? new MenuModel());
    }

    #endregion
}
