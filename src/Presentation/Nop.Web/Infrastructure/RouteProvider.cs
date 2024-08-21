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
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        //get language pattern
        //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
        //use it only for URLs of pages that the user can go to
        var lang = GetLanguageRoutePattern();

        //areas
        endpointRouteBuilder.MapControllerRoute(name: "areaRoute",
            pattern: $"{{area:exists}}/{{controller=Home}}/{{action=Index}}/{{id?}}");

        //home page
        endpointRouteBuilder.MapControllerRoute(name: "Homepage",
            pattern: $"{lang}",
            defaults: new { controller = "Home", action = "Index" });

        //login
        endpointRouteBuilder.MapControllerRoute(name: "Login",
            pattern: $"{lang}/login/",
            defaults: new { controller = "Customer", action = "Login" });

        // multi-factor verification digit code page
        endpointRouteBuilder.MapControllerRoute(name: "MultiFactorVerification",
            pattern: $"{lang}/multi-factor-verification/",
            defaults: new { controller = "Customer", action = "MultiFactorVerification" });

        //register
        endpointRouteBuilder.MapControllerRoute(name: "Register",
            pattern: $"{lang}/register/",
            defaults: new { controller = "Customer", action = "Register" });

        //logout
        endpointRouteBuilder.MapControllerRoute(name: "Logout",
            pattern: $"{lang}/logout/",
            defaults: new { controller = "Customer", action = "Logout" });

        //shopping cart
        endpointRouteBuilder.MapControllerRoute(name: "ShoppingCart",
            pattern: $"{lang}/cart/",
            defaults: new { controller = "ShoppingCart", action = "Cart" });

        //estimate shipping (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "EstimateShipping",
            pattern: $"cart/estimateshipping",
            defaults: new { controller = "ShoppingCart", action = "GetEstimateShipping" });

        //select shipping option (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "SelectShippingOption",
            pattern: $"cart/selectshippingoption",
            defaults: new { controller = "ShoppingCart", action = "SelectShippingOption" });

        //wishlist
        endpointRouteBuilder.MapControllerRoute(name: "Wishlist",
            pattern: $"{lang}/wishlist/{{customerGuid?}}",
            defaults: new { controller = "ShoppingCart", action = "Wishlist" });

        //checkout attribute change (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "CheckoutAttributeChange",
            pattern: "shoppingcart/checkoutattributechange/{{isEditable}}",
            defaults: new { controller = "ShoppingCart", action = "CheckoutAttributeChange" });

        //customer account links
        endpointRouteBuilder.MapControllerRoute(name: "CustomerInfo",
            pattern: $"{lang}/customer/info",
            defaults: new { controller = "Customer", action = "Info" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerAddresses",
            pattern: $"{lang}/customer/addresses",
            defaults: new { controller = "Customer", action = "Addresses" });

        //customer address delete (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "CustomerAddressDelete",
            pattern: $"customer/addressdelete",
            defaults: new { controller = "Customer", action = "AddressDelete" });

        //remove external association (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "CustomerRemoveExternalAssociation",
            pattern: $"customer/removeexternalassociation",
            defaults: new { controller = "Customer", action = "RemoveExternalAssociation" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerOrders",
            pattern: $"{lang}/order/history",
            defaults: new { controller = "Order", action = "CustomerOrders" });

        //contact us
        endpointRouteBuilder.MapControllerRoute(name: "ContactUs",
            pattern: $"{lang}/contactus",
            defaults: new { controller = "Common", action = "ContactUs" });

        //product search
        endpointRouteBuilder.MapControllerRoute(name: "ProductSearch",
            pattern: $"{lang}/search/",
            defaults: new { controller = "Catalog", action = "Search" });

        //autocomplete search term (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "ProductSearchAutoComplete",
            pattern: $"catalog/searchtermautocomplete",
            defaults: new { controller = "Catalog", action = "SearchTermAutoComplete" });

        //change currency
        endpointRouteBuilder.MapControllerRoute(name: "ChangeCurrency",
            pattern: $"{lang}/changecurrency/{{customercurrency:min(0)}}",
            defaults: new { controller = "Common", action = "SetCurrency" });

        //change language
        endpointRouteBuilder.MapControllerRoute(name: "ChangeLanguage",
            pattern: $"{lang}/changelanguage/{{langid:min(0)}}",
            defaults: new { controller = "Common", action = "SetLanguage" });

        //change tax
        endpointRouteBuilder.MapControllerRoute(name: "ChangeTaxType",
            pattern: $"{lang}/changetaxtype/{{customertaxtype:min(0)}}",
            defaults: new { controller = "Common", action = "SetTaxType" });

        //set store theme
        endpointRouteBuilder.MapControllerRoute(name: "SetStoreTheme",
            pattern: $"{lang}/setstoretheme/{{themeName}}/{{returnUrl}}",
            defaults: new { controller = "Common", action = "SetStoreTheme" });

        //recently viewed products
        endpointRouteBuilder.MapControllerRoute(name: "RecentlyViewedProducts",
            pattern: $"{lang}/recentlyviewedproducts/",
            defaults: new { controller = "Product", action = "RecentlyViewedProducts" });

        //new products
        endpointRouteBuilder.MapControllerRoute(name: "NewProducts",
            pattern: $"{lang}/newproducts/",
            defaults: new { controller = "Catalog", action = "NewProducts" });

        //blog
        endpointRouteBuilder.MapControllerRoute(name: "Blog",
            pattern: $"{lang}/blog",
            defaults: new { controller = "Blog", action = "List" });

        //news
        endpointRouteBuilder.MapControllerRoute(name: "NewsArchive",
            pattern: $"{lang}/news",
            defaults: new { controller = "News", action = "List" });

        //forum
        endpointRouteBuilder.MapControllerRoute(name: "Boards",
            pattern: $"{lang}/boards",
            defaults: new { controller = "Boards", action = "Index" });

        //compare products
        endpointRouteBuilder.MapControllerRoute(name: "CompareProducts",
            pattern: $"{lang}/compareproducts/",
            defaults: new { controller = "Product", action = "CompareProducts" });

        //product tags
        endpointRouteBuilder.MapControllerRoute(name: "ProductTagsAll",
            pattern: $"{lang}/producttag/all/",
            defaults: new { controller = "Catalog", action = "ProductTagsAll" });

        //manufacturers
        endpointRouteBuilder.MapControllerRoute(name: "ManufacturerList",
            pattern: $"{lang}/manufacturer/all/",
            defaults: new { controller = "Catalog", action = "ManufacturerAll" });

        //vendors
        endpointRouteBuilder.MapControllerRoute(name: "VendorList",
            pattern: $"{lang}/vendor/all/",
            defaults: new { controller = "Catalog", action = "VendorAll" });

        //add product to cart (without any attributes and options). used on catalog pages. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "AddProductToCart-Catalog",
            pattern: $"addproducttocart/catalog/{{productId:min(0)}}/{{shoppingCartTypeId:min(0)}}/{{quantity:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "AddProductToCart_Catalog" });

        //add product to cart (with attributes and options). used on the product details pages. (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "AddProductToCart-Details",
            pattern: $"addproducttocart/details/{{productId:min(0)}}/{{shoppingCartTypeId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "AddProductToCart_Details" });

        //comparing products (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "AddProductToCompare",
            pattern: $"compareproducts/add/{{productId:min(0)}}",
            defaults: new { controller = "Product", action = "AddProductToCompareList" });

        //product email a friend
        endpointRouteBuilder.MapControllerRoute(name: "ProductEmailAFriend",
            pattern: $"{lang}/productemailafriend/{{productId:min(0)}}",
            defaults: new { controller = "Product", action = "ProductEmailAFriend" });

        //product estimate shipping (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "ProductEstimateShipping",
            pattern: "product/estimateshipping/{{ProductId:min(0)}}",
            defaults: new { controller = "Product", action = "EstimateShipping" });

        //reviews
        endpointRouteBuilder.MapControllerRoute(name: "CustomerProductReviews",
            pattern: $"{lang}/customer/productreviews",
            defaults: new { controller = "Product", action = "CustomerProductReviews" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerProductReviewsPaged",
            pattern: $"{lang}/customer/productreviews/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Product", action = "CustomerProductReviews" });

        //back in stock notifications (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "BackInStockSubscribePopup",
            pattern: $"backinstocksubscribe/{{productId:min(0)}}",
            defaults: new { controller = "BackInStockSubscription", action = "SubscribePopup" });

        endpointRouteBuilder.MapControllerRoute(name: "BackInStockSubscribeSend",
            pattern: $"backinstocksubscribesend/{{productId:min(0)}}",
            defaults: new { controller = "BackInStockSubscription", action = "SubscribePopupPOST" });

        //downloads (file result)
        endpointRouteBuilder.MapControllerRoute(name: "GetSampleDownload",
            pattern: $"download/sample/{{productid:min(0)}}",
            defaults: new { controller = "Download", action = "Sample" });

        //downloads
        endpointRouteBuilder.MapControllerRoute(name: "DownloadGetFileUpload",
            pattern: $"download/getfileupload/{{downloadId}}",
            defaults: new { controller = "Download", action = "GetFileUpload" });

        //checkout pages
        endpointRouteBuilder.MapControllerRoute(name: "Checkout",
            pattern: $"{lang}/checkout/",
            defaults: new { controller = "Checkout", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: "CheckoutOnePage",
            pattern: $"{lang}/onepagecheckout/",
            defaults: new { controller = "Checkout", action = "OnePageCheckout" });

        endpointRouteBuilder.MapControllerRoute(name: "CheckoutShippingAddress",
            pattern: $"{lang}/checkout/shippingaddress",
            defaults: new { controller = "Checkout", action = "ShippingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: "CheckoutSelectShippingAddress",
            pattern: $"{lang}/checkout/selectshippingaddress",
            defaults: new { controller = "Checkout", action = "SelectShippingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: "CheckoutBillingAddress",
            pattern: $"{lang}/checkout/billingaddress",
            defaults: new { controller = "Checkout", action = "BillingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: "CheckoutSelectBillingAddress",
            pattern: $"{lang}/checkout/selectbillingaddress",
            defaults: new { controller = "Checkout", action = "SelectBillingAddress" });

        endpointRouteBuilder.MapControllerRoute(name: "CheckoutShippingMethod",
            pattern: $"{lang}/checkout/shippingmethod",
            defaults: new { controller = "Checkout", action = "ShippingMethod" });

        endpointRouteBuilder.MapControllerRoute(name: "CheckoutPaymentMethod",
            pattern: $"{lang}/checkout/paymentmethod",
            defaults: new { controller = "Checkout", action = "PaymentMethod" });

        endpointRouteBuilder.MapControllerRoute(name: "CheckoutPaymentInfo",
            pattern: $"{lang}/checkout/paymentinfo",
            defaults: new { controller = "Checkout", action = "PaymentInfo" });

        endpointRouteBuilder.MapControllerRoute(name: "CheckoutConfirm",
            pattern: $"{lang}/checkout/confirm",
            defaults: new { controller = "Checkout", action = "Confirm" });

        endpointRouteBuilder.MapControllerRoute(name: "CheckoutCompleted",
            pattern: $"{lang}/checkout/completed/{{orderId:int?}}",
            defaults: new { controller = "Checkout", action = "Completed" });

        //subscribe newsletters (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "SubscribeNewsletter",
            pattern: $"subscribenewsletter",
            defaults: new { controller = "Newsletter", action = "SubscribeNewsletter" });

        //email wishlist
        endpointRouteBuilder.MapControllerRoute(name: "EmailWishlist",
            pattern: $"{lang}/emailwishlist",
            defaults: new { controller = "ShoppingCart", action = "EmailWishlist" });

        //login page for checkout as guest
        endpointRouteBuilder.MapControllerRoute(name: "LoginCheckoutAsGuest",
            pattern: $"{lang}/login/checkoutasguest",
            defaults: new { controller = "Customer", action = "Login", checkoutAsGuest = true });

        //register result page
        endpointRouteBuilder.MapControllerRoute(name: "RegisterResult",
            pattern: $"{lang}/registerresult/{{resultId:min(0)}}",
            defaults: new { controller = "Customer", action = "RegisterResult" });

        //check username availability (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "CheckUsernameAvailability",
            pattern: $"customer/checkusernameavailability",
            defaults: new { controller = "Customer", action = "CheckUsernameAvailability" });

        //passwordrecovery
        endpointRouteBuilder.MapControllerRoute(name: "PasswordRecovery",
            pattern: $"{lang}/passwordrecovery",
            defaults: new { controller = "Customer", action = "PasswordRecovery" });

        //password recovery confirmation
        endpointRouteBuilder.MapControllerRoute(name: "PasswordRecoveryConfirm",
            pattern: $"{lang}/passwordrecovery/confirm",
            defaults: new { controller = "Customer", action = "PasswordRecoveryConfirm" });

        //topics (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "TopicPopup",
            pattern: $"t-popup/{{SystemName}}",
            defaults: new { controller = "Topic", action = "TopicDetailsPopup" });

        //blog
        endpointRouteBuilder.MapControllerRoute(name: "BlogByTag",
            pattern: $"{lang}/blog/tag/{{tag}}",
            defaults: new { controller = "Blog", action = "BlogByTag" });

        endpointRouteBuilder.MapControllerRoute(name: "BlogByMonth",
            pattern: $"{lang}/blog/month/{{month}}",
            defaults: new { controller = "Blog", action = "BlogByMonth" });

        //blog RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: "BlogRSS",
            pattern: $"blog/rss/{{languageId:min(0)}}",
            defaults: new { controller = "Blog", action = "ListRss" });

        //news RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: "NewsRSS",
            pattern: $"news/rss/{{languageId:min(0)}}",
            defaults: new { controller = "News", action = "ListRss" });

        //set review helpfulness (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "SetProductReviewHelpfulness",
            pattern: $"setproductreviewhelpfulness",
            defaults: new { controller = "Product", action = "SetProductReviewHelpfulness" });

        //customer account links
        endpointRouteBuilder.MapControllerRoute(name: "CustomerReturnRequests",
            pattern: $"{lang}/returnrequest/history",
            defaults: new { controller = "ReturnRequest", action = "CustomerReturnRequests" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerDownloadableProducts",
            pattern: $"{lang}/customer/downloadableproducts",
            defaults: new { controller = "Customer", action = "DownloadableProducts" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerBackInStockSubscriptions",
            pattern: $"{lang}/backinstocksubscriptions/manage/{{pageNumber:int?}}",
            defaults: new { controller = "BackInStockSubscription", action = "CustomerSubscriptions" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerRewardPoints",
            pattern: $"{lang}/rewardpoints/history",
            defaults: new { controller = "Order", action = "CustomerRewardPoints" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerRewardPointsPaged",
            pattern: $"{lang}/rewardpoints/history/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Order", action = "CustomerRewardPoints" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerChangePassword",
            pattern: $"{lang}/customer/changepassword",
            defaults: new { controller = "Customer", action = "ChangePassword" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerAvatar",
            pattern: $"{lang}/customer/avatar",
            defaults: new { controller = "Customer", action = "Avatar" });

        endpointRouteBuilder.MapControllerRoute(name: "AccountActivation",
            pattern: $"{lang}/customer/activation",
            defaults: new { controller = "Customer", action = "AccountActivation" });

        endpointRouteBuilder.MapControllerRoute(name: "EmailRevalidation",
            pattern: $"{lang}/customer/revalidateemail",
            defaults: new { controller = "Customer", action = "EmailRevalidation" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerForumSubscriptions",
            pattern: $"{lang}/boards/forumsubscriptions/{{pageNumber:int?}}",
            defaults: new { controller = "Boards", action = "CustomerForumSubscriptions" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerAddressEdit",
            pattern: $"{lang}/customer/addressedit/{{addressId:min(0)}}",
            defaults: new { controller = "Customer", action = "AddressEdit" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerAddressAdd",
            pattern: $"{lang}/customer/addressadd",
            defaults: new { controller = "Customer", action = "AddressAdd" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerMultiFactorAuthenticationProviderConfig",
            pattern: $"{lang}/customer/providerconfig",
            defaults: new { controller = "Customer", action = "ConfigureMultiFactorAuthenticationProvider" });

        //customer profile page
        endpointRouteBuilder.MapControllerRoute(name: "CustomerProfile",
            pattern: $"{lang}/profile/{{id:min(0)}}",
            defaults: new { controller = "Profile", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: "CustomerProfilePaged",
            pattern: $"{lang}/profile/{{id:min(0)}}/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "Profile", action = "Index" });

        //orders
        endpointRouteBuilder.MapControllerRoute(name: "OrderDetails",
            pattern: $"{lang}/orderdetails/{{orderId:min(0)}}",
            defaults: new { controller = "Order", action = "Details" });

        endpointRouteBuilder.MapControllerRoute(name: "ShipmentDetails",
            pattern: $"{lang}/orderdetails/shipment/{{shipmentId}}",
            defaults: new { controller = "Order", action = "ShipmentDetails" });

        endpointRouteBuilder.MapControllerRoute(name: "ReturnRequest",
            pattern: $"{lang}/returnrequest/{{orderId:min(0)}}",
            defaults: new { controller = "ReturnRequest", action = "ReturnRequest" });

        endpointRouteBuilder.MapControllerRoute(name: "ReOrder",
            pattern: $"{lang}/reorder/{{orderId:min(0)}}",
            defaults: new { controller = "Order", action = "ReOrder" });

        //pdf invoice (file result)
        endpointRouteBuilder.MapControllerRoute(name: "GetOrderPdfInvoice",
            pattern: $"orderdetails/pdf/{{orderId}}",
            defaults: new { controller = "Order", action = "GetPdfInvoice" });

        endpointRouteBuilder.MapControllerRoute(name: "PrintOrderDetails",
            pattern: $"{lang}/orderdetails/print/{{orderId}}",
            defaults: new { controller = "Order", action = "PrintOrderDetails" });

        //order downloads (file result)
        endpointRouteBuilder.MapControllerRoute(name: "GetDownload",
            pattern: $"download/getdownload/{{orderItemId:guid}}/{{agree?}}",
            defaults: new { controller = "Download", action = "GetDownload" });

        endpointRouteBuilder.MapControllerRoute(name: "GetLicense",
            pattern: $"download/getlicense/{{orderItemId:guid}}/",
            defaults: new { controller = "Download", action = "GetLicense" });

        endpointRouteBuilder.MapControllerRoute(name: "DownloadUserAgreement",
            pattern: $"customer/useragreement/{{orderItemId:guid}}",
            defaults: new { controller = "Customer", action = "UserAgreement" });

        endpointRouteBuilder.MapControllerRoute(name: "GetOrderNoteFile",
            pattern: $"download/ordernotefile/{{ordernoteid:min(0)}}",
            defaults: new { controller = "Download", action = "GetOrderNoteFile" });

        //contact vendor
        endpointRouteBuilder.MapControllerRoute(name: "ContactVendor",
            pattern: $"{lang}/contactvendor/{{vendorId}}",
            defaults: new { controller = "Common", action = "ContactVendor" });

        //apply for vendor account
        endpointRouteBuilder.MapControllerRoute(name: "ApplyVendorAccount",
            pattern: $"{lang}/vendor/apply",
            defaults: new { controller = "Vendor", action = "ApplyVendor" });

        //vendor info
        endpointRouteBuilder.MapControllerRoute(name: "CustomerVendorInfo",
            pattern: $"{lang}/customer/vendorinfo",
            defaults: new { controller = "Vendor", action = "Info" });

        //customer GDPR
        endpointRouteBuilder.MapControllerRoute(name: "GdprTools",
            pattern: $"{lang}/customer/gdpr",
            defaults: new { controller = "Customer", action = "GdprTools" });

        //customer check gift card balance 
        endpointRouteBuilder.MapControllerRoute(name: "CheckGiftCardBalance",
            pattern: $"{lang}/customer/checkgiftcardbalance",
            defaults: new { controller = "Customer", action = "CheckGiftCardBalance" });

        //customer multi-factor authentication settings 
        endpointRouteBuilder.MapControllerRoute(name: "MultiFactorAuthenticationSettings",
            pattern: $"{lang}/customer/multifactorauthentication",
            defaults: new { controller = "Customer", action = "MultiFactorAuthentication" });

        //poll vote (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "PollVote",
            pattern: $"poll/vote",
            defaults: new { controller = "Poll", action = "Vote" });

        //comparing products
        endpointRouteBuilder.MapControllerRoute(name: "RemoveProductFromCompareList",
            pattern: $"{lang}/compareproducts/remove/{{productId}}",
            defaults: new { controller = "Product", action = "RemoveProductFromCompareList" });

        endpointRouteBuilder.MapControllerRoute(name: "ClearCompareList",
            pattern: $"{lang}/clearcomparelist/",
            defaults: new { controller = "Product", action = "ClearCompareList" });

        //new RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: "NewProductsRSS",
            pattern: $"newproducts/rss",
            defaults: new { controller = "Catalog", action = "NewProductsRss" });

        //get state list by country ID (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "GetStatesByCountryId",
            pattern: $"country/getstatesbycountryid/",
            defaults: new { controller = "Country", action = "GetStatesByCountryId" });

        //EU Cookie law accept button handler (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "EuCookieLawAccept",
            pattern: $"eucookielawaccept",
            defaults: new { controller = "Common", action = "EuCookieLawAccept" });

        //authenticate topic (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "TopicAuthenticate",
            pattern: $"topic/authenticate",
            defaults: new { controller = "Topic", action = "Authenticate" });

        //prepare top menu (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "GetCatalogRoot",
            pattern: $"catalog/getcatalogroot",
            defaults: new { controller = "Catalog", action = "GetCatalogRoot" });

        endpointRouteBuilder.MapControllerRoute(name: "GetCatalogSubCategories",
            pattern: $"catalog/getcatalogsubcategories",
            defaults: new { controller = "Catalog", action = "GetCatalogSubCategories" });

        //Catalog products (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "GetCategoryProducts",
            pattern: $"category/products/",
            defaults: new { controller = "Catalog", action = "GetCategoryProducts" });

        endpointRouteBuilder.MapControllerRoute(name: "GetManufacturerProducts",
            pattern: $"manufacturer/products/",
            defaults: new { controller = "Catalog", action = "GetManufacturerProducts" });

        endpointRouteBuilder.MapControllerRoute(name: "GetTagProducts",
            pattern: $"tag/products",
            defaults: new { controller = "Catalog", action = "GetTagProducts" });

        endpointRouteBuilder.MapControllerRoute(name: "SearchProducts",
            pattern: "product/search",
            defaults: new { controller = "Catalog", action = "SearchProducts" });

        endpointRouteBuilder.MapControllerRoute(name: "GetVendorProducts",
            pattern: $"vendor/products",
            defaults: new { controller = "Catalog", action = "GetVendorProducts" });

        endpointRouteBuilder.MapControllerRoute(name: "GetNewProducts",
            pattern: $"newproducts/products/",
            defaults: new { controller = "Catalog", action = "GetNewProducts" });

        //product combinations (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "GetProductCombinations",
            pattern: $"product/combinations",
            defaults: new { controller = "Product", action = "GetProductCombinations" });

        //product attributes with "upload file" type (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "UploadFileProductAttribute",
            pattern: $"uploadfileproductattribute/{{attributeId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "UploadFileProductAttribute" });

        //checkout attributes with "upload file" type (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "UploadFileCheckoutAttribute",
            pattern: $"uploadfilecheckoutattribute/{{attributeId:min(0)}}",
            defaults: new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" });

        //attribute change (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "ProductDetailsAttributeChange",
            pattern: $"shoppingcart/productdetails_attributechange/{{productId:min(0)}}/{{validateAttributeConditions}}/{{loadPicture}}",
            defaults: new { controller = "ShoppingCart", action = "ProductDetails_AttributeChange" });

        //return request with "upload file" support (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "UploadFileReturnRequest",
            pattern: $"uploadfilereturnrequest",
            defaults: new { controller = "ReturnRequest", action = "UploadFileReturnRequest" });

        //forums
        endpointRouteBuilder.MapControllerRoute(name: "ActiveDiscussions",
            pattern: $"{lang}/boards/activediscussions",
            defaults: new { controller = "Boards", action = "ActiveDiscussions" });

        endpointRouteBuilder.MapControllerRoute(name: "ActiveDiscussionsPaged",
            pattern: $"{lang}/boards/activediscussions/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "ActiveDiscussions" });

        //forums RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: "ActiveDiscussionsRSS",
            pattern: $"boards/activediscussionsrss",
            defaults: new { controller = "Boards", action = "ActiveDiscussionsRSS" });

        endpointRouteBuilder.MapControllerRoute(name: "PostEdit",
            pattern: $"{lang}/boards/postedit/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "PostEdit" });

        endpointRouteBuilder.MapControllerRoute(name: "PostDelete",
            pattern: $"{lang}/boards/postdelete/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "PostDelete" });

        endpointRouteBuilder.MapControllerRoute(name: "PostCreate",
            pattern: $"{lang}/boards/postcreate/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "PostCreate" });

        endpointRouteBuilder.MapControllerRoute(name: "PostCreateQuote",
            pattern: $"{lang}/boards/postcreate/{{id:min(0)}}/{{quote:min(0)}}",
            defaults: new { controller = "Boards", action = "PostCreate" });

        endpointRouteBuilder.MapControllerRoute(name: "TopicEdit",
            pattern: $"{lang}/boards/topicedit/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicEdit" });

        endpointRouteBuilder.MapControllerRoute(name: "TopicDelete",
            pattern: $"{lang}/boards/topicdelete/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicDelete" });

        endpointRouteBuilder.MapControllerRoute(name: "TopicCreate",
            pattern: $"{lang}/boards/topiccreate/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicCreate" });

        endpointRouteBuilder.MapControllerRoute(name: "TopicMove",
            pattern: $"{lang}/boards/topicmove/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicMove" });

        //topic watch (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "TopicWatch",
            pattern: $"boards/topicwatch/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "TopicWatch" });

        endpointRouteBuilder.MapControllerRoute(name: "TopicSlug",
            pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "Topic" });

        endpointRouteBuilder.MapControllerRoute(name: "TopicSlugPaged",
            pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "Topic" });

        //forum watch (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "ForumWatch",
            pattern: $"boards/forumwatch/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "ForumWatch" });

        //forums RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: "ForumRSS",
            pattern: $"boards/forumrss/{{id:min(0)}}",
            defaults: new { controller = "Boards", action = "ForumRSS" });

        endpointRouteBuilder.MapControllerRoute(name: "ForumSlug",
            pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "Forum" });

        endpointRouteBuilder.MapControllerRoute(name: "ForumSlugPaged",
            pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
            defaults: new { controller = "Boards", action = "Forum" });

        endpointRouteBuilder.MapControllerRoute(name: "ForumGroupSlug",
            pattern: $"{lang}/boards/forumgroup/{{id:min(0)}}/{{slug?}}",
            defaults: new { controller = "Boards", action = "ForumGroup" });

        endpointRouteBuilder.MapControllerRoute(name: "Search",
            pattern: $"{lang}/boards/search",
            defaults: new { controller = "Boards", action = "Search" });

        //post vote (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "PostVote",
            pattern: "boards/postvote",
            defaults: new { controller = "Boards", action = "PostVote" });

        //private messages
        endpointRouteBuilder.MapControllerRoute(name: "PrivateMessages",
            pattern: $"{lang}/privatemessages/{{tab?}}",
            defaults: new { controller = "PrivateMessages", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: "PrivateMessagesPaged",
            pattern: $"{lang}/privatemessages/{{tab?}}/page/{{pageNumber:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: "PrivateMessagesInbox",
            pattern: $"{lang}/inboxupdate",
            defaults: new { controller = "PrivateMessages", action = "InboxUpdate" });

        endpointRouteBuilder.MapControllerRoute(name: "PrivateMessagesSent",
            pattern: $"{lang}/sentupdate",
            defaults: new { controller = "PrivateMessages", action = "SentUpdate" });

        endpointRouteBuilder.MapControllerRoute(name: "SendPM",
            pattern: $"{lang}/sendpm/{{toCustomerId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "SendPM" });

        endpointRouteBuilder.MapControllerRoute(name: "SendPMReply",
            pattern: $"{lang}/sendpm/{{toCustomerId:min(0)}}/{{replyToMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "SendPM" });

        endpointRouteBuilder.MapControllerRoute(name: "ViewPM",
            pattern: $"{lang}/viewpm/{{privateMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "ViewPM" });

        endpointRouteBuilder.MapControllerRoute(name: "DeletePM",
            pattern: $"{lang}/deletepm/{{privateMessageId:min(0)}}",
            defaults: new { controller = "PrivateMessages", action = "DeletePM" });

        //activate newsletters
        endpointRouteBuilder.MapControllerRoute(name: "NewsletterActivation",
            pattern: $"{lang}/newsletter/subscriptionactivation/{{token:guid}}/{{active}}",
            defaults: new { controller = "Newsletter", action = "SubscriptionActivation" });

        //robots.txt (file result)
        endpointRouteBuilder.MapControllerRoute(name: "robots.txt",
            pattern: $"robots.txt",
            defaults: new { controller = "Common", action = "RobotsTextFile" });

        //sitemap
        endpointRouteBuilder.MapControllerRoute(name: "Sitemap",
            pattern: $"{lang}/sitemap",
            defaults: new { controller = "Common", action = "Sitemap" });

        //sitemap.xml (file result)
        endpointRouteBuilder.MapControllerRoute(name: "sitemap.xml",
            pattern: $"sitemap.xml",
            defaults: new { controller = "Common", action = "SitemapXml" });

        endpointRouteBuilder.MapControllerRoute(name: "sitemap-indexed.xml",
            pattern: $"sitemap-{{Id:min(0)}}.xml",
            defaults: new { controller = "Common", action = "SitemapXml" });

        //store closed
        endpointRouteBuilder.MapControllerRoute(name: "StoreClosed",
            pattern: $"{lang}/storeclosed",
            defaults: new { controller = "Common", action = "StoreClosed" });

        //install
        endpointRouteBuilder.MapControllerRoute(name: "Installation",
            pattern: $"{NopInstallationDefaults.InstallPath}",
            defaults: new { controller = "Install", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(name: "InstallationChangeLanguage",
            pattern: $"{NopInstallationDefaults.InstallPath}/ChangeLanguage/{{language}}",
            defaults: new { controller = "Install", action = "ChangeLanguage" });

        //restart application (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: "InstallationRestartApplication",
            pattern: $"{NopInstallationDefaults.InstallPath}/restartapplication",
            defaults: new { controller = "Install", action = "RestartApplication" });

        //page not found
        endpointRouteBuilder.MapControllerRoute(name: "PageNotFound",
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