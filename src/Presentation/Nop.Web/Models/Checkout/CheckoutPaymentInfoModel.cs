using System.Web.Routing;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Checkout
{
    public class CheckoutPaymentInfoModel : BaseNopModel
    {
        public string PaymentInfoActionName { get; set; }
        public string PaymentInfoControllerName { get; set; }
        public RouteValueDictionary PaymentInfoRouteValues { get; set; }
    }
}