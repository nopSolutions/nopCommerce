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
                            "p/{productId}/{SeName}",
                            new { controller = "Catalog", action = "Product", SeName = UrlParameter.Optional },
                            new { productId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("RecentlyViewedProducts",
                            "recentlyviewedproducts/",
                            new { controller = "Catalog", action = "RecentlyViewedProducts" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("RecentlyAddedProducts",
                            "newproducts/",
                            new { controller = "Catalog", action = "RecentlyAddedProducts" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("RecentlyAddedProductsRSS",
                            "newproducts/rss",
                            new { controller = "Catalog", action = "RecentlyAddedProductsRss" },
                            new[] { "Nop.Web.Controllers" });
            
            //comparing products
            routes.MapRoute("AddProductToCompare",
                            "compareproducts/add/{productId}",
                            new { controller = "Catalog", action = "AddProductToCompareList" },
                            new { productId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("CompareProducts",
                            "compareproducts/",
                            new { controller = "Catalog", action = "CompareProducts" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("RemoveProductFromCompareList",
                            "compareproducts/remove/{productId}",
                            new { controller = "Catalog", action = "RemoveProductFromCompareList"},
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("ClearCompareList",
                            "clearcomparelist/",
                            new { controller = "Catalog", action = "ClearCompareList" },
                            new[] { "Nop.Web.Controllers" });
            
            //product email a friend
            routes.MapRoute("ProductEmailAFriend",
                            "productemailafriend/{productId}",
                            new { controller = "Catalog", action = "ProductEmailAFriend" },
                            new { productId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });

            //catalog
            routes.MapRoute("Category",
                            "c/{categoryId}/{SeName}",
                            new { controller = "Catalog", action = "Category", SeName = UrlParameter.Optional },
                            new { categoryId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("ManufacturerList",
                            "manufacturer/all/",
                            new { controller = "Catalog", action = "ManufacturerAll" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("Manufacturer",
                            "m/{manufacturerId}/{SeName}",
                            new { controller = "Catalog", action = "Manufacturer", SeName = UrlParameter.Optional },
                            new { manufacturerId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });

            //reviews
            routes.MapRoute("ProductReviews",
                            "productreviews/{productId}",
                            new { controller = "Catalog", action = "ProductReviews" },
                            new[] { "Nop.Web.Controllers" });

            //login, register
            routes.MapRoute("Login",
                            "login/",
                            new { controller = "Customer", action = "Login" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("LoginCheckoutAsGuest",
                            "login/checkoutAsGuest",
                            new { controller = "Customer", action = "Login", checkoutAsGuest = true },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("Register",
                            "register/",
                            new { controller = "Customer", action = "Register" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("Logout",
                            "logout/",
                            new { controller = "Customer", action = "Logout" },
                            new[] { "Nop.Web.Controllers" });

            //shopping cart
            routes.MapRoute("AddProductToCart",
                            "cart/addproduct/{productId}",
                            new { controller = "ShoppingCart", action = "AddProductToCart" },
                            new { productId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("ShoppingCart",
                            "cart/",
                            new { controller = "ShoppingCart", action = "Cart" },
                            new[] { "Nop.Web.Controllers" });
            //wishlist
            routes.MapRoute("Wishlist",
                            "wishlist/{customerGuid}",
                            new { controller = "ShoppingCart", action = "Wishlist", customerGuid = UrlParameter.Optional },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("EmailWishlist",
                            "emailwishlist",
                            new { controller = "ShoppingCart", action = "EmailWishlist" },
                            new[] { "Nop.Web.Controllers" });
            
            //checkout
            routes.MapRoute("Checkout",
                            "checkout/",
                            new { controller = "Checkout", action = "Index" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("CheckoutOnePage",
                            "onepagecheckout/",
                            new { controller = "Checkout", action = "OnePageCheckout" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("CheckoutShippingAddress",
                            "checkout/shippingaddress",
                            new { controller = "Checkout", action = "ShippingAddress" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("CheckoutBillingAddress",
                            "checkout/billingaddress",
                            new { controller = "Checkout", action = "BillingAddress" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("CheckoutShippingMethod",
                            "checkout/shippingmethod",
                            new { controller = "Checkout", action = "ShippingMethod" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("CheckoutPaymentMethod",
                            "checkout/paymentmethod",
                            new { controller = "Checkout", action = "PaymentMethod" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("CheckoutPaymentInfo",
                            "checkout/paymentinfo",
                            new { controller = "Checkout", action = "PaymentInfo" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("CheckoutConfirm",
                            "checkout/confirm",
                            new { controller = "Checkout", action = "Confirm" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("CheckoutCompleted",
                            "checkout/payment",
                            new { controller = "Checkout", action = "Completed" },
                            new[] { "Nop.Web.Controllers" });

            //orders
            routes.MapRoute("OrderDetails",
                            "orderdetails/{orderId}",
                            new { controller = "Order", action = "Details" },
                            new { orderId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("ReturnRequest",
                            "returnrequest/{orderId}",
                            new { controller = "Order", action = "ReturnRequest" },
                            new { orderId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("GetOrderPdfInvoice",
                            "orderdetails/pdf/{orderId}",
                            new { controller = "Order", action = "GetPdfInvoice" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("PrintOrderDetails",
                            "orderdetails/print/{orderId}",
                            new { controller = "Order", action = "PrintOrderDetails" },
                            new[] { "Nop.Web.Controllers" });


            //contact us
            routes.MapRoute("ContactUs",
                            "contactus",
                            new { controller = "Common", action = "ContactUs" },
                            new[] { "Nop.Web.Controllers" });

            //passwordrecovery
            routes.MapRoute("PasswordRecovery",
                            "passwordrecovery",
                            new { controller = "Customer", action = "PasswordRecovery" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("PasswordRecoveryConfirm",
                            "passwordrecovery/confirm/{prt}/{customerEmail}",
                            new { controller = "Customer", action = "PasswordRecoveryConfirm" },
                            new[] { "Nop.Web.Controllers" });

            //newsletters
            routes.MapRoute("NewsletterActivation",
                            "newsletter/subscriptionactivation/{token}/{active}",
                            new { controller = "Newsletter", action = "SubscriptionActivation" },
                            new[] { "Nop.Web.Controllers" });

            //customer
            routes.MapRoute("AccountActivation",
                            "customer/activation/{token}/{email}",
                            new { controller = "Customer", action = "AccountActivation" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("CustomerProfile",
                            "profile/{id}",
                            new { controller = "Profile", action = "Info" },
                            new[] { "Nop.Web.Controllers" });


            //blog
            routes.MapRoute("Blog",
                            "blog",
                            new { controller = "Blog", action = "List" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("BlogRSS",
                            "blog/rss/{languageId}",
                            new { controller = "Blog", action = "ListRss" },
                            new { languageId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("BlogPost",
                            "blog/{blogPostId}/{SeName}",
                            new { controller = "Blog", action = "BlogPost", SeName = UrlParameter.Optional },
                            new { blogPostId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("BlogByTag",
                            "blog/tag/{tag}",
                            new { controller = "Blog", action = "List" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("BlogByMonth",
                            "blog/month/{month}",
                            new { controller = "Blog", action = "List" },
                            new[] { "Nop.Web.Controllers" });


            //news
            routes.MapRoute("NewsArchive",
                            "news",
                            new { controller = "News", action = "List" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("NewsRSS",
                            "news/rss/{languageId}",
                            new { controller = "News", action = "ListRss" },
                            new { languageId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("NewsItem",
                            "news/{newsItemId}/{SeName}",
                            new { controller = "News", action = "NewsItem", SeName = UrlParameter.Optional },
                            new { newsItemId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });

            //topics
            routes.MapRoute("Topic",
                            "t/{SystemName}",
                            new { controller = "Topic", action = "TopicDetails" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("TopicPopup",
                            "t-popup/{SystemName}",
                            new { controller = "Topic", action = "TopicDetailsPopup" },
                            new[] { "Nop.Web.Controllers" });
            //sitemaps
            routes.MapRoute("Sitemap",
                            "sitemap",
                            new { controller = "Common", action = "Sitemap" },
                            new[] { "Nop.Web.Controllers" });
            routes.MapRoute("SitemapSEO",
                            "sitemapseo",
                            new { controller = "Common", action = "SitemapSeo" },
                            new[] { "Nop.Web.Controllers" });

            //product tags
            routes.MapRoute("ProductsByTag",
                            "productag/{productTagId}/{SeName}",
                            new { controller = "Catalog", action = "ProductsByTag", SeName = UrlParameter.Optional },
                            new { productTagId = @"\d+" },
                            new[] { "Nop.Web.Controllers" });
            
            //product search
            routes.MapRoute("ProductSearch",
                            "search/",
                            new { controller = "Catalog", action = "Search" },
                            new[] { "Nop.Web.Controllers" });
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
