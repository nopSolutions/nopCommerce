using System.Web.Mvc;
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

        [AllowHtml]
        [NopResourceDisplayName("ContactVendor.Email")]
        public string Email { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("ContactVendor.Subject")]
        public string Subject { get; set; }
        public bool SubjectEnabled { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("ContactVendor.Enquiry")]
        public string Enquiry { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("ContactVendor.FullName")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}