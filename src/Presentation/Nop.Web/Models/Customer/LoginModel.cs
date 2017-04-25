using System.ComponentModel.DataAnnotations;
#if NET451
using System.Web.Mvc;
#endif
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Validators.Customer;

namespace Nop.Web.Models.Customer
{
    [Validator(typeof(LoginValidator))]
    public partial class LoginModel : BaseNopModel
    {
        public bool CheckoutAsGuest { get; set; }

        [NopResourceDisplayName("Account.Login.Fields.Email")]
        	
#if NET451
		[AllowHtml]
#endif
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [NopResourceDisplayName("Account.Login.Fields.UserName")]
        	
#if NET451
		[AllowHtml]
#endif
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [NopResourceDisplayName("Account.Login.Fields.Password")]
        	
#if NET451
		[AllowHtml]
#endif
        public string Password { get; set; }

        [NopResourceDisplayName("Account.Login.Fields.RememberMe")]
        public bool RememberMe { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}