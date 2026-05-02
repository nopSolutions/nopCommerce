using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a model of products that use the specification attribute
/// </summary>
public partial record SpecificationAttributeProductModel : BaseNopEntityModel
{
    #region Properties

    public int SpecificationAttributeId { get; set; }

    public int ProductId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.UsedByProducts.Product")]
    public string ProductName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.UsedByProducts.Published")]
    public bool Published { get; set; }

    #endregion
}