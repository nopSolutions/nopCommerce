using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Models
{
    [Validator(typeof(UserValidator))]
    public class UserModel : BaseNopEntityModel
    {
        public bool AllowUsersToChangeUsernames { get; set; }
        public bool UsernamesEnabled { get; set; }

        [NopResourceDisplayName("Admin.Users.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }
        [NopResourceDisplayName("Admin.Users.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }
        [NopResourceDisplayName("Admin.Users.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }
        [NopResourceDisplayName("Admin.Users.Fields.SecurityQuestion")]
        [AllowHtml]
        public string SecurityQuestion { get; set; }
        [NopResourceDisplayName("Admin.Users.Fields.SecurityAnswer")]
        [AllowHtml]
        public string SecurityAnswer { get; set; }
        [NopResourceDisplayName("Admin.Users.Fields.Comments")]
        [AllowHtml]
        public string Comments { get; set; }
        [NopResourceDisplayName("Admin.Users.Fields.IsApproved")]
        public bool IsApproved { get; set; }
        [NopResourceDisplayName("Admin.Users.Fields.IsLockedOut")]
        public bool IsLockedOut { get; set; }
        [NopResourceDisplayName("Admin.Users.Fields.CreatedOn")]
        public DateTime CreatedOnUtc { get; set; }
        [NopResourceDisplayName("Admin.Users.Fields.CreatedOn")]
        public string CreatedOnStr { get; set; }
    }
}