namespace Nop.Core.Http;

/// <summary>
/// Represents route names
/// </summary>
public static partial class NopRouteNames
{
    /// <summary>
    /// Represents general route names for standard requests
    /// </summary>
    public static partial class General
    {
        /// <summary>
        /// Gets the Apply for vendor account route name
        /// </summary>
        public const string APPLY_VENDOR_ACCOUNT = "ApplyVendorAccount";

        /// <summary>
        /// Gets the login route name
        /// </summary>
        public const string LOGIN = "Login";

        /// <summary>
        /// Gets the homepage route name
        /// </summary>
        public const string HOMEPAGE = "Homepage";

        /// <summary>
        /// Gets the customer addresses route name
        /// </summary>
        public const string CUSTOMER_ADDRESSES = "CustomerAddresses";

        /// <summary>
        /// Gets the customer account route name
        /// </summary>
        public const string CUSTOMER_INFO = "CustomerInfo";

        /// <summary>
        /// Gets the customer orders route name
        /// </summary>
        public const string CUSTOMER_ORDERS = "CustomerOrders";

        /// <summary>
        /// Gets the contact us route name
        /// </summary>
        public const string CONTACT_US = "ContactUs";

        /// <summary>
        /// Gets the product search route name
        /// </summary>
        public const string SEARCH = "ProductSearch";

        /// <summary>
        /// Gets the compare products route name
        /// </summary>
        public const string COMPARE_PRODUCTS = "CompareProducts";

        /// <summary>
        /// Gets the new products route name
        /// </summary>
        public const string NEW_PRODUCTS = "NewProducts";

        /// <summary>
        /// Gets the blog route name
        /// </summary>
        public const string BLOG = "Blog";

        /// <summary>
        /// Gets the news route name
        /// </summary>
        public const string NEWS = "NewsArchive";

        /// <summary>
        /// Gets the forums route name
        /// </summary>
        public const string BOARDS = "Boards";

        /// <summary>
        /// Gets the product tags route name
        /// </summary>
        public const string PRODUCT_TAGS = "ProductTagsAll";

        /// <summary>
        /// Gets the recently viewed products route name
        /// </summary>
        public const string RECENTLY_VIEWED_PRODUCTS = "RecentlyViewedProducts";

        /// <summary>
        /// Gets the manufacturers route name
        /// </summary>
        public const string MANUFACTURERS = "ManufacturerList";

        /// <summary>
        /// Gets the vendors route name
        /// </summary>
        public const string VENDORS = "VendorList";

        /// <summary>
        /// Gets the sitemap route name
        /// </summary>
        public const string SITEMAP = "Sitemap";

        /// <summary>
        /// Gets the shopping cart route name
        /// </summary>
        public const string CART = "ShoppingCart";

        /// <summary>
        /// Gets the wishlist route name
        /// </summary>
        public const string WISHLIST = "Wishlist";

        /// <summary>
        /// Gets the gift card balance route name
        /// </summary>
        public const string CHECK_GIFT_CARD_BALANCE = "CheckGiftCardBalance";
    }

    /// <summary>
    /// Represents route names for standard requests
    /// </summary>
    public static partial class Standard
    {
        /// <summary>
        /// Gets the logout route name
        /// </summary>
        public const string LOGOUT = "Logout";

        /// <summary>
        /// Gets the register route name
        /// </summary>
        public const string REGISTER = "Register";

        /// <summary>
        /// Gets the checkout route name
        /// </summary>
        public const string CHECKOUT = "Checkout";

        /// <summary>
        /// Gets the one page checkout route name
        /// </summary>
        public const string CHECKOUT_ONE_PAGE = "CheckoutOnePage";

        /// <summary>
        /// Gets the checkout shipping address route name
        /// </summary>
        public const string CHECKOUT_SHIPPING_ADDRESS = "CheckoutShippingAddress";

        /// <summary>
        /// Gets the checkout select shipping address route name
        /// </summary>
        public const string CHECKOUT_SELECT_SHIPPING_ADDRESS = "CheckoutSelectShippingAddress";

        /// <summary>
        /// Gets the checkout billing address route name
        /// </summary>
        public const string CHECKOUT_BILLING_ADDRESS = "CheckoutBillingAddress";

        /// <summary>
        /// Gets the checkout select billing address route name
        /// </summary>
        public const string CHECKOUT_SELECT_BILLING_ADDRESS = "CheckoutSelectBillingAddress";

