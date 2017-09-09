using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Validators.Vendors;

namespace Nop.Web.Models.Vendors
{
    [Validator(typeof(ApplyVendorValidator))]
    public partial class ApplyVendorModel : BaseNopModel
    {
        [NopResourceDisplayName("Vendors.ApplyAccount.Name")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Vendors.ApplyAccount.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Vendors.ApplyAccount.Description")]
        public string Description { get; set; }
        
        public bool DisplayCaptcha { get; set; }

        public bool TermsOfServiceEnabled { get; set; }
        public bool TermsOfServicePopup { get; set; }

        public bool DisableFormInput { get; set; }
        public string Result { get; set; }
    }
}