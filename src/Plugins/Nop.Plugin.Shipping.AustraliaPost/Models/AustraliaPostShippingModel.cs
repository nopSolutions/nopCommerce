using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Shipping.AustraliaPost.Models
{
    public class AustraliaPostShippingModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Shipping.AustraliaPost.Fields.ApiKey")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge")]
        public decimal AdditionalHandlingCharge { get; set; }
    }
}