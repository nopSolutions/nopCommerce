using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a copy product model
/// </summary>
public partial record CopyProductModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Products.Copy.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.Copy.CopyMultimedia")]
    public bool CopyMultimedia { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.Copy.Published")]
    public bool Published { get; set; }

    #endregion
}