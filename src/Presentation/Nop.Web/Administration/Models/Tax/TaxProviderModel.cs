using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Tax
{
    public partial class TaxProviderModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Configuration.Tax.Providers.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Tax.Providers.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Tax.Providers.Fields.IsPrimaryTaxProvider")]
        public bool IsPrimaryTaxProvider { get; set; }





        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}