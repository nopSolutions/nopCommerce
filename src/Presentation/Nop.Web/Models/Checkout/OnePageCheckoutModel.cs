using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Checkout
{
    public class OnePageCheckoutModel : BaseNopModel
    {
        public bool ShippingRequired { get; set; }
    }
}