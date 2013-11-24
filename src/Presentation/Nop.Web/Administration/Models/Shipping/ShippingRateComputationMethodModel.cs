using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Shipping
{
    public partial class ShippingRateComputationMethodModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.IsActive")]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.Logo")]
        public string LogoUrl { get; set; }
        





        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}