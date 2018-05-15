using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Worldpay.Models.Customer
{
    public class WorldpayCustomerModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Payments.Worldpay.Fields.WorldpayCustomerId")]
        public string WorldpayCustomerId { get; set; }

        public bool CustomerExists { get; set; }
    }
}