using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public class LoginModel : BaseNopModel
    {
        //TODO localize attribute
        //TODO add validation rules
        
        [DisplayName("Email")]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [DisplayName("User name")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Remember me?")]
        public bool RememberMe { get; set; }
    }
}