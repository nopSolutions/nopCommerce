using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using System.Net.Mail;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    public partial class WorkflowMessageService:IWorkflowMessageService
    {
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IQueuedEmailService _queuedEmailService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="messageTemplateService">MessageTemplateService</param>
        /// <param name="queuedEmailService">QueuedEmailService</param>
        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService)
        {
            _messageTemplateService = messageTemplateService;
            _queuedEmailService = queuedEmailService;
        }

        /// <summary>
        /// Sends an order completed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderCompletedCustomerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
            //if (order == null)
            //    throw new ArgumentNullException("order");

            //var messageTemplate = _messageTemplateService.GetMessageTemplateById(19); //string templateName = "OrderCompleted.CustomerNotification";

            ////var isActive = messageTemplate.GetLocalized((mt) => mt.IsActive);
            ////LocalizedMessageTemplate localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            ////if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
            ////    return 0;

            //var emailAccount = messageTemplate.EmailAccount;

            //string subject = ReplaceMessageTemplateTokens(order, messageTemplate.Subject, languageId);
            //string body = ReplaceMessageTemplateTokens(order, messageTemplate.Body, languageId);
            //string bcc = localizedMessageTemplate.BccEmailAddresses;

            //var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            //var to = new MailAddress(order.BillingEmail, order.BillingFullName);
            //var email = new QueuedEmail(){}
            //var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
            //    DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            //return queuedEmail.QueuedEmailId;
        }


        #region IWorkflowMessageService Members


        public int SendOrderPlacedStoreOwnerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendQuantityBelowStoreOwnerNotification(ProductVariant productVariant, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderPlacedCustomerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderShippedCustomerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderDeliveredCustomerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderCancelledCustomerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendCustomerWelcomeMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendCustomerEmailValidationMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendCustomerPasswordRecoveryMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewCustomerNotificationMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewVATSubmittedStoreOwnerNotification(Customer customer, string vatName, string vatAddress, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendProductEmailAFriendMessage(Customer customer, int languageId, Product product, string friendsEmail, string personalMessage)
        {
            throw new NotImplementedException();
        }

        public int SendProductReviewNotificationMessage(ProductReview productReview, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewsLetterSubscriptionActivationMessage(int newsLetterSubscriptionId, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewsLetterSubscriptionDeactivationMessage(int newsLetterSubscriptionId, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendGiftCardNotification(GiftCard giftCard, int languageId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
