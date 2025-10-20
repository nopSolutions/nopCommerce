using Nop.Core.Domain.Menus;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Menus;

/// <summary>
/// Represents a menu item model
/// </summary>
public partial record MenuItemModel : BaseNopEntityModel
{
    #region Properties

    public string Title { get; set; }
    public string Url { get; set; }
    public string CssClass { get; set; }
    public int? ParentId { get; set; }
    public PictureModel Picture { get; set; }
    public int NumberOfSubItemsPerGridElement { get; set; }
    public int NumberOfItemsPerGridRow { get; set; }
    public int MaximumNumberEntities { get; set; }
    public MenuItemType MenuItemType { get; set; }
    public MenuItemTemplate Template { get; set; }
    public int EntityId { get; set; }
    public List<MenuItemModel> ChildrenItems { get; set; } = new();

    #endregion
}