        /// <summary>
        /// Gets the checkout shipping method route name
        /// </summary>
        public const string CHECKOUT_SHIPPING_METHOD = "CheckoutShippingMethod";

        /// <summary>
        /// Gets the checkout payment method route name
        /// </summary>
        public const string CHECKOUT_PAYMENT_METHOD = "CheckoutPaymentMethod";

        /// <summary>
        /// Gets the checkout payment info route name
        /// </summary>
        public const string CHECKOUT_PAYMENT_INFO = "CheckoutPaymentInfo";

        /// <summary>
        /// Gets the checkout confirm route name
        /// </summary>
        public const string CHECKOUT_CONFIRM = "CheckoutConfirm";

        /// <summary>
        /// Gets the checkout completed route name
        /// </summary>
        public const string CHECKOUT_COMPLETED = "CheckoutCompleted";

        /// <summary>
        /// Gets the customer orders paged route name
        /// </summary>
        public const string CUSTOMER_ORDERS_PAGED = "CustomerOrdersPaged";

        /// <summary>
        /// Gets the customer reviews route name
        /// </summary>
        public const string CUSTOMER_PRODUCT_REVIEWS = "CustomerProductReviews";

        /// <summary>
        /// Gets the customer reviews paged route name
        /// </summary>
        public const string CUSTOMER_PRODUCT_REVIEWS_PAGED = "CustomerProductReviewsPaged";

        /// <summary>
        /// Gets the customer recurring payments route name
        /// </summary>
        public const string CUSTOMER_RECURRING_PAYMENTS = "CustomerRecurringPayments";

        /// <summary>
        /// Gets the change currency route name
        /// </summary>
        public const string CHANGE_CURRENCY = "ChangeCurrency";

        /// <summary>
        /// Gets the change language route name
        /// </summary>
        public const string CHANGE_LANGUAGE = "ChangeLanguage";

        /// <summary>
        /// Gets the change tax type route name
        /// </summary>
        public const string CHANGE_TAX_TYPE = "ChangeTaxType";

        /// <summary>
        /// Gets the file upload route name
        /// </summary>
        public const string DOWNLOAD_GET_FILE_UPLOAD = "DownloadGetFileUpload";

        /// <summary>
        /// Gets the email wishlist route name
        /// </summary>
        public const string EMAIL_WISHLIST = "EmailWishlist";

        /// <summary>
        /// Gets the sample download route name
        /// </summary>
        public const string GET_SAMPLE_DOWNLOAD = "GetSampleDownload";

        /// <summary>
        /// Gets the login page for checkout as guest route name
        /// </summary>
        public const string LOGIN_CHECKOUT_AS_GUEST = "LoginCheckoutAsGuest";

        /// <summary>
        /// Gets the multi-factor verification route name
        /// </summary>
        public const string MULTIFACTOR_VERIFICATION = "MultiFactorVerification";

        /// <summary>
        /// Gets the product email a friend route name
        /// </summary>
        public const string PRODUCT_EMAIL_FRIEND = "ProductEmailAFriend";

        /// <summary>
        /// Gets the set store theme route name
        /// </summary>
        public const string SET_STORE_THEME = "SetStoreTheme";

        /// <summary>
        /// Gets the vendor reviews route name
        /// </summary>
        public const string VENDOR_REVIEWS = "VendorReviews";

        /// <summary>
        /// Gets the register result page route name
        /// </summary>
        public const string REGISTER_RESULT = "RegisterResult";

        /// <summary>
        /// Gets the password recovery route name
        /// </summary>
        public const string PASSWORD_RECOVERY = "PasswordRecovery";

        /// <summary>
        /// Gets the password recovery confirmation route name
        /// </summary>
        public const string PASSWORD_RECOVERY_CONFIRM = "PasswordRecoveryConfirm";

        /// <summary>
        /// Gets the blog by tag route name
        /// </summary>
        public const string BLOG_BY_TAG = "BlogByTag";

        /// <summary>
        /// Gets the blog by month route name
        /// </summary>
        public const string BLOG_BY_MONTH = "BlogByMonth";

        /// <summary>
        /// Gets the blog RSS route name
        /// </summary>
        public const string BLOG_RSS = "BlogRSS";

        /// <summary>
        /// Gets the news RSS route name
        /// </summary>
        public const string NEWS_RSS = "NewsRSS";

