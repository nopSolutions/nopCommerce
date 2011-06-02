using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Domain.Logging;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Logging
{
    public class LogModel : BaseNopEntityModel
    {
        public int LogLevelId { get; set; }

        [AllowHtml]
        public string Message { get; set; }

        [AllowHtml]
        public string Exception { get; set; }

        [AllowHtml]
        public string IpAddress { get; set; }

        public int CustomerId { get; set; }

        [AllowHtml]
        public string PageUrl { get; set; }

        [AllowHtml]
        public string ReferrerUrl { get; set; }

        public DateTime CreatedOnUtc { get; set; }
    }
}