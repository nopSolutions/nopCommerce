using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.AbcFrontend.Infrastructure
{
    public class RouteProvider : IRouteProvider
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
            endpointRouteBuilder.MapControllerRoute("CustomCheckoutShippingMethod",
                            "checkout/shippingmethod",
                            new { controller = "CustomCheckout", action = "ShippingMethod" });

            endpointRouteBuilder.MapControllerRoute("CustomCheckoutPaymentMethod",
                            "checkout/paymentmethod",
                            new { controller = "CustomCheckout", action = "PaymentMethod" });

            endpointRouteBuilder.MapControllerRoute("CustomCheckoutPaymentInfo",
                            "checkout/paymentinfo",
                            new { controller = "CustomCheckout", action = "PaymentInfo" });

            endpointRouteBuilder.MapControllerRoute("CustomCheckoutConfirm",
                            "checkout/confirm",
                            new { controller = "CustomCheckout", action = "Confirm" });

            endpointRouteBuilder.MapControllerRoute("WarrantySelection",
                            "checkout/warranty",
                            new { controller = "CustomCheckout", action = "WarrantySelection" });

            endpointRouteBuilder.MapControllerRoute("CustomCheckoutOnePage",
                            "onepagecheckout/",
                            new { controller = "CustomCheckout", action = "OnePageCheckout" });

            endpointRouteBuilder.MapControllerRoute("ShoppingCartRemoveItem",
                            "ShoppingCart/RemoveItem",
                            new { controller = "CustomShoppingCart", action = "RemoveItem" });

            endpointRouteBuilder.MapControllerRoute("AddProductToCart-Pickup",
                            "addproducttocart/pickup/{productId}/{shoppingCartTypeId}",
                            new { controller = "CustomShoppingCart", action = "AddProductToCart_Pickup" },
                            new { productId = @"\d+", shoppingCartTypeId = @"\d+" });

            endpointRouteBuilder.MapControllerRoute("CustomAddProductToCart-Catalog",
                            "addproducttocart/catalog/{productId}/{shoppingCartTypeId}/{quantity}",
                            new { controller = "CustomShoppingCart", action = "AddProductToCart_Catalog" },
                            new { productId = @"\d+", shoppingCartTypeId = @"\d+", quantity = @"\d+" });

            endpointRouteBuilder.MapControllerRoute("CustomAddProductToCart-Details",
                            "addproducttocart/details/{productId}/{shoppingCartTypeId}",
                            new { controller = "CustomShoppingCart", action = "AddProductToCart_Details" },
                            new { productId = @"\d+", shoppingCartTypeId = @"\d+" });
        }
    }
}