        /// <summary>
        /// Gets the customer return request route name
        /// </summary>
        public const string CUSTOMER_RETURN_REQUESTS = "CustomerReturnRequests";

        /// <summary>
        /// Gets the customer downloadable products route name
        /// </summary>
        public const string CUSTOMER_DOWNLOADABLE_PRODUCTS = "CustomerDownloadableProducts";

        /// <summary>
        /// Gets the customer back in stock subscriptions route name
        /// </summary>
        public const string CUSTOMER_BACK_IN_STOCK_SUBSCRIPTIONS = "CustomerBackInStockSubscriptions";

        /// <summary>
        /// Gets the customer reward points route name
        /// </summary>
        public const string CUSTOMER_REWARD_POINTS = "CustomerRewardPoints";

        /// <summary>
        /// Gets the customer reward points paged route name
        /// </summary>
        public const string CUSTOMER_REWARD_POINTS_PAGED = "CustomerRewardPointsPaged";

        /// <summary>
        /// Gets the customer change password route name
        /// </summary>
        public const string CUSTOMER_CHANGE_PASSWORD = "CustomerChangePassword";

        /// <summary>
        /// Gets the customer avatar route name
        /// </summary>
        public const string CUSTOMER_AVATAR = "CustomerAvatar";

        /// <summary>
        /// Gets the account activation route name
        /// </summary>
        public const string ACCOUNT_ACTIVATION = "AccountActivation";

        /// <summary>
        /// Gets the email revalidation route name
        /// </summary>
        public const string EMAIL_REVALIDATION = "EmailRevalidation";

        /// <summary>
        /// Gets the customer forum subscriptions route name
        /// </summary>
        public const string CUSTOMER_FORUM_SUBSCRIPTIONS = "CustomerForumSubscriptions";

        /// <summary>
        /// Gets the customer address edit route name
        /// </summary>
        public const string CUSTOMER_ADDRESS_EDIT = "CustomerAddressEdit";

        /// <summary>
        /// Gets the customer address add route name
        /// </summary>
        public const string CUSTOMER_ADDRESS_ADD = "CustomerAddressAdd";

        /// <summary>
        /// Gets the customer multi-factor authentication config route name
        /// </summary>
        public const string CUSTOMER_MULTI_FACTOR_AUTHENTICATION_PROVIDER_CONFIG = "CustomerMultiFactorAuthenticationProviderConfig";

        /// <summary>
        /// Gets the customer profile route name
        /// </summary>
        public const string CUSTOMER_PROFILE = "CustomerProfile";

        /// <summary>
        /// Gets the customer profile paged route name
        /// </summary>
        public const string CUSTOMER_PROFILE_PAGED = "CustomerProfilePaged";

        /// <summary>
        /// Gets the order details route name
        /// </summary>
        public const string ORDER_DETAILS = "OrderDetails";

        /// <summary>
        /// Gets the shipment details route name
        /// </summary>
        public const string SHIPMENT_DETAILS = "ShipmentDetails";

        /// <summary>
        /// Gets the return request route name
        /// </summary>
        public const string RETURN_REQUEST = "ReturnRequest";

        /// <summary>
        /// Gets the reorder route name
        /// </summary>
        public const string RE_ORDER = "ReOrder";

        /// <summary>
        /// Gets the pdf invoice (file result) route name
        /// </summary>
        public const string GET_ORDER_PDF_INVOICE = "GetOrderPdfInvoice";

        /// <summary>
        /// Gets the print order details route name
        /// </summary>
        public const string PRINT_ORDER_DETAILS = "PrintOrderDetails";

        /// <summary>
        /// Gets the order downloads (file result) route name
        /// </summary>
        public const string GET_DOWNLOAD = "GetDownload";

        /// <summary>
        /// Gets the order downloads (file result) route name
        /// </summary>
        public const string GET_LICENSE = "GetLicense";

        /// <summary>
        /// Gets the download user agreement route name
        /// </summary>
        public const string DOWNLOAD_USER_AGREEMENT = "DownloadUserAgreement";

        /// <summary>
        /// Gets the get order note (file result) route name
        /// </summary>
        public const string GET_ORDER_NOTE_FILE = "GetOrderNoteFile";

        /// <summary>
        /// Gets the contact vendor route name
        /// </summary>
        public const string CONTACT_VENDOR = "ContactVendor";

