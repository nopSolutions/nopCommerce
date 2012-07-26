using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;

namespace Nop.Services.Messages
{
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ILanguageService _languageService;
        private readonly ITokenizer _tokenizer;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IMessageTokenProvider _messageTokenProvider;

        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService, ILanguageService languageService,
            ITokenizer tokenizer, IEmailAccountService emailAccountService,
            IMessageTokenProvider messageTokenProvider,
            EmailAccountSettings emailAccountSettings,
            IEventPublisher eventPublisher)
        {
            this._messageTemplateService = messageTemplateService;
            this._queuedEmailService = queuedEmailService;
            this._languageService = languageService;
            this._tokenizer = tokenizer;
            this._emailAccountService = emailAccountService;
            this._messageTokenProvider = messageTokenProvider;

            this._emailAccountSettings = emailAccountSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities
        
        private int SendNotification(MessageTemplate messageTemplate, 
            EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens,
            string toEmailAddress, string toName)
        {
            //retrieve localized message template data
            var bcc = messageTemplate.GetLocalized((mt) => mt.BccEmailAddresses, languageId);
            var subject = messageTemplate.GetLocalized((mt) => mt.Subject, languageId);
            var body = messageTemplate.GetLocalized((mt) => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);
            
            var email = new QueuedEmail()
            {
                Priority = 5,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id
            };

            _queuedEmailService.InsertQueuedEmail(email);
            return email.Id;
        }
        
        private IList<Token> GenerateTokens(Customer customer)
        {
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            return tokens;
        }

        private IList<Token> GenerateTokens(Customer customer, Product product)
        {
            var tokens = new List<Token>();

            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _messageTokenProvider.AddProductTokens(tokens, product);

            return tokens;
        }

        private IList<Token> GenerateTokens(Order order, int languageId)
        {
            var tokens = new List<Token>();

            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

            return tokens;
        }
        private IList<Token> GenerateTokens(OrderNote orderNote, int languageId)
        {
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddOrderNoteTokens(tokens, orderNote);
            _messageTokenProvider.AddOrderTokens(tokens, orderNote.Order, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, orderNote.Order.Customer);

            return tokens;
        }
        private IList<Token> GenerateTokens(Shipment shipment, int languageId)
        {
            var tokens = new List<Token>();

            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddShipmentTokens(tokens, shipment, languageId);
            _messageTokenProvider.AddOrderTokens(tokens, shipment.Order, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, shipment.Order.Customer);

            return tokens;
        }
        private IList<Token> GenerateTokens(RecurringPayment recurringPayment, int languageId)
        {
            var tokens = new List<Token>();

            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddOrderTokens(tokens, recurringPayment.InitialOrder, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, recurringPayment.InitialOrder.Customer);
            _messageTokenProvider.AddRecurringPaymentTokens(tokens, recurringPayment);

            return tokens;
        }


        private IList<Token> GenerateTokens(ReturnRequest returnRequest,  OrderProductVariant opv)
        {
            var tokens = new List<Token>();

            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddCustomerTokens(tokens, returnRequest.Customer);
            _messageTokenProvider.AddReturnRequestTokens(tokens, returnRequest, opv);

            return tokens;
        }

        private IList<Token> GenerateTokens(GiftCard giftCard)
        {
            var tokens = new List<Token>();

            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddGiftCardTokens(tokens, giftCard);

            return tokens;
        }

        private IList<Token> GenerateTokens(NewsLetterSubscription subscription)
        {
            var tokens = new List<Token>();

            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddNewsLetterSubscriptionTokens(tokens, subscription);
            
            return tokens;
        }

        private IList<Token> GenerateTokens(ProductReview productReview)
        {
            var tokens = new List<Token>();

            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddProductReviewTokens(tokens, productReview);

            return tokens;
        }

        private IList<Token> GenerateTokens(BlogComment blogComment)
        {
            var tokens = new List<Token>();

            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddBlogCommentTokens(tokens, blogComment);

            return tokens;
        }

        private IList<Token> GenerateTokens(NewsComment newsComment)
        {
            var tokens = new List<Token>();

            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddNewsCommentTokens(tokens, newsComment);

            return tokens;
        }
        private IList<Token> GenerateTokens(ProductVariant productVariant)
        {
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddProductVariantTokens(tokens, productVariant);
            return tokens;
        }

        private IList<Token> GenerateTokens(ForumTopic forumTopic)
        {
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddForumTopicTokens(tokens, forumTopic);
            _messageTokenProvider.AddForumTokens(tokens, forumTopic.Forum);
            return tokens;            
        }

        private IList<Token> GenerateTokens(ForumPost forumPost, int friendlyForumTopicPageIndex,
            int appendPostIdentifier)
        {
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddForumPostTokens(tokens, forumPost);
            _messageTokenProvider.AddForumTopicTokens(tokens, forumPost.ForumTopic,
                friendlyForumTopicPageIndex, appendPostIdentifier);
            _messageTokenProvider.AddForumTokens(tokens, forumPost.ForumTopic.Forum);

            return tokens;
        }
        
        private IList<Token> GenerateTokens(PrivateMessage privateMessage)
        {
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddPrivateMessageTokens(tokens, privateMessage);
            return tokens;
        }
        private IList<Token> GenerateTokens(BackInStockSubscription stockSubscription)
        {
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddCustomerTokens(tokens, stockSubscription.Customer);
            _messageTokenProvider.AddBackInStockTokens(tokens, stockSubscription);
            return tokens;
        }
        
        private MessageTemplate GetLocalizedActiveMessageTemplate(string messageTemplateName, int languageId)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateByName(messageTemplateName);
            if (messageTemplate == null)
                return null;

            //var isActive = messageTemplate.GetLocalized((mt) => mt.IsActive, languageId);
            //use
            var isActive = messageTemplate.IsActive;
            if (!isActive)
                return null;

            return messageTemplate;
        }

        private EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccounId = messageTemplate.GetLocalized(mt => mt.EmailAccountId, languageId);
            var emailAccount = _emailAccountService.GetEmailAccountById(emailAccounId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            return emailAccount;

        }

        private int EnsureLanguageIsActive(int languageId)
        {
            var language = _languageService.GetLanguageById(languageId);
            if (language == null || !language.Published)
                language = _languageService.GetAllLanguages().FirstOrDefault();
            return language.Id;
        }

        #endregion

        #region Methods

        #region Customer workflow

        /// <summary>
        /// Sends 'New customer' notification message to a store owner
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerRegisteredNotificationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewCustomer.Notification", languageId);
            if (messageTemplate == null)
                return 0;

            var customerTokens = GenerateTokens(customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, customerTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, customerTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a welcome message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerWelcomeMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.WelcomeMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var customerTokens = GenerateTokens(customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, customerTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = customer.Email;
            var toName = customer.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, customerTokens, 
                toEmail, toName);
        }

        /// <summary>
        /// Sends an email validation message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerEmailValidationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.EmailValidationMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var customerTokens = GenerateTokens(customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, customerTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = customer.Email;
            var toName = customer.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, customerTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends password recovery message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendCustomerPasswordRecoveryMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
            
            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.PasswordRecovery", languageId);
            if (messageTemplate == null)
                return 0;

            var customerTokens = GenerateTokens(customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, customerTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = customer.Email;
            var toName = customer.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, customerTokens,
                toEmail, toName);
        }

        #endregion

        #region Order workflow

        /// <summary>
        /// Sends an order placed notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderPlacedStoreOwnerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderPlaced.StoreOwnerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, orderTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends an order placed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderPlacedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderPlaced.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, orderTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a shipment sent notification to a customer
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendShipmentSentCustomerNotification(Shipment shipment, int languageId)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            var order = shipment.Order;
            if (order == null)
                throw new Exception("Order cannot be loaded");

            languageId = EnsureLanguageIsActive(languageId);
            var messageTemplate = GetLocalizedActiveMessageTemplate("ShipmentSent.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var shipmentTokens = GenerateTokens(shipment, languageId);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, shipmentTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, shipmentTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a shipment delivered notification to a customer
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendShipmentDeliveredCustomerNotification(Shipment shipment, int languageId)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            var order = shipment.Order;
            if (order == null)
                throw new Exception("Order cannot be loaded");

            languageId = EnsureLanguageIsActive(languageId);
            var messageTemplate = GetLocalizedActiveMessageTemplate("ShipmentDelivered.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var shipmentTokens = GenerateTokens(shipment, languageId);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, shipmentTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, shipmentTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends an order completed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderCompletedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderCompleted.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, orderTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends an order cancelled notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderCancelledCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderCancelled.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, orderTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a new order note added notification to a customer
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewOrderNoteAddedCustomerNotification(OrderNote orderNote, int languageId)
        {
            if (orderNote == null)
                throw new ArgumentNullException("orderNote");
           
            var order = orderNote.Order;

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.NewOrderNote", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(orderNote, languageId);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, orderTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a "Recurring payment cancelled" notification to a store owner
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendRecurringPaymentCancelledStoreOwnerNotification(RecurringPayment recurringPayment, int languageId)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException("recurringPayment");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("RecurringPaymentCancelled.StoreOwnerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var tokens = GenerateTokens(recurringPayment, languageId);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }

        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription,
            int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewsLetterSubscription.ActivationMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(subscription);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, orderTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = subscription.Email;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription,
            int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewsLetterSubscription.DeactivationMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(subscription);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, orderTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = subscription.Email;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        #endregion
        
        #region Send a message to a friend

        /// <summary>
        /// Sends "email a friend" message
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="product">Product instance</param>
        /// <param name="customerEmail">Customer's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendProductEmailAFriendMessage(Customer customer, int languageId,
            Product product, string customerEmail, string friendsEmail, string personalMessage)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (product == null)
                throw new ArgumentNullException("product");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Service.EmailAFriend", languageId);
            if (messageTemplate == null)
                return 0;

            var customerProductTokens = GenerateTokens(customer, product);
            customerProductTokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage, true));
            customerProductTokens.Add(new Token("EmailAFriend.Email", customerEmail));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, customerProductTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = friendsEmail;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, customerProductTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends wishlist "email a friend" message
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="customerEmail">Customer's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendWishlistEmailAFriendMessage(Customer customer, int languageId,
             string customerEmail, string friendsEmail, string personalMessage)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Wishlist.EmailAFriend", languageId);
            if (messageTemplate == null)
                return 0;

            var customerTokens = GenerateTokens(customer);
            customerTokens.Add(new Token("Wishlist.PersonalMessage", personalMessage, true));
            customerTokens.Add(new Token("Wishlist.Email", customerEmail));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, customerTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = friendsEmail;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, customerTokens,
                toEmail, toName);
        }

        #endregion

        #region Return requests

        /// <summary>
        /// Sends 'New Return Request' message to a store owner
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="opv">Order product variant</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewReturnRequestStoreOwnerNotification(ReturnRequest returnRequest, OrderProductVariant opv, int languageId)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewReturnRequest.StoreOwnerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(returnRequest, opv);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, orderTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends 'Return Request status changed' message to a customer
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="opv">Order product variant</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendReturnRequestStatusChangedCustomerNotification(ReturnRequest returnRequest, OrderProductVariant opv, int languageId)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("ReturnRequestStatusChanged.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(returnRequest, opv);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, orderTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = returnRequest.Customer.Email;
            var toName = returnRequest.Customer.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }
        
        #endregion

        #region Forum Notifications

        /// <summary>
        /// Sends a forum subscription message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewForumTopicMessage(Customer customer,
            ForumTopic forumTopic, Forum forum, int languageId)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            var messageTemplate = GetLocalizedActiveMessageTemplate("Forums.NewForumTopic", languageId);
            if (messageTemplate == null || !messageTemplate.IsActive)
            {
                return 0;
            }

            var tokens = GenerateTokens(forumTopic);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = customer.Email;
            var toName = customer.GetFullName();

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }

        /// <summary>
        /// Sends a forum subscription message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="forumPost">Forum post</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewForumPostMessage(Customer customer,
            ForumPost forumPost, ForumTopic forumTopic,
            Forum forum, int friendlyForumTopicPageIndex, int languageId)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            var messageTemplate = GetLocalizedActiveMessageTemplate("Forums.NewForumPost", languageId);
            if (messageTemplate == null || !messageTemplate.IsActive)
            {
                return 0;
            }

            var tokens = GenerateTokens(forumPost, friendlyForumTopicPageIndex, forumPost.Id);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);            
            var toEmail = customer.Email;
            var toName = customer.GetFullName();

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }

        /// <summary>
        /// Sends a private message notification
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendPrivateMessageNotification(PrivateMessage privateMessage, int languageId)
        {
            if (privateMessage == null)
            {
                throw new ArgumentNullException("privateMessage");
            }

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.NewPM", languageId);
            if (messageTemplate == null || !messageTemplate.IsActive)
            {
                return 0;
            }

            var privateMessageTokens = GenerateTokens(privateMessage);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, privateMessageTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);            
            var toEmail = privateMessage.ToCustomer.Email;
            var toName = privateMessage.ToCustomer.GetFullName();

            return SendNotification(messageTemplate, emailAccount, languageId, privateMessageTokens, toEmail, toName);
        }

        #endregion

        #region Misc

        /// <summary>
        /// Sends a gift card notification
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendGiftCardNotification(GiftCard giftCard, int languageId)
        {
            if (giftCard == null)
                throw new ArgumentNullException("giftCard");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("GiftCard.Notification", languageId);
            if (messageTemplate == null)
                return 0;

            var giftCardTokens = GenerateTokens(giftCard);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, giftCardTokens);
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = giftCard.RecipientEmail;
            var toName = giftCard.RecipientName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, giftCardTokens,
                toEmail, toName);
        }
        
        /// <summary>
        /// Sends a product review notification message to a store owner
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendProductReviewNotificationMessage(ProductReview productReview,
            int languageId)
        {
            if (productReview == null)
                throw new ArgumentNullException("productReview");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Product.ProductReview", languageId);
            if (messageTemplate == null)
                return 0;

            var productReviewTokens = GenerateTokens(productReview);
            
            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, productReviewTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, productReviewTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a "quantity below" notification to a store owner
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendQuantityBelowStoreOwnerNotification(ProductVariant productVariant, int languageId)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("QuantityBelow.StoreOwnerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var productVariantTokens = GenerateTokens(productVariant);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, productVariantTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, productVariantTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a "new VAT sumitted" notification to a store owner
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="vatName">Received VAT name</param>
        /// <param name="vatAddress">Received VAT address</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewVatSubmittedStoreOwnerNotification(Customer customer,
            string vatName, string vatAddress, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewVATSubmitted.StoreOwnerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var vatSubmittedTokens = GenerateTokens(customer);
            vatSubmittedTokens.Add(new Token("VatValidationResult.Name", vatName));
            vatSubmittedTokens.Add(new Token("VatValidationResult.Address", vatAddress));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, vatSubmittedTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, vatSubmittedTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a blog comment notification message to a store owner
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendBlogCommentNotificationMessage(BlogComment blogComment, int languageId)
        {
            if (blogComment == null)
                throw new ArgumentNullException("blogComment");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Blog.BlogComment", languageId);
            if (messageTemplate == null)
                return 0;

            var blogCommentTokens = GenerateTokens(blogComment);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, blogCommentTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, blogCommentTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a news comment notification message to a store owner
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewsCommentNotificationMessage(NewsComment newsComment, int languageId)
        {
            if (newsComment == null)
                throw new ArgumentNullException("newsComment");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("News.NewsComment", languageId);
            if (messageTemplate == null)
                return 0;

            var newsCommentTokens = GenerateTokens(newsComment);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, newsCommentTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, newsCommentTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a 'Back in stock' notification message to a customer
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendBackInStockNotification(BackInStockSubscription subscription, int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.BackInStock", languageId);
            if (messageTemplate == null)
                return 0;

            var subscriptionTokens = GenerateTokens(subscription);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, subscriptionTokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var customer = subscription.Customer;
            var toEmail = customer.Email;
            var toName = customer.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, subscriptionTokens,
                toEmail, toName);
        }

        #endregion

        #endregion
    }
}
