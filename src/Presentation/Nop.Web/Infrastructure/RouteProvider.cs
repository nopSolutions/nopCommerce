using Nop.Services.Installation;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure.Routing;

namespace Nop.Web.Infrastructure;

/// <summary>
/// Represents provider that provided basic routes
/// </summary>
public partial class RouteProvider : BaseRouteProvider, IRouteProvider
{
    #region Methods

    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        //get language pattern
        //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
        //use it only for URLs of pages that the user can go to
        var lang = GetLanguageRoutePattern();

        //areas
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.AreaRoute,
            pattern: $"{{area:exists}}/{{controller=Home}}/{{action=Index}}/{{id?}}");

        //home page
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.Homepage,
            pattern: $"{lang}",
            defaults: new { controller = "Home", action = "Index" });

        //login
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.Login,
            pattern: $"{lang}/login/",
            defaults: new { controller = "Customer", action = "Login" });

        // multi-factor verification digit code page
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.MultiFactorVerification,
            pattern: $"{lang}/multi-factor-verification/",
            defaults: new { controller = "Customer", action = "MultiFactorVerification" });

        //register
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.Register,
            pattern: $"{lang}/register/",
            defaults: new { controller = "Customer", action = "Register" });

        //logout
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.Logout,
            pattern: $"{lang}/logout/",
            defaults: new { controller = "Customer", action = "Logout" });

        //shopping cart
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ShoppingCart,
            pattern: $"{lang}/cart/",
            defaults: new { controller = "ShoppingCart", action = "Cart" });

        //estimate shipping (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.EstimateShipping,
            pattern: $"cart/estimateshipping",
            defaults: new { controller = "ShoppingCart", action = "GetEstimateShipping" });

        //select shipping option (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.SelectShippingOption,
            pattern: $"cart/selectshippingoption",
            defaults: new { controller = "ShoppingCart", action = "SelectShippingOption" });

        //wishlist
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.Wishlist,
            pattern: $"{lang}/wishlist/{{customerGuid?}}",
            defaults: new { controller = "ShoppingCart", action = "Wishlist" });

        //checkout attribute change (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckoutAttributeChange,
            pattern: "shoppingcart/checkoutattributechange/{{isEditable}}",
            defaults: new { controller = "ShoppingCart", action = "CheckoutAttributeChange" });

        //customer account links
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerInfo,
            pattern: $"{lang}/customer/info",
            defaults: new { controller = "Customer", action = "Info" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerAddresses,
            pattern: $"{lang}/customer/addresses",
            defaults: new { controller = "Customer", action = "Addresses" });

        //customer address delete (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerAddressDelete,
            pattern: $"customer/addressdelete",
            defaults: new { controller = "Customer", action = "AddressDelete" });

        //remove external association (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerRemoveExternalAssociation,
            pattern: $"customer/removeexternalassociation",
            defaults: new { controller = "Customer", action = "RemoveExternalAssociation" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerOrders,
            pattern: $"{lang}/order/history/{{limit?}}",
            defaults: new { controller = "Order", action = "CustomerOrders" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerOrdersPaged,
            pattern: $"{lang}/order/history/{{limit?}}/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Order", action = "CustomerOrders" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerRecurringPayments,
            pattern: $"{lang}/customer/recurringpayments",
            defaults: new { controller = "Order", action = "CustomerRecurringPayments" });

        //contact us
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ContactUs,
            pattern: $"{lang}/contactus",
            defaults: new { controller = "Common", action = "ContactUs" });

        //product search
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ProductSearch,
            pattern: $"{lang}/search/",
            defaults: new { controller = "Catalog", action = "Search" });

        //autocomplete search term (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ProductSearchAutoComplete,
            pattern: $"catalog/searchtermautocomplete",
            defaults: new { controller = "Catalog", action = "SearchTermAutoComplete" });

        //change currency
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ChangeCurrency,
            pattern: $"{lang}/changecurrency/{{customercurrency:min(0)}}",
            defaults: new { controller = "Common", action = "SetCurrency" });

        //change language
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ChangeLanguage,
            pattern: $"{lang}/changelanguage/{{langid:min(0)}}",
            defaults: new { controller = "Common", action = "SetLanguage" });

        //change tax
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ChangeTaxType,
            pattern: $"{lang}/changetaxtype/{{customertaxtype:min(0)}}",
            defaults: new { controller = "Common", action = "SetTaxType" });

        //set store theme
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.SetStoreTheme,
            pattern: $"{lang}/setstoretheme/{{themeName}}/{{returnUrl}}",
            defaults: new { controller = "Common", action = "SetStoreTheme" });

        //recently viewed products
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.RecentlyViewedProducts,
            pattern: $"{lang}/recentlyviewedproducts/",
            defaults: new { controller = "Product", action = "RecentlyViewedProducts" });

        //new products
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.NewProducts,
            pattern: $"{lang}/newproducts/",
            defaults: new { controller = "Catalog", action = "NewProducts" });

        //blog
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.Blog,
            pattern: $"{lang}/blog",
            defaults: new { controller = "Blog", action = "List" });

        //news
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.NewsArchive,
            pattern: $"{lang}/news",
            defaults: new { controller = "News", action = "List" });

        //forum
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.Boards,
            pattern: $"{lang}/boards",
            defaults: new { controller = "Boards", action = "Index" });

        //compare products
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CompareProducts,
            pattern: $"{lang}/compareproducts/",
            defaults: new { controller = "Product", action = "CompareProducts" });

        //product tags
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ProductTagsAll,
            pattern: $"{lang}/producttag/all/",
            defaults: new { controller = "Catalog", action = "ProductTagsAll" });

        //manufacturers
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ManufacturerList,
            pattern: $"{lang}/manufacturer/all/",
            defaults: new { controller = "Catalog", action = "ManufacturerAll" });

        //vendors
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.VendorList,
            pattern: $"{lang}/vendor/all/",
            defaults: new { controller = "Catalog", action = "VendorAll" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.VendorReviews,
            pattern: $"{lang}/vendor/{{vendorId:min(0)}}/reviews",
            defaults: new { controller = "Catalog", action = "VendorReviews" });

        //add product to cart (without any attributes and options). used on catalog pages. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.AddProductToCart_Catalog,
            pattern: $"addproducttocart/catalog/{{productId:min(0)}}/{{shoppingCartTypeId:min(0)}}/{{quantity:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "AddProductToCart_Catalog" });

        //add product to cart (with attributes and options). used on the product details pages. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.AddProductToCart_Details,
            pattern: $"addproducttocart/details/{{productId:min(0)}}/{{shoppingCartTypeId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "AddProductToCart_Details" });

        //move product to custom wishlist - catalog/product detail page. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.MoveProductToCustomWishList,
            pattern: $"moveproducttocustomwishlist/{{productId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "MoveProductToCustomWishlist" });

        //move shopping cart to wishlist. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.MoveToCustomWishlist,
            pattern: $"movetocustomwishlist",
            defaults: new { controller = "ShoppingCart", action = "MoveToCustomWishlist" });

        //delete custom wishlist. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.DeleteCustomWishlist,
            pattern: $"deletecustomwishlist/{{wishlistId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "DeleteWishlist" });

        // add custom wishlist. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.AddWishlist,
            pattern: $"addcustomwishlist",
            defaults: new { controller = "ShoppingCart", action = "AddWishlist" });

        //comparing products (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.AddProductToCompare,
            pattern: $"compareproducts/add/{{productId:min(0)}}",
            defaults: new { controller = "Product", action = "AddProductToCompareList" });

        //product email a friend
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ProductEmailAFriend,
            pattern: $"{lang}/productemailafriend/{{productId:min(0)}}",
            defaults: new { controller = "Product", action = "ProductEmailAFriend" });

        //product estimate shipping (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ProductEstimateShipping,
            pattern: "product/estimateshipping/{{ProductId:min(0)}}",
            defaults: new { controller = "Product", action = "EstimateShipping" });

        //reviews
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerProductReviews,
            pattern: $"{lang}/customer/productreviews",
            defaults: new { controller = "Product", action = "CustomerProductReviews" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerProductReviewsPaged,
            pattern: $"{lang}/customer/productreviews/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Product", action = "CustomerProductReviews" });

        //back in stock notifications (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.BackInStockSubscribePopup,
            pattern: $"backinstocksubscribe/{{productId:min(0)}}",
            defaults: new { controller = "BackInStockSubscription", action = "SubscribePopup" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.BackInStockSubscribeSend,
            pattern: $"backinstocksubscribesend/{{productId:min(0)}}",
            defaults: new { controller = "BackInStockSubscription", action = "SubscribePopupPOST" });

        //downloads (file result)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetSampleDownload,
            pattern: $"download/sample/{{productid:min(0)}}",
            defaults: new { controller = "Download", action = "Sample" });

        //downloads
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.DownloadGetFileUpload,
            pattern: $"download/getfileupload/{{downloadId}}",
            defaults: new { controller = "Download", action = "GetFileUpload" });

        //checkout pages
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.Checkout,
            pattern: $"{lang}/checkout/",
            defaults: new { controller = "Checkout", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckoutOnePage,
            pattern: $"{lang}/onepagecheckout/",
            defaults: new { controller = "Checkout", action = "OnePageCheckout" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckoutShippingAddress,
            pattern: $"{lang}/checkout/shippingaddress",
            defaults: new { controller = "Checkout", action = "ShippingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckoutSelectShippingAddress,
            pattern: $"{lang}/checkout/selectshippingaddress",
            defaults: new { controller = "Checkout", action = "SelectShippingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckoutBillingAddress,
            pattern: $"{lang}/checkout/billingaddress",
            defaults: new { controller = "Checkout", action = "BillingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckoutSelectBillingAddress,
            pattern: $"{lang}/checkout/selectbillingaddress",
            defaults: new { controller = "Checkout", action = "SelectBillingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckoutShippingMethod,
            pattern: $"{lang}/checkout/shippingmethod",
            defaults: new { controller = "Checkout", action = "ShippingMethod" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckoutPaymentMethod,
            pattern: $"{lang}/checkout/paymentmethod",
            defaults: new { controller = "Checkout", action = "PaymentMethod" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckoutPaymentInfo,
            pattern: $"{lang}/checkout/paymentinfo",
            defaults: new { controller = "Checkout", action = "PaymentInfo" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckoutConfirm,
            pattern: $"{lang}/checkout/confirm",
            defaults: new { controller = "Checkout", action = "Confirm" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckoutCompleted,
            pattern: $"{lang}/checkout/completed/{{orderId:int?}}",
            defaults: new { controller = "Checkout", action = "Completed" });

        //subscribe newsletters (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.SubscribeNewsletter,
            pattern: $"subscribenewsletter",
            defaults: new { controller = "Newsletter", action = "SubscribeNewsletter" });

        //email wishlist
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.EmailWishlist,
            pattern: $"{lang}/emailwishlist",
            defaults: new { controller = "ShoppingCart", action = "EmailWishlist" });

        //login page for checkout as guest
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.LoginCheckoutAsGuest,
            pattern: $"{lang}/login/checkoutasguest",
            defaults: new { controller = "Customer", action = "Login", checkoutAsGuest = true });

        //register result page
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.RegisterResult,
            pattern: $"{lang}/registerresult/{{resultId:min(0)}}",
            defaults: new { controller = "Customer", action = "RegisterResult" });

        //check username availability (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckUsernameAvailability,
            pattern: $"customer/checkusernameavailability",
            defaults: new { controller = "Customer", action = "CheckUsernameAvailability" });

        //passwordrecovery
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PasswordRecovery,
            pattern: $"{lang}/passwordrecovery",
            defaults: new { controller = "Customer", action = "PasswordRecovery" });

        //password recovery confirmation
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PasswordRecoveryConfirm,
            pattern: $"{lang}/passwordrecovery/confirm",
            defaults: new { controller = "Customer", action = "PasswordRecoveryConfirm" });

        //topics (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.TopicPopup,
            pattern: $"t-popup/{{SystemName}}",
            defaults: new { controller = "Topic", action = "TopicDetailsPopup" });

        //blog
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.BlogByTag,
            pattern: $"{lang}/blog/tag/{{tag}}",
            defaults: new { controller = "Blog", action = "BlogByTag" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.BlogByMonth,
            pattern: $"{lang}/blog/month/{{month}}",
            defaults: new { controller = "Blog", action = "BlogByMonth" });

        //blog RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.BlogRSS,
            pattern: $"blog/rss/{{languageId:min(0)}}",
            defaults: new { controller = "Blog", action = "ListRss" });

        //news RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.NewsRSS,
            pattern: $"news/rss/{{languageId:min(0)}}",
            defaults: new { controller = "News", action = "ListRss" });

        //set review helpfulness (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.SetProductReviewHelpfulness,
            pattern: $"setproductreviewhelpfulness",
            defaults: new { controller = "Product", action = "SetProductReviewHelpfulness" });

        //customer account links
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerReturnRequests,
            pattern: $"{lang}/returnrequest/history",
            defaults: new { controller = "ReturnRequest", action = "CustomerReturnRequests" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerDownloadableProducts,
            pattern: $"{lang}/customer/downloadableproducts",
            defaults: new { controller = "Customer", action = "DownloadableProducts" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerBackInStockSubscriptions,
            pattern: $"{lang}/backinstocksubscriptions/manage/{{pageNumber:int?}}",
            defaults: new { controller = "BackInStockSubscription", action = "CustomerSubscriptions" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerRewardPoints,
            pattern: $"{lang}/rewardpoints/history",
            defaults: new { controller = "Order", action = "CustomerRewardPoints" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerRewardPointsPaged,
            pattern: $"{lang}/rewardpoints/history/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Order", action = "CustomerRewardPoints" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerChangePassword,
            pattern: $"{lang}/customer/changepassword",
            defaults: new { controller = "Customer", action = "ChangePassword" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerAvatar,
            pattern: $"{lang}/customer/avatar",
            defaults: new { controller = "Customer", action = "Avatar" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.AccountActivation,
            pattern: $"{lang}/customer/activation",
            defaults: new { controller = "Customer", action = "AccountActivation" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.EmailRevalidation,
            pattern: $"{lang}/customer/revalidateemail",
            defaults: new { controller = "Customer", action = "EmailRevalidation" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerForumSubscriptions,
            pattern: $"{lang}/boards/forumsubscriptions/{{pageNumber:int?}}",
            defaults: new { controller = "Boards", action = "CustomerForumSubscriptions" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerAddressEdit,
            pattern: $"{lang}/customer/addressedit/{{addressId:min(0)}}",
            defaults: new { controller = "Customer", action = "AddressEdit" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerAddressAdd,
            pattern: $"{lang}/customer/addressadd",
            defaults: new { controller = "Customer", action = "AddressAdd" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerMultiFactorAuthenticationProviderConfig,
            pattern: $"{lang}/customer/providerconfig",
            defaults: new { controller = "Customer", action = "ConfigureMultiFactorAuthenticationProvider" });

        //customer profile page
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerProfile,
            pattern: $"{lang}/profile/{{id:min(0)}}",
            defaults: new { controller = "Profile", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerProfilePaged,
            pattern: $"{lang}/profile/{{id:min(0)}}/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Profile", action = "Index" });

        //orders
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.OrderDetails,
            pattern: $"{lang}/orderdetails/{{orderId:min(0)}}",
            defaults: new { controller = "Order", action = "Details" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ShipmentDetails,
            pattern: $"{lang}/orderdetails/shipment/{{shipmentId}}",
            defaults: new { controller = "Order", action = "ShipmentDetails" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ReturnRequest,
            pattern: $"{lang}/returnrequest/{{orderId:min(0)}}",
            defaults: new { controller = "ReturnRequest", action = "ReturnRequest" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ReOrder,
            pattern: $"{lang}/reorder/{{orderId:min(0)}}",
            defaults: new { controller = "Order", action = "ReOrder" });

        //pdf invoice (file result)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetOrderPdfInvoice,
            pattern: $"orderdetails/pdf/{{orderId}}",
            defaults: new { controller = "Order", action = "GetPdfInvoice" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PrintOrderDetails,
            pattern: $"{lang}/orderdetails/print/{{orderId}}",
            defaults: new { controller = "Order", action = "PrintOrderDetails" });

        //order downloads (file result)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetDownload,
            pattern: $"download/getdownload/{{orderItemId:guid}}/{{agree?}}",
            defaults: new { controller = "Download", action = "GetDownload" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetLicense,
            pattern: $"download/getlicense/{{orderItemId:guid}}/",
            defaults: new { controller = "Download", action = "GetLicense" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.DownloadUserAgreement,
            pattern: $"customer/useragreement/{{orderItemId:guid}}",
            defaults: new { controller = "Customer", action = "UserAgreement" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetOrderNoteFile,
            pattern: $"download/ordernotefile/{{ordernoteid:min(0)}}",
            defaults: new { controller = "Download", action = "GetOrderNoteFile" });

        //contact vendor
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ContactVendor,
            pattern: $"{lang}/contactvendor/{{vendorId}}",
            defaults: new { controller = "Common", action = "ContactVendor" });

        //apply for vendor account
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ApplyVendorAccount,
            pattern: $"{lang}/vendor/apply",
            defaults: new { controller = "Vendor", action = "ApplyVendor" });

        //vendor info
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CustomerVendorInfo,
            pattern: $"{lang}/customer/vendorinfo",
            defaults: new { controller = "Vendor", action = "Info" });

        //customer GDPR
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GdprTools,
            pattern: $"{lang}/customer/gdpr",
            defaults: new { controller = "Customer", action = "GdprTools" });

        //customer check gift card balance 
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.CheckGiftCardBalance,
            pattern: $"{lang}/customer/checkgiftcardbalance",
            defaults: new { controller = "Customer", action = "CheckGiftCardBalance" });

        //customer multi-factor authentication settings 
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.MultiFactorAuthenticationSettings,
            pattern: $"{lang}/customer/multifactorauthentication",
            defaults: new { controller = "Customer", action = "MultiFactorAuthentication" });

        //poll vote (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PollVote,
            pattern: $"poll/vote",
            defaults: new { controller = "Poll", action = "Vote" });

        //comparing products
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.RemoveProductFromCompareList,
            pattern: $"{lang}/compareproducts/remove/{{productId}}",
            defaults: new { controller = "Product", action = "RemoveProductFromCompareList" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ClearCompareList,
            pattern: $"{lang}/clearcomparelist/",
            defaults: new { controller = "Product", action = "ClearCompareList" });

        //new RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.NewProductsRSS,
            pattern: $"newproducts/rss",
            defaults: new { controller = "Catalog", action = "NewProductsRss" });

        //get state list by country ID (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetStatesByCountryId,
            pattern: $"country/getstatesbycountryid/",
            defaults: new { controller = "Country", action = "GetStatesByCountryId" });

        //EU Cookie law accept button handler (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.EuCookieLawAccept,
            pattern: $"eucookielawaccept",
            defaults: new { controller = "Common", action = "EuCookieLawAccept" });

        //authenticate topic (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.TopicAuthenticate,
            pattern: $"topic/authenticate",
            defaults: new { controller = "Topic", action = "Authenticate" });

        //prepare top menu (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetCatalogRoot,
            pattern: $"catalog/getcatalogroot",
            defaults: new { controller = "Catalog", action = "GetCatalogRoot" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetCatalogSubCategories,
            pattern: $"catalog/getcatalogsubcategories",
            defaults: new { controller = "Catalog", action = "GetCatalogSubCategories" });

        //Catalog products (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetCategoryProducts,
            pattern: $"category/products/",
            defaults: new { controller = "Catalog", action = "GetCategoryProducts" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetManufacturerProducts,
            pattern: $"manufacturer/products/",
            defaults: new { controller = "Catalog", action = "GetManufacturerProducts" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetTagProducts,
            pattern: $"tag/products",
            defaults: new { controller = "Catalog", action = "GetTagProducts" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.SearchProducts,
            pattern: "product/search",
            defaults: new { controller = "Catalog", action = "SearchProducts" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetVendorProducts,
            pattern: $"vendor/products",
            defaults: new { controller = "Catalog", action = "GetVendorProducts" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetNewProducts,
            pattern: $"newproducts/products/",
            defaults: new { controller = "Catalog", action = "GetNewProducts" });

        //product combinations (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.GetProductCombinations,
            pattern: $"product/combinations",
            defaults: new { controller = "Product", action = "GetProductCombinations" });

        //product attributes with "upload file" type (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.UploadFileProductAttribute,
            pattern: $"uploadfileproductattribute/{{attributeId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "UploadFileProductAttribute" });

        //checkout attributes with "upload file" type (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.UploadFileCheckoutAttribute,
            pattern: $"uploadfilecheckoutattribute/{{attributeId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" });

        //attribute change (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ProductDetailsAttributeChange,
            pattern: $"shoppingcart/productdetails_attributechange/{{productId:min(0)}}/{{validateAttributeConditions}}/{{loadPicture}}",
            defaults: new { controller = "ShoppingCart", action = "ProductDetails_AttributeChange" });

        //return request with "upload file" support (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.UploadFileReturnRequest,
            pattern: $"uploadfilereturnrequest",
            defaults: new { controller = "ReturnRequest", action = "UploadFileReturnRequest" });

        //forums
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ActiveDiscussions,
            pattern: $"{lang}/boards/activediscussions",
            defaults: new { controller = "Boards", action = "ActiveDiscussions" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ActiveDiscussionsPaged,
            pattern: $"{lang}/boards/activediscussions/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "ActiveDiscussions" });

        //forums RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ActiveDiscussionsRSS,
            pattern: $"boards/activediscussionsrss",
            defaults: new { controller = "Boards", action = "ActiveDiscussionsRSS" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PostEdit,
            pattern: $"{lang}/boards/postedit/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "PostEdit" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PostDelete,
            pattern: $"{lang}/boards/postdelete/{{id:int?}}",
            defaults: new { controller = "Boards", action = "PostDelete" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PostCreate,
            pattern: $"{lang}/boards/postcreate/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "PostCreate" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PostCreateQuote,
            pattern: $"{lang}/boards/postcreate/{{id:min(0)}}/{{quote:min(0)}}",
            defaults: new { controller = "Boards", action = "PostCreate" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.TopicEdit,
            pattern: $"{lang}/boards/topicedit/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicEdit" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.TopicDelete,
            pattern: $"{lang}/boards/topicdelete/{{id:int?}}",
            defaults: new { controller = "Boards", action = "TopicDelete" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.TopicCreate,
            pattern: $"{lang}/boards/topiccreate/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicCreate" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.TopicMove,
            pattern: $"{lang}/boards/topicmove/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicMove" });

        //topic watch (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.TopicWatch,
            pattern: $"boards/topicwatch/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicWatch" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.TopicSlug,
            pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "Topic" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.TopicSlugPaged,
            pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "Topic" });

        //forum watch (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ForumWatch,
            pattern: $"boards/forumwatch/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "ForumWatch" });

        //forums RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ForumRSS,
            pattern: $"boards/forumrss/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "ForumRSS" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ForumSlug,
            pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "Forum" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ForumSlugPaged,
            pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "Forum" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ForumGroupSlug,
            pattern: $"{lang}/boards/forumgroup/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "ForumGroup" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.Search,
            pattern: $"{lang}/boards/search",
            defaults: new { controller = "Boards", action = "Search" });

        //post vote (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PostVote,
            pattern: "boards/postvote",
            defaults: new { controller = "Boards", action = "PostVote" });

        //private messages
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PrivateMessages,
            pattern: $"{lang}/privatemessages/{{tab?}}",
            defaults: new { controller = "PrivateMessages", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PrivateMessagesPaged,
            pattern: $"{lang}/privatemessages/{{tab?}}/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PrivateMessagesInbox,
            pattern: $"{lang}/inboxupdate",
            defaults: new { controller = "PrivateMessages", action = "InboxUpdate" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PrivateMessagesSent,
            pattern: $"{lang}/sentupdate",
            defaults: new { controller = "PrivateMessages", action = "SentUpdate" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.SendPM,
            pattern: $"{lang}/sendpm/{{toCustomerId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "SendPM" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.SendPMReply,
            pattern: $"{lang}/sendpm/{{toCustomerId:min(0)}}/{{replyToMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "SendPM" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.ViewPM,
            pattern: $"{lang}/viewpm/{{privateMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "ViewPM" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.DeletePM,
            pattern: $"{lang}/deletepm/{{privateMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "DeletePM" });

        //activate newsletters
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.NewsletterActivation,
            pattern: $"{lang}/newsletter/subscriptionactivation/{{token:guid}}/{{active}}",
            defaults: new { controller = "Newsletter", action = "SubscriptionActivation" });

        //robots.txt (file result)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.RobotsTxt,
            pattern: $"robots.txt",
            defaults: new { controller = "Common", action = "RobotsTextFile" });

        //sitemap
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.Sitemap,
            pattern: $"{lang}/sitemap",
            defaults: new { controller = "Common", action = "Sitemap" });

        //sitemap.xml (file result)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.SitemapXml,
            pattern: $"sitemap.xml",
            defaults: new { controller = "Common", action = "SitemapXml" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.SitemapIndexedXml,
            pattern: $"sitemap-{{Id:min(0)}}.xml",
            defaults: new { controller = "Common", action = "SitemapXml" });

        //store closed
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.StoreClosed,
            pattern: $"{lang}/storeclosed",
            defaults: new { controller = "Common", action = "StoreClosed" });

        //install
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.Installation,
            pattern: $"{NopInstallationDefaults.InstallPath}",
            defaults: new { controller = "Install", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: RouteNames.InstallationChangeLanguage,
            pattern: $"{NopInstallationDefaults.InstallPath}/ChangeLanguage/{{language}}",
            defaults: new { controller = "Install", action = "ChangeLanguage" });

        //restart application (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.InstallationRestartApplication,
            pattern: $"{NopInstallationDefaults.InstallPath}/restartapplication",
            defaults: new { controller = "Install", action = "RestartApplication" });

        //page not found
        endpointRouteBuilder.MapControllerRoute(name: RouteNames.PageNotFound,
            pattern: $"{lang}/page-not-found",
            defaults: new { controller = "Common", action = "PageNotFound" });

        //fallback is intended to handle cases when no other endpoint has matched
        //we use it to invoke [CheckLanguageSeoCode] and give a chance to find a localized route
        endpointRouteBuilder.MapFallbackToController(RouteNames.FallbackRedirect, "Common");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;

    #endregion
}