        /// <summary>
        /// Gets the vendor info route name
        /// </summary>
        public const string CUSTOMER_VENDOR_INFO = "CustomerVendorInfo";

        /// <summary>
        /// Gets the customer GDPR tools route name
        /// </summary>
        public const string GDPR_TOOLS = "GdprTools";

        /// <summary>
        /// Gets the customer multi-factor authentication settings route name
        /// </summary>
        public const string MULTI_FACTOR_AUTHENTICATION_SETTINGS = "MultiFactorAuthenticationSettings";

        /// <summary>
        /// Gets the remove product from compare list route name
        /// </summary>
        public const string REMOVE_PRODUCT_FROM_COMPARE_LIST = "RemoveProductFromCompareList";

        /// <summary>
        /// Gets the clear compare list route name
        /// </summary>
        public const string CLEAR_COMPARE_LIST = "ClearCompareList";

        /// <summary>
        /// Gets the new products RSS (file result) route name
        /// </summary>
        public const string NEW_PRODUCTS_RSS = "NewProductsRSS";

        /// <summary>
        /// Gets the active discussions route name
        /// </summary>
        //forums
        public const string ACTIVE_DISCUSSIONS = "ActiveDiscussions";

        /// <summary>
        /// Gets the active discussions paged route name
        /// </summary>
        public const string ACTIVE_DISCUSSIONS_PAGED = "ActiveDiscussionsPaged";

        /// <summary>
        /// Gets the forums RSS (file result) route name
        /// </summary>
        public const string ACTIVE_DISCUSSIONS_RSS = "ActiveDiscussionsRSS";

        /// <summary>
        /// Gets the post edit route name
        /// </summary>
        public const string POST_EDIT = "PostEdit";

        /// <summary>
        /// Gets the post delete route name
        /// </summary>
        public const string POST_DELETE = "PostDelete";

        /// <summary>
        /// Gets the post create route name
        /// </summary>
        public const string POST_CREATE = "PostCreate";

        /// <summary>
        /// Gets the post create quote route name
        /// </summary>
        public const string POST_CREATE_QUOTE = "PostCreateQuote";

        /// <summary>
        /// Gets the topic edit route name
        /// </summary>
        public const string TOPIC_EDIT = "TopicEdit";

        /// <summary>
        /// Gets the topic delete route name
        /// </summary>
        public const string TOPIC_DELETE = "TopicDelete";

        /// <summary>
        /// Gets the topic create route name
        /// </summary>
        public const string TOPIC_CREATE = "TopicCreate";

        /// <summary>
        /// Gets the topic move route name
        /// </summary>
        public const string TOPIC_MOVE = "TopicMove";

        /// <summary>
        /// Gets the topic slug route name
        /// </summary>
        public const string TOPIC_SLUG = "TopicSlug";

        /// <summary>
        /// Gets the topic slug paged route name
        /// </summary>
        public const string TOPIC_SLUG_PAGED = "TopicSlugPaged";

        /// <summary>
        /// Gets the forums RSS (file result) route name
        /// </summary>
        public const string FORUM_RSS = "ForumRSS";

        /// <summary>
        /// Gets the forum slug route name
        /// </summary>
        public const string FORUM_SLUG = "ForumSlug";

        /// <summary>
        /// Gets the forum slug paged route name
        /// </summary>
        public const string FORUM_SLUG_PAGED = "ForumSlugPaged";

        /// <summary>
        /// Gets the forum group slug route name
        /// </summary>
        public const string FORUM_GROUP_SLUG = "ForumGroupSlug";

        /// <summary>
        /// Gets the forum search route name
        /// </summary>
        public const string BOARDS_SEARCH = "Search";

        /// <summary>
        /// Gets the private messages route name
        /// </summary>
        public const string PRIVATE_MESSAGES = "PrivateMessages";

        /// <summary>
        /// Gets the private messages paged route name
        /// </summary>
        public const string PRIVATE_MESSAGES_PAGED = "PrivateMessagesPaged";

        /// <summary>
        /// Gets the private messages inbox route name
        /// </summary>
        public const string PRIVATE_MESSAGES_INBOX = "PrivateMessagesInbox";

        /// <summary>
        /// Gets the private messages sent route name
        /// </summary>
        public const string PRIVATE_MESSAGES_SENT = "PrivateMessagesSent";

