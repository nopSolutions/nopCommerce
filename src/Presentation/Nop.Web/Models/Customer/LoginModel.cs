using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public partial class LoginModel : BaseNopModel
    {
        public bool CheckoutAsGuest { get; set; }

        [NopResourceDisplayName("Account.Login.Fields.Email")]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [NopResourceDisplayName("Account.Login.Fields.UserName")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [NopResourceDisplayName("Account.Login.Fields.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("Account.Login.Fields.RememberMe")]
        public bool RememberMe { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}