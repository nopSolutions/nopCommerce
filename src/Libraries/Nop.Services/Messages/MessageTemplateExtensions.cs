using System.Collections.Generic;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Represents message template  extensions
    /// </summary>
    public static class MessageTemplateExtensions
    {
        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        public static IEnumerable<string> GetTokenGroups(this MessageTemplate messageTemplate)
        {
            //groups depend on which tokens are added at the appropriate methods in IWorkflowMessageService
            switch (messageTemplate.Name)
            {
                case MessageTemplateSystemNames.CustomerRegisteredNotification:
                case MessageTemplateSystemNames.CustomerWelcomeMessage:
                case MessageTemplateSystemNames.CustomerEmailValidationMessage:
                case MessageTemplateSystemNames.CustomerEmailRevalidationMessage:
                case MessageTemplateSystemNames.CustomerPasswordRecoveryMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.OrderPlacedVendorNotification:
                case MessageTemplateSystemNames.OrderPlacedStoreOwnerNotification:
                case MessageTemplateSystemNames.OrderPaidStoreOwnerNotification:
                case MessageTemplateSystemNames.OrderPaidCustomerNotification:
                case MessageTemplateSystemNames.OrderPaidVendorNotification:
                case MessageTemplateSystemNames.OrderPlacedCustomerNotification:
                case MessageTemplateSystemNames.OrderCompletedCustomerNotification:
                case MessageTemplateSystemNames.OrderCancelledCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.ShipmentSentCustomerNotification:
                case MessageTemplateSystemNames.ShipmentDeliveredCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ShipmentTokens, TokenGroupNames.OrderTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.OrderRefundedStoreOwnerNotification:
                case MessageTemplateSystemNames.OrderRefundedCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.RefundedOrderTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.NewOrderNoteAddedCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderNoteTokens, TokenGroupNames.OrderTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification:
                case MessageTemplateSystemNames.RecurringPaymentCancelledCustomerNotification:
                case MessageTemplateSystemNames.RecurringPaymentFailedCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.RecurringPaymentTokens };

                case MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage:
                case MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.SubscriptionTokens };

                case MessageTemplateSystemNames.EmailAFriendMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.ProductTokens, TokenGroupNames.EmailAFriendTokens };

                case MessageTemplateSystemNames.WishlistToFriendMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.WishlistToFriendTokens };

                case MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification:
                case MessageTemplateSystemNames.NewReturnRequestCustomerNotification:
                case MessageTemplateSystemNames.ReturnRequestStatusChangedCustomerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.OrderTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.ReturnRequestTokens };

                case MessageTemplateSystemNames.NewForumTopicMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ForumTopicTokens, TokenGroupNames.ForumTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.NewForumPostMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ForumPostTokens, TokenGroupNames.ForumTopicTokens, TokenGroupNames.ForumTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.PrivateMessageNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.PrivateMessageTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.NewVendorAccountApplyStoreOwnerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.VendorTokens };

                case MessageTemplateSystemNames.VendorInformationChangeNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.VendorTokens };

                case MessageTemplateSystemNames.GiftCardNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.GiftCardTokens};

                case MessageTemplateSystemNames.ProductReviewNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ProductReviewTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ProductTokens };

                case MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ProductTokens, TokenGroupNames.AttributeCombinationTokens };

                case MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.VatValidation };

                case MessageTemplateSystemNames.BlogCommentNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.BlogCommentTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.NewsCommentNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.NewsCommentTokens, TokenGroupNames.CustomerTokens };

                case MessageTemplateSystemNames.BackInStockNotification:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.CustomerTokens, TokenGroupNames.ProductBackInStockTokens };

                case MessageTemplateSystemNames.ContactUsMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ContactUs };

                case MessageTemplateSystemNames.ContactVendorMessage:
                    return new[] { TokenGroupNames.StoreTokens, TokenGroupNames.ContactVendor };

                default:
                    return new string[] { };
            }
        }
    }
}