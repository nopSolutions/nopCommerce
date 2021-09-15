using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute group localized model
    /// </summary>
    public record SpecificationAttributeGroupLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.Name")]
        public string Name { get; set; }
    }
}
