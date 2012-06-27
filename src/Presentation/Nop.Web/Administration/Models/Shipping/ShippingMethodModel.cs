using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Shipping;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Shipping
{
    [Validator(typeof(ShippingMethodValidator))]
    public partial class ShippingMethodModel : BaseNopEntityModel, ILocalizedModel<ShippingMethodLocalizedModel>
    {
        public ShippingMethodModel()
        {
            Locales = new List<ShippingMethodLocalizedModel>();
        }
        [NopResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<ShippingMethodLocalizedModel> Locales { get; set; }
    }

    public partial class ShippingMethodLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

    }
}