using System;
using System.Web.Mvc;

using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

using Telerik.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Nop.Admin.Models
{
    public class QueuedEmailListModel : BaseNopModel
    {
        public QueuedEmailListModel()
        {
            SearchLoadNotSent = true;
            SearchMaxSentTries = 10;
        }

        public int QueuedEmailsCount { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.StartDate")]
        [UIHint("Date")]
        public DateTime? SearchStartDate { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.EndDate")]
        [UIHint("Date")]
        public DateTime? SearchEndDate { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.FromEmail")]
        public string SearchFromEmail { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.ToEmail")]
        public string SearchToEmail { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.LoadNotSent")]
        public bool SearchLoadNotSent { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.MaxSentTries")]
        public int SearchMaxSentTries { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedEmails.List.GoDirectlyToNumber")]
        public string GoDirectlyToNumber { get; set; }
    }
}