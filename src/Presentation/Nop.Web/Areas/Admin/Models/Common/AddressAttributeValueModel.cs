using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Common;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Common
{
    [Validator(typeof(AddressAttributeValueValidator))]
    public partial class AddressAttributeValueModel : BaseNopEntityModel, ILocalizedModel<AddressAttributeValueLocalizedModel>
    {
        public AddressAttributeValueModel()
        {
            Locales = new List<AddressAttributeValueLocalizedModel>();
        }

        public int AddressAttributeId { get; set; }

        [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<AddressAttributeValueLocalizedModel> Locales { get; set; }
    }

    public partial class AddressAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
        public string Name { get; set; }
    }
}