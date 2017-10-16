using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    [Validator(typeof(SpecificationAttributeValidator))]
    public partial class SpecificationAttributeModel : BaseNopEntityModel, ILocalizedModel<SpecificationAttributeLocalizedModel>
    {
        public SpecificationAttributeModel()
        {
            Locales = new List<SpecificationAttributeLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}


        public IList<SpecificationAttributeLocalizedModel> Locales { get; set; }
    }

    public partial class SpecificationAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.Fields.Name")]
        public string Name { get; set; }
    }
}