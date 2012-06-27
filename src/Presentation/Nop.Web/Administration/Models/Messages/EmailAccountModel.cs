using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Messages
{
    [Validator(typeof(EmailAccountValidator))]
    public partial class EmailAccountModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.DisplayName")]
        [AllowHtml]
        public string DisplayName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Host")]
        [AllowHtml]
        public string Host { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Port")]
        public int Port { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.EnableSsl")]
        public bool EnableSsl { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.UseDefaultCredentials")]
        public bool UseDefaultCredentials { get; set; }

        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.IsDefaultEmailAccount")]
        public bool IsDefaultEmailAccount { get; set; }


        [NopResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.SendTestEmailTo")]
        [AllowHtml]
        public string SendTestEmailTo { get; set; }

    }
}