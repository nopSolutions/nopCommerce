using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Framework.Models;

/// <summary>
/// Represents the store mapping supported model
/// </summary>
public partial interface IStoreMappingSupportedModel
{
    #region Properties

    /// <summary>
    /// Gets or sets identifiers of the selected stores
    /// </summary>
    IList<int> SelectedStoreIds { get; set; }

    /// <summary>
    /// Gets or sets items for the all available stores
    /// </summary>
    IList<SelectListItem> AvailableStores { get; set; }

    #endregion
}