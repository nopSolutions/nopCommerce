#if NET451
using System.Web.Routing;
#endif
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Models.Checkout
{
    public partial class CheckoutPaymentInfoModel : BaseNopModel
    {
        public string PaymentInfoActionName { get; set; }
        public string PaymentInfoControllerName { get; set; }
#if NET451
        public RouteValueDictionary PaymentInfoRouteValues { get; set; }
#endif

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool DisplayOrderTotals { get; set; }
    }
}