        /// <summary>
        /// Gets the send PM route name
        /// </summary>
        public const string SEND_PM = "SendPM";

        /// <summary>
        /// Gets the send PM reply route name
        /// </summary>
        public const string SEND_PM_REPLY = "SendPMReply";

        /// <summary>
        /// Gets the view PM route name
        /// </summary>
        public const string VIEW_PM = "ViewPM";

        /// <summary>
        /// Gets the delete PM route name
        /// </summary>
        public const string DELETE_PM = "DeletePM";

        /// <summary>
        /// Gets the newsletter activation route name
        /// </summary>
        public const string NEWSLETTER_ACTIVATION = "NewsletterActivation";

        /// <summary>
        /// Gets the robots.txt (file result) route name
        /// </summary>
        public const string ROBOTS_TXT = "robots.txt";

        /// <summary>
        /// Gets the sitemap.xml (file result) route name
        /// </summary>
        public const string SITEMAP_XML = "sitemap.xml";

        /// <summary>
        /// Gets the sitemap-indexed.xml (file result) route name
        /// </summary>
        public const string SITEMAP_INDEXED_XML = "sitemap-indexed.xml";

        /// <summary>
        /// Gets the store closed route name
        /// </summary>
        public const string STORE_CLOSED = "StoreClosed";

        /// <summary>
        /// Gets the install route name
        /// </summary>
        public const string INSTALLATION = "Installation";

        /// <summary>
        /// Gets the install change language route name
        /// </summary>
        public const string INSTALLATION_CHANGE_LANGUAGE = "InstallationChangeLanguage";

        /// <summary>
        /// Gets the page not found route name
        /// </summary>
        public const string PAGE_NOT_FOUND = "PageNotFound";
    }

    /// <summary>
    /// Represents route names for AJAX requests
    /// </summary>
    public static partial class Ajax
    {
        /// <summary>
        /// Gets the add product to cart route name (without any attributes and options). Used on catalog pages
        /// </summary>
        public const string ADD_PRODUCT_TO_CART_CATALOG = "AddProductToCart-Catalog";

        /// <summary>
        /// Gets the add product to cart route name (with attributes and options). Used on product details pages
        /// </summary>
        public const string ADD_PRODUCT_TO_CART_DETAILS = "AddProductToCart-Details";

        /// <summary>
        /// Gets the comparing products route name
        /// </summary>
        public const string ADD_PRODUCT_TO_COMPARE = "AddProductToCompare";

        /// <summary>
        /// Gets the add custom wishlist route name
        /// </summary>
        public const string ADD_WISHLIST = "AddWishlist";

        /// <summary>
        /// Gets the back in stock subscribe send route name
        /// </summary>
        public const string BACK_IN_STOCK_SUBSCRIBE_SEND = "BackInStockSubscribeSend";

        /// <summary>
        /// Gets the back in stock notifications route name
        /// </summary>
        public const string BACK_IN_STOCK_SUBSCRIBE_POPUP = "BackInStockSubscribePopup";

        /// <summary>
        /// Gets the select shipping option route name
        /// </summary>
        public const string SELECT_SHIPPING_OPTION = "SelectShippingOption";

        /// <summary>
        /// Gets the check username availability route name
        /// </summary>
        public const string CHECK_USERNAME_AVAILABILITY = "CheckUsernameAvailability";

        /// <summary>
        /// Gets the checkout attribute change route name
        /// </summary>
        public const string CHECKOUT_ATTRIBUTE_CHANGE = "CheckoutAttributeChange";

        /// <summary>
        /// Gets the customer address delete route name
        /// </summary>
        public const string CUSTOMER_ADDRESS_DELETE = "CustomerAddressDelete";

        /// <summary>
        /// Gets the customer remove external association route name
        /// </summary>
        public const string CUSTOMER_REMOVE_EXTERNAL_ASSOCIATION = "CustomerRemoveExternalAssociation";

        /// <summary>
        /// Gets the delete custom wishlist route name
        /// </summary>
        public const string DELETE_CUSTOM_WISHLIST = "DeleteCustomWishlist";

        /// <summary>
        /// Gets the estimate shipping route name
        /// </summary>
        public const string ESTIMATE_SHIPPING = "EstimateShipping";

