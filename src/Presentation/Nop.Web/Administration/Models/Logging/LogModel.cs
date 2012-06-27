using System;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Logging
{
    public partial class LogModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.System.Log.Fields.LogLevel")]
        public string LogLevel { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.ShortMessage")]
        [AllowHtml]
        public string ShortMessage { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.FullMessage")]
        [AllowHtml]
        public string FullMessage { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.IPAddress")]
        [AllowHtml]
        public string IpAddress { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.Customer")]
        public int? CustomerId { get; set; }
        [NopResourceDisplayName("Admin.System.Log.Fields.Customer")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.PageURL")]
        [AllowHtml]
        public string PageUrl { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.ReferrerURL")]
        [AllowHtml]
        public string ReferrerUrl { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}