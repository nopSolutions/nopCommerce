#if NET451
using System.Web.Mvc;
#endif
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Validators.Vendors;

namespace Nop.Web.Models.Vendors
{
    [Validator(typeof(VendorInfoValidator))]
    public class VendorInfoModel : BaseNopModel
    {
        [NopResourceDisplayName("Account.VendorInfo.Name")]
        	
#if NET451
		[AllowHtml]
#endif
        public string Name { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Email")]
        	
#if NET451
		[AllowHtml]
#endif
        public string Email { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Description")]
        	
#if NET451
		[AllowHtml]
#endif
        public string Description { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Picture")]
        public string PictureUrl { get; set; }
    }
}