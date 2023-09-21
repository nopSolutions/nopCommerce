using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace AbcWarehouse.Plugin.Payments.UniFi
{
    public partial class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return int.MaxValue;
            }
        }

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("UniFiTransactionLookup",
                 "checkout/UniFi/TransactionLookup/{transactionToken}",
                 new { controller = "UniFiPayments", action = "TransactionLookup" }
            );
        }
    }
}
