using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Menus;

/// <summary>
/// Represents the menu
/// </summary>
public partial class Menu : BaseEntity, IAclSupported, IStoreMappingSupported, ISoftDeletedEntity, ILocalizedEntity
{
    /// <summary>
    /// Gets or sets the menu name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the menu type identifier
    /// </summary>
    public int MenuTypeId { get; set; }

    /// <summary>
    /// Gets or sets the CSS class
    /// </summary>
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the menu is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the menu is subject to ACL
    /// </summary>
    public bool SubjectToAcl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the menu is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the menu has been deleted
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether all categories should be automatically loaded 
    /// </summary>
    public bool DisplayAllCategories { get; set; }

    /// <summary>
    /// Gets or sets the menu type
    /// </summary>
    public MenuType MenuType
    {
        get => (MenuType)MenuTypeId;
        set => MenuTypeId = (int)value;
    }
}
