namespace Nop.Core.Domain.Messages;

/// <summary>
/// Represents message template system names
/// </summary>
public static partial class MessageTemplateSystemNames
{
    #region Customer

    /// <summary>
    /// Represents system name of notification about new registration
    /// </summary>
    public const string CUSTOMER_REGISTERED_STORE_OWNER_NOTIFICATION = "NewCustomer.Notification";

    /// <summary>
    /// Represents system name of customer welcome message
    /// </summary>
    public const string CUSTOMER_WELCOME_MESSAGE = "Customer.WelcomeMessage";

    /// <summary>
    /// Represents system name of email validation message
    /// </summary>
    public const string CUSTOMER_EMAIL_VALIDATION_MESSAGE = "Customer.EmailValidationMessage";

    /// <summary>
    /// Represents system name of email revalidation message
    /// </summary>
    public const string CUSTOMER_EMAIL_REVALIDATION_MESSAGE = "Customer.EmailRevalidationMessage";

    /// <summary>
    /// Represents system name of password recovery message
    /// </summary>
    public const string CUSTOMER_PASSWORD_RECOVERY_MESSAGE = "Customer.PasswordRecovery";

    /// <summary>
    /// Represents system name of delete customer request notification
    /// </summary>
    public const string DELETE_CUSTOMER_REQUEST_STORE_OWNER_NOTIFICATION = "Customer.Gdpr.DeleteRequest";

    #endregion

    #region Order

    /// <summary>
    /// Represents system name of notification vendor about placed order
    /// </summary>
    public const string ORDER_PLACED_VENDOR_NOTIFICATION = "OrderPlaced.VendorNotification";

    /// <summary>
    /// Represents system name of notification store owner about placed order
    /// </summary>
    public const string ORDER_PLACED_STORE_OWNER_NOTIFICATION = "OrderPlaced.StoreOwnerNotification";

    /// <summary>
    /// Represents system name of notification affiliate about placed order
    /// </summary>
    public const string ORDER_PLACED_AFFILIATE_NOTIFICATION = "OrderPlaced.AffiliateNotification";

    /// <summary>
    /// Represents system name of notification store owner about paid order
    /// </summary>
    public const string ORDER_PAID_STORE_OWNER_NOTIFICATION = "OrderPaid.StoreOwnerNotification";

    /// <summary>
    /// Represents system name of notification customer about paid order
    /// </summary>
    public const string ORDER_PAID_CUSTOMER_NOTIFICATION = "OrderPaid.CustomerNotification";

    /// <summary>
    /// Represents system name of notification vendor about paid order
    /// </summary>
    public const string ORDER_PAID_VENDOR_NOTIFICATION = "OrderPaid.VendorNotification";

    /// <summary>
    /// Represents system name of notification affiliate about paid order
    /// </summary>
    public const string ORDER_PAID_AFFILIATE_NOTIFICATION = "OrderPaid.AffiliateNotification";

    /// <summary>
    /// Represents system name of notification customer about placed order
    /// </summary>
    public const string ORDER_PLACED_CUSTOMER_NOTIFICATION = "OrderPlaced.CustomerNotification";

    /// <summary>
    /// Represents system name of notification customer about sent shipment
    /// </summary>
    public const string SHIPMENT_SENT_CUSTOMER_NOTIFICATION = "ShipmentSent.CustomerNotification";

    /// <summary>
    /// Represents system name of notification customer about ready for pickup shipment
    /// </summary>
    public const string SHIPMENT_READY_FOR_PICKUP_CUSTOMER_NOTIFICATION = "ShipmentReadyForPickup.CustomerNotification";

    /// <summary>
    /// Represents system name of notification customer about delivered shipment
    /// </summary>
    public const string SHIPMENT_DELIVERED_CUSTOMER_NOTIFICATION = "ShipmentDelivered.CustomerNotification";

    /// <summary>
    /// Represents system name of notification customer about processing order
    /// </summary>
    public const string ORDER_PROCESSING_CUSTOMER_NOTIFICATION = "OrderProcessing.CustomerNotification";

    /// <summary>
    /// Represents system name of notification customer about completed order
    /// </summary>
    public const string ORDER_COMPLETED_CUSTOMER_NOTIFICATION = "OrderCompleted.CustomerNotification";

    /// <summary>
    /// Represents system name of notification customer about cancelled order
    /// </summary>
    public const string ORDER_CANCELLED_CUSTOMER_NOTIFICATION = "OrderCancelled.CustomerNotification";

    /// <summary>
    /// Represents system name of notification vendor about cancelled order
    /// </summary>
    public const string ORDER_CANCELLED_VENDOR_NOTIFICATION = "OrderCancelled.VendorNotification";

    /// <summary>
    /// Represents system name of notification store owner about refunded order
    /// </summary>
    public const string ORDER_REFUNDED_STORE_OWNER_NOTIFICATION = "OrderRefunded.StoreOwnerNotification";

    /// <summary>
    /// Represents system name of notification customer about refunded order
    /// </summary>
    public const string ORDER_REFUNDED_CUSTOMER_NOTIFICATION = "OrderRefunded.CustomerNotification";

    /// <summary>
    /// Represents system name of notification customer about new order note
    /// </summary>
    public const string NEW_ORDER_NOTE_ADDED_CUSTOMER_NOTIFICATION = "Customer.NewOrderNote";

    /// <summary>
    /// Represents system name of notification store owner about cancelled recurring order
    /// </summary>
    public const string RECURRING_PAYMENT_CANCELLED_STORE_OWNER_NOTIFICATION = "RecurringPaymentCancelled.StoreOwnerNotification";

