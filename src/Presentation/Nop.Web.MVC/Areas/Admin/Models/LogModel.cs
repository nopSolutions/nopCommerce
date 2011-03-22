using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Logging;
using Nop.Web.Framework;

namespace Nop.Web.MVC.Areas.Admin.Models
{
    public class LogModel : BaseNopEntityModel
    {
        public int LogLevelId { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string IpAddress { get; set; }
        public int CustomerId { get; set; }
        public string PageUrl { get; set; }
        public string ReferrerUrl { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}