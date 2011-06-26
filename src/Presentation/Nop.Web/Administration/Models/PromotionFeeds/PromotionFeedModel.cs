using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.PromotionFeeds
{
    public class PromotionFeedModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Promotions.Feeds.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [NopResourceDisplayName("Admin.Promotions.Feeds.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}