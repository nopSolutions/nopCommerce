using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;

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
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //reorder routes so the most used ones are on top. It can improve performance.

            //home page
            routeBuilder.MapLocalizedRoute("HomePage", "",
				new { controller = "Home", action = "Index" });

            //widgets
            //we have this route for performance optimization because named routes are MUCH faster than usual Html.Action(...)
            //and this route is highly used
            routeBuilder.MapRoute("WidgetsByZone", "widgetsbyzone/",
				new { controller = "Widget", action = "WidgetsByZone" });

            //login
            routeBuilder.MapLocalizedRoute("Login", "login/",
				new { controller = "Customer", action = "Login" });

            //register
            routeBuilder.MapLocalizedRoute("Register", "register/",
				new { controller = "Customer", action = "Register" });

            //logout
            routeBuilder.MapLocalizedRoute("Logout", "logout/",
				new { controller = "Customer", action = "Logout" });

            //shopping cart
            routeBuilder.MapLocalizedRoute("ShoppingCart", "cart/",
				new { controller = "ShoppingCart", action = "Cart" });

            //estimate shipping
            routeBuilder.MapLocalizedRoute("EstimateShipping", "cart/estimateshipping",
				new {controller = "ShoppingCart", action = "GetEstimateShipping"});

            //wishlist
            routeBuilder.MapLocalizedRoute("Wishlist", "wishlist/{customerGuid?}",
				new { controller = "ShoppingCart", action = "Wishlist"});

            //customer account links
            routeBuilder.MapLocalizedRoute("CustomerInfo", "customer/info",
				new { controller = "Customer", action = "Info" });

            routeBuilder.MapLocalizedRoute("CustomerAddresses", "customer/addresses",
				new { controller = "Customer", action = "Addresses" });

            routeBuilder.MapLocalizedRoute("CustomerOrders", "order/history",
				new { controller = "Order", action = "CustomerOrders" });

            //contact us
            routeBuilder.MapLocalizedRoute("ContactUs", "contactus",
				new { controller = "Common", action = "ContactUs" });

            //sitemap
            routeBuilder.MapLocalizedRoute("Sitemap", "sitemap",
				new { controller = "Common", action = "Sitemap" });

            //product search
            routeBuilder.MapLocalizedRoute("ProductSearch", "search/",
				new { controller = "Catalog", action = "Search" });

            routeBuilder.MapLocalizedRoute("ProductSearchAutoComplete", "catalog/searchtermautocomplete",
				new { controller = "Catalog", action = "SearchTermAutoComplete" });

            //change currency (AJAX link)
            routeBuilder.MapLocalizedRoute("ChangeCurrency", "changecurrency/{customercurrency}",
				new { controller = "Common", action = "SetCurrency" }, new { customercurrency = @"\d+" });

            //change language (AJAX link)
            routeBuilder.MapLocalizedRoute("ChangeLanguage", "changelanguage/{langid}",
				new { controller = "Common", action = "SetLanguage" }, new { langid = @"\d+" });

            //change tax (AJAX link)
            routeBuilder.MapLocalizedRoute("ChangeTaxType", "changetaxtype/{customertaxtype}",
				new { controller = "Common", action = "SetTaxType" }, new { customertaxtype = @"\d+" });

            //recently viewed products
            routeBuilder.MapLocalizedRoute("RecentlyViewedProducts", "recentlyviewedproducts/",
				new { controller = "Product", action = "RecentlyViewedProducts" });

            //new products
            routeBuilder.MapLocalizedRoute("NewProducts", "newproducts/",
				new { controller = "Product", action = "NewProducts" });

            //blog
            routeBuilder.MapLocalizedRoute("Blog", "blog",
				new { controller = "Blog", action = "List" });

            //news
            routeBuilder.MapLocalizedRoute("NewsArchive", "news",
				new { controller = "News", action = "List" });

            //forum
            routeBuilder.MapLocalizedRoute("Boards", "boards",
				new { controller = "Boards", action = "Index" });

            //compare products
            routeBuilder.MapLocalizedRoute("CompareProducts", "compareproducts/",
				new { controller = "Product", action = "CompareProducts" });

            //product tags
            routeBuilder.MapLocalizedRoute("ProductTagsAll", "producttag/all/",
				new { controller = "Catalog", action = "ProductTagsAll" });

            //manufacturers
            routeBuilder.MapLocalizedRoute("ManufacturerList", "manufacturer/all/",
				new { controller = "Catalog", action = "ManufacturerAll" });

            //vendors
            routeBuilder.MapLocalizedRoute("VendorList", "vendor/all/",
				new { controller = "Catalog", action = "VendorAll" });


            //add product to cart (without any attributes and options). used on catalog pages.
            routeBuilder.MapLocalizedRoute("AddProductToCart-Catalog", "addproducttocart/catalog/{productId}/{shoppingCartTypeId}/{quantity}",
				new { controller = "ShoppingCart", action = "AddProductToCart_Catalog" }, new { productId = @"\d+", shoppingCartTypeId = @"\d+", quantity = @"\d+" });

            //add product to cart (with attributes and options). used on the product details pages.
            routeBuilder.MapLocalizedRoute("AddProductToCart-Details", "addproducttocart/details/{productId}/{shoppingCartTypeId}",
				new { controller = "ShoppingCart", action = "AddProductToCart_Details" }, new { productId = @"\d+", shoppingCartTypeId = @"\d+" });

            //product tags
            routeBuilder.MapLocalizedRoute("ProductsByTag", "producttag/{productTagId}/{SeName?}",
				new { controller = "Catalog", action = "ProductsByTag" }, new { productTagId = @"\d+" });

            //comparing products
            routeBuilder.MapLocalizedRoute("AddProductToCompare", "compareproducts/add/{productId}",
				new { controller = "Product", action = "AddProductToCompareList" }, new { productId = @"\d+" });

            //product email a friend
            routeBuilder.MapLocalizedRoute("ProductEmailAFriend", "productemailafriend/{productId}",
				new { controller = "Product", action = "ProductEmailAFriend" }, new { productId = @"\d+" });

            //reviews
            routeBuilder.MapLocalizedRoute("ProductReviews", "productreviews/{productId}",
				new { controller = "Product", action = "ProductReviews" });

            routeBuilder.MapLocalizedRoute("CustomerProductReviews", "customer/productreviews",
				new { controller = "Product", action = "CustomerProductReviews" });

            routeBuilder.MapLocalizedRoute("CustomerProductReviewsPaged", "customer/productreviews/page/{page}",
				new { controller = "Product", action = "CustomerProductReviews" }, new { page = @"\d+" });

            //back in stock notifications
            routeBuilder.MapLocalizedRoute("BackInStockSubscribePopup", "backinstocksubscribe/{productId}",
				new { controller = "BackInStockSubscription", action = "SubscribePopup" }, new { productId = @"\d+" });

            routeBuilder.MapLocalizedRoute("BackInStockSubscribeSend", "backinstocksubscribesend/{productId}",
				new { controller = "BackInStockSubscription", action = "SubscribePopupPOST" }, new { productId = @"\d+" });

            //downloads
            routeBuilder.MapRoute("GetSampleDownload", "download/sample/{productid}",
				new { controller = "Download", action = "Sample" }, new { productid = @"\d+" });

            //checkout pages
            routeBuilder.MapLocalizedRoute("Checkout", "checkout/",
				new { controller = "Checkout", action = "Index" });

            routeBuilder.MapLocalizedRoute("CheckoutOnePage", "onepagecheckout/",
				new { controller = "Checkout", action = "OnePageCheckout" });

            routeBuilder.MapLocalizedRoute("CheckoutShippingAddress", "checkout/shippingaddress",
				new { controller = "Checkout", action = "ShippingAddress" });

            routeBuilder.MapLocalizedRoute("CheckoutSelectShippingAddress", "checkout/selectshippingaddress",
				new { controller = "Checkout", action = "SelectShippingAddress" });

            routeBuilder.MapLocalizedRoute("CheckoutBillingAddress", "checkout/billingaddress",
				new { controller = "Checkout", action = "BillingAddress" });

            routeBuilder.MapLocalizedRoute("CheckoutSelectBillingAddress", "checkout/selectbillingaddress",
				new { controller = "Checkout", action = "SelectBillingAddress" });

            routeBuilder.MapLocalizedRoute("CheckoutShippingMethod", "checkout/shippingmethod",
				new { controller = "Checkout", action = "ShippingMethod" });

            routeBuilder.MapLocalizedRoute("CheckoutPaymentMethod", "checkout/paymentmethod",
				new { controller = "Checkout", action = "PaymentMethod" });

            routeBuilder.MapLocalizedRoute("CheckoutPaymentInfo", "checkout/paymentinfo",
				new { controller = "Checkout", action = "PaymentInfo" });

            routeBuilder.MapLocalizedRoute("CheckoutConfirm", "checkout/confirm",
				new { controller = "Checkout", action = "Confirm" });

            routeBuilder.MapLocalizedRoute("CheckoutCompleted", "checkout/completed/{orderId?}",
				new { controller = "Checkout", action = "Completed" }, new { orderId = @"\d+" });

            //subscribe newsletters
            routeBuilder.MapLocalizedRoute("SubscribeNewsletter", "subscribenewsletter",
				new { controller = "Newsletter", action = "SubscribeNewsletter" });

            //email wishlist
            routeBuilder.MapLocalizedRoute("EmailWishlist", "emailwishlist",
				new { controller = "ShoppingCart", action = "EmailWishlist" });

            //login page for checkout as guest
            routeBuilder.MapLocalizedRoute("LoginCheckoutAsGuest", "login/checkoutasguest",
				new { controller = "Customer", action = "Login", checkoutAsGuest = true });

            //register result page
            routeBuilder.MapLocalizedRoute("RegisterResult", "registerresult/{resultId}",
				new { controller = "Customer", action = "RegisterResult" }, new { resultId = @"\d+" });

            //check username availability
            routeBuilder.MapLocalizedRoute("CheckUsernameAvailability", "customer/checkusernameavailability",
				new { controller = "Customer", action = "CheckUsernameAvailability" });

            //passwordrecovery
            routeBuilder.MapLocalizedRoute("PasswordRecovery", "passwordrecovery",
				new { controller = "Customer", action = "PasswordRecovery" });

            //password recovery confirmation
            routeBuilder.MapLocalizedRoute("PasswordRecoveryConfirm", "passwordrecovery/confirm",
				new { controller = "Customer", action = "PasswordRecoveryConfirm" });

            //topics
            routeBuilder.MapLocalizedRoute("TopicPopup", "t-popup/{SystemName}",
				new { controller = "Topic", action = "TopicDetailsPopup" });
            
            //blog
            routeBuilder.MapLocalizedRoute("BlogByTag", "blog/tag/{tag}",
				new { controller = "Blog", action = "BlogByTag" });

            routeBuilder.MapLocalizedRoute("BlogByMonth", "blog/month/{month}",
				new { controller = "Blog", action = "BlogByMonth" });

            //blog RSS
            routeBuilder.MapLocalizedRoute("BlogRSS", "blog/rss/{languageId}",
				new { controller = "Blog", action = "ListRss" }, new { languageId = @"\d+" });

            //news RSS
            routeBuilder.MapLocalizedRoute("NewsRSS", "news/rss/{languageId}",
				new { controller = "News", action = "ListRss" }, new { languageId = @"\d+" });

            //set review helpfulness (AJAX link)
            routeBuilder.MapRoute("SetProductReviewHelpfulness", "setproductreviewhelpfulness",
				new { controller = "Product", action = "SetProductReviewHelpfulness" });

            //customer account links
            routeBuilder.MapLocalizedRoute("CustomerReturnRequests", "returnrequest/history",
				new { controller = "ReturnRequest", action = "CustomerReturnRequests" });

            routeBuilder.MapLocalizedRoute("CustomerDownloadableProducts", "customer/downloadableproducts",
				new { controller = "Customer", action = "DownloadableProducts" });

            routeBuilder.MapLocalizedRoute("CustomerBackInStockSubscriptions", "backinstocksubscriptions/manage",
				new { controller = "BackInStockSubscription", action = "CustomerSubscriptions" });

            routeBuilder.MapLocalizedRoute("CustomerBackInStockSubscriptionsPaged", "backinstocksubscriptions/manage/{page?}",
				new { controller = "BackInStockSubscription", action = "CustomerSubscriptions" }, new { page = @"\d+" });

            routeBuilder.MapLocalizedRoute("CustomerRewardPoints", "rewardpoints/history",
				new { controller = "Order", action = "CustomerRewardPoints" });

            routeBuilder.MapLocalizedRoute("CustomerRewardPointsPaged", "rewardpoints/history/page/{page}",
				new { controller = "Order", action = "CustomerRewardPoints" }, new { page = @"\d+" });

            routeBuilder.MapLocalizedRoute("CustomerChangePassword", "customer/changepassword",
				new { controller = "Customer", action = "ChangePassword" });

            routeBuilder.MapLocalizedRoute("CustomerAvatar", "customer/avatar",
				new { controller = "Customer", action = "Avatar" });

            routeBuilder.MapLocalizedRoute("AccountActivation", "customer/activation",
				new { controller = "Customer", action = "AccountActivation" });

            routeBuilder.MapLocalizedRoute("EmailRevalidation", "customer/revalidateemail",
				new { controller = "Customer", action = "EmailRevalidation" });

            routeBuilder.MapLocalizedRoute("CustomerForumSubscriptions", "boards/forumsubscriptions",
				new { controller = "Boards", action = "CustomerForumSubscriptions" });

            routeBuilder.MapLocalizedRoute("CustomerForumSubscriptionsPaged", "boards/forumsubscriptions/{page?}",
				new { controller = "Boards", action = "CustomerForumSubscriptions" }, new { page = @"\d+" });

            routeBuilder.MapLocalizedRoute("CustomerAddressEdit", "customer/addressedit/{addressId}",
				new { controller = "Customer", action = "AddressEdit" }, new { addressId = @"\d+" });

            routeBuilder.MapLocalizedRoute("CustomerAddressAdd", "customer/addressadd",
				new { controller = "Customer", action = "AddressAdd" });

            //customer profile page
            routeBuilder.MapLocalizedRoute("CustomerProfile", "profile/{id}",
				new { controller = "Profile", action = "Index" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("CustomerProfilePaged", "profile/{id}/page/{page}",
				new { controller = "Profile", action = "Index" }, new { id = @"\d+", page = @"\d+" });

            //orders
            routeBuilder.MapLocalizedRoute("OrderDetails", "orderdetails/{orderId}",
				new { controller = "Order", action = "Details" }, new { orderId = @"\d+" });

            routeBuilder.MapLocalizedRoute("ShipmentDetails", "orderdetails/shipment/{shipmentId}",
				new { controller = "Order", action = "ShipmentDetails" });

            routeBuilder.MapLocalizedRoute("ReturnRequest", "returnrequest/{orderId}",
				new { controller = "ReturnRequest", action = "ReturnRequest" }, new { orderId = @"\d+" });

            routeBuilder.MapLocalizedRoute("ReOrder", "reorder/{orderId}",
				new { controller = "Order", action = "ReOrder" }, new { orderId = @"\d+" });

            routeBuilder.MapLocalizedRoute("GetOrderPdfInvoice", "orderdetails/pdf/{orderId}",
				new { controller = "Order", action = "GetPdfInvoice" });

            routeBuilder.MapLocalizedRoute("PrintOrderDetails", "orderdetails/print/{orderId}",
				new { controller = "Order", action = "PrintOrderDetails" });

            //order downloads
            routeBuilder.MapRoute("GetDownload", "download/getdownload/{orderItemId}/{agree?}",
				new { controller = "Download", action = "GetDownload" }, new { orderItemId = new GuidConstraint(false) });

            routeBuilder.MapRoute("GetLicense", "download/getlicense/{orderItemId}/",
				new { controller = "Download", action = "GetLicense" }, new { orderItemId = new GuidConstraint(false) });

            routeBuilder.MapLocalizedRoute("DownloadUserAgreement", "customer/useragreement/{orderItemId}",
				new { controller = "Customer", action = "UserAgreement" }, new { orderItemId = new GuidConstraint(false) });

            routeBuilder.MapRoute("GetOrderNoteFile", "download/ordernotefile/{ordernoteid}",
				new { controller = "Download", action = "GetOrderNoteFile" }, new { ordernoteid = @"\d+" });

            //contact vendor
            routeBuilder.MapLocalizedRoute("ContactVendor", "contactvendor/{vendorId}",
				new { controller = "Common", action = "ContactVendor" });

            //apply for vendor account
            routeBuilder.MapLocalizedRoute("ApplyVendorAccount", "vendor/apply",
				new { controller = "Vendor", action = "ApplyVendor" });

            //vendor info
            routeBuilder.MapLocalizedRoute("CustomerVendorInfo", "customer/vendorinfo",
				new { controller = "Vendor", action = "Info" });

            //poll vote AJAX link
            routeBuilder.MapLocalizedRoute("PollVote", "poll/vote",
				new { controller = "Poll", action = "Vote" });

            //comparing products
            routeBuilder.MapLocalizedRoute("RemoveProductFromCompareList", "compareproducts/remove/{productId}",
				new { controller = "Product", action = "RemoveProductFromCompareList" });

            routeBuilder.MapLocalizedRoute("ClearCompareList", "clearcomparelist/",
				new { controller = "Product", action = "ClearCompareList" });

            //new RSS
            routeBuilder.MapLocalizedRoute("NewProductsRSS", "newproducts/rss",
				new { controller = "Product", action = "NewProductsRss" });
            
            //get state list by country ID  (AJAX link)
            routeBuilder.MapRoute("GetStatesByCountryId", "country/getstatesbycountryid/",
				new { controller = "Country", action = "GetStatesByCountryId" });

            //EU Cookie law accept button handler (AJAX link)
            routeBuilder.MapRoute("EuCookieLawAccept", "eucookielawaccept",
				new { controller = "Common", action = "EuCookieLawAccept" });

            //authenticate topic AJAX link
            routeBuilder.MapLocalizedRoute("TopicAuthenticate", "topic/authenticate",
				new { controller = "Topic", action = "Authenticate" });

            //product attributes with "upload file" type
            routeBuilder.MapLocalizedRoute("UploadFileProductAttribute", "uploadfileproductattribute/{attributeId}",
				new { controller = "ShoppingCart", action = "UploadFileProductAttribute" }, new { attributeId = @"\d+" });

            //checkout attributes with "upload file" type
            routeBuilder.MapLocalizedRoute("UploadFileCheckoutAttribute", "uploadfilecheckoutattribute/{attributeId}",
				new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" }, new { attributeId = @"\d+" });

            //return request with "upload file" tsupport
            routeBuilder.MapLocalizedRoute("UploadFileReturnRequest", "uploadfilereturnrequest",
				new { controller = "ReturnRequest", action = "UploadFileReturnRequest" });

            //forums
            routeBuilder.MapLocalizedRoute("ActiveDiscussions", "boards/activediscussions",
				new { controller = "Boards", action = "ActiveDiscussions" });

            routeBuilder.MapLocalizedRoute("ActiveDiscussionsPaged", "boards/activediscussions/page/{page?}",
				new { controller = "Boards", action = "ActiveDiscussions" }, new { page = @"\d+" });

            routeBuilder.MapLocalizedRoute("ActiveDiscussionsRSS", "boards/activediscussionsrss",
				new { controller = "Boards", action = "ActiveDiscussionsRSS" });

            routeBuilder.MapLocalizedRoute("PostEdit", "boards/postedit/{id}",
				new { controller = "Boards", action = "PostEdit" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("PostDelete", "boards/postdelete/{id}",
				new { controller = "Boards", action = "PostDelete" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("PostCreate", "boards/postcreate/{id}",
				new { controller = "Boards", action = "PostCreate" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("PostCreateQuote", "boards/postcreate/{id}/{quote}",
				new { controller = "Boards", action = "PostCreate" }, new { id = @"\d+", quote = @"\d+" });

            routeBuilder.MapLocalizedRoute("TopicEdit", "boards/topicedit/{id}",
				new { controller = "Boards", action = "TopicEdit" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("TopicDelete", "boards/topicdelete/{id}",
				new { controller = "Boards", action = "TopicDelete" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("TopicCreate", "boards/topiccreate/{id}",
				new { controller = "Boards", action = "TopicCreate" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("TopicMove", "boards/topicmove/{id}",
				new { controller = "Boards", action = "TopicMove" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("TopicWatch", "boards/topicwatch/{id}",
				new { controller = "Boards", action = "TopicWatch" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("TopicSlug", "boards/topic/{id}/{slug?}",
				new { controller = "Boards", action = "Topic" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("TopicSlugPaged", "boards/topic/{id}/{slug?}/page/{page?}",
				new { controller = "Boards", action = "Topic" }, new { id = @"\d+", page = @"\d+" });

            routeBuilder.MapLocalizedRoute("ForumWatch", "boards/forumwatch/{id}",
				new { controller = "Boards", action = "ForumWatch" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("ForumRSS", "boards/forumrss/{id}",
				new { controller = "Boards", action = "ForumRSS" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("ForumSlug", "boards/forum/{id}/{slug?}",
				new { controller = "Boards", action = "Forum" }, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("ForumSlugPaged", "boards/forum/{id}/{slug?}/page/{page?}",
				new { controller = "Boards", action = "Forum" }, new { id = @"\d+", page = @"\d+" });

            routeBuilder.MapLocalizedRoute("ForumGroupSlug", "boards/forumgroup/{id}/{slug?}",
				new { controller = "Boards", action = "ForumGroup"}, new { id = @"\d+" });

            routeBuilder.MapLocalizedRoute("Search", "boards/search",
				new { controller = "Boards", action = "Search" });

            //private messages
            routeBuilder.MapLocalizedRoute("PrivateMessages", "privatemessages/{tab?}",
				new { controller = "PrivateMessages", action = "Index" });

            routeBuilder.MapLocalizedRoute("PrivateMessagesPaged", "privatemessages/{tab?}/page/{page}",
				new { controller = "PrivateMessages", action = "Index" }, new { page = @"\d+" });

            routeBuilder.MapLocalizedRoute("PrivateMessagesInbox", "inboxupdate",
				new { controller = "PrivateMessages", action = "InboxUpdate" });

            routeBuilder.MapLocalizedRoute("PrivateMessagesSent", "sentupdate",
				new { controller = "PrivateMessages", action = "SentUpdate" });

            routeBuilder.MapLocalizedRoute("SendPM", "sendpm/{toCustomerId}",
				new { controller = "PrivateMessages", action = "SendPM" }, new { toCustomerId = @"\d+" });

            routeBuilder.MapLocalizedRoute("SendPMReply", "sendpm/{toCustomerId}/{replyToMessageId}",
				new { controller = "PrivateMessages", action = "SendPM" }, new { toCustomerId = @"\d+", replyToMessageId = @"\d+" });

            routeBuilder.MapLocalizedRoute("ViewPM", "viewpm/{privateMessageId}",
				new { controller = "PrivateMessages", action = "ViewPM" }, new { privateMessageId = @"\d+" });

            routeBuilder.MapLocalizedRoute("DeletePM", "deletepm/{privateMessageId}",
				new { controller = "PrivateMessages", action = "DeletePM" }, new { privateMessageId = @"\d+" });

            //activate newsletters
            routeBuilder.MapLocalizedRoute("NewsletterActivation", "newsletter/subscriptionactivation/{token}/{active}",
				new { controller = "Newsletter", action = "SubscriptionActivation" }, new { token = new GuidConstraint(false) });

            //robots.txt
            routeBuilder.MapRoute("robots.txt", "robots.txt",
				new { controller = "Common", action = "RobotsTextFile" });

            //sitemap (XML)
            routeBuilder.MapLocalizedRoute("sitemap.xml", "sitemap.xml",
				new { controller = "Common", action = "SitemapXml" });

            routeBuilder.MapLocalizedRoute("sitemap-indexed.xml", "sitemap-{Id}.xml",
				new { controller = "Common", action = "SitemapXml" }, new { Id = @"\d+" });

            //store closed
            routeBuilder.MapLocalizedRoute("StoreClosed", "storeclosed",
				new { controller = "Common", action = "StoreClosed" });

            //install
            routeBuilder.MapRoute("Installation", "install",
				new { controller = "Install", action = "Index" });
            
            //page not found
            routeBuilder.MapLocalizedRoute("PageNotFound", "page-not-found", 
                new { controller = "Common", action = "PageNotFound" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider (more the better)
        /// </summary>
        public int Priority
        {
            get { return 0; }
        }

        #endregion
    }
}
