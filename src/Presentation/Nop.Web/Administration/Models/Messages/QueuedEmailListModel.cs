using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Messages
{
    public partial class QueuedEmailListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.System.QueuedEmails.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchStartDate { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchEndDate { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.FromEmail")]
        [AllowHtml]
        public string SearchFromEmail { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.ToEmail")]
        [AllowHtml]
        public string SearchToEmail { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.LoadNotSent")]
        public bool SearchLoadNotSent { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.MaxSentTries")]
        public int SearchMaxSentTries { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.GoDirectlyToNumber")]
        public int GoDirectlyToNumber { get; set; }
    }
}