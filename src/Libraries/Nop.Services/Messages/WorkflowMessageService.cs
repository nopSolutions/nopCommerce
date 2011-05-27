using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
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

        #endregion

        #region Ctor

        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService, ILanguageService languageService,
            ITokenizer tokenizer, IEmailAccountService emailAccountService,
            IMessageTokenProvider messageTokenProvider, EmailAccountSettings emailAccountSettings)
        {
            this._messageTemplateService = messageTemplateService;
            this._queuedEmailService = queuedEmailService;
            this._languageService = languageService;
            this._tokenizer = tokenizer;
            this._emailAccountService = emailAccountService;
            this._messageTokenProvider = messageTokenProvider;

            this._emailAccountSettings = emailAccountSettings;
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
            var subjectReplaced = _tokenizer.Replace(subject, tokens);
            var bodyReplaced = _tokenizer.Replace(body, tokens);
            
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

        private IList<Token> GenerateTokens(ProductVariant productVariant)
        {
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens);
            _messageTokenProvider.AddProductVariantTokens(tokens, productVariant);
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
        public int SendCustomerRegisteredNotificationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewCustomer.Notification", languageId);
            if (messageTemplate == null)
                return 0;

            var customerTokens = GenerateTokens(customer);

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
        public int SendCustomerWelcomeMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var user = customer.GetDefaultUserAccount();
            if (user == null)
                throw new ArgumentException("Can't load associated user account");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.WelcomeMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var customerTokens = GenerateTokens(customer);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = user.Email;
            var toName = string.Format("{0} {1}", customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName), customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName));
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
        public int SendCustomerEmailValidationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            //search all users because the latest one could be not approved yet
            var user = customer.GetDefaultUserAccount(false);
            if (user == null)
                throw new ArgumentException("Can't load associated user account");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.EmailValidationMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var customerTokens = GenerateTokens(customer);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = user.Email;
            var toName = string.Format("{0} {1}", customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName), customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName));
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
        public int SendCustomerPasswordRecoveryMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
            
            var user = customer.GetDefaultUserAccount();
            if (user == null)
                throw new ArgumentException("Can't load associated user account");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Customer.PasswordRecovery", languageId);
            if (messageTemplate == null)
                return 0;

            var customerTokens = GenerateTokens(customer);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = user.Email;
            var toName = string.Format("{0} {1}", customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName), customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName));
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
        public int SendOrderPlacedStoreOwnerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderPlaced.StoreOwnerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

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
        public int SendOrderPlacedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderPlaced.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends an order shipped notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderShippedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderShipped.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends an order delivered notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderDeliveredCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderDelivered.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends an order completed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderCompletedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderCompleted.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

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
        public int SendOrderCancelledCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("OrderCancelled.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = order.BillingAddress.Email;
            var toName = string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName);
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
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
        public int SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription,
            int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewsLetterSubscription.ActivationMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(subscription);

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
        public int SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription,
            int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewsLetterSubscription.DeactivationMessage", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(subscription);

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
        public int SendProductEmailAFriendMessage(Customer customer, int languageId,
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
            customerProductTokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage));
            //UNDONE use customerEmail

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
        public int SendWishlistEmailAFriendMessage(Customer customer, int languageId,
             string customerEmail, string friendsEmail, string personalMessage)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Wishlist.EmailAFriend", languageId);
            if (messageTemplate == null)
                return 0;

            var customerTokens = GenerateTokens(customer);
            customerTokens.Add(new Token("Wishlist.URLForCustomer", "UNDONE Not implemented"));
            customerTokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage));
            //UNDONE use customerEmail

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
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewReturnRequestStoreOwnerNotification(ReturnRequest returnRequest, OrderProductVariant opv, int languageId)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("NewReturnRequest.StoreOwnerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(returnRequest, opv);

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
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendReturnRequestStatusChangedCustomerNotification(ReturnRequest returnRequest, OrderProductVariant opv, int languageId)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            var user = returnRequest.Customer.GetDefaultUserAccount();
            if (user == null)
                throw new ArgumentException("Can't load associated user account");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("ReturnRequestStatusChanged.CustomerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(returnRequest, opv);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = user.Email;
            var toName = string.Format("{0} {1}", returnRequest.Customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName), returnRequest.Customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName));
            return SendNotification(messageTemplate, emailAccount,
                languageId, orderTokens,
                toEmail, toName);
        }
        
        #endregion

        #region Misc

        /// <summary>
        /// Sends a gift card notification
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendGiftCardNotification(GiftCard giftCard, int languageId)
        {
            if (giftCard == null)
                throw new ArgumentNullException("giftCard");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("GiftCard.Notification", languageId);
            if (messageTemplate == null)
                return 0;

            var giftCardTokens = GenerateTokens(giftCard);

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
        public int SendProductReviewNotificationMessage(ProductReview productReview,
            int languageId)
        {
            if (productReview == null)
                throw new ArgumentNullException("productReview");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("Product.ProductReview", languageId);
            if (messageTemplate == null)
                return 0;

            var productReviewTokens = GenerateTokens(productReview);
            
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
        public int SendQuantityBelowStoreOwnerNotification(ProductVariant productVariant, int languageId)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("QuantityBelow.StoreOwnerNotification", languageId);
            if (messageTemplate == null)
                return 0;

            var productVariantTokens = GenerateTokens(productVariant);

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
        public int SendNewVatSubmittedStoreOwnerNotification(Customer customer,
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

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, vatSubmittedTokens,
                toEmail, toName);
        }
        
        #endregion

        #endregion
    }
}
