using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Vendors;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    [Validator(typeof(VendorAttributeValidator))]
    public partial class VendorAttributeModel : BaseNopEntityModel, ILocalizedModel<VendorAttributeLocalizedModel>
    {
        public VendorAttributeModel()
        {
            Locales = new List<VendorAttributeLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }
        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.AttributeControlType")]
        public string AttributeControlTypeName { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<VendorAttributeLocalizedModel> Locales { get; set; }
    }

    public partial class VendorAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.Name")]
        public string Name { get; set; }
    }
}