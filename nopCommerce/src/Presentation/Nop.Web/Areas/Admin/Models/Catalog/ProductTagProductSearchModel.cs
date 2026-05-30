using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents tagged products search model
/// </summary>
public partial record ProductTagProductSearchModel : BaseSearchModel
{
    #region Properties

    public int ProductTagId { get; set; }

    #endregion
}
