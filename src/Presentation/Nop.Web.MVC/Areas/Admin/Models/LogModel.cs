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
        public LogModel(){}

        public LogModel(Log log)
        {
            Id = log.Id;
            LogLevelId = log.LogLevelId;
            Message = log.Message;
            Exception = log.Exception;
            IpAddress = log.IpAddress;
            CustomerId = log.CustomerId;
            PageUrl = log.PageUrl;
            ReferrerUrl = log.ReferrerUrl;
            CreatedOnUtc = log.CreatedOnUtc;
        }

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