using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Vendors;

namespace Nop.Web.Models.Vendors
{
    [Validator(typeof(VendorInfoValidator))]
    public class VendorInfoModel : BaseNopModel
    {
        [NopResourceDisplayName("Account.VendorInfo.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Picture")]
        public string PictureUrl { get; set; }
    }
}