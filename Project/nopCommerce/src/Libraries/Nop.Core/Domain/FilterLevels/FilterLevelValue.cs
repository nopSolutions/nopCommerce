using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.FilterLevels;

/// <summary>
/// Represents a set of hierarchical filter level values
/// </summary>
/// <remarks>This class provides properties to store values for up to three levels of filtering.</remarks>
public partial class FilterLevelValue : BaseEntity, ILocalizedEntity
{
    /// <summary>
    /// Gets or sets the first level filter value
    /// </summary>
    public string FilterLevel1Value { get; set; }

    /// <summary>
    /// Gets or sets the second level filter value
    /// </summary>
    public string FilterLevel2Value { get; set; }

    /// <summary>
    /// Gets or sets the third level filter value
    /// </summary>
    public string FilterLevel3Value { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance update
    /// </summary>
    public DateTime UpdatedOnUtc { get; set; }
}
