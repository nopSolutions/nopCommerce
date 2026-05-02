using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a predefined product attribute value search model
/// </summary>
public partial record PredefinedProductAttributeValueSearchModel : BaseSearchModel
{
    #region Properties

    public int ProductAttributeId { get; set; }

    #endregion
}