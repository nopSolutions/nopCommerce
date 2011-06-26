using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Checkout
{
    public class CheckoutCompletedModel : BaseNopModel
    {
        public int OrderId { get; set; }
    }
}