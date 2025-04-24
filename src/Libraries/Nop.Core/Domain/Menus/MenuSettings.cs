using Nop.Core.Configuration;

namespace Nop.Core.Domain.Menus;

/// <summary>
/// Menu settings
/// </summary>
public partial class MenuSettings : ISettings
{
    /// <summary>
    /// The number of sub-items for entity item in the grid view
    /// </summary>
    public int NumberOfSubItemsPerGridElement { get; set; }

    /// <summary>
    /// Number of elements in a grid row
    /// </summary>
    public int NumberOfItemsPerGridRow { get; set; }

    /// <summary>
    /// Maximum number of loaded sub-entities
    /// </summary>
    public int MaximumNumberEntities { get; set; }

    /// <summary>
    /// Picture size of pictures in the grid view
    /// </summary>
    public int GridThumbPictureSize { get; set; }

    /// <summary>
    /// Maximum number of menu levels
    /// </summary>
    public int MaximumMainMenuLevels { get; set; }
}
