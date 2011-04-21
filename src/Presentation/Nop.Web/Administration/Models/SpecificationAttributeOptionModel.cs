using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;


namespace Nop.Admin.Models
{
    [Validator(typeof(SpecificationAttributeOptionValidator))]
    public class SpecificationAttributeOptionModel : BaseNopEntityModel, ILocalizedModel<SpecificationAttributeOptionLocalizedModel>
    {
        public SpecificationAttributeOptionModel()
        {
            Locales = new List<SpecificationAttributeOptionLocalizedModel>();
        }

        #region Model

        public int SpecificationAttributeId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}
        
        public IList<SpecificationAttributeOptionLocalizedModel> Locales { get; set; }

        #endregion
    }

    public class SpecificationAttributeOptionLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.Options.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}