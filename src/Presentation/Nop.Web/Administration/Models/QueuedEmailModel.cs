using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Admin.Models
{
    public class QueuedEmailModel: BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.Id")]
        public override int Id { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.Priority")]
        public int Priority { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.From")]
        public string From { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.FromName")]
        public string FromName { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.To")]
        public string To { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.ToName")]
        public string ToName { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.CC")]
        public string CC { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.Bcc")]
        public string Bcc { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.Subject")]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.SentTries")]
        public int SentTries { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.SentOnUtc")]
        [DisplayFormatAttribute(DataFormatString="Sent on {0}", NullDisplayText="Not sent yet")]
        public DateTime? SentOnUtc { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.Fields.EmailAccountName")]
        public string EmailAccountName { get; set; }


    }
}