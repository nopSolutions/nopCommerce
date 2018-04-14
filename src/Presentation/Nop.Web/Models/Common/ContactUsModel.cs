using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Web.Validators.Common;

namespace Nop.Web.Models.Common
{
    [Validator(typeof(ContactUsValidator))]
    public partial class ContactUsModel : BaseNopModel
    {
        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("ContactUs.Email")]
        public string Email { get; set; }
        
        [NopResourceDisplayName("ContactUs.Subject")]
        public string Subject { get; set; }
        public bool SubjectEnabled { get; set; }

        [NopResourceDisplayName("ContactUs.Enquiry")]
        public string Enquiry { get; set; }

        [NopResourceDisplayName("ContactUs.FullName")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}