        /// <summary>
        /// Gets the move product to custom wishlist route name. Used on catalog/product detail page.
        /// </summary>
        public const string MOVE_PRODUCT_TO_CUSTOM_WISHLIST = "MoveProductToCustomWishList";

        /// <summary>
        /// Gets the move shopping cart to custom wishlist route name. Used on catalog/product detail page.
        /// </summary>
        public const string MOVE_CART_TO_CUSTOM_WISHLIST = "MoveToCustomWishlist";

        /// <summary>
        /// Gets the product estimate shipping route name
        /// </summary>
        public const string PRODUCT_ESTIMATE_SHIPPING = "ProductEstimateShipping";

        /// <summary>
        /// Gets the product search autocomplete route name
        /// </summary>
        public const string PRODUCT_SEARCH_AUTOCOMPLETE = "ProductSearchAutoComplete";

        /// <summary>
        /// Gets the set review helpfulness route name
        /// </summary>
        public const string SET_PRODUCT_REVIEW_HELPFULNESS = "SetProductReviewHelpfulness";

        /// <summary>
        /// Gets the subscribe newsletters route name
        /// </summary>
        public const string SUBSCRIBE_NEWSLETTER = "SubscribeNewsletter";

        /// <summary>
        /// Gets the topics route name
        /// </summary>
        public const string TOPIC_POPUP = "TopicPopup";

        /// <summary>
        /// Gets the poll vote route name
        /// </summary>
        public const string POLL_VOTE = "PollVote";

        /// <summary>
        /// Gets the state list by country ID route name
        /// </summary>
        public const string GET_STATES_BY_COUNTRY_ID = "GetStatesByCountryId";

        /// <summary>
        /// Gets the EU Cookie law accept button handler route name
        /// </summary>
        public const string EU_COOKIE_LAW_ACCEPT = "EuCookieLawAccept";

        /// <summary>
        /// Gets the authenticate topic route name
        /// </summary>
        public const string TOPIC_AUTHENTICATE = "TopicAuthenticate";

        /// <summary>
        /// Gets the category products route name
        /// </summary>
        public const string GET_CATEGORY_PRODUCTS = "GetCategoryProducts";

        /// <summary>
        /// Gets the manufacturer products route name
        /// </summary>
        public const string GET_MANUFACTURER_PRODUCTS = "GetManufacturerProducts";

        /// <summary>
        /// Gets the tag products route name
        /// </summary>
        public const string GET_TAG_PRODUCTS = "GetTagProducts";

        /// <summary>
        /// Gets the search products route name
        /// </summary>
        public const string SEARCH_PRODUCTS = "SearchProducts";

        /// <summary>
        /// Gets the vendor products route name
        /// </summary>
        public const string GET_VENDOR_PRODUCTS = "GetVendorProducts";

        /// <summary>
        /// Gets the new products route name
        /// </summary>
        public const string GET_NEW_PRODUCTS = "GetNewProducts";

        /// <summary>
        /// Gets the product combinations route name
        /// </summary>
        public const string GET_PRODUCT_COMBINATIONS = "GetProductCombinations";

        /// <summary>
        /// Gets the product attributes with "upload file" type route name
        /// </summary>
        public const string UPLOAD_FILE_PRODUCT_ATTRIBUTE = "UploadFileProductAttribute";

        /// <summary>
        /// Gets the checkout attributes with "upload file" type route name
        /// </summary>
        public const string UPLOAD_FILE_CHECKOUT_ATTRIBUTE = "UploadFileCheckoutAttribute";

        /// <summary>
        /// Gets the attribute change type route name
        /// </summary>
        public const string PRODUCT_DETAILS_ATTRIBUTE_CHANGE = "ProductDetailsAttributeChange";

        /// <summary>
        /// Gets the restart application route name
        /// </summary>
        public const string INSTALLATION_RESTART_APPLICATION = "InstallationRestartApplication";

        /// <summary>
        /// Gets the post vote route name
        /// </summary>
        public const string POST_VOTE = "PostVote";

        /// <summary>
        /// Gets the topic watch route name
        /// </summary>
        public const string TOPIC_WATCH = "TopicWatch";

        /// <summary>
        /// Gets the forum watch route name
        /// </summary>
        public const string FORUM_WATCH = "ForumWatch";

        /// <summary>
        /// Gets the return request with "upload file" support route name
        /// </summary>
        public const string UPLOAD_FILE_RETURN_REQUEST = "UploadFileReturnRequest";
    }
}