using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public class LoginModel : BaseNopModel
    {
        public bool UsernamesEnabled { get; set; }

        //TODO [Required()]
        //TODO localize
        [DisplayName("Email")]
        public string Email { get; set; }

        //TODO [Required]
        //TODO localize
        [DisplayName("User name")]
        public string UserName { get; set; }

        //TODO [Required]
        //TODO localize
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        //TODO localize
        [DisplayName("Remember me?")]
        public bool RememberMe { get; set; }
    }
}