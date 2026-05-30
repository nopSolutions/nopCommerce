using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a search model of products that use the product attribute
/// </summary>
public partial record ProductAttributeProductSearchModel : BaseSearchModel
{
    #region Properties

    public int ProductAttributeId { get; set; }

    #endregion
}