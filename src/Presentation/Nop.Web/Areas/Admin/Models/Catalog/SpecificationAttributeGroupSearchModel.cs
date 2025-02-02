using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a specification attribute group search model
/// </summary>
public partial record SpecificationAttributeGroupSearchModel : BaseSearchModel
{
    // this is was empty 
    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.SearchGroupName")]
    public string SpecificationAttributeSearchGroupName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.SearchName")]
    public string SpecificationAttributeName { get; set; }

}