#if NET451
using System.Web.Mvc;
#endif
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Common;

namespace Nop.Web.Models.Common
{
    [Validator(typeof(ContactVendorValidator))]
    public partial class ContactVendorModel : BaseNopModel
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }

        	
#if NET451
		[AllowHtml]
#endif
        [NopResourceDisplayName("ContactVendor.Email")]
        public string Email { get; set; }

        	
#if NET451
		[AllowHtml]
#endif
        [NopResourceDisplayName("ContactVendor.Subject")]
        public string Subject { get; set; }
        public bool SubjectEnabled { get; set; }

        	
#if NET451
		[AllowHtml]
#endif
        [NopResourceDisplayName("ContactVendor.Enquiry")]
        public string Enquiry { get; set; }

        	
#if NET451
		[AllowHtml]
#endif
        [NopResourceDisplayName("ContactVendor.FullName")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}