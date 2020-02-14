using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.AustraliaPost.Models
{
    public class AustraliaPostShippingModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Shipping.AustraliaPost.Fields.ApiKey")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge")]
        public decimal AdditionalHandlingCharge { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.AustraliaPost.Fields.IgnoreServices")]
        public string IgnoreServices { get; set; }
    }
}