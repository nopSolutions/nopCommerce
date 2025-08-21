using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Menus;

/// <summary>
/// Represents the menu item
/// </summary>
public partial class MenuItem : BaseEntity, IAclSupported, IStoreMappingSupported, ILocalizedEntity
{
    /// <summary>
    /// Gets or sets the title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the link URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the menu item type identifier
    /// </summary>
    public int MenuItemTypeId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the related entity
    /// </summary>
    public int? EntityId { get; set; }

    /// <summary>
    /// Gets or sets the number of sub-items for entity item in the grid view
    /// </summary>
    public int? NumberOfSubItemsPerGridElement { get; set; }

    /// <summary>
    /// Gets or sets the number of elements in a grid row
    /// </summary>
    public int? NumberOfItemsPerGridRow { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of loaded sub-entities
    /// </summary>
    public int? MaximumNumberEntities { get; set; }

    /// <summary>
    /// Gets or sets the route name
    /// </summary>
    public string RouteName { get; set; }

    /// <summary>
    /// Gets or sets the template identifier
    /// </summary>
    public int TemplateId { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the CSS class
    /// </summary>
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the menu item is subject to ACL
    /// </summary>
    public bool SubjectToAcl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the menu item is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the menu item is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the parent menu item
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// Gets or sets the menu identifier
    /// </summary>
    public int MenuId { get; set; }

    /// <summary>
    /// Gets or sets the menu item type
    /// </summary>
    public MenuItemType MenuItemType
    {
        get => (MenuItemType)MenuItemTypeId;
        set => MenuItemTypeId = (int)value;
    }

    /// <summary>
    /// Gets or sets the menu item template
    /// </summary>
    public MenuItemTemplate Template
    {
        get => (MenuItemTemplate)TemplateId;
        set => TemplateId = (int)value;
    }
}