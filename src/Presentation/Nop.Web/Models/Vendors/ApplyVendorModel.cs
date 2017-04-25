#if NET451
using System.Web.Mvc;
#endif
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Validators.Vendors;

namespace Nop.Web.Models.Vendors
{
    [Validator(typeof(ApplyVendorValidator))]
    public partial class ApplyVendorModel : BaseNopModel
    {
        [NopResourceDisplayName("Vendors.ApplyAccount.Name")]
        	
#if NET451
		[AllowHtml]
#endif
        public string Name { get; set; }

        [NopResourceDisplayName("Vendors.ApplyAccount.Email")]
        	
#if NET451
		[AllowHtml]
#endif
        public string Email { get; set; }

        [NopResourceDisplayName("Vendors.ApplyAccount.Description")]
        	
#if NET451
		[AllowHtml]
#endif
        public string Description { get; set; }
        
        public bool DisplayCaptcha { get; set; }

        public bool DisableFormInput { get; set; }
        public string Result { get; set; }
    }
}