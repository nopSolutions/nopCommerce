using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Pos.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {


        private string GetLanguageRoutePattern()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //this pattern is set once at the application start, when we don't have the selected language yet
                    //so we use 'en' by default for the language value, later it'll be replaced with the working language code
                    var code = "en";
                    return $"{{{NopRoutingDefaults.RouteValue.Language}:maxlength(2):{NopRoutingDefaults.LanguageParameterTransformer}={code}}}";
                }
            }

            return string.Empty;
        }

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Plugin.Pos.Index", "pos", new { controller = "Pos", action = "Index" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Pos.Neworder", "pos/neworder", new { controller = "Pos", action = "Neworder" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Pos.Orderlist", "pos/OrderList", new { controller = "Pos", action = "OrderList" });
            endpointRouteBuilder.MapControllerRoute("Details", "pos/Details", new { controller = "Pos", action = "Details" });


            //CheckoutPos Pages

            //var lang = GetLanguageRoutePattern();

         

            var lang = GetLanguageRoutePattern();

            endpointRouteBuilder.MapControllerRoute(name: "Checkout",
               pattern: $"{lang}/checkoutpos/",
                defaults: new { controller = "CheckoutPos", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutOnePagePos",
                pattern: $"{lang}/checkoutpos/onepagecheckout/",
                defaults: new { controller = "CheckoutPos", action = "OnePageCheckout" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutShippingAddressPos",
                pattern: $"{lang}/checkoutpos/shippingaddress",
                defaults: new { controller = "CheckoutPos", action = "ShippingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutSelectShippingAddressPos",
                pattern: $"{lang}/checkoutpos/selectshippingaddress",
                defaults: new { controller = "CheckoutPos", action = "SelectShippingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutBillingAddressPos",
                pattern: $"{lang}/checkoutpos/billingaddress",
                defaults: new { controller = "CheckoutPos", action = "BillingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutSelectBillingAddressPos",
                pattern: $"{lang}/checkoutpos/selectbillingaddress",
                defaults: new { controller = "CheckoutPos", action = "SelectBillingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutShippingMethodPos",
                pattern: $"{lang}/checkoutpos/shippingmethod",
                defaults: new { controller = "CheckoutPos", action = "ShippingMethod" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutPaymentMethodPos",
                pattern: $"{lang}/checkoutpos/paymentmethod",
                defaults: new { controller = "CheckoutPos", action = "PaymentMethod" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutPaymentInfoPos",
                pattern: $"{lang}/checkoutpos/paymentinfo",
                defaults: new { controller = "CheckoutPos", action = "PaymentInfo" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutConfirmPos",
                pattern: $"{lang}/checkoutpos/confirm",
                defaults: new { controller = "CheckoutPos", action = "Confirm" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutCompletedPos",
                pattern: $"{lang}/checkoutpos/completed/{{orderId:int?}}",
                defaults: new { controller = "CheckoutPos", action = "Completed" });



        }

        public int Priority => 0;
    }
}

