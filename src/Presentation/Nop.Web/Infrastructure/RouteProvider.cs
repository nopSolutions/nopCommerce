using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Web.Infrastructure
{
    /// <summary>
    /// Represents provider that provided basic routes
    /// </summary>
    public partial class RouteProvider : IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var pattern = string.Empty;
            if (DataSettingsManager.DatabaseIsInstalled)
            {
                var localizationSettings = endpointRouteBuilder.ServiceProvider.GetRequiredService<LocalizationSettings>();
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    var langservice = endpointRouteBuilder.ServiceProvider.GetRequiredService<ILanguageService>();
                    var languages = langservice.GetAllLanguages().ToList();
                    pattern = "{language:lang=" + languages.FirstOrDefault().UniqueSeoCode + "}/";
                }
            }

            //areas
            endpointRouteBuilder.MapControllerRoute(name: "areaRoute",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            //home page
            endpointRouteBuilder.MapControllerRoute("Homepage", pattern,
                new { controller = "Home", action = "Index" });

            //login
            endpointRouteBuilder.MapControllerRoute("Login", $"{pattern}login/",
                new { controller = "Customer", action = "Login" });

            //register
            endpointRouteBuilder.MapControllerRoute("Register", $"{pattern}register/",
                new { controller = "Customer", action = "Register" });

            //logout
            endpointRouteBuilder.MapControllerRoute("Logout", $"{pattern}logout/",
                new { controller = "Customer", action = "Logout" });

            //shopping cart
            endpointRouteBuilder.MapControllerRoute("ShoppingCart", $"{pattern}cart/",
                new { controller = "ShoppingCart", action = "Cart" });

            //estimate shipping
            endpointRouteBuilder.MapControllerRoute("EstimateShipping", $"{pattern}cart/estimateshipping",
                new { controller = "ShoppingCart", action = "GetEstimateShipping" });

            //wishlist
            endpointRouteBuilder.MapControllerRoute("Wishlist", pattern + "wishlist/{customerGuid?}",
                new { controller = "ShoppingCart", action = "Wishlist" });

            //customer account links
            endpointRouteBuilder.MapControllerRoute("CustomerInfo", $"{pattern}customer/info",
                new { controller = "Customer", action = "Info" });

            endpointRouteBuilder.MapControllerRoute("CustomerAddresses", $"{pattern}customer/addresses",
                new { controller = "Customer", action = "Addresses" });

            endpointRouteBuilder.MapControllerRoute("CustomerOrders", $"{pattern}order/history",
                new { controller = "Order", action = "CustomerOrders" });

            //contact us
            endpointRouteBuilder.MapControllerRoute("ContactUs", $"{pattern}contactus",
                new { controller = "Common", action = "ContactUs" });

            //sitemap
            endpointRouteBuilder.MapControllerRoute("Sitemap", $"{pattern}sitemap",
                new { controller = "Common", action = "Sitemap" });

            //product search
            endpointRouteBuilder.MapControllerRoute("ProductSearch", $"{pattern}search/",
                new { controller = "Catalog", action = "Search" });

            endpointRouteBuilder.MapControllerRoute("ProductSearchAutoComplete", $"{pattern}catalog/searchtermautocomplete",
                new { controller = "Catalog", action = "SearchTermAutoComplete" });

            //change currency (AJAX link)
            endpointRouteBuilder.MapControllerRoute("ChangeCurrency", pattern + "changecurrency/{customercurrency:min(0)}",
                new { controller = "Common", action = "SetCurrency" });

            //change language (AJAX link)
            endpointRouteBuilder.MapControllerRoute("ChangeLanguage", pattern + "changelanguage/{langid:min(0)}",
                new { controller = "Common", action = "SetLanguage" });

            //change tax (AJAX link)
            endpointRouteBuilder.MapControllerRoute("ChangeTaxType", pattern + "changetaxtype/{customertaxtype:min(0)}",
                new { controller = "Common", action = "SetTaxType" });

            //recently viewed products
            endpointRouteBuilder.MapControllerRoute("RecentlyViewedProducts", $"{pattern}recentlyviewedproducts/",
                new { controller = "Product", action = "RecentlyViewedProducts" });

            //new products
            endpointRouteBuilder.MapControllerRoute("NewProducts", $"{pattern}newproducts/",
                new { controller = "Product", action = "NewProducts" });

            //blog
            endpointRouteBuilder.MapControllerRoute("Blog", $"{pattern}blog",
                new { controller = "Blog", action = "List" });

            //news
            endpointRouteBuilder.MapControllerRoute("NewsArchive", $"{pattern}news",
                new { controller = "News", action = "List" });

            //forum
            endpointRouteBuilder.MapControllerRoute("Boards", $"{pattern}boards",
                new { controller = "Boards", action = "Index" });

            //compare products
            endpointRouteBuilder.MapControllerRoute("CompareProducts", $"{pattern}compareproducts/",
                new { controller = "Product", action = "CompareProducts" });

            //product tags
            endpointRouteBuilder.MapControllerRoute("ProductTagsAll", $"{pattern}producttag/all/",
                new { controller = "Catalog", action = "ProductTagsAll" });

            //manufacturers
            endpointRouteBuilder.MapControllerRoute("ManufacturerList", $"{pattern}manufacturer/all/",
                new { controller = "Catalog", action = "ManufacturerAll" });

            //vendors
            endpointRouteBuilder.MapControllerRoute("VendorList", $"{pattern}vendor/all/",
                new { controller = "Catalog", action = "VendorAll" });

            //add product to cart (without any attributes and options). used on catalog pages.
            endpointRouteBuilder.MapControllerRoute("AddProductToCart-Catalog",
                pattern + "addproducttocart/catalog/{productId:min(0)}/{shoppingCartTypeId:min(0)}/{quantity:min(0)}",
                new { controller = "ShoppingCart", action = "AddProductToCart_Catalog" });

            //add product to cart (with attributes and options). used on the product details pages.
            endpointRouteBuilder.MapControllerRoute("AddProductToCart-Details",
                pattern + "addproducttocart/details/{productId:min(0)}/{shoppingCartTypeId:min(0)}",
                new { controller = "ShoppingCart", action = "AddProductToCart_Details" });

            //comparing products
            endpointRouteBuilder.MapControllerRoute("AddProductToCompare", "compareproducts/add/{productId:min(0)}",
                new { controller = "Product", action = "AddProductToCompareList" });

            //product email a friend
            endpointRouteBuilder.MapControllerRoute("ProductEmailAFriend",
                pattern + "productemailafriend/{productId:min(0)}",
                new { controller = "Product", action = "ProductEmailAFriend" });

            //reviews
            endpointRouteBuilder.MapControllerRoute("ProductReviews2", pattern + "productreviews/{productId2}",
                new { controller = "Product", action = "ProductReviews2" });

            endpointRouteBuilder.MapControllerRoute("ProductReviews", pattern + "productreviews/{productId}",
                new { controller = "Product", action = "ProductReviews" });

            endpointRouteBuilder.MapControllerRoute("CustomerProductReviews", $"{pattern}customer/productreviews",
                new { controller = "Product", action = "CustomerProductReviews" });

            endpointRouteBuilder.MapControllerRoute("CustomerProductReviewsPaged",
                pattern + "customer/productreviews/page/{pageNumber:min(0)}",
                new { controller = "Product", action = "CustomerProductReviews" });

            //back in stock notifications
            endpointRouteBuilder.MapControllerRoute("BackInStockSubscribePopup",
                pattern + "backinstocksubscribe/{productId:min(0)}",
                new { controller = "BackInStockSubscription", action = "SubscribePopup" });

            endpointRouteBuilder.MapControllerRoute("BackInStockSubscribeSend",
                pattern + "backinstocksubscribesend/{productId:min(0)}",
                new { controller = "BackInStockSubscription", action = "SubscribePopupPOST" });

            //downloads
            endpointRouteBuilder.MapControllerRoute("GetSampleDownload",
                pattern + "download/sample/{productid:min(0)}",
                new { controller = "Download", action = "Sample" });

            //checkout pages
            endpointRouteBuilder.MapControllerRoute("Checkout", $"{pattern}checkout/",
                new { controller = "Checkout", action = "Index" });

            endpointRouteBuilder.MapControllerRoute("CheckoutOnePage", $"{pattern}onepagecheckout/",
                new { controller = "Checkout", action = "OnePageCheckout" });

            endpointRouteBuilder.MapControllerRoute("CheckoutShippingAddress", $"{pattern}checkout/shippingaddress",
                new { controller = "Checkout", action = "ShippingAddress" });

            endpointRouteBuilder.MapControllerRoute("CheckoutSelectShippingAddress", $"{pattern}checkout/selectshippingaddress",
                new { controller = "Checkout", action = "SelectShippingAddress" });

            endpointRouteBuilder.MapControllerRoute("CheckoutBillingAddress", $"{pattern}checkout/billingaddress",
                new { controller = "Checkout", action = "BillingAddress" });

            endpointRouteBuilder.MapControllerRoute("CheckoutSelectBillingAddress", $"{pattern}checkout/selectbillingaddress",
                new { controller = "Checkout", action = "SelectBillingAddress" });

            endpointRouteBuilder.MapControllerRoute("CheckoutShippingMethod", $"{pattern}checkout/shippingmethod",
                new { controller = "Checkout", action = "ShippingMethod" });

            endpointRouteBuilder.MapControllerRoute("CheckoutPaymentMethod", $"{pattern}checkout/paymentmethod",
                new { controller = "Checkout", action = "PaymentMethod" });

            endpointRouteBuilder.MapControllerRoute("CheckoutPaymentInfo", $"{pattern}checkout/paymentinfo",
                new { controller = "Checkout", action = "PaymentInfo" });

            endpointRouteBuilder.MapControllerRoute("CheckoutConfirm", $"{pattern}checkout/confirm",
                new { controller = "Checkout", action = "Confirm" });

            endpointRouteBuilder.MapControllerRoute("CheckoutCompleted",
                pattern + "checkout/completed/{orderId:int}",
                new { controller = "Checkout", action = "Completed" });

            //subscribe newsletters
            endpointRouteBuilder.MapControllerRoute("SubscribeNewsletter", $"{pattern}subscribenewsletter",
                new { controller = "Newsletter", action = "SubscribeNewsletter" });

            //email wishlist
            endpointRouteBuilder.MapControllerRoute("EmailWishlist", $"{pattern}emailwishlist",
                new { controller = "ShoppingCart", action = "EmailWishlist" });

            //login page for checkout as guest
            endpointRouteBuilder.MapControllerRoute("LoginCheckoutAsGuest", $"{pattern}login/checkoutasguest",
                new { controller = "Customer", action = "Login", checkoutAsGuest = true });

            //register result page
            endpointRouteBuilder.MapControllerRoute("RegisterResult",
                pattern + "registerresult/{resultId:min(0)}",
                new { controller = "Customer", action = "RegisterResult" });

            //check username availability
            endpointRouteBuilder.MapControllerRoute("CheckUsernameAvailability", $"{pattern}customer/checkusernameavailability",
                new { controller = "Customer", action = "CheckUsernameAvailability" });

            //passwordrecovery
            endpointRouteBuilder.MapControllerRoute("PasswordRecovery", $"{pattern}passwordrecovery",
                new { controller = "Customer", action = "PasswordRecovery" });

            //password recovery confirmation
            endpointRouteBuilder.MapControllerRoute("PasswordRecoveryConfirm", $"{pattern}passwordrecovery/confirm",
                new { controller = "Customer", action = "PasswordRecoveryConfirm" });

            //topics
            endpointRouteBuilder.MapControllerRoute("TopicPopup",
                pattern + "t-popup/{SystemName}",
                new { controller = "Topic", action = "TopicDetailsPopup" });

            //blog
            endpointRouteBuilder.MapControllerRoute("BlogByTag",
                pattern + "blog/tag/{tag}",
                new { controller = "Blog", action = "BlogByTag" });

            endpointRouteBuilder.MapControllerRoute("BlogByMonth",
                pattern + "blog/month/{month}",
                new { controller = "Blog", action = "BlogByMonth" });

            //blog RSS
            endpointRouteBuilder.MapControllerRoute("BlogRSS",
                pattern + "blog/rss/{languageId:min(0)}",
                new { controller = "Blog", action = "ListRss" });

            //news RSS
            endpointRouteBuilder.MapControllerRoute("NewsRSS",
                pattern + "news/rss/{languageId:min(0)}",
                new { controller = "News", action = "ListRss" });

            //set review helpfulness (AJAX link)
            endpointRouteBuilder.MapControllerRoute("SetProductReviewHelpfulness", $"{pattern}setproductreviewhelpfulness",
                new { controller = "Product", action = "SetProductReviewHelpfulness" });

            //customer account links
            endpointRouteBuilder.MapControllerRoute("CustomerReturnRequests", $"{pattern}returnrequest/history",
                new { controller = "ReturnRequest", action = "CustomerReturnRequests" });

            endpointRouteBuilder.MapControllerRoute("CustomerDownloadableProducts", $"{pattern}customer/downloadableproducts",
                new { controller = "Customer", action = "DownloadableProducts" });

            endpointRouteBuilder.MapControllerRoute("CustomerBackInStockSubscriptions",
                pattern + "backinstocksubscriptions/manage/{pageNumber:int?}",
                new { controller = "BackInStockSubscription", action = "CustomerSubscriptions" });

            endpointRouteBuilder.MapControllerRoute("CustomerRewardPoints", $"{pattern}rewardpoints/history",
                new { controller = "Order", action = "CustomerRewardPoints" });

            endpointRouteBuilder.MapControllerRoute("CustomerRewardPointsPaged",
                pattern + "rewardpoints/history/page/{pageNumber:min(0)}",
                new { controller = "Order", action = "CustomerRewardPoints" });

            endpointRouteBuilder.MapControllerRoute("CustomerChangePassword", $"{pattern}customer/changepassword",
                new { controller = "Customer", action = "ChangePassword" });

            endpointRouteBuilder.MapControllerRoute("CustomerAvatar", $"{pattern}customer/avatar",
                new { controller = "Customer", action = "Avatar" });

            endpointRouteBuilder.MapControllerRoute("AccountActivation", $"{pattern}customer/activation",
                new { controller = "Customer", action = "AccountActivation" });

            endpointRouteBuilder.MapControllerRoute("EmailRevalidation", $"{pattern}customer/revalidateemail",
                new { controller = "Customer", action = "EmailRevalidation" });

            endpointRouteBuilder.MapControllerRoute("CustomerForumSubscriptions",
                pattern + "boards/forumsubscriptions/{pageNumber:int?}",
                new { controller = "Boards", action = "CustomerForumSubscriptions" });

            endpointRouteBuilder.MapControllerRoute("CustomerAddressEdit",
                pattern + "customer/addressedit/{addressId:min(0)}",
                new { controller = "Customer", action = "AddressEdit" });

            endpointRouteBuilder.MapControllerRoute("CustomerAddressAdd", $"{pattern}customer/addressadd",
                new { controller = "Customer", action = "AddressAdd" });

            //customer profile page
            endpointRouteBuilder.MapControllerRoute("CustomerProfile",
                pattern + "profile/{id:min(0)}",
                new { controller = "Profile", action = "Index" });

            endpointRouteBuilder.MapControllerRoute("CustomerProfilePaged",
                pattern + "profile/{id:min(0)}/page/{pageNumber:min(0)}",
                new { controller = "Profile", action = "Index" });

            //orders
            endpointRouteBuilder.MapControllerRoute("OrderDetails",
                pattern + "orderdetails/{orderId:min(0)}",
                new { controller = "Order", action = "Details" });

            endpointRouteBuilder.MapControllerRoute("ShipmentDetails",
                pattern + "orderdetails/shipment/{shipmentId}",
                new { controller = "Order", action = "ShipmentDetails" });

            endpointRouteBuilder.MapControllerRoute("ReturnRequest",
                pattern + "returnrequest/{orderId:min(0)}",
                new { controller = "ReturnRequest", action = "ReturnRequest" });

            endpointRouteBuilder.MapControllerRoute("ReOrder",
                pattern + "reorder/{orderId:min(0)}",
                new { controller = "Order", action = "ReOrder" });

            endpointRouteBuilder.MapControllerRoute("GetOrderPdfInvoice",
                pattern + "orderdetails/pdf/{orderId}",
                new { controller = "Order", action = "GetPdfInvoice" });

            endpointRouteBuilder.MapControllerRoute("PrintOrderDetails",
                pattern + "orderdetails/print/{orderId}",
                new { controller = "Order", action = "PrintOrderDetails" });

            //order downloads
            endpointRouteBuilder.MapControllerRoute("GetDownload",
                pattern + "download/getdownload/{orderItemId:guid}/{agree?}",
                new { controller = "Download", action = "GetDownload" });

            endpointRouteBuilder.MapControllerRoute("GetLicense",
                pattern + "download/getlicense/{orderItemId:guid}/",
                new { controller = "Download", action = "GetLicense" });

            endpointRouteBuilder.MapControllerRoute("DownloadUserAgreement",
                pattern + "customer/useragreement/{orderItemId:guid}",
                new { controller = "Customer", action = "UserAgreement" });

            endpointRouteBuilder.MapControllerRoute("GetOrderNoteFile",
                pattern + "download/ordernotefile/{ordernoteid:min(0)}",
                new { controller = "Download", action = "GetOrderNoteFile" });

            //contact vendor
            endpointRouteBuilder.MapControllerRoute("ContactVendor",
                pattern + "contactvendor/{vendorId}",
                new { controller = "Common", action = "ContactVendor" });

            //apply for vendor account
            endpointRouteBuilder.MapControllerRoute("ApplyVendorAccount", $"{pattern}vendor/apply",
                new { controller = "Vendor", action = "ApplyVendor" });

            //vendor info
            endpointRouteBuilder.MapControllerRoute("CustomerVendorInfo", $"{pattern}customer/vendorinfo",
                new { controller = "Vendor", action = "Info" });

            //customer GDPR
            endpointRouteBuilder.MapControllerRoute("GdprTools", $"{pattern}customer/gdpr",
                new { controller = "Customer", action = "GdprTools" });

            //customer check gift card balance 
            endpointRouteBuilder.MapControllerRoute("CheckGiftCardBalance", $"{pattern}customer/checkgiftcardbalance",
                new { controller = "Customer", action = "CheckGiftCardBalance" });

            //poll vote AJAX link
            endpointRouteBuilder.MapControllerRoute("PollVote", "poll/vote",
                new { controller = "Poll", action = "Vote" });

            //comparing products
            endpointRouteBuilder.MapControllerRoute("RemoveProductFromCompareList",
                pattern + "compareproducts/remove/{productId}",
                new { controller = "Product", action = "RemoveProductFromCompareList" });

            endpointRouteBuilder.MapControllerRoute("ClearCompareList", $"{pattern}clearcomparelist/",
                new { controller = "Product", action = "ClearCompareList" });

            //new RSS
            endpointRouteBuilder.MapControllerRoute("NewProductsRSS", $"{pattern}newproducts/rss",
                new { controller = "Product", action = "NewProductsRss" });

            //get state list by country ID  (AJAX link)
            endpointRouteBuilder.MapControllerRoute("GetStatesByCountryId", $"{pattern}country/getstatesbycountryid/",
                new { controller = "Country", action = "GetStatesByCountryId" });

            //EU Cookie law accept button handler (AJAX link)
            endpointRouteBuilder.MapControllerRoute("EuCookieLawAccept", $"{pattern}eucookielawaccept",
                new { controller = "Common", action = "EuCookieLawAccept" });

            //authenticate topic AJAX link
            endpointRouteBuilder.MapControllerRoute("TopicAuthenticate", $"{pattern}topic/authenticate",
                new { controller = "Topic", action = "Authenticate" });

            //prepare top menu (AJAX link)
            endpointRouteBuilder.MapControllerRoute("GetCatalogRoot", $"{pattern}catalog/getcatalogroot",
                new { controller = "Catalog", action = "GetCatalogRoot" });
            
            endpointRouteBuilder.MapControllerRoute("GetCatalogSubCategories", $"{pattern}catalog/getcatalogsubcategories",
                new { controller = "Catalog", action = "GetCatalogSubCategories" });

            //product attributes with "upload file" type
            endpointRouteBuilder.MapControllerRoute("UploadFileProductAttribute",
                pattern + "uploadfileproductattribute/{attributeId:min(0)}",
                new { controller = "ShoppingCart", action = "UploadFileProductAttribute" });

            //checkout attributes with "upload file" type
            endpointRouteBuilder.MapControllerRoute("UploadFileCheckoutAttribute",
                pattern + "uploadfilecheckoutattribute/{attributeId:min(0)}",
                new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" });

            //return request with "upload file" support
            endpointRouteBuilder.MapControllerRoute("UploadFileReturnRequest", $"{pattern}uploadfilereturnrequest",
                new { controller = "ReturnRequest", action = "UploadFileReturnRequest" });

            //forums
            endpointRouteBuilder.MapControllerRoute("ActiveDiscussions", $"{pattern}boards/activediscussions",
                new { controller = "Boards", action = "ActiveDiscussions" });

            endpointRouteBuilder.MapControllerRoute("ActiveDiscussionsPaged",
                pattern + "boards/activediscussions/page/{pageNumber:int}",
                new { controller = "Boards", action = "ActiveDiscussions" });

            endpointRouteBuilder.MapControllerRoute("ActiveDiscussionsRSS", $"{pattern}boards/activediscussionsrss",
                new { controller = "Boards", action = "ActiveDiscussionsRSS" });

            endpointRouteBuilder.MapControllerRoute("PostEdit",
                pattern + "boards/postedit/{id:min(0)}",
                new { controller = "Boards", action = "PostEdit" });

            endpointRouteBuilder.MapControllerRoute("PostDelete",
                pattern + "boards/postdelete/{id:min(0)}",
                new { controller = "Boards", action = "PostDelete" });

            endpointRouteBuilder.MapControllerRoute("PostCreate",
                pattern + "boards/postcreate/{id:min(0)}",
                new { controller = "Boards", action = "PostCreate" });

            endpointRouteBuilder.MapControllerRoute("PostCreateQuote",
                pattern + "boards/postcreate/{id:min(0)}/{quote:min(0)}",
                new { controller = "Boards", action = "PostCreate" });

            endpointRouteBuilder.MapControllerRoute("TopicEdit",
                pattern + "boards/topicedit/{id:min(0)}",
                new { controller = "Boards", action = "TopicEdit" });

            endpointRouteBuilder.MapControllerRoute("TopicDelete",
                pattern + "boards/topicdelete/{id:min(0)}",
                new { controller = "Boards", action = "TopicDelete" });

            endpointRouteBuilder.MapControllerRoute("TopicCreate",
                pattern + "boards/topiccreate/{id:min(0)}",
                new { controller = "Boards", action = "TopicCreate" });

            endpointRouteBuilder.MapControllerRoute("TopicMove",
                pattern + "boards/topicmove/{id:min(0)}",
                new { controller = "Boards", action = "TopicMove" });

            endpointRouteBuilder.MapControllerRoute("TopicWatch",
                pattern + "boards/topicwatch/{id:min(0)}",
                new { controller = "Boards", action = "TopicWatch" });

            endpointRouteBuilder.MapControllerRoute("TopicSlug",
                pattern + "boards/topic/{id:min(0)}/{slug?}",
                new { controller = "Boards", action = "Topic" });

            endpointRouteBuilder.MapControllerRoute("TopicSlugPaged",
                pattern + "boards/topic/{id:min(0)}/{slug?}/page/{pageNumber:int}",
                new { controller = "Boards", action = "Topic" });

            endpointRouteBuilder.MapControllerRoute("ForumWatch",
                pattern + "boards/forumwatch/{id:min(0)}",
                new { controller = "Boards", action = "ForumWatch" });

            endpointRouteBuilder.MapControllerRoute("ForumRSS",
                pattern + "boards/forumrss/{id:min(0)}",
                new { controller = "Boards", action = "ForumRSS" });

            endpointRouteBuilder.MapControllerRoute("ForumSlug",
                pattern + "boards/forum/{id:min(0)}/{slug?}",
                new { controller = "Boards", action = "Forum" });

            endpointRouteBuilder.MapControllerRoute("ForumSlugPaged",
                pattern + "boards/forum/{id:min(0)}/{slug?}/page/{pageNumber:int}",
                new { controller = "Boards", action = "Forum" });

            endpointRouteBuilder.MapControllerRoute("ForumGroupSlug",
                pattern + "boards/forumgroup/{id:min(0)}/{slug?}",
                new { controller = "Boards", action = "ForumGroup" });

            endpointRouteBuilder.MapControllerRoute("Search", $"{pattern}boards/search",
                new { controller = "Boards", action = "Search" });

            //private messages
            endpointRouteBuilder.MapControllerRoute("PrivateMessages",
                pattern + "privatemessages/{tab?}",
                new { controller = "PrivateMessages", action = "Index" });

            endpointRouteBuilder.MapControllerRoute("PrivateMessagesPaged",
                pattern + "privatemessages/{tab?}/page/{pageNumber:min(0)}",
                new { controller = "PrivateMessages", action = "Index" });

            endpointRouteBuilder.MapControllerRoute("PrivateMessagesInbox", $"{pattern}inboxupdate",
                new { controller = "PrivateMessages", action = "InboxUpdate" });

            endpointRouteBuilder.MapControllerRoute("PrivateMessagesSent", $"{pattern}sentupdate",
                new { controller = "PrivateMessages", action = "SentUpdate" });

            endpointRouteBuilder.MapControllerRoute("SendPM",
                pattern + "sendpm/{toCustomerId:min(0)}",
                new { controller = "PrivateMessages", action = "SendPM" });

            endpointRouteBuilder.MapControllerRoute("SendPMReply",
                pattern + "sendpm/{toCustomerId:min(0)}/{replyToMessageId:min(0)}",
                new { controller = "PrivateMessages", action = "SendPM" });

            endpointRouteBuilder.MapControllerRoute("ViewPM",
                pattern + "viewpm/{privateMessageId:min(0)}",
                new { controller = "PrivateMessages", action = "ViewPM" });

            endpointRouteBuilder.MapControllerRoute("DeletePM",
                pattern + "deletepm/{privateMessageId:min(0)}",
                new { controller = "PrivateMessages", action = "DeletePM" });

            //activate newsletters
            endpointRouteBuilder.MapControllerRoute("NewsletterActivation",
                pattern + "newsletter/subscriptionactivation/{token:guid}/{active}",
                new { controller = "Newsletter", action = "SubscriptionActivation" });

            //robots.txt
            endpointRouteBuilder.MapControllerRoute("robots.txt", $"{pattern}robots.txt",
                new { controller = "Common", action = "RobotsTextFile" });

            //sitemap (XML)
            endpointRouteBuilder.MapControllerRoute("sitemap.xml", $"{pattern}sitemap.xml",
                new { controller = "Common", action = "SitemapXml" });

            endpointRouteBuilder.MapControllerRoute("sitemap-indexed.xml",
                pattern + "sitemap-{Id:min(0)}.xml",
                new { controller = "Common", action = "SitemapXml" });
            
            //store closed
            endpointRouteBuilder.MapControllerRoute("StoreClosed", $"{pattern}storeclosed",
                new { controller = "Common", action = "StoreClosed" });

            //install
            endpointRouteBuilder.MapControllerRoute("Installation", $"{pattern}install",
                new { controller = "Install", action = "Index" });

            //error page
            endpointRouteBuilder.MapControllerRoute("Error", "error",
                new { controller = "Common", action = "Error" });

            //page not found
            endpointRouteBuilder.MapControllerRoute("PageNotFound", $"{pattern}page-not-found",
                new { controller = "Common", action = "PageNotFound" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;

        #endregion
    }
}
