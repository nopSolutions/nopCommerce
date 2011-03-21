using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using Nop.Core.Domain.Localization;
using Nop.Web.Framework;
using Nop.Web.MVC.Areas.Admin.Validators;
using Nop.Core.Domain.Messages;

namespace Nop.Web.MVC.Areas.Admin.Models
{
    //[Validator(typeof(LanguageValidator))]
    public class EmailAccountModel : BaseNopEntityModel
    {
        public EmailAccountModel()
        {
        }

        public EmailAccountModel(EmailAccount emailAccount)
            :this()
        {
            Id = emailAccount.Id;
            Email = emailAccount.Email;
            DisplayName = emailAccount.DisplayName;
            Host = emailAccount.Host;
            Port = emailAccount.Port;
            Username = emailAccount.Username;
            Password = emailAccount.Password;
            EnableSsl = emailAccount.EnableSsl;
            UseDefaultCredentials = emailAccount.UseDefaultCredentials;
        }

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