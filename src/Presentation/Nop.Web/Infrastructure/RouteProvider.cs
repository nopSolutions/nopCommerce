using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Web.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //products
            routes.MapRoute("Product",
                            "product/{productId}/{SeName}",
                            new { controller = "Catalog", action = "Product", SeName = UrlParameter.Optional });

            //catalog
            routes.MapRoute("Category",
                            "category/{categoryId}/{SeName}",
                            new { controller = "Catalog", action = "Category", SeName = UrlParameter.Optional });
            routes.MapRoute("ManufacturerList",
                            "manufacturer/all/",
                            new { controller = "Catalog", action = "ManufacturerAll"});
            routes.MapRoute("Manufacturer",
                            "manufacturer/{manufacturerId}/{SeName}",
                            new { controller = "Catalog", action = "Manufacturer", SeName = UrlParameter.Optional });

            //reviews
            routes.MapRoute("ProductReviews",
                            "productreviews/{productId}",
                            new { controller = "Catalog", action = "ProductReviews" });

            //shopping cart
            routes.MapRoute("AddProductToCart",
                            "cart/addproduct/{productId}",
                            new { controller = "ShoppingCart", action = "AddProductToCart" });
            routes.MapRoute("ShoppingCart",
                            "cart/",
                            new { controller = "ShoppingCart", action = "Cart" });
            //wishlist
            routes.MapRoute("Wishlist",
                            "wishlist/",
                            new { controller = "ShoppingCart", action = "Wishlist" });
            routes.MapRoute("EmailWishlist",
                            "wishlist/email",
                            new { controller = "ShoppingCart", action = "EmailWishlist" });

            //checkout
            routes.MapRoute("Checkout",
                            "checkout/",
                            new { controller = "Checkout", action = "Index" });
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
