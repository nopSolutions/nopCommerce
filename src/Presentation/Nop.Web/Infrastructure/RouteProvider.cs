using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Seo;

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
        /// <param name="endpointRoute">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRoute)
        {
            //reorder routes so the most used ones are on top. It can improve performance

            //areas
            endpointRoute.MapControllerRoute(name: "areaRoute", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            //home page
            endpointRoute.MapControllerRoute("Homepage", "",
				new { controller = "Home", action = "Index" });

            //login
            endpointRoute.MapControllerRoute("Login", "login/",
				new { controller = "Customer", action = "Login" });

            //register
            endpointRoute.MapControllerRoute("Register", "register/",
				new { controller = "Customer", action = "Register" });

            //logout
            endpointRoute.MapControllerRoute("Logout", "logout/",
				new { controller = "Customer", action = "Logout" });

            //shopping cart
            endpointRoute.MapControllerRoute("ShoppingCart", "cart/",
				new { controller = "ShoppingCart", action = "Cart" });            

            //estimate shipping
            endpointRoute.MapControllerRoute("EstimateShipping", "cart/estimateshipping",
				new {controller = "ShoppingCart", action = "GetEstimateShipping"});

            //wishlist
            endpointRoute.MapControllerRoute("Wishlist", "wishlist/{customerGuid?}",
				new { controller = "ShoppingCart", action = "Wishlist"});

            //customer account links
            endpointRoute.MapControllerRoute("CustomerInfo", "customer/info",
				new { controller = "Customer", action = "Info" });

            endpointRoute.MapControllerRoute("CustomerAddresses", "customer/addresses",
				new { controller = "Customer", action = "Addresses" });

            endpointRoute.MapControllerRoute("CustomerOrders", "order/history",
				new { controller = "Order", action = "CustomerOrders" });

            //contact us
            endpointRoute.MapControllerRoute("ContactUs", "contactus",
				new { controller = "Common", action = "ContactUs" });

            //sitemap
            endpointRoute.MapControllerRoute("Sitemap", "sitemap",
				new { controller = "Common", action = "Sitemap" });

            //product search
            endpointRoute.MapControllerRoute("ProductSearch", "search/",
				new { controller = "Catalog", action = "Search" });                     

            endpointRoute.MapControllerRoute("ProductSearchAutoComplete", "catalog/searchtermautocomplete",
				new { controller = "Catalog", action = "SearchTermAutoComplete" });

            //change currency (AJAX link)
            endpointRoute.MapControllerRoute("ChangeCurrency", "changecurrency/{customercurrency:min(0)}",
				new { controller = "Common", action = "SetCurrency" });

            //change language (AJAX link)
            endpointRoute.MapControllerRoute("ChangeLanguage", "changelanguage/{langid:min(0)}",
				new { controller = "Common", action = "SetLanguage" });

            //change tax (AJAX link)
            endpointRoute.MapControllerRoute("ChangeTaxType", "changetaxtype/{customertaxtype:min(0)}",
				new { controller = "Common", action = "SetTaxType" });

            //recently viewed products
            endpointRoute.MapControllerRoute("RecentlyViewedProducts", "recentlyviewedproducts/",
				new { controller = "Product", action = "RecentlyViewedProducts" });

            //new products
            endpointRoute.MapControllerRoute("NewProducts", "newproducts/",
				new { controller = "Product", action = "NewProducts" });

            //blog
            endpointRoute.MapControllerRoute("Blog", "blog",
				new { controller = "Blog", action = "List" });

            //news
            endpointRoute.MapControllerRoute("NewsArchive", "news",
				new { controller = "News", action = "List" });

            //forum
            endpointRoute.MapControllerRoute("Boards", "boards",
				new { controller = "Boards", action = "Index" });

            //compare products
            endpointRoute.MapControllerRoute("CompareProducts", "compareproducts/",
				new { controller = "Product", action = "CompareProducts" });

            //product tags
            endpointRoute.MapControllerRoute("ProductTagsAll", "producttag/all/",
				new { controller = "Catalog", action = "ProductTagsAll" });

            //manufacturers
            endpointRoute.MapControllerRoute("ManufacturerList", "manufacturer/all/",
				new { controller = "Catalog", action = "ManufacturerAll" });

            //vendors
            endpointRoute.MapControllerRoute("VendorList", "vendor/all/",
				new { controller = "Catalog", action = "VendorAll" });

            //add product to cart (without any attributes and options). used on catalog pages.
            endpointRoute.MapControllerRoute("AddProductToCart-Catalog", "addproducttocart/catalog/{productId:min(0)}/{shoppingCartTypeId:min(0)}/{quantity:min(0)}",
				new { controller = "ShoppingCart", action = "AddProductToCart_Catalog" });

            //add product to cart (with attributes and options). used on the product details pages.
            endpointRoute.MapControllerRoute("AddProductToCart-Details", "addproducttocart/details/{productId:min(0)}/{shoppingCartTypeId:min(0)}",
				new { controller = "ShoppingCart", action = "AddProductToCart_Details" });

            //comparing products
            endpointRoute.MapControllerRoute("AddProductToCompare", "compareproducts/add/{productId:min(0)}",
				new { controller = "Product", action = "AddProductToCompareList" });

            //product email a friend
            endpointRoute.MapControllerRoute("ProductEmailAFriend", "productemailafriend/{productId:min(0)}",
				new { controller = "Product", action = "ProductEmailAFriend" });

            //reviews
            endpointRoute.MapControllerRoute("ProductReviews", "productreviews/{productId}",
				new { controller = "Product", action = "ProductReviews" });

            endpointRoute.MapControllerRoute("CustomerProductReviews", "customer/productreviews",
				new { controller = "Product", action = "CustomerProductReviews" });

            endpointRoute.MapControllerRoute("CustomerProductReviewsPaged", "customer/productreviews/page/{pageNumber:min(0)}",
				new { controller = "Product", action = "CustomerProductReviews" });

            //back in stock notifications
            endpointRoute.MapControllerRoute("BackInStockSubscribePopup", "backinstocksubscribe/{productId:min(0)}",
				new { controller = "BackInStockSubscription", action = "SubscribePopup" });

            endpointRoute.MapControllerRoute("BackInStockSubscribeSend", "backinstocksubscribesend/{productId:min(0)}",
				new { controller = "BackInStockSubscription", action = "SubscribePopupPOST" });

            //downloads
            endpointRoute.MapControllerRoute("GetSampleDownload", "download/sample/{productid:min(0)}",
				new { controller = "Download", action = "Sample" });

            //checkout pages
            endpointRoute.MapControllerRoute("Checkout", "checkout/",
				new { controller = "Checkout", action = "Index" });

            endpointRoute.MapControllerRoute("CheckoutOnePage", "onepagecheckout/",
				new { controller = "Checkout", action = "OnePageCheckout" });

            endpointRoute.MapControllerRoute("CheckoutShippingAddress", "checkout/shippingaddress",
				new { controller = "Checkout", action = "ShippingAddress" });

            endpointRoute.MapControllerRoute("CheckoutSelectShippingAddress", "checkout/selectshippingaddress",
				new { controller = "Checkout", action = "SelectShippingAddress" });

            endpointRoute.MapControllerRoute("CheckoutBillingAddress", "checkout/billingaddress",
				new { controller = "Checkout", action = "BillingAddress" });

            endpointRoute.MapControllerRoute("CheckoutSelectBillingAddress", "checkout/selectbillingaddress",
				new { controller = "Checkout", action = "SelectBillingAddress" });

            endpointRoute.MapControllerRoute("CheckoutShippingMethod", "checkout/shippingmethod",
				new { controller = "Checkout", action = "ShippingMethod" });

            endpointRoute.MapControllerRoute("CheckoutPaymentMethod", "checkout/paymentmethod",
				new { controller = "Checkout", action = "PaymentMethod" });

            endpointRoute.MapControllerRoute("CheckoutPaymentInfo", "checkout/paymentinfo",
				new { controller = "Checkout", action = "PaymentInfo" });

            endpointRoute.MapControllerRoute("CheckoutConfirm", "checkout/confirm",
				new { controller = "Checkout", action = "Confirm" });

            endpointRoute.MapControllerRoute("CheckoutCompleted", "checkout/completed/{orderId:int}",
                new { controller = "Checkout", action = "Completed" });

            //subscribe newsletters
            endpointRoute.MapControllerRoute("SubscribeNewsletter", "subscribenewsletter",
				new { controller = "Newsletter", action = "SubscribeNewsletter" });

            //email wishlist
            endpointRoute.MapControllerRoute("EmailWishlist", "emailwishlist",
				new { controller = "ShoppingCart", action = "EmailWishlist" });

            //login page for checkout as guest
            endpointRoute.MapControllerRoute("LoginCheckoutAsGuest", "login/checkoutasguest",
				new { controller = "Customer", action = "Login", checkoutAsGuest = true });

            //register result page
            endpointRoute.MapControllerRoute("RegisterResult", "registerresult/{resultId:min(0)}",
				new { controller = "Customer", action = "RegisterResult" });

            //check username availability
            endpointRoute.MapControllerRoute("CheckUsernameAvailability", "customer/checkusernameavailability",
				new { controller = "Customer", action = "CheckUsernameAvailability" });

            //passwordrecovery
            endpointRoute.MapControllerRoute("PasswordRecovery", "passwordrecovery",
				new { controller = "Customer", action = "PasswordRecovery" });

            //password recovery confirmation
            endpointRoute.MapControllerRoute("PasswordRecoveryConfirm", "passwordrecovery/confirm",
				new { controller = "Customer", action = "PasswordRecoveryConfirm" });

            //topics
            endpointRoute.MapControllerRoute("TopicPopup", "t-popup/{SystemName}",
				new { controller = "Topic", action = "TopicDetailsPopup" });
            
            //blog
            endpointRoute.MapControllerRoute("BlogByTag", "blog/tag/{tag}",
				new { controller = "Blog", action = "BlogByTag" });

            endpointRoute.MapControllerRoute("BlogByMonth", "blog/month/{month}",
				new { controller = "Blog", action = "BlogByMonth" });

            //blog RSS
            endpointRoute.MapControllerRoute("BlogRSS", "blog/rss/{languageId:min(0)}",
				new { controller = "Blog", action = "ListRss" });

            //news RSS
            endpointRoute.MapControllerRoute("NewsRSS", "news/rss/{languageId:min(0)}",
				new { controller = "News", action = "ListRss" });

            //set review helpfulness (AJAX link)
            endpointRoute.MapControllerRoute("SetProductReviewHelpfulness", "setproductreviewhelpfulness",
				new { controller = "Product", action = "SetProductReviewHelpfulness" });

            //customer account links
            endpointRoute.MapControllerRoute("CustomerReturnRequests", "returnrequest/history",
				new { controller = "ReturnRequest", action = "CustomerReturnRequests" });

            endpointRoute.MapControllerRoute("CustomerDownloadableProducts", "customer/downloadableproducts",
				new { controller = "Customer", action = "DownloadableProducts" });

            endpointRoute.MapControllerRoute("CustomerBackInStockSubscriptions", "backinstocksubscriptions/manage/{pageNumber:int?}",
                new { controller = "BackInStockSubscription", action = "CustomerSubscriptions" });

            endpointRoute.MapControllerRoute("CustomerRewardPoints", "rewardpoints/history",
				new { controller = "Order", action = "CustomerRewardPoints" });

            endpointRoute.MapControllerRoute("CustomerRewardPointsPaged", "rewardpoints/history/page/{pageNumber:min(0)}",
				new { controller = "Order", action = "CustomerRewardPoints" });

            endpointRoute.MapControllerRoute("CustomerChangePassword", "customer/changepassword",
				new { controller = "Customer", action = "ChangePassword" });

            endpointRoute.MapControllerRoute("CustomerAvatar", "customer/avatar",
				new { controller = "Customer", action = "Avatar" });

            endpointRoute.MapControllerRoute("AccountActivation", "customer/activation",
				new { controller = "Customer", action = "AccountActivation" });

            endpointRoute.MapControllerRoute("EmailRevalidation", "customer/revalidateemail",
				new { controller = "Customer", action = "EmailRevalidation" });

            endpointRoute.MapControllerRoute("CustomerForumSubscriptions", "boards/forumsubscriptions/{pageNumber:int?}",
				new { controller = "Boards", action = "CustomerForumSubscriptions" });

            endpointRoute.MapControllerRoute("CustomerAddressEdit", "customer/addressedit/{addressId:min(0)}",
				new { controller = "Customer", action = "AddressEdit" });

            endpointRoute.MapControllerRoute("CustomerAddressAdd", "customer/addressadd",
				new { controller = "Customer", action = "AddressAdd" });

            //customer profile page
            endpointRoute.MapControllerRoute("CustomerProfile", "profile/{id:min(0)}",
				new { controller = "Profile", action = "Index" });

            endpointRoute.MapControllerRoute("CustomerProfilePaged", "profile/{id:min(0)}/page/{pageNumber:min(0)}",
				new { controller = "Profile", action = "Index" });

            //orders
            endpointRoute.MapControllerRoute("OrderDetails", "orderdetails/{orderId:min(0)}",
				new { controller = "Order", action = "Details" });

            endpointRoute.MapControllerRoute("ShipmentDetails", "orderdetails/shipment/{shipmentId}",
				new { controller = "Order", action = "ShipmentDetails" });

            endpointRoute.MapControllerRoute("ReturnRequest", "returnrequest/{orderId:min(0)}",
				new { controller = "ReturnRequest", action = "ReturnRequest" });

            endpointRoute.MapControllerRoute("ReOrder", "reorder/{orderId:min(0)}",
				new { controller = "Order", action = "ReOrder" });

            endpointRoute.MapControllerRoute("GetOrderPdfInvoice", "orderdetails/pdf/{orderId}",
				new { controller = "Order", action = "GetPdfInvoice" });

            endpointRoute.MapControllerRoute("PrintOrderDetails", "orderdetails/print/{orderId}",
				new { controller = "Order", action = "PrintOrderDetails" });

            //order downloads
            endpointRoute.MapControllerRoute("GetDownload", "download/getdownload/{orderItemId:guid}/{agree?}",
				new { controller = "Download", action = "GetDownload" });

            endpointRoute.MapControllerRoute("GetLicense", "download/getlicense/{orderItemId:guid}/",
				new { controller = "Download", action = "GetLicense" });

            endpointRoute.MapControllerRoute("DownloadUserAgreement", "customer/useragreement/{orderItemId:guid}",
				new { controller = "Customer", action = "UserAgreement" });

            endpointRoute.MapControllerRoute("GetOrderNoteFile", "download/ordernotefile/{ordernoteid:min(0)}",
				new { controller = "Download", action = "GetOrderNoteFile" });

            //contact vendor
            endpointRoute.MapControllerRoute("ContactVendor", "contactvendor/{vendorId}",
				new { controller = "Common", action = "ContactVendor" });

            //apply for vendor account
            endpointRoute.MapControllerRoute("ApplyVendorAccount", "vendor/apply",
				new { controller = "Vendor", action = "ApplyVendor" });

            //vendor info
            endpointRoute.MapControllerRoute("CustomerVendorInfo", "customer/vendorinfo",
				new { controller = "Vendor", action = "Info" });

            //customer GDPR
            endpointRoute.MapControllerRoute("GdprTools", "customer/gdpr",
                new { controller = "Customer", action = "GdprTools" });

            //customer check gift card balance 
            endpointRoute.MapControllerRoute("CheckGiftCardBalance", "customer/checkgiftcardbalance",
                new { controller = "Customer", action = "CheckGiftCardBalance" });

            //poll vote AJAX link
            endpointRoute.MapControllerRoute("PollVote", "poll/vote",
				new { controller = "Poll", action = "Vote" });

            //comparing products
            endpointRoute.MapControllerRoute("RemoveProductFromCompareList", "compareproducts/remove/{productId}",
				new { controller = "Product", action = "RemoveProductFromCompareList" });

            endpointRoute.MapControllerRoute("ClearCompareList", "clearcomparelist/",
				new { controller = "Product", action = "ClearCompareList" });

            //new RSS
            endpointRoute.MapControllerRoute("NewProductsRSS", "newproducts/rss",
				new { controller = "Product", action = "NewProductsRss" });
            
            //get state list by country ID  (AJAX link)
            endpointRoute.MapControllerRoute("GetStatesByCountryId", "country/getstatesbycountryid/",
				new { controller = "Country", action = "GetStatesByCountryId" });

            //EU Cookie law accept button handler (AJAX link)
            endpointRoute.MapControllerRoute("EuCookieLawAccept", "eucookielawaccept",
				new { controller = "Common", action = "EuCookieLawAccept" });

            //authenticate topic AJAX link
            endpointRoute.MapControllerRoute("TopicAuthenticate", "topic/authenticate",
				new { controller = "Topic", action = "Authenticate" });

            //product attributes with "upload file" type
            endpointRoute.MapControllerRoute("UploadFileProductAttribute", "uploadfileproductattribute/{attributeId:min(0)}",
				new { controller = "ShoppingCart", action = "UploadFileProductAttribute" });

            //checkout attributes with "upload file" type
            endpointRoute.MapControllerRoute("UploadFileCheckoutAttribute", "uploadfilecheckoutattribute/{attributeId:min(0)}",
				new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" });

            //return request with "upload file" support
            endpointRoute.MapControllerRoute("UploadFileReturnRequest", "uploadfilereturnrequest",
				new { controller = "ReturnRequest", action = "UploadFileReturnRequest" });

            //forums
            endpointRoute.MapControllerRoute("ActiveDiscussions", "boards/activediscussions",
				new { controller = "Boards", action = "ActiveDiscussions" });

            endpointRoute.MapControllerRoute("ActiveDiscussionsPaged", "boards/activediscussions/page/{pageNumber:int}",
                new { controller = "Boards", action = "ActiveDiscussions" });

            endpointRoute.MapControllerRoute("ActiveDiscussionsRSS", "boards/activediscussionsrss",
				new { controller = "Boards", action = "ActiveDiscussionsRSS" });

            endpointRoute.MapControllerRoute("PostEdit", "boards/postedit/{id:min(0)}",
				new { controller = "Boards", action = "PostEdit" });

            endpointRoute.MapControllerRoute("PostDelete", "boards/postdelete/{id:min(0)}",
				new { controller = "Boards", action = "PostDelete" });

            endpointRoute.MapControllerRoute("PostCreate", "boards/postcreate/{id:min(0)}",
				new { controller = "Boards", action = "PostCreate" });

            endpointRoute.MapControllerRoute("PostCreateQuote", "boards/postcreate/{id:min(0)}/{quote:min(0)}",
				new { controller = "Boards", action = "PostCreate" });

            endpointRoute.MapControllerRoute("TopicEdit", "boards/topicedit/{id:min(0)}",
				new { controller = "Boards", action = "TopicEdit" });

            endpointRoute.MapControllerRoute("TopicDelete", "boards/topicdelete/{id:min(0)}",
				new { controller = "Boards", action = "TopicDelete" });

            endpointRoute.MapControllerRoute("TopicCreate", "boards/topiccreate/{id:min(0)}",
				new { controller = "Boards", action = "TopicCreate" });

            endpointRoute.MapControllerRoute("TopicMove", "boards/topicmove/{id:min(0)}",
				new { controller = "Boards", action = "TopicMove" });

            endpointRoute.MapControllerRoute("TopicWatch", "boards/topicwatch/{id:min(0)}",
				new { controller = "Boards", action = "TopicWatch" });

            endpointRoute.MapControllerRoute("TopicSlug", "boards/topic/{id:min(0)}/{slug?}",
				new { controller = "Boards", action = "Topic" });

            endpointRoute.MapControllerRoute("TopicSlugPaged", "boards/topic/{id:min(0)}/{slug?}/page/{pageNumber:int}",
                new { controller = "Boards", action = "Topic" });

            endpointRoute.MapControllerRoute("ForumWatch", "boards/forumwatch/{id:min(0)}",
				new { controller = "Boards", action = "ForumWatch" });

            endpointRoute.MapControllerRoute("ForumRSS", "boards/forumrss/{id:min(0)}",
				new { controller = "Boards", action = "ForumRSS" });

            endpointRoute.MapControllerRoute("ForumSlug", "boards/forum/{id:min(0)}/{slug?}",
				new { controller = "Boards", action = "Forum" });

            endpointRoute.MapControllerRoute("ForumSlugPaged", "boards/forum/{id:min(0)}/{slug?}/page/{pageNumber:int}",
                new { controller = "Boards", action = "Forum" });

            endpointRoute.MapControllerRoute("ForumGroupSlug", "boards/forumgroup/{id:min(0)}/{slug?}",
				new { controller = "Boards", action = "ForumGroup"});

            endpointRoute.MapControllerRoute("Search", "boards/search",
				new { controller = "Boards", action = "Search" });

            //private messages
            endpointRoute.MapControllerRoute("PrivateMessages", "privatemessages/{tab?}",
				new { controller = "PrivateMessages", action = "Index" });

            endpointRoute.MapControllerRoute("PrivateMessagesPaged", "privatemessages/{tab?}/page/{pageNumber:min(0)}",
				new { controller = "PrivateMessages", action = "Index" });

            endpointRoute.MapControllerRoute("PrivateMessagesInbox", "inboxupdate",
				new { controller = "PrivateMessages", action = "InboxUpdate" });

            endpointRoute.MapControllerRoute("PrivateMessagesSent", "sentupdate",
				new { controller = "PrivateMessages", action = "SentUpdate" });

            endpointRoute.MapControllerRoute("SendPM", "sendpm/{toCustomerId:min(0)}",
				new { controller = "PrivateMessages", action = "SendPM" });

            endpointRoute.MapControllerRoute("SendPMReply", "sendpm/{toCustomerId:min(0)}/{replyToMessageId:min(0)}",
				new { controller = "PrivateMessages", action = "SendPM" });

            endpointRoute.MapControllerRoute("ViewPM", "viewpm/{privateMessageId:min(0)}",
				new { controller = "PrivateMessages", action = "ViewPM" });

            endpointRoute.MapControllerRoute("DeletePM", "deletepm/{privateMessageId:min(0)}",
				new { controller = "PrivateMessages", action = "DeletePM" });

            //activate newsletters
            endpointRoute.MapControllerRoute("NewsletterActivation", "newsletter/subscriptionactivation/{token:guid}/{active}",
				new { controller = "Newsletter", action = "SubscriptionActivation" });

            //robots.txt
            endpointRoute.MapControllerRoute("robots.txt", "robots.txt",
				new { controller = "Common", action = "RobotsTextFile" });

            //sitemap (XML)
            endpointRoute.MapControllerRoute("sitemap.xml", "sitemap.xml",
				new { controller = "Common", action = "SitemapXml" });

            endpointRoute.MapControllerRoute("sitemap-indexed.xml", "sitemap-{Id:min(0)}.xml",
				new { controller = "Common", action = "SitemapXml" });

            //store closed
            endpointRoute.MapControllerRoute("StoreClosed", "storeclosed",
				new { controller = "Common", action = "StoreClosed" });

            //install
            endpointRoute.MapControllerRoute("Installation", "install",
				new { controller = "Install", action = "Index" });

            //error page
            endpointRoute.MapControllerRoute("Error", "error",
                new { controller = "Common", action = "Error" });

            //page not found
            endpointRoute.MapControllerRoute("PageNotFound", "page-not-found", 
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
