using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using Nop.Core.Domain.Localization;
using Nop.Web.Framework;
using Nop.Core.Domain.Messages;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models
{
    //[Validator(typeof(LanguageValidator))]
    public class EmailAccountModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.DisplayName")]
        public string DisplayName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Host")]
        public string Host { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Port")]
        public int Port { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Username")]
        public string Username { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.EnableSsl")]
        public bool EnableSsl { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.UseDefaultCredentials")]
        public bool UseDefaultCredentials { get; set; }
    }
}