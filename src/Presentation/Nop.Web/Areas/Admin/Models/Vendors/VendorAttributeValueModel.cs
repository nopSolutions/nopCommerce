using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Vendors;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    [Validator(typeof(VendorAttributeValueValidator))]
    public partial class VendorAttributeValueModel : BaseNopEntityModel, ILocalizedModel<VendorAttributeValueLocalizedModel>
    {
        public VendorAttributeValueModel()
        {
            Locales = new List<VendorAttributeValueLocalizedModel>();
        }

        public int VendorAttributeId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<VendorAttributeValueLocalizedModel> Locales { get; set; }
    }

    public partial class VendorAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.Name")]
        public string Name { get; set; }
    }
}