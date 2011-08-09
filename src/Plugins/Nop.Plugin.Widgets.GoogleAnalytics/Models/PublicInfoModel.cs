using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Models
{
    public class PublicInfoModel : BaseNopModel
    {
        public string GoogleId { get; set; }
        public string JavaScript { get; set; }
    }
}