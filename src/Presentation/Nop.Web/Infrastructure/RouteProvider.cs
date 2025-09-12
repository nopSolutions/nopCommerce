using Nop.Core.Http;
using Nop.Services.Installation;
using Nop.Web.Framework.Mvc.Routing;

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
    public virtual void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        //get language pattern
        //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
        //use it only for URLs of pages that the user can go to
        var lang = GetLanguageRoutePattern();

        //areas
        endpointRouteBuilder.MapControllerRoute(name: "areaRoute",
            pattern: $"{{area:exists}}/{{controller=Home}}/{{action=Index}}/{{id?}}");

        //home page
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.HOMEPAGE,
            pattern: $"{lang}",
            defaults: new { controller = "Home", action = "Index" });

        //login
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.LOGIN,
            pattern: $"{lang}/login/",
            defaults: new { controller = "Customer", action = "Login" });

        // multi-factor verification digit code page
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.MULTIFACTOR_VERIFICATION,
            pattern: $"{lang}/multi-factor-verification/",
            defaults: new { controller = "Customer", action = "MultiFactorVerification" });

        //register
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.REGISTER,
            pattern: $"{lang}/register/",
            defaults: new { controller = "Customer", action = "Register" });

        //logout
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.LOGOUT,
            pattern: $"{lang}/logout/",
            defaults: new { controller = "Customer", action = "Logout" });

        //shopping cart
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.CART,
            pattern: $"{lang}/cart/",
            defaults: new { controller = "ShoppingCart", action = "Cart" });

        //estimate shipping (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.ESTIMATE_SHIPPING,
            pattern: $"cart/estimateshipping",
            defaults: new { controller = "ShoppingCart", action = "GetEstimateShipping" });

        //select shipping option (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.SELECT_SHIPPING_OPTION,
            pattern: $"cart/selectshippingoption",
            defaults: new { controller = "ShoppingCart", action = "SelectShippingOption" });

        //wishlist
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.WISHLIST,
            pattern: $"{lang}/wishlist/{{customerGuid?}}",
            defaults: new { controller = "ShoppingCart", action = "Wishlist" });

        //checkout attribute change (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.CHECKOUT_ATTRIBUTE_CHANGE,
            pattern: "shoppingcart/checkoutattributechange/{{isEditable}}",
            defaults: new { controller = "ShoppingCart", action = "CheckoutAttributeChange" });

        //customer account links
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.CUSTOMER_INFO,
            pattern: $"{lang}/customer/info",
            defaults: new { controller = "Customer", action = "Info" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.CUSTOMER_ADDRESSES,
            pattern: $"{lang}/customer/addresses",
            defaults: new { controller = "Customer", action = "Addresses" });

        //customer address delete (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.CUSTOMER_ADDRESS_DELETE,
            pattern: $"customer/addressdelete",
            defaults: new { controller = "Customer", action = "AddressDelete" });

        //remove external association (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.CUSTOMER_REMOVE_EXTERNAL_ASSOCIATION,
            pattern: $"customer/removeexternalassociation",
            defaults: new { controller = "Customer", action = "RemoveExternalAssociation" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.CUSTOMER_ORDERS,
            pattern: $"{lang}/order/history/{{limit?}}",
            defaults: new { controller = "Order", action = "CustomerOrders" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_ORDERS_PAGED,
            pattern: $"{lang}/order/history/{{limit?}}/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Order", action = "CustomerOrders" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_RECURRING_PAYMENTS,
            pattern: $"{lang}/customer/recurringpayments",
            defaults: new { controller = "Order", action = "CustomerRecurringPayments" });

        //contact us
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.CONTACT_US,
            pattern: $"{lang}/contactus",
            defaults: new { controller = "Common", action = "ContactUs" });

        //product search
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.SEARCH,
            pattern: $"{lang}/search/",
            defaults: new { controller = "Catalog", action = "Search" });

        //autocomplete search term (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.PRODUCT_SEARCH_AUTOCOMPLETE,
            pattern: $"catalog/searchtermautocomplete",
            defaults: new { controller = "Catalog", action = "SearchTermAutoComplete" });

        //change currency
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHANGE_CURRENCY,
            pattern: $"{lang}/changecurrency/{{customercurrency:min(0)}}",
            defaults: new { controller = "Common", action = "SetCurrency" });

        //change language
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHANGE_LANGUAGE,
            pattern: $"{lang}/changelanguage/{{langid:min(0)}}",
            defaults: new { controller = "Common", action = "SetLanguage" });

        //change tax
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHANGE_TAX_TYPE,
            pattern: $"{lang}/changetaxtype/{{customertaxtype:min(0)}}",
            defaults: new { controller = "Common", action = "SetTaxType" });

        //set store theme
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.SET_STORE_THEME,
            pattern: $"{lang}/setstoretheme/{{themeName}}/{{returnUrl}}",
            defaults: new { controller = "Common", action = "SetStoreTheme" });

        //recently viewed products
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.RECENTLY_VIEWED_PRODUCTS,
            pattern: $"{lang}/recentlyviewedproducts/",
            defaults: new { controller = "Product", action = "RecentlyViewedProducts" });

        //new products
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.NEW_PRODUCTS,
            pattern: $"{lang}/newproducts/",
            defaults: new { controller = "Catalog", action = "NewProducts" });

        //blog
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.BLOG,
            pattern: $"{lang}/blog",
            defaults: new { controller = "Blog", action = "List" });

        //news
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.NEWS,
            pattern: $"{lang}/news",
            defaults: new { controller = "News", action = "List" });

        //forum
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.BOARDS,
            pattern: $"{lang}/boards",
            defaults: new { controller = "Boards", action = "Index" });

        //compare products
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.COMPARE_PRODUCTS,
            pattern: $"{lang}/compareproducts/",
            defaults: new { controller = "Product", action = "CompareProducts" });

        //product tags
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.PRODUCT_TAGS,
            pattern: $"{lang}/producttag/all/",
            defaults: new { controller = "Catalog", action = "ProductTagsAll" });

        //manufacturers
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.MANUFACTURERS,
            pattern: $"{lang}/manufacturer/all/",
            defaults: new { controller = "Catalog", action = "ManufacturerAll" });

        //vendors
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.VENDORS,
            pattern: $"{lang}/vendor/all/",
            defaults: new { controller = "Catalog", action = "VendorAll" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.VENDOR_REVIEWS,
            pattern: $"{lang}/vendor/{{vendorId:min(0)}}/reviews",
            defaults: new { controller = "Catalog", action = "VendorReviews" });

        //add product to cart (without any attributes and options). used on catalog pages. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.ADD_PRODUCT_TO_CART_CATALOG,
            pattern: $"addproducttocart/catalog/{{productId:min(0)}}/{{shoppingCartTypeId:min(0)}}/{{quantity:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "AddProductToCart_Catalog" });

        //add product to cart (with attributes and options). used on the product details pages. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.ADD_PRODUCT_TO_CART_DETAILS,
            pattern: $"addproducttocart/details/{{productId:min(0)}}/{{shoppingCartTypeId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "AddProductToCart_Details" });

        //move product to custom wishlist - catalog/product detail page. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.MOVE_PRODUCT_TO_CUSTOM_WISHLIST,
            pattern: $"moveproducttocustomwishlist/{{productId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "MoveProductToCustomWishlist" });

        //move shopping cart to wishlist. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.MOVE_CART_TO_CUSTOM_WISHLIST,
            pattern: $"movetocustomwishlist",
            defaults: new { controller = "ShoppingCart", action = "MoveToCustomWishlist" });

        //delete custom wishlist. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.DELETE_CUSTOM_WISHLIST,
            pattern: $"deletecustomwishlist/{{wishlistId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "DeleteWishlist" });

        // add custom wishlist. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.ADD_WISHLIST,
            pattern: $"addcustomwishlist",
            defaults: new { controller = "ShoppingCart", action = "AddWishlist" });

        //comparing products (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.ADD_PRODUCT_TO_COMPARE,
            pattern: $"compareproducts/add/{{productId:min(0)}}",
            defaults: new { controller = "Product", action = "AddProductToCompareList" });

        //product email a friend
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PRODUCT_EMAIL_FRIEND,
            pattern: $"{lang}/productemailafriend/{{productId:min(0)}}",
            defaults: new { controller = "Product", action = "ProductEmailAFriend" });

        //product estimate shipping (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.PRODUCT_ESTIMATE_SHIPPING,
            pattern: "product/estimateshipping/{{ProductId:min(0)}}",
            defaults: new { controller = "Product", action = "EstimateShipping" });

        //reviews
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_PRODUCT_REVIEWS,
            pattern: $"{lang}/customer/productreviews",
            defaults: new { controller = "Product", action = "CustomerProductReviews" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_PRODUCT_REVIEWS_PAGED,
            pattern: $"{lang}/customer/productreviews/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Product", action = "CustomerProductReviews" });

        //back in stock notifications (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.BACK_IN_STOCK_SUBSCRIBE_POPUP,
            pattern: $"backinstocksubscribe/{{productId:min(0)}}",
            defaults: new { controller = "BackInStockSubscription", action = "SubscribePopup" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.BACK_IN_STOCK_SUBSCRIBE_SEND,
            pattern: $"backinstocksubscribesend/{{productId:min(0)}}",
            defaults: new { controller = "BackInStockSubscription", action = "SubscribePopupPOST" });

        //downloads (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.GET_SAMPLE_DOWNLOAD,
            pattern: $"download/sample/{{productid:min(0)}}",
            defaults: new { controller = "Download", action = "Sample" });

        //downloads
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.DOWNLOAD_GET_FILE_UPLOAD,
            pattern: $"download/getfileupload/{{downloadId}}",
            defaults: new { controller = "Download", action = "GetFileUpload" });

        //checkout pages
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHECKOUT,
            pattern: $"{lang}/checkout/",
            defaults: new { controller = "Checkout", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHECKOUT_ONE_PAGE,
            pattern: $"{lang}/onepagecheckout/",
            defaults: new { controller = "Checkout", action = "OnePageCheckout" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHECKOUT_SHIPPING_ADDRESS,
            pattern: $"{lang}/checkout/shippingaddress",
            defaults: new { controller = "Checkout", action = "ShippingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHECKOUT_SELECT_SHIPPING_ADDRESS,
            pattern: $"{lang}/checkout/selectshippingaddress",
            defaults: new { controller = "Checkout", action = "SelectShippingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHECKOUT_BILLING_ADDRESS,
            pattern: $"{lang}/checkout/billingaddress",
            defaults: new { controller = "Checkout", action = "BillingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHECKOUT_SELECT_BILLING_ADDRESS,
            pattern: $"{lang}/checkout/selectbillingaddress",
            defaults: new { controller = "Checkout", action = "SelectBillingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHECKOUT_SHIPPING_METHOD,
            pattern: $"{lang}/checkout/shippingmethod",
            defaults: new { controller = "Checkout", action = "ShippingMethod" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHECKOUT_PAYMENT_METHOD,
            pattern: $"{lang}/checkout/paymentmethod",
            defaults: new { controller = "Checkout", action = "PaymentMethod" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHECKOUT_PAYMENT_INFO,
            pattern: $"{lang}/checkout/paymentinfo",
            defaults: new { controller = "Checkout", action = "PaymentInfo" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHECKOUT_CONFIRM,
            pattern: $"{lang}/checkout/confirm",
            defaults: new { controller = "Checkout", action = "Confirm" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CHECKOUT_COMPLETED,
            pattern: $"{lang}/checkout/completed/{{orderId:int?}}",
            defaults: new { controller = "Checkout", action = "Completed" });

        //subscribe newsletters (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.SUBSCRIBE_NEWSLETTER,
            pattern: $"subscribenewsletter",
            defaults: new { controller = "Newsletter", action = "SubscribeNewsletter" });

        //email wishlist
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.EMAIL_WISHLIST,
            pattern: $"{lang}/emailwishlist",
            defaults: new { controller = "ShoppingCart", action = "EmailWishlist" });

        //login page for checkout as guest
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.LOGIN_CHECKOUT_AS_GUEST,
            pattern: $"{lang}/login/checkoutasguest",
            defaults: new { controller = "Customer", action = "Login", checkoutAsGuest = true });

        //register result page
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.REGISTER_RESULT,
            pattern: $"{lang}/registerresult/{{resultId:min(0)}}",
            defaults: new { controller = "Customer", action = "RegisterResult" });

        //check username availability (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.CHECK_USERNAME_AVAILABILITY,
            pattern: $"customer/checkusernameavailability",
            defaults: new { controller = "Customer", action = "CheckUsernameAvailability" });

        //passwordrecovery
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PASSWORD_RECOVERY,
            pattern: $"{lang}/passwordrecovery",
            defaults: new { controller = "Customer", action = "PasswordRecovery" });

        //password recovery confirmation
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PASSWORD_RECOVERY_CONFIRM,
            pattern: $"{lang}/passwordrecovery/confirm",
            defaults: new { controller = "Customer", action = "PasswordRecoveryConfirm" });

        //topics (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.TOPIC_POPUP,
            pattern: $"t-popup/{{SystemName}}",
            defaults: new { controller = "Topic", action = "TopicDetailsPopup" });

        //blog
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BLOG_BY_TAG,
            pattern: $"{lang}/blog/tag/{{tag}}",
            defaults: new { controller = "Blog", action = "BlogByTag" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BLOG_BY_MONTH,
            pattern: $"{lang}/blog/month/{{month}}",
            defaults: new { controller = "Blog", action = "BlogByMonth" });

        //blog RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BLOG_RSS,
            pattern: $"blog/rss/{{languageId:min(0)}}",
            defaults: new { controller = "Blog", action = "ListRss" });

        //news RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.NEWS_RSS,
            pattern: $"news/rss/{{languageId:min(0)}}",
            defaults: new { controller = "News", action = "ListRss" });

        //set review helpfulness (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.SET_PRODUCT_REVIEW_HELPFULNESS,
            pattern: $"setproductreviewhelpfulness",
            defaults: new { controller = "Product", action = "SetProductReviewHelpfulness" });

        //customer account links
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_RETURN_REQUESTS,
            pattern: $"{lang}/returnrequest/history",
            defaults: new { controller = "ReturnRequest", action = "CustomerReturnRequests" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_DOWNLOADABLE_PRODUCTS,
            pattern: $"{lang}/customer/downloadableproducts",
            defaults: new { controller = "Customer", action = "DownloadableProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_BACK_IN_STOCK_SUBSCRIPTIONS,
            pattern: $"{lang}/backinstocksubscriptions/manage/{{pageNumber:int?}}",
            defaults: new { controller = "BackInStockSubscription", action = "CustomerSubscriptions" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_REWARD_POINTS,
            pattern: $"{lang}/rewardpoints/history",
            defaults: new { controller = "Order", action = "CustomerRewardPoints" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_REWARD_POINTS_PAGED,
            pattern: $"{lang}/rewardpoints/history/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Order", action = "CustomerRewardPoints" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_CHANGE_PASSWORD,
            pattern: $"{lang}/customer/changepassword",
            defaults: new { controller = "Customer", action = "ChangePassword" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_AVATAR,
            pattern: $"{lang}/customer/avatar",
            defaults: new { controller = "Customer", action = "Avatar" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.ACCOUNT_ACTIVATION,
            pattern: $"{lang}/customer/activation",
            defaults: new { controller = "Customer", action = "AccountActivation" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.EMAIL_REVALIDATION,
            pattern: $"{lang}/customer/revalidateemail",
            defaults: new { controller = "Customer", action = "EmailRevalidation" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_FORUM_SUBSCRIPTIONS,
            pattern: $"{lang}/boards/forumsubscriptions/{{pageNumber:int?}}",
            defaults: new { controller = "Boards", action = "CustomerForumSubscriptions" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_ADDRESS_EDIT,
            pattern: $"{lang}/customer/addressedit/{{addressId:min(0)}}",
            defaults: new { controller = "Customer", action = "AddressEdit" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_ADDRESS_ADD,
            pattern: $"{lang}/customer/addressadd",
            defaults: new { controller = "Customer", action = "AddressAdd" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_MULTI_FACTOR_AUTHENTICATION_PROVIDER_CONFIG,
            pattern: $"{lang}/customer/providerconfig",
            defaults: new { controller = "Customer", action = "ConfigureMultiFactorAuthenticationProvider" });

        //customer profile page
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_PROFILE,
            pattern: $"{lang}/profile/{{id:min(0)}}",
            defaults: new { controller = "Profile", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_PROFILE_PAGED,
            pattern: $"{lang}/profile/{{id:min(0)}}/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Profile", action = "Index" });

        //orders
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.ORDER_DETAILS,
            pattern: $"{lang}/orderdetails/{{orderId:min(0)}}",
            defaults: new { controller = "Order", action = "Details" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.SHIPMENT_DETAILS,
            pattern: $"{lang}/orderdetails/shipment/{{shipmentId}}",
            defaults: new { controller = "Order", action = "ShipmentDetails" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.RETURN_REQUEST,
            pattern: $"{lang}/returnrequest/{{orderId:min(0)}}",
            defaults: new { controller = "ReturnRequest", action = "ReturnRequest" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.RE_ORDER,
            pattern: $"{lang}/reorder/{{orderId:min(0)}}",
            defaults: new { controller = "Order", action = "ReOrder" });

        //pdf invoice (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.GET_ORDER_PDF_INVOICE,
            pattern: $"orderdetails/pdf/{{orderId}}",
            defaults: new { controller = "Order", action = "GetPdfInvoice" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PRINT_ORDER_DETAILS,
            pattern: $"{lang}/orderdetails/print/{{orderId}}",
            defaults: new { controller = "Order", action = "PrintOrderDetails" });

        //cancel order
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CANCEL_ORDER,
            pattern: $"{lang}/orderdetails/cancelorder/{{orderId}}",
            defaults: new { controller = "Order", action = "CancelOrder" });

        //order downloads (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.GET_DOWNLOAD,
            pattern: $"download/getdownload/{{orderItemId:guid}}/{{agree?}}",
            defaults: new { controller = "Download", action = "GetDownload" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.GET_LICENSE,
            pattern: $"download/getlicense/{{orderItemId:guid}}/",
            defaults: new { controller = "Download", action = "GetLicense" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.DOWNLOAD_USER_AGREEMENT,
            pattern: $"customer/useragreement/{{orderItemId:guid}}",
            defaults: new { controller = "Customer", action = "UserAgreement" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.GET_ORDER_NOTE_FILE,
            pattern: $"download/ordernotefile/{{ordernoteid:min(0)}}",
            defaults: new { controller = "Download", action = "GetOrderNoteFile" });

        //contact vendor
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CONTACT_VENDOR,
            pattern: $"{lang}/contactvendor/{{vendorId}}",
            defaults: new { controller = "Common", action = "ContactVendor" });

        //apply for vendor account
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.APPLY_VENDOR_ACCOUNT,
            pattern: $"{lang}/vendor/apply",
            defaults: new { controller = "Vendor", action = "ApplyVendor" });

        //vendor info
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CUSTOMER_VENDOR_INFO,
            pattern: $"{lang}/customer/vendorinfo",
            defaults: new { controller = "Vendor", action = "Info" });

        //customer GDPR
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.GDPR_TOOLS,
            pattern: $"{lang}/customer/gdpr",
            defaults: new { controller = "Customer", action = "GdprTools" });

        //customer check gift card balance 
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.CHECK_GIFT_CARD_BALANCE,
            pattern: $"{lang}/customer/checkgiftcardbalance",
            defaults: new { controller = "Customer", action = "CheckGiftCardBalance" });

        //customer multi-factor authentication settings 
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.MULTI_FACTOR_AUTHENTICATION_SETTINGS,
            pattern: $"{lang}/customer/multifactorauthentication",
            defaults: new { controller = "Customer", action = "MultiFactorAuthentication" });

        //poll vote (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.POLL_VOTE,
            pattern: $"poll/vote",
            defaults: new { controller = "Poll", action = "Vote" });

        //comparing products
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.REMOVE_PRODUCT_FROM_COMPARE_LIST,
            pattern: $"{lang}/compareproducts/remove/{{productId}}",
            defaults: new { controller = "Product", action = "RemoveProductFromCompareList" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.CLEAR_COMPARE_LIST,
            pattern: $"{lang}/clearcomparelist/",
            defaults: new { controller = "Product", action = "ClearCompareList" });

        //new RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.NEW_PRODUCTS_RSS,
            pattern: $"newproducts/rss",
            defaults: new { controller = "Catalog", action = "NewProductsRss" });

        //get state list by country ID (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_STATES_BY_COUNTRY_ID,
            pattern: $"country/getstatesbycountryid/",
            defaults: new { controller = "Country", action = "GetStatesByCountryId" });

        //EU Cookie law accept button handler (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.EU_COOKIE_LAW_ACCEPT,
            pattern: $"eucookielawaccept",
            defaults: new { controller = "Common", action = "EuCookieLawAccept" });

        //authenticate topic (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.TOPIC_AUTHENTICATE,
            pattern: $"topic/authenticate",
            defaults: new { controller = "Topic", action = "Authenticate" });

        //Catalog products (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_CATEGORY_PRODUCTS,
            pattern: $"category/products/",
            defaults: new { controller = "Catalog", action = "GetCategoryProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_MANUFACTURER_PRODUCTS,
            pattern: $"manufacturer/products/",
            defaults: new { controller = "Catalog", action = "GetManufacturerProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_TAG_PRODUCTS,
            pattern: $"tag/products",
            defaults: new { controller = "Catalog", action = "GetTagProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.SEARCH_PRODUCTS,
            pattern: "product/search",
            defaults: new { controller = "Catalog", action = "SearchProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_VENDOR_PRODUCTS,
            pattern: $"vendor/products",
            defaults: new { controller = "Catalog", action = "GetVendorProducts" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_NEW_PRODUCTS,
            pattern: $"newproducts/products/",
            defaults: new { controller = "Catalog", action = "GetNewProducts" });

        //product combinations (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.GET_PRODUCT_COMBINATIONS,
            pattern: $"product/combinations",
            defaults: new { controller = "Product", action = "GetProductCombinations" });

        //product attributes with "upload file" type (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.UPLOAD_FILE_PRODUCT_ATTRIBUTE,
            pattern: $"uploadfileproductattribute/{{attributeId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "UploadFileProductAttribute" });

        //checkout attributes with "upload file" type (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.UPLOAD_FILE_CHECKOUT_ATTRIBUTE,
            pattern: $"uploadfilecheckoutattribute/{{attributeId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" });

        //attribute change (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.PRODUCT_DETAILS_ATTRIBUTE_CHANGE,
            pattern: $"shoppingcart/productdetails_attributechange/{{productId:min(0)}}/{{validateAttributeConditions}}/{{loadPicture}}",
            defaults: new { controller = "ShoppingCart", action = "ProductDetails_AttributeChange" });

        //return request with "upload file" support (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.UPLOAD_FILE_RETURN_REQUEST,
            pattern: $"uploadfilereturnrequest",
            defaults: new { controller = "ReturnRequest", action = "UploadFileReturnRequest" });

        //forums
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.ACTIVE_DISCUSSIONS,
            pattern: $"{lang}/boards/activediscussions",
            defaults: new { controller = "Boards", action = "ActiveDiscussions" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.ACTIVE_DISCUSSIONS_PAGED,
            pattern: $"{lang}/boards/activediscussions/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "ActiveDiscussions" });

        //forums RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.ACTIVE_DISCUSSIONS_RSS,
            pattern: $"boards/activediscussionsrss",
            defaults: new { controller = "Boards", action = "ActiveDiscussionsRSS" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.POST_EDIT,
            pattern: $"{lang}/boards/postedit/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "PostEdit" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.POST_DELETE,
            pattern: $"{lang}/boards/postdelete/{{id:int?}}",
            defaults: new { controller = "Boards", action = "PostDelete" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.POST_CREATE,
            pattern: $"{lang}/boards/postcreate/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "PostCreate" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.POST_CREATE_QUOTE,
            pattern: $"{lang}/boards/postcreate/{{id:min(0)}}/{{quote:min(0)}}",
            defaults: new { controller = "Boards", action = "PostCreate" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_EDIT,
            pattern: $"{lang}/boards/topicedit/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicEdit" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_DELETE,
            pattern: $"{lang}/boards/topicdelete/{{id:int?}}",
            defaults: new { controller = "Boards", action = "TopicDelete" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_CREATE,
            pattern: $"{lang}/boards/topiccreate/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicCreate" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_MOVE,
            pattern: $"{lang}/boards/topicmove/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicMove" });

        //topic watch (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.TOPIC_WATCH,
            pattern: $"boards/topicwatch/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicWatch" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_SLUG,
            pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "Topic" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.TOPIC_SLUG_PAGED,
            pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "Topic" });

        //forum watch (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.FORUM_WATCH,
            pattern: $"boards/forumwatch/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "ForumWatch" });

        //forums RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.FORUM_RSS,
            pattern: $"boards/forumrss/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "ForumRSS" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.FORUM_SLUG,
            pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "Forum" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.FORUM_SLUG_PAGED,
            pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "Forum" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.FORUM_GROUP_SLUG,
            pattern: $"{lang}/boards/forumgroup/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "ForumGroup" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.BOARDS_SEARCH,
            pattern: $"{lang}/boards/search",
            defaults: new { controller = "Boards", action = "Search" });

        //post vote (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.POST_VOTE,
            pattern: "boards/postvote",
            defaults: new { controller = "Boards", action = "PostVote" });

        //private messages
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PRIVATE_MESSAGES,
            pattern: $"{lang}/privatemessages/{{tab?}}",
            defaults: new { controller = "PrivateMessages", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PRIVATE_MESSAGES_PAGED,
            pattern: $"{lang}/privatemessages/{{tab?}}/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PRIVATE_MESSAGES_INBOX,
            pattern: $"{lang}/inboxupdate",
            defaults: new { controller = "PrivateMessages", action = "InboxUpdate" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PRIVATE_MESSAGES_SENT,
            pattern: $"{lang}/sentupdate",
            defaults: new { controller = "PrivateMessages", action = "SentUpdate" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.SEND_PM,
            pattern: $"{lang}/sendpm/{{toCustomerId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "SendPM" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.SEND_PM_REPLY,
            pattern: $"{lang}/sendpm/{{toCustomerId:min(0)}}/{{replyToMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "SendPM" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.VIEW_PM,
            pattern: $"{lang}/viewpm/{{privateMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "ViewPM" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.DELETE_PM,
            pattern: $"{lang}/deletepm/{{privateMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "DeletePM" });

        //activate newsletters
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.NEWSLETTER_ACTIVATION,
            pattern: $"{lang}/newsletter/subscriptionactivation/{{token:guid}}/{{active}}",
            defaults: new { controller = "Newsletter", action = "SubscriptionActivation" });

        //robots.txt (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.ROBOTS_TXT,
            pattern: $"robots.txt",
            defaults: new { controller = "Common", action = "RobotsTextFile" });

        //sitemap
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.General.SITEMAP,
            pattern: $"{lang}/sitemap",
            defaults: new { controller = "Common", action = "Sitemap" });

        //sitemap.xml (file result)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.SITEMAP_XML,
            pattern: $"sitemap.xml",
            defaults: new { controller = "Common", action = "SitemapXml" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.SITEMAP_INDEXED_XML,
            pattern: $"sitemap-{{Id:min(0)}}.xml",
            defaults: new { controller = "Common", action = "SitemapXml" });

        //store closed
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.STORE_CLOSED,
            pattern: $"{lang}/storeclosed",
            defaults: new { controller = "Common", action = "StoreClosed" });

        //install
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.INSTALLATION,
            pattern: $"{NopInstallationDefaults.InstallPath}",
            defaults: new { controller = "Install", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.INSTALLATION_CHANGE_LANGUAGE,
            pattern: $"{NopInstallationDefaults.InstallPath}/ChangeLanguage/{{language}}",
            defaults: new { controller = "Install", action = "ChangeLanguage" });

        //restart application (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Ajax.INSTALLATION_RESTART_APPLICATION,
            pattern: $"{NopInstallationDefaults.InstallPath}/restartapplication",
            defaults: new { controller = "Install", action = "RestartApplication" });

        //page not found
        endpointRouteBuilder.MapControllerRoute(name: NopRouteNames.Standard.PAGE_NOT_FOUND,
            pattern: $"{lang}/page-not-found",
            defaults: new { controller = "Common", action = "PageNotFound" });

        //fallback is intended to handle cases when no other endpoint has matched
        //we use it to invoke [CheckLanguageSeoCode] and give a chance to find a localized route
        endpointRouteBuilder.MapFallbackToController("FallbackRedirect", "Common");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;

    #endregion
}