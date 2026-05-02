using Nop.Core.Configuration;

namespace Nop.Core.Domain.FilterLevels;

/// <summary>
/// Filter level settings
/// </summary>
public partial class FilterLevelSettings : ISettings
{
    public FilterLevelSettings()
    {
        FilterLevelEnumDisabled = new List<int>();
    }

    /// <summary>
    /// Gets or sets a value indicating whether the filter levels is enabled
    /// </summary>
    public bool FilterLevelEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the filter levels should be displayed on the home page
    /// </summary>
    public bool DisplayOnHomePage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the filter levels should be displayed on the product details page
    /// </summary>
    public bool DisplayOnProductDetailsPage { get; set; }

    /// <summary>
    /// Gets or sets a list of disabled values of FilterLevelEnum
    /// </summary>
    public List<int> FilterLevelEnumDisabled { get; set; }
}
