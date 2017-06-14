using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Shipping.CanadaPost.Models
{
    public class CanadaPostShippingModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.CustomerNumber")]
        public string CustomerNumber { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.ContractId")]
        public string ContractId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.Api")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.CanadaPost.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
    }
}