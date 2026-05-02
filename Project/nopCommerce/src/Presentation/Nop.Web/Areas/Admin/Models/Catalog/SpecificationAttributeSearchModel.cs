using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a specification attribute search model
/// </summary>
public partial record SpecificationAttributeSearchModel : BaseSearchModel
{
    #region Properties

    public int SpecificationAttributeGroupId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.List.SearchName")]
    public string AttributeName { get; set; }

    #endregion
}