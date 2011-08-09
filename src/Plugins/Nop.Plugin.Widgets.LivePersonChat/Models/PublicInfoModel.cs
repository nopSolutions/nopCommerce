using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.LivePersonChat.Models
{
    public class PublicInfoModel : BaseNopModel
    {
        public string ButtonCode { get; set; }
        public string MonitoringCode { get; set; }
    }
}