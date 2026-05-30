using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product video search model
/// </summary>
public partial record ProductVideoSearchModel : BaseSearchModel
{
    #region Properties

    public int ProductId { get; set; }

    #endregion
}