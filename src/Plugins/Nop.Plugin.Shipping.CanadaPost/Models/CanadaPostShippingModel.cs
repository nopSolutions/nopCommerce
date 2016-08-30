using Nop.Web.Framework;

namespace Nop.Plugin.Shipping.CanadaPost.Models
{
    public class CanadaPostShippingModel
    {
        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.CustomerNumber")]
        public string CustomerNumber { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.Api")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
    }
}