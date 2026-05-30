using Nop.Core.Domain.Menus;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Menus;

/// <summary>
/// Represents a menu model
/// </summary>
public partial record MenuModel : BaseNopEntityModel
{
    #region Properties

    public MenuType MenuType { get; set; }
    public string Name { get; set; }
    public string CssClass { get; set; }
    public IList<MenuItemModel> Items { get; set; } = new List<MenuItemModel>();

    #endregion
}
