using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.LiveChat.LivePerson.Models
{
    public class PublicInfoModel : BaseNopModel
    {
        public string ButtonCode { get; set; }
        public string MonitoringCode { get; set; }
    }
}