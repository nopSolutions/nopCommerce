using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Misc.AbcCore.Infrastructure
{
    public class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        public int Priority
        {
            get
            {
                // negative to make sure this is the last route provider applied
                return -int.MaxValue;
            }
        }

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("AbcPromoProductList",
                            "Admin/AbcPromo/Products/{abcPromoId}",
                            new { controller = "AbcPromo", action = "Products", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute("CustomProductEdit",
                            "Admin/Product/Edit/{id}",
                            new { controller = "CustomProduct", action = "Edit", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute("CustomProductEdit",
                            "Admin/Product/Edit",
                            new { controller = "CustomProduct", action = "Edit", area = "Admin" });

            // Add to Cart Slideout
            endpointRouteBuilder.MapControllerRoute("CartSlideout_GetDeliveryOptions",
                            "AddToCart/GetDeliveryOptions",
                            new { controller = "CartSlideout", action = "GetDeliveryOptions"});
            endpointRouteBuilder.MapControllerRoute("CartSlideout_GetEditCartItemInfo",
                            "AddToCart/GetEditCartItemInfo",
                            new { controller = "CartSlideout", action = "GetEditCartItemInfo"});

            endpointRouteBuilder.MapControllerRoute("GetProductAttributeValue",
                            "api/ProductAttributeValue/{productAttributeValueId}",
                            new { controller = "Api", action = "GetProductAttributeValue"});
            endpointRouteBuilder.MapControllerRoute("CartSlideout_GetAddCartItemInfo",
                            "AddToCart/GetAddCartItemInfo",
                            new { controller = "CartSlideout", action = "GetAddCartItemInfo"});

            // generic routes
            var lang = GetLanguageRoutePattern();
            var genericPattern = $"{lang}/{{SeName}}";

            endpointRouteBuilder.MapControllerRoute(name: "AbcProduct",
                pattern: genericPattern,
                defaults: new { controller = "AbcProduct", action = "ProductDetails" });
        }
    }
}
