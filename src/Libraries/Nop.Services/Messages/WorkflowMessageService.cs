using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Vendors;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Stores;

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
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;

        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService, ILanguageService languageService,
            ITokenizer tokenizer, IEmailAccountService emailAccountService,
            IMessageTokenProvider messageTokenProvider,
            IStoreService storeService,
            IStoreContext storeContext,
            EmailAccountSettings emailAccountSettings,
            IEventPublisher eventPublisher)
        {
            this._messageTemplateService = messageTemplateService;
            this._queuedEmailService = queuedEmailService;
            this._languageService = languageService;
            this._tokenizer = tokenizer;
            this._emailAccountService = emailAccountService;
            this._messageTokenProvider = messageTokenProvider;
            this._storeService = storeService;
            this._storeContext = storeContext;

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
        
        private MessageTemplate GetLocalizedActiveMessageTemplate(string messageTemplateName, 
            int languageId, int storeId)
        {
            //TODO remove languageId parameter
            var messageTemplate = _messageTemplateService.GetMessageTemplateByName(messageTemplateName, storeId);

            //no template found
            if (messageTemplate == null)
                return null;

            //ensure it's active
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

        private int EnsureLanguageIsActive(int languageId, int storeId)
        {
            //load language by specified ID
            var language = _languageService.GetLanguageById(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified store
                language = _languageService.GetAllLanguages(storeId: storeId).FirstOrDefault();
            }
            if (language == null || !language.Published)
            {
                //load any language
                language = _languageService.GetAllLanguages().FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");
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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewCustomer.Notification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.WelcomeMessage", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = customer.Email;
            var toName = customer.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens, 
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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.EmailValidationMessage", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);


            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = customer.Email;
            var toName = customer.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.PasswordRecovery", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);


            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = customer.Email;
            var toName = customer.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }

        #endregion

        #region Order workflow

        /// <summary>
        /// Sends an order placed notification to a vendor
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="vendor">Vendor instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendOrderPlacedVendorNotification(Order order, Vendor vendor, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (vendor == null)
                throw new ArgumentNullException("vendor");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderPlaced.VendorNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = vendor.Email;
            var toName = vendor.Name;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }

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

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderPlaced.StoreOwnerNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderPlaced.CustomerNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("ShipmentSent.CustomerNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddShipmentTokens(tokens, shipment, languageId);
            _messageTokenProvider.AddOrderTokens(tokens, shipment.Order, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, shipment.Order.Customer);
            
            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("ShipmentDelivered.CustomerNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddShipmentTokens(tokens, shipment, languageId);
            _messageTokenProvider.AddOrderTokens(tokens, shipment.Order, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, shipment.Order.Customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderCompleted.CustomerNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderCancelled.CustomerNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddOrderTokens(tokens, order, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, order.Customer);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.NewOrderNote", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddOrderNoteTokens(tokens, orderNote);
            _messageTokenProvider.AddOrderTokens(tokens, orderNote.Order, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, orderNote.Order.Customer);
            
            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeService.GetStoreById(recurringPayment.InitialOrder.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("RecurringPaymentCancelled.StoreOwnerNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddOrderTokens(tokens, recurringPayment.InitialOrder, languageId);
            _messageTokenProvider.AddCustomerTokens(tokens, recurringPayment.InitialOrder.Customer);
            _messageTokenProvider.AddRecurringPaymentTokens(tokens, recurringPayment);

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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewsLetterSubscription.ActivationMessage", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddNewsLetterSubscriptionTokens(tokens, subscription);
            
            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = subscription.Email;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Service.EmailAFriend", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //toekns
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            _messageTokenProvider.AddProductTokens(tokens, product);
            tokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage, true));
            tokens.Add(new Token("EmailAFriend.Email", customerEmail));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = friendsEmail;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Wishlist.EmailAFriend", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            tokens.Add(new Token("Wishlist.PersonalMessage", personalMessage, true));
            tokens.Add(new Token("Wishlist.Email", customerEmail));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = friendsEmail;
            var toName = "";
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeService.GetStoreById(opv.Order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewReturnRequest.StoreOwnerNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddCustomerTokens(tokens, returnRequest.Customer);
            _messageTokenProvider.AddReturnRequestTokens(tokens, returnRequest, opv);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeService.GetStoreById(opv.Order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("ReturnRequestStatusChanged.CustomerNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddCustomerTokens(tokens, returnRequest.Customer);
            _messageTokenProvider.AddReturnRequestTokens(tokens, returnRequest, opv);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = returnRequest.Customer.Email;
            var toName = returnRequest.Customer.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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
            var store = _storeContext.CurrentStore;

            var messageTemplate = GetLocalizedActiveMessageTemplate("Forums.NewForumTopic", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddForumTopicTokens(tokens, forumTopic);
            _messageTokenProvider.AddForumTokens(tokens, forumTopic.Forum);

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

            var store = _storeContext.CurrentStore;

            var messageTemplate = GetLocalizedActiveMessageTemplate("Forums.NewForumPost", languageId, store.Id);
            if (messageTemplate == null )
            {
                return 0;
            }

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddForumPostTokens(tokens, forumPost);
            _messageTokenProvider.AddForumTopicTokens(tokens, forumPost.ForumTopic,
                friendlyForumTopicPageIndex, forumPost.Id);
            _messageTokenProvider.AddForumTokens(tokens, forumPost.ForumTopic.Forum);

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

            var store = _storeService.GetStoreById(privateMessage.StoreId) ?? _storeContext.CurrentStore;

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.NewPM", languageId, store.Id);
            if (messageTemplate == null )
            {
                return 0;
            }

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddPrivateMessageTokens(tokens, privateMessage);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);            
            var toEmail = privateMessage.ToCustomer.Email;
            var toName = privateMessage.ToCustomer.GetFullName();

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
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

            Store store = null;
            var order = giftCard.PurchasedWithOrderProductVariant != null ? 
                giftCard.PurchasedWithOrderProductVariant.Order : 
                null;
            if (order != null)
                store = _storeService.GetStoreById(order.StoreId);
            if (store == null)
                store = _storeContext.CurrentStore;

            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("GiftCard.Notification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddGiftCardTokens(tokens, giftCard);
            
            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = giftCard.RecipientEmail;
            var toName = giftCard.RecipientName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Product.ProductReview", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddProductReviewTokens(tokens, productReview);
            
            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("QuantityBelow.StoreOwnerNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddProductVariantTokens(tokens, productVariant);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewVATSubmitted.StoreOwnerNotification", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddCustomerTokens(tokens, customer);
            tokens.Add(new Token("VatValidationResult.Name", vatName));
            tokens.Add(new Token("VatValidationResult.Address", vatAddress));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Blog.BlogComment", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddBlogCommentTokens(tokens, blogComment);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("News.NewsComment", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddNewsCommentTokens(tokens, newsComment);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
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

            var store = _storeService.GetStoreById(subscription.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.BackInStock", languageId, store.Id);
            if (messageTemplate == null)
                return 0;

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store);
            _messageTokenProvider.AddCustomerTokens(tokens, subscription.Customer);
            _messageTokenProvider.AddBackInStockTokens(tokens, subscription);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var customer = subscription.Customer;
            var toEmail = customer.Email;
            var toName = customer.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, tokens,
                toEmail, toName);
        }

        #endregion

        #endregion
    }
}
