using Nop.Web.Framework;

namespace Nop.Plugin.Shipping.AustraliaPost.Models
{
    public class AustraliaPostShippingModel
    {
        [NopResourceDisplayName("Plugins.Shipping.AustraliaPost.Fields.ApiKey")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge")]
        public decimal AdditionalHandlingCharge { get; set; }
    }
}