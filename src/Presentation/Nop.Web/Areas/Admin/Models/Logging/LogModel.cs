using System;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Logging
{
    public partial class LogModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.System.Log.Fields.LogLevel")]
        public string LogLevel { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.ShortMessage")]
        public string ShortMessage { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.FullMessage")]
        public string FullMessage { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.IPAddress")]
        public string IpAddress { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.Customer")]
        public int? CustomerId { get; set; }
        [NopResourceDisplayName("Admin.System.Log.Fields.Customer")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.PageURL")]
        public string PageUrl { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.ReferrerURL")]
        public string ReferrerUrl { get; set; }

        [NopResourceDisplayName("Admin.System.Log.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}