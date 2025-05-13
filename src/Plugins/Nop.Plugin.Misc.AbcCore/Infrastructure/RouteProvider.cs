using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.AbcCore.Infrastructure
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
            // AbcCategory - get
            endpointRouteBuilder.MapControllerRoute("AbcCategoryEdit",
                            "Admin/Category/Edit/{id}",
                            new { controller = "AbcCategory", action = "Edit", area = "Admin" });

            // AbcCategory - post
            endpointRouteBuilder.MapControllerRoute("AbcCategoryEdit",
                            "Admin/Category/Edit/",
                            new { controller = "AbcCategory", action = "Edit", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute("AbcPromoProductList",
                            "Admin/AbcPromo/Products/{abcPromoId}",
                            new { controller = "AbcPromo", action = "Products", area = "Admin" });

            // Need to determine a way to allow custom category/product to exist
            // endpointRouteBuilder.MapControllerRoute("CustomProductEdit",
            //                 "Admin/Product/Edit/{id}",
            //                 new { controller = "CustomProduct", action = "Edit", area = "Admin" });

            // endpointRouteBuilder.MapControllerRoute("CustomProductEdit",
            //                 "Admin/Product/Edit",
            //                 new { controller = "CustomProduct", action = "Edit", area = "Admin" });

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
        }
    }
}