    /// <summary>
    /// Represents system name of notification customer about cancelled recurring order
    /// </summary>
    public const string RECURRING_PAYMENT_CANCELLED_CUSTOMER_NOTIFICATION = "RecurringPaymentCancelled.CustomerNotification";

    /// <summary>
    /// Represents system name of notification customer about failed payment for the recurring payments
    /// </summary>
    public const string RECURRING_PAYMENT_FAILED_CUSTOMER_NOTIFICATION = "RecurringPaymentFailed.CustomerNotification";

    #endregion

    #region Newsletter

    /// <summary>
    /// Represents system name of subscription activation message
    /// </summary>
    public const string NEWSLETTER_SUBSCRIPTION_ACTIVATION_MESSAGE = "NewsLetterSubscription.ActivationMessage";

    /// <summary>
    /// Represents system name of subscription deactivation message
    /// </summary>
    public const string NEWSLETTER_SUBSCRIPTION_DEACTIVATION_MESSAGE = "NewsLetterSubscription.DeactivationMessage";

    #endregion

    #region To friend

    /// <summary>
    /// Represents system name of 'Email a friend' message
    /// </summary>
    public const string EMAIL_A_FRIEND_MESSAGE = "Service.EmailAFriend";

    /// <summary>
    /// Represents system name of 'Email a friend' message with wishlist
    /// </summary>
    public const string WISHLIST_TO_FRIEND_MESSAGE = "Wishlist.EmailAFriend";

    #endregion

    #region Return requests

    /// <summary>
    /// Represents system name of notification store owner about new return request
    /// </summary>
    public const string NEW_RETURN_REQUEST_STORE_OWNER_NOTIFICATION = "NewReturnRequest.StoreOwnerNotification";

    /// <summary>
    /// Represents system name of notification customer about new return request
    /// </summary>
    public const string NEW_RETURN_REQUEST_CUSTOMER_NOTIFICATION = "NewReturnRequest.CustomerNotification";

    /// <summary>
    /// Represents system name of notification customer about changing return request status
    /// </summary>
    public const string RETURN_REQUEST_STATUS_CHANGED_CUSTOMER_NOTIFICATION = "ReturnRequestStatusChanged.CustomerNotification";

    #endregion

    #region Forum

    /// <summary>
    /// Represents system name of notification about new forum topic
    /// </summary>
    public const string NEW_FORUM_TOPIC_MESSAGE = "Forums.NewForumTopic";

    /// <summary>
    /// Represents system name of notification about new forum post
    /// </summary>
    public const string NEW_FORUM_POST_MESSAGE = "Forums.NewForumPost";

    /// <summary>
    /// Represents system name of notification about new private message
    /// </summary>
    public const string PRIVATE_MESSAGE_NOTIFICATION = "Customer.NewPM";

    #endregion

    #region Misc

    /// <summary>
    /// Represents system name of notification store owner about applying new vendor account
    /// </summary>
    public const string NEW_VENDOR_ACCOUNT_APPLY_STORE_OWNER_NOTIFICATION = "VendorAccountApply.StoreOwnerNotification";

    /// <summary>
    /// Represents system name of notification vendor about changing information
    /// </summary>
    public const string VENDOR_INFORMATION_CHANGE_STORE_OWNER_NOTIFICATION = "VendorInformationChange.StoreOwnerNotification";

    /// <summary>
    /// Represents system name of notification about gift card
    /// </summary>
    public const string GIFT_CARD_NOTIFICATION = "GiftCard.Notification";

    /// <summary>
    /// Represents system name of notification store owner about new product review
    /// </summary>
    public const string PRODUCT_REVIEW_STORE_OWNER_NOTIFICATION = "Product.ProductReview";

    /// <summary>
    /// Represents system name of notification customer about product review reply
    /// </summary>
    public const string PRODUCT_REVIEW_REPLY_CUSTOMER_NOTIFICATION = "ProductReview.Reply.CustomerNotification";

    /// <summary>
    /// Represents system name of notification store owner about below quantity of product
    /// </summary>
    public const string QUANTITY_BELOW_STORE_OWNER_NOTIFICATION = "QuantityBelow.StoreOwnerNotification";

    /// <summary>
    /// Represents system name of notification store owner about below quantity of product attribute combination
    /// </summary>
    public const string QUANTITY_BELOW_ATTRIBUTE_COMBINATION_STORE_OWNER_NOTIFICATION = "QuantityBelow.AttributeCombination.StoreOwnerNotification";

    /// <summary>
    /// Represents system name of notification store owner about submitting new VAT
    /// </summary>
    public const string NEW_VAT_SUBMITTED_STORE_OWNER_NOTIFICATION = "NewVATSubmitted.StoreOwnerNotification";

    /// <summary>
    /// Represents system name of notification store owner about new blog comment
    /// </summary>
    public const string BLOG_COMMENT_STORE_OWNER_NOTIFICATION = "Blog.BlogComment";

    /// <summary>
    /// Represents system name of notification store owner about new news comment
    /// </summary>
    public const string NEWS_COMMENT_STORE_OWNER_NOTIFICATION = "News.NewsComment";

    /// <summary>
    /// Represents system name of notification customer about product receipt
    /// </summary>
    public const string BACK_IN_STOCK_NOTIFICATION = "Customer.BackInStock";

    /// <summary>
    /// Represents system name of 'Contact us' message
    /// </summary>
    public const string CONTACT_US_MESSAGE = "Service.ContactUs";

    /// <summary>
    /// Represents system name of 'Contact vendor' message
    /// </summary>
    public const string CONTACT_VENDOR_MESSAGE = "Service.ContactVendor";

    #endregion
}