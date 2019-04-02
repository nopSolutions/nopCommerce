using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute localized model
    /// </summary>
    public partial class SpecificationAttributeLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.CustomValue")]
        public string ValueRaw { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.CustomValue")]
        public string Value { get; set; }
    }